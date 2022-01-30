using Microsoft.Practices.Unity;
using NLog;
using SD.ACMA.BusinessLogic.PaymentGateway;
using SD.ACMA.DatabaseIntermediary;
using SD.ACMA.DNCR.Infrastructure;
using SD.ACMA.DNCR.Infrastructure.Configuration;
using SD.ACMA.InterfaceTier;
using SD.ACMA.POCO.Industry;
using SD.Core.Data.Repository.PetaPoco.Business;
using SD.Core.Data.Repository.PetaPoco.UnityOfWorkContainer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace SD.ACMA.DNCRProject.CreditCardReconciliationService
{
    public partial class CreditCardReconciliationService : ServiceBase
    {
        private Timer _serviceTimer = new Timer();
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        public CreditCardReconciliationService()
        {
            InitializeComponent();

            Factory.Instance.RegisterContainers();
        }

        public void OnDebug()
        {
            try
            {
                Task[] tasks = new Task[2];
                tasks[0] = Task.Factory.StartNew(() => ProcessPendingCreditCardPayments());
                tasks[1] = Task.Factory.StartNew(() => ProcessPendingUpdatable());

                Task.WaitAll(tasks);
            }
            catch (Exception ex)
            {
                WriteToEventViewer(string.Format("ErrorMessage: {0}. Stack Trace: {1}", ex.Message, ex.StackTrace));
            }
        }

        protected override void OnStart(string[] args)
        {
            int loopInterval = ConfigurationHelper.Instance.LoopInterval;

            _serviceTimer.Elapsed += new ElapsedEventHandler(serviceTimer_Elapsed);
            _serviceTimer.Interval = loopInterval;
            _serviceTimer.Enabled = true;
            _serviceTimer.Start();
        }

        protected override void OnStop()
        {
            _serviceTimer.Enabled = false;
        }

        private void serviceTimer_Elapsed(object sender, EventArgs e)
        {
            _serviceTimer.Stop();

            try
            {
                Task[] tasks = new Task[2];
                tasks[0] = Task.Factory.StartNew(() => ProcessPendingCreditCardPayments());
                tasks[1] = Task.Factory.StartNew(() => ProcessPendingUpdatable());

                Task.WaitAll(tasks);
            }
            catch (Exception ex)
            {
                WriteToEventViewer(string.Format("ErrorMessage: {0}. Stack Trace: {1}", ex.Message, ex.StackTrace));
            }
            finally
            {
                _serviceTimer.Start();
            }
        }

        public void ProcessPendingCreditCardPayments()
        {
            try
            {
                ICreditCardPaymentService _creditCardPaymentService = Factory.Instance.TaskContainer1.Resolve<ICreditCardPaymentService>();
                IPaymentGatewayService _paymentGatewayService = Factory.Instance.TaskContainer1.Resolve<IPaymentGatewayService>();

                string receipt = null;
                string response = null;
                string message = null;
                string receiptNumber = null;
                DateTime? settlementDate = null;
                Enums.PaymentStatusEnum paymentStatus = Enums.PaymentStatusEnum.UNKNOWN;
                string creditCardReference = null;
                long? transactionNo = null;
                int? transactionAmountInCents = null;
                string authorizeId = null;
                string transactionType = null;
                string cardType = null;
                int? orderId = null;

                TimeSpan dateDiff;

                var pendingCreditCardPayments = _creditCardPaymentService.GetPendingPayments();

                foreach (var item in pendingCreditCardPayments)
                {
                    _paymentGatewayService.QueryPayment(item.TransactionId, out receipt, out paymentStatus, out response, out message, out creditCardReference, out receiptNumber, out settlementDate, out transactionNo, out transactionAmountInCents, out authorizeId, out transactionType, out cardType);

                    if (paymentStatus == Enums.PaymentStatusEnum.UNKNOWN)
                    {
                        dateDiff = DateTime.Now - item.CreatedAt;

                        if (dateDiff.TotalHours > ConfigurationHelper.Instance.PendingPaymentTimeOutHours)
                        {
                            paymentStatus = Enums.PaymentStatusEnum.TIMEOUT;
                        }
                    }

                    _creditCardPaymentService.UpdateStatus(item.TransactionId, receipt, response, message, receiptNumber, settlementDate, paymentStatus, creditCardReference, transactionNo, transactionAmountInCents, authorizeId, transactionType, cardType, out orderId);
                }
            }
            catch (Exception ex)
            {
                WriteToEventViewer(string.Format("ErrorMessage: {0}. Stack Trace: {1}", ex.Message, ex.StackTrace));
            }
        }

        public void ProcessPendingUpdatable()
        {
            try
            {
                ICreditCardPaymentService _creditCardPaymentService = Factory.Instance.TaskContainer2.Resolve<ICreditCardPaymentService>();
                IIndustryDataInterchange _industryDataInterchange = Factory.Instance.TaskContainer2.Resolve<IIndustryDataInterchange>();

                var suppressionPeriodStart = DateTime.Now.AddSeconds(-1 * ConfigurationHelper.Instance.SuppressUpdatableCheckAfterCreationPeriod);
                var pendingUpdatable = _creditCardPaymentService.GetUpdatable().Where(ccp => ccp.CreatedAt < suppressionPeriodStart);

                foreach (var item in pendingUpdatable)
                {
                    if (string.IsNullOrEmpty(item.TransactionStatus))
                        item.TransactionStatus = Enums.PaymentStatusEnum.UNKNOWN.ToString();

                    switch ((Enums.PaymentStatusEnum)Enum.Parse(typeof(Enums.PaymentStatusEnum), item.TransactionStatus))
                    {
                        case Enums.PaymentStatusEnum.SUCCESS:
                            PayForOrderArguments payForOrder_SUCCESS = new PayForOrderArguments()
                            {
                                AccountUserID = item.AccountId,
                                Amount = item.TransactionAmountInCents.HasValue ? item.TransactionAmountInCents.Value / 100 : 0,
                                AuthorizeID = item.AuthorizeID,
                                CardType = item.CardType,
                                CreditCardReference = item.CreditCardReference,
                                OrderID = item.OrderId,
                                ReceiptNo = item.ReceiptNo,
                                ResponseCode = item.ResponseCode,
                                TransactionNo = item.TransactionNo.HasValue ? item.TransactionNo.Value : 0,
                                TransactionID = item.TransactionId,
                                TransactionType = item.TransactionType,
                                ExpectedSettelmentDate = item.SettlementDate,
                                AgentUserID = null,
                                IsCSR = false,
                                ExternalMessage = item.Message
                            };

                            var response_SUCCESS = _industryDataInterchange.PayForOrder(payForOrder_SUCCESS);

                            if (response_SUCCESS != null)
                            {
                                // Mark the payment as processed if response is success or if response is not success and was declined by the portal service: e.g. Payment can not be processed for Cancelled/Closed/Expired/Refunded/PartiallyRefunded or FullyPaid Orders!
                                if (response_SUCCESS.IsSuccessful || (!response_SUCCESS.IsSuccessful && response_SUCCESS.Errors != null))
                                    _creditCardPaymentService.SetTransactionAsProcessed(item.TransactionId, response_SUCCESS.IsSuccessful, response_SUCCESS.Errors != null ? response_SUCCESS.Errors.Message : null);
                            }
                            break;
                        case Enums.PaymentStatusEnum.DECLINED:
                            PayForOrderArguments payForOrder_DECLINED = new PayForOrderArguments()
                            {
                                AccountUserID = item.AccountId,
                                Amount = item.TransactionAmountInCents.HasValue ? item.TransactionAmountInCents.Value / 100 : 0,
                                AuthorizeID = item.AuthorizeID,
                                CardType = item.CardType,
                                CreditCardReference = item.CreditCardReference,
                                OrderID = item.OrderId,
                                ReceiptNo = item.ReceiptNo,
                                ResponseCode = item.ResponseCode,
                                TransactionNo = item.TransactionNo.HasValue ? item.TransactionNo.Value : 0,
                                TransactionID = item.TransactionId,
                                TransactionType = item.TransactionType,
                                ExpectedSettelmentDate = item.SettlementDate,
                                AgentUserID = null,
                                IsCSR = false,
                                ExternalMessage = item.Message
                            };

                            var response_DECLINED = _industryDataInterchange.RecordPayForOrderFail(payForOrder_DECLINED);

                            if (response_DECLINED != null)
                            {
                                // Mark the payment as processed if response is success or if response is not success and was declined by the portal service: e.g. Payment can not be processed for Cancelled/Closed/Expired/Refunded/PartiallyRefunded or FullyPaid Orders!
                                if (response_DECLINED.IsSuccessful || (!response_DECLINED.IsSuccessful && response_DECLINED.Errors != null))
                                    _creditCardPaymentService.SetTransactionAsProcessed(item.TransactionId, response_DECLINED.IsSuccessful, response_DECLINED.Errors != null ? response_DECLINED.Errors.Message : null);
                            }
                            break;
                        case Enums.PaymentStatusEnum.TIMEOUT:
                            _creditCardPaymentService.SetTransactionAsProcessed(item.TransactionId, null, null);
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                WriteToEventViewer(string.Format("ErrorMessage: {0}. Stack Trace: {1}", ex.Message, ex.StackTrace));
            }
        }

        public void WriteToEventViewer(string message)
        {
            _logger.Error(message);

            //string cs = "SD.ACMA.DNCRProject.CreditCardReconciliationService";

            //EventLog elog = new EventLog();

            //if (!EventLog.SourceExists(cs))
            //{
            //    EventLog.CreateEventSource(cs, cs);
            //}

            //elog.Source = cs;
            //elog.EnableRaisingEvents = true;
            //elog.WriteEntry(message);
        }
    }
}
