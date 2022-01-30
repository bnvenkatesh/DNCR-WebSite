using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Configuration;
using NLog;
using System.IO;
using System.Data.SqlClient;
using SD.ACMA.DNCR.Infrastructure.Extensions;


namespace SD.ACMA.Service.EmailInParser
{
    using System.Diagnostics;
    using SD.ACMA.BusinessLogic.Avanade;
    using SD.ACMA.POCO.Consumer;
    using System.Net.Mail;
    using System.Runtime.InteropServices.ComTypes;

    public class EmailInParserService
    {
        /// <summary>
        /// Internal logging object
        /// </summary>
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Internal reference to parent service
        /// </summary>
        private EmailInParser _service;

        /// <summary>
        /// Internal status of processing loop
        /// </summary>
        private Boolean _running = false;

        /// <summary>
        /// Indicates the current state of the primary processing loop
        /// </summary>
        public Boolean IsRunning { get { return this._running; } }

        private DateTime _nextPollTime = DateTime.MinValue;
        private DateTime _nextHeartBeat = DateTime.MinValue;

        //heartbeat properties
        private string _previousFileError;
        public string PreviousFileError
        {
            get
            {
                return _previousFileError;
            }
            set
            {
                _previousFileError = value;
            }
        }

        private int _fileErrorCount = 0;
        public int FileErrorCount
        {
            get
            {
                return _fileErrorCount;
            }
            set
            {
                _fileErrorCount = value;
            }
        }

        private int _queryFailCount = 0;
        public int QueryFailCount
        {
            get
            {
                return _queryFailCount;
            }
            set
            {
                _queryFailCount = value;
            }
        }

        /// <summary>
        /// getting the config settings
        /// </summary>
        #region Configuration Settings
        private String _instanceName = string.Format("{0}_{1}", ConfigurationManager.AppSettings["Instance.Name"] ?? "EmailInParser", System.Environment.MachineName);

        private Int32 _processingLoopMS = Int32.Parse(ConfigurationManager.AppSettings["App.ProcessingLoopMS"] ?? "1000");
        private Int32 _batchPollMSGreen = Int32.Parse(ConfigurationManager.AppSettings["App.BatchPollMSGreen"] ?? "5000");
        private Int32 _batchPollMSYellow = Int32.Parse(ConfigurationManager.AppSettings["App.BatchPollMSYellow"] ?? "10000");
        private Int32 _batchPollMSRed = Int32.Parse(ConfigurationManager.AppSettings["App.BatchPollMSRed"] ?? "30000");
        private Int32 _heartbeatMS = Int32.Parse(ConfigurationManager.AppSettings["App.HeartbeatMS"] ?? "60000");

        private Int32 _greenThreshold = Int32.Parse(ConfigurationManager.AppSettings["App.GreenThreshold"] ?? "0");
        private Int32 _yellowThreshold = Int32.Parse(ConfigurationManager.AppSettings["App.YellowThreshold"] ?? "5");

        private String _emailFolderPath = ConfigurationManager.AppSettings["App.EmailFolderPath"] ?? @"C:\Temp\EmailIn\";
        private String _emailProcessedFolderPath = ConfigurationManager.AppSettings["App.EmailProcessedFolderPath"] ?? @"C:\Tmp\ACMA.EmailsProcessed\";

        private Int32 _priority = Int32.Parse(ConfigurationManager.AppSettings["App.Priority"] ?? "1");
        private Int32 _batchSize = Int32.Parse(ConfigurationManager.AppSettings["App.BatchSize"] ?? "100");
        private Int32 _clashThreshold = Int32.Parse(ConfigurationManager.AppSettings["App.ClashThreshold"] ?? "10");
        private Int32 _maxRetry = Int32.Parse(ConfigurationManager.AppSettings["App.MaxRetry"] ?? "5");

        private String _withoutToken_FirstScenario_ToEmail = ConfigurationManager.AppSettings["WithoutToken_FirstScenario_ToEmail"];
        private String _withoutToken_SecondScenario_ToEmail = ConfigurationManager.AppSettings["WithoutToken_SecondScenario_ToEmail"];
        private String _withoutToken_RestOfScenario_ToEmail = ConfigurationManager.AppSettings["WithoutToken_RestOfScenario_ToEmail"];

        private String _withoutToken_FirstScenario_RerouteEmail = ConfigurationManager.AppSettings["WithoutToken_FirstScenario_RerouteEmail"];
        private String _withoutToken_SecondScenario_RerouteEmail = ConfigurationManager.AppSettings["WithoutToken_SecondScenario_RerouteEmail"];
        private String _withoutToken_RestOfScenario_RerouteEmail = ConfigurationManager.AppSettings["WithoutToken_RestOfScenario_RerouteEmail"];

        private String _sMTP_Server = ConfigurationManager.AppSettings["SMTP_Server"];

        #endregion


        public EmailInParserService(EmailInParser service)
            : base()
        {
            this._service = service;

            // TODO: Get Avanade web service reference here
        }

        public void OnStart(String[] args)
        {
            
            _logger.Debug("OnStart started");
            _logger.Debug("args:{0}", args != null ? String.Join(",", args) : "[null]");

            _logger.Debug("CONFIGURATION");
            _logger.Debug("_instanceName:{0}", _instanceName);
            _logger.Debug("_processingLoopMS:{0}", _processingLoopMS);
            _logger.Debug("_batchPollMSGreen:{0}", _batchPollMSGreen);
            _logger.Debug("_batchPollMSYellow:{0}", _batchPollMSYellow);
            _logger.Debug("_batchPollMSRed:{0}", _batchPollMSRed);
            _logger.Debug("_heartbeatMS:{0}", _heartbeatMS);
            _logger.Debug("_greenThreshold:{0}", _greenThreshold);
            _logger.Debug("_yellowThreshold:{0}", _yellowThreshold);
            _logger.Debug("_emailFolderPath:{0}", _emailFolderPath);
            _logger.Debug("_priority:{0}", _priority);
            _logger.Debug("_batchSize:{0}", _batchSize);
            _logger.Debug("_clashThreshold:{0}", _clashThreshold);
            _logger.Debug("END CONFIGURATION");

            this._running = true;

            _logger.Debug("Creating Heartbeat({0})", this._instanceName);

            while (this.IsRunning)
            {
                if (_nextPollTime <= DateTime.Now)
                {
                    //process email files
                    DateTime batchStart = DateTime.Now;

                    ProcessEmails();

                    DateTime batchComplete = DateTime.Now;

                    TimeSpan timeToCompleteProcess = batchComplete - batchStart;
                    _logger.Debug("BATCHSTATS: Instance:{0} | Time taken:{1}", this._instanceName, timeToCompleteProcess);
                }

                if (this._running)
                {
                    Thread.Sleep(_processingLoopMS);
                }
            }

            _logger.Debug("OnStart ended");
        }


        private void ProcessEmails()
        {
            string workDir = AppDomain.CurrentDomain.BaseDirectory;
            DirectoryInfo directory = new DirectoryInfo(_emailFolderPath);
            string currentFileProcessed = "";
            int clashCount = 0;
            int fileProcessedCount = 0;

            //get the list of files
            var fileList = directory.EnumerateFiles("*.eml").OrderBy(f => f.LastAccessTimeUtc).ToList();

            //loop through file list
            foreach (FileInfo file in fileList)
            {
                try
                {
                    currentFileProcessed = file.FullName;

                    if (File.Exists(file.FullName))
                    {
                        //rename the file, prevent others from sharing it
                        using (FileStream stream = new FileStream(file.FullName, FileMode.Open, FileAccess.ReadWrite, FileShare.Delete))
                        {
                            File.Move(file.FullName, file.FullName + ".taken");
                        }
                        CDO.Message msg = new CDO.Message();
                        /*ADODB.Stream adodbStream = new ADODB.Stream();
                        adodbStream.Open(Type.Missing, ADODB.ConnectModeEnum.adModeUnknown, ADODB.StreamOpenOptionsEnum.adOpenStreamUnspecified, String.Empty, String.Empty);
                        adodbStream.LoadFromFile(file.FullName + ".taken");
                        adodbStream.Flush();
                        msg.DataSource.OpenObject(adodbStream, "_Stream");
                        msg.DataSource.Save();*/

                        ADODB.Stream adodbStream = msg.GetStream();
                        adodbStream.Type = ADODB.StreamTypeEnum.adTypeText;
                        adodbStream.LoadFromFile(file.FullName + ".taken");
                        adodbStream.Flush();

                        var token = GetEmailToken(msg);

                        _logger.Debug("Original email | From: {0} | To: {1}", msg.From, msg.To);

                        if (!string.IsNullOrEmpty(token))
                        {
                            _logger.Debug("Token Found: {0}", token);
                            //call Avanade web service
                            bool result = this.PushFileToWebService(adodbStream, msg, token);
                            _logger.Debug("Push attempt to WEb service for token: {0}", token);

                            if (!result)
                            {
                                for (int i = 1; i < _maxRetry; i++)
                                {
                                    result = this.PushFileToWebService(adodbStream, msg, token);
                                    _logger.Debug("Push attempt to WEb service for token: {0}", token);

                                    if (result)
                                        break;
                                }

                                if (!result)
                                {
                                    var rerouteEmailAddress = GetEmailReroute(msg.To.Split('<', '>')[1]);
                                    SendEmail(msg, msg.From, rerouteEmailAddress, _sMTP_Server);
                                }
                            }
                        }
                        else
                        {
                            //Forward to CSR mailbox
                            var rerouteEmailAddress = GetEmailReroute(msg.To.Split('<', '>')[1]);
                            SendEmail(msg, msg.From, rerouteEmailAddress, _sMTP_Server);
                        }

                        //checking for blockers
                        if (PreviousFileError == currentFileProcessed)
                        {
                            FileErrorCount += 1;
                        }
                        else
                        {
                            FileErrorCount = 0;
                        }

                        PreviousFileError = currentFileProcessed;

                        //move to Processed folder
                        File.Move(file.FullName + ".taken", _emailProcessedFolderPath + file.Name + ".taken");
                    }

                    fileProcessedCount += 1;

                    //refresh the list if threshold OR batch size is reached
                    if (clashCount >= _clashThreshold || fileProcessedCount >= _batchSize)
                    {
                        fileList = directory.EnumerateFiles("*.eml").OrderBy(f => f.CreationTime).ToList();
                        _logger.Debug("Refreshed file list. clashCount:{0}, fileProcessedCount: {1}, fileList Count:{2}", clashCount, fileProcessedCount, fileList.Count);
                        clashCount = 0; //reset clash count
                        fileProcessedCount = 0; //reset processed file count
                    }

                }
                catch (IOException ioEx)
                {
                    //IO exception has occurred. Will most likely be due to a locked file
                    clashCount += 1;

                }
                catch (Exception ex)
                {
                    _logger.ErrorException("Error occurred while processing file.", ex);

                    //check if previous file error has happened again
                    if (PreviousFileError == currentFileProcessed)
                    {
                        FileErrorCount += 1;
                    }
                    else
                    {
                        FileErrorCount = 0;
                    }
                    PreviousFileError = currentFileProcessed;
                }
            }
        }

        private string GetEmailReroute(string toEmail)
        {
            if (toEmail.ToLower() == _withoutToken_FirstScenario_ToEmail.ToLower())
            {
                _logger.Debug("First scenario email");
                return _withoutToken_FirstScenario_RerouteEmail;
            }

            if (toEmail.ToLower() == _withoutToken_SecondScenario_ToEmail.ToLower())
            {
                _logger.Debug("Second scenario email");
                return _withoutToken_SecondScenario_RerouteEmail;
            }

            _logger.Debug("Rest of scenario email");
            return _withoutToken_RestOfScenario_RerouteEmail;
        }

        private bool SendEmail(CDO.Message msg, string fromEmailAddress, string toEmailAddress, string smtpServer)
        {
            try
            {
                _logger.Debug("Sending email | From: {0} | To: {1}", fromEmailAddress, toEmailAddress);
                MailMessage mail = new MailMessage(fromEmailAddress, toEmailAddress);
                SmtpClient client = new SmtpClient
                {
                    //DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory,
                    //PickupDirectoryLocation = @"c:\tmp\mail\",
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Host = smtpServer,
                    Port = 25
                };

                mail.Subject = msg.Subject;

                if (!string.IsNullOrEmpty(msg.HTMLBody))
                {
                    mail.Body = msg.HTMLBody;
                    mail.IsBodyHtml = true;
                }
                else
                {
                    mail.Body = msg.TextBody;
                    mail.IsBodyHtml = false;
                }

                if (msg.Attachments.Count > 0)
                {
                    foreach (CDO.IBodyPart attachment in msg.Attachments)
                    {
                        var streamString = attachment.GetDecodedContentStream().Read();
                        mail.Attachments.Add(new Attachment(new MemoryStream(streamString), attachment.FileName));
                    }
                }
                
                client.Send(mail);

                return true;
            }
            catch (Exception ex)
            {
                _logger.Debug(string.Format("Error occurred forwarding email to CSR mailbox.", ex));
                return false;
            }
        }

        /// <summary>
        /// Call the Avanade web service and pass the email message
        /// </summary>
        /// <param name="fileDetails"></param>
        /// <returns></returns>
        private bool PushFileToWebService(ADODB.Stream stream, CDO.Message msg, string emailToken)
        {
            try
            {
                var consumerWebServiceWrapper = Factory.CreateConsumerDataInterchangeInstance();
                
                // Very important to reset the stream position back to the beginning
                // otherwise the attempt to read the stream will just fetch a string of NULL bytes
                stream.Position = 0;
                IStream iStream = (IStream)stream;
                byte[] byteArray = new byte[stream.Size];
                IntPtr ptrCharsRead = IntPtr.Zero;
                iStream.Read(byteArray, (int)stream.Size, ptrCharsRead);

                var arg = new AttachMailResponseRequest
                {
                    EmailData = byteArray,
                    From = msg.From.Substring(msg.From.IndexOf("<") + 1, ((msg.From.IndexOf(">") - 1) - msg.From.IndexOf("<"))),
                    Subject = msg.Subject,
                    Token = emailToken
                };

                var toEmail = msg.To.Substring(msg.To.IndexOf("<") + 1, ((msg.To.IndexOf(">") - 1) - msg.To.IndexOf("<")));

                if (toEmail.ToLower().Contains("enquiry") || toEmail.ToLower().Contains("enquiries"))
                    arg.IsEnquiry = true;
                else
                    arg.IsEnquiry = false;

                arg.Body = msg.HTMLBody ?? msg.TextBody;

                var result = consumerWebServiceWrapper.SendAttachMailResponse(arg);

                if (result.IsSuccessful)
                {
                    return true;
                }
                else
                {
                    _logger.Error(result.Errors.Message);
                    if (result.Errors.FaultReasons != null)
                    {
                        foreach (var fault in result.Errors.FaultReasons)
                        {
                            _logger.Error(string.Format("{0}::{1}", fault.PropertyName, fault.Message));
                        }
                    }

                }

                if (!string.IsNullOrEmpty(result.Message))
                    _logger.Log(LogLevel.Info, result.Message);

                return false;

            }
            catch (Exception ex)
            {
                _logger.ErrorException("Error occurred while saving the file to EmailInQueue.", ex);
                return false;
            }
        }

        private string GetEmailToken(CDO.Message msg)
        {
            try
            {
                var addressContainingToken = msg.To.Substring(msg.To.IndexOf("<") + 1,
                    ((msg.To.IndexOf(">") - 1) - msg.To.IndexOf("<"))); //Or can be in msg.From
                var atIndex = addressContainingToken.IndexOf("@");
                var firstPeriodLeftOfAt = addressContainingToken.Substring(0, atIndex).LastIndexOf(".");

                if (firstPeriodLeftOfAt == -1)
                    return null;

                return addressContainingToken.Substring(firstPeriodLeftOfAt + 1, ((atIndex - 1) - firstPeriodLeftOfAt));
            }
            catch (Exception ex)
            {
                _logger.ErrorException("Error getting email token", ex);
                return null;
            }
        }

        //heartbeat functions

        public bool GreenCondition()
        {
            bool result = (FileErrorCount <= _greenThreshold &&
                            QueryFailCount <= _greenThreshold);

            return result;
        }

        public bool YellowCondition()
        {
            bool result = (FileErrorCount <= _yellowThreshold &&
                            QueryFailCount <= _yellowThreshold);

            return result;
        }

        public string CallerStatusString()
        {
            string result = string.Format("{0}|{1}", FileErrorCount, QueryFailCount);
            return result;
        }


        /// <summary>
        /// Call Primary Processing Loop Asynchronously
        /// </summary>
        public void StartAsync()
        {
            _logger.Debug("StartAsync started");
            new Thread(EmailInParserService.Run).Start(this);
            _logger.Debug("StartAsync ended");
        }

        /// <summary>
        /// Shutdown service
        /// </summary>
        public void Halt()
        {
            _logger.Debug("Halt started");
            if (this._service != null) this._service.Stop();
            _logger.Debug("Halt ended");
        }

        /// <summary>
        /// Halt Primary Processing Loop
        /// </summary>
        public void OnStop()
        {
            _logger.Debug("OnStop started");
            this._running = false;
            _logger.Debug("OnStop ended");
        }

        /// <summary>
        /// Internal ThreadStart Helper For Async Execution
        /// </summary>
        /// <param name="instance"></param>
        private static void Run(Object instance)
        {
            _logger.Debug("Run started");
            (instance as EmailInParserService).OnStart(null);
            _logger.Debug("Run ended");
        }
    }
}
