using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Data.Odbc;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using System.Web;

namespace SD.ACMA.DNCRProject.Website.HealthCheck
{
    public class BaseHealthCheck
    {
        private string _key = ConfigurationManager.AppSettings["Healthcheck.Key"] ?? "amIOnline";
        private string _verboseKey = ConfigurationManager.AppSettings["Healthcheck.Verbose.Key"] ?? "giveMeTheDetails";
        private string _checkKey = ConfigurationManager.AppSettings["HealthCheck.Methods.CheckKey"] ?? "pleaseCheckMe";
        private string _createKey = ConfigurationManager.AppSettings["HealthCheck.Methods.CreateKey"] ?? "pleaseAddMe";
        private string _updateKey = ConfigurationManager.AppSettings["HealthCheck.Methods.UpdateKey"] ?? "pleaseUpdateMe";
        private string _removeKey = ConfigurationManager.AppSettings["HealthCheck.Methods.RemoveKey"] ?? "pleaseRemoveMe";
        private string _quickWashKey = ConfigurationManager.AppSettings["HealthCheck.Methods.QuickWashKey"] ?? "pleaseWashMe";
        private string  _uploadListKey = ConfigurationManager.AppSettings["HealthCheck.Methods.UploadListKey"] ?? "pleaseUploadMe";
        private string _connectionstringsToIgnore = ConfigurationManager.AppSettings["HealthCheck.ConnectionStrings.Ignore"] ?? string.Empty;
        private string _soapKey = ConfigurationManager.AppSettings["HealthCheck.Methods.SoapKey"] ?? "pleaseSoapMe";
        private List<string> ConnectionStringsToIgnore
        {
            get
            {
                List<string> value = new List<string>();

                if (!string.IsNullOrEmpty(_connectionstringsToIgnore))
                {
                    value = _connectionstringsToIgnore.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
                }

                return value;
            }
        }


        public RequestType AuthorizeRequest(HttpRequest request, HttpResponse response)
        {
            string key = request["key"];

            if (key == _key)
            {
                return RequestType.DbCheck;
            }
            if (key == _verboseKey)
            {
                return RequestType.Verbose;
            }
            if (key == _checkKey)
            {
                return RequestType.Check;
            }
            if (key == _createKey)
            {
                return RequestType.Create;
            }
            if (key == _updateKey)
            {
                return RequestType.Update;
            }
            if (key == _removeKey)
            {
                return RequestType.Remove;
            }
            if (key == _quickWashKey)
            {
                return RequestType.QuickWash;
            }
            if (key == _uploadListKey)
            {
                return RequestType.UploadList;
            }
            if (key== _soapKey)
            {
                return RequestType.SOAP;
            }

            response.StatusCode = 403;
            response.StatusDescription = "Not On My Watch";
            response.End();
            return RequestType.Invalid;
        }

        public void DisableCaching(HttpResponse response)
        {
            response.DisableKernelCache();
            response.Expires = -1;
            response.CacheControl = "no-cache";
            response.Cache.SetCacheability(HttpCacheability.NoCache);
            response.Cache.SetNoServerCaching();
        }

        

        public string GetGeneralInfo(HttpRequest request, HttpServerUtility server)
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendFormat("<h1>Healthcheck: {0}</h1>\r\n", server.MachineName);
            builder.AppendFormat("<p>Type: {0}</p>\r\n", this.GetType().Name);

            builder.AppendLine("<h2>Headers:</h2>");
            builder.AppendLine("<ul>");
            foreach (String key in request.Headers.AllKeys)
                builder.AppendFormat("<li>{0}: {1}</li>\r\n", key, request.Headers[key]);
            builder.AppendLine("</ul>");

            builder.AppendLine("<h2>Server:</h2>");
            builder.AppendLine("<ul>");
            foreach (String key in request.ServerVariables.AllKeys)
                builder.AppendFormat("<li>{0}: {1}</li>\r\n", key, request.ServerVariables[key]);
            builder.AppendLine("</ul>");

            builder.AppendLine("<h2>Cookies:</h2>");
            builder.AppendLine("<ul>");
            foreach (String key in request.Cookies.AllKeys)
                builder.AppendFormat("<li>{0}: {1}</li>\r\n", key, request.Cookies[key].Value);
            builder.AppendLine("</ul>");

            builder.AppendLine("<h2>Form:</h2>");
            builder.AppendLine("<ul>");
            foreach (String key in request.Form.AllKeys)
                builder.AppendFormat("<li>{0}: {1}</li>\r\n", key, request.Form[key]);
            builder.AppendLine("</ul>");

            builder.AppendLine("<h2>Query:</h2>");
            builder.AppendLine("<ul>");
            foreach (String key in request.QueryString.AllKeys)
                builder.AppendFormat("<li>{0}: {1}</li>\r\n", key, request.QueryString[key]);
            builder.AppendLine("</ul>");

            builder.AppendLine("<h2>App Settings:</h2>");
            builder.AppendLine("<ul>");
            foreach (String key in ConfigurationManager.AppSettings.AllKeys)
                builder.AppendFormat("<li>{0}: {1}</li>\r\n", key, ConfigurationManager.AppSettings[key]);
            builder.AppendLine("</ul>");

            builder.AppendLine("<h2>Connection Strings:</h2>");
            builder.AppendLine("<ul>");
            foreach (ConnectionStringSettings cs in ConfigurationManager.ConnectionStrings)
                builder.AppendFormat("<li>{0}</li>\r\n", cs.Name);
            builder.AppendLine("</ul>");

            builder.AppendLine("<h2>Request:</h2>");
            builder.AppendLine("<ul>");
            builder.AppendFormat("<li>Scheme: {0}</li>\r\n", request.Url.Scheme);
            builder.AppendFormat("<li>Host: {0}</li>\r\n", request.Url.Host);
            builder.AppendFormat("<li>Port: {0}</li>\r\n", request.Url.Port);
            builder.AppendFormat("<li>Path: {0}</li>\r\n", request.Url.LocalPath);
            builder.AppendFormat("<li>Query: {0}</li>\r\n", request.Url.Query);
            builder.AppendFormat("<li>Fragment: {0}</li>\r\n", request.Url.Fragment);
            builder.AppendLine("</ul>");

            builder.AppendLine("<h2>Misc:</h2>");
            builder.AppendLine("<ul>");
            builder.AppendFormat("<li>Assembly: {0}</li>\r\n",  this.GetType().Assembly.FullName);
            builder.AppendFormat("<li>App Pool User: {0}</li>\r\n", WindowsIdentity.GetCurrent().Name);
            builder.AppendFormat("<li>Type: {0}</li>\r\n", request.RequestType);
            builder.AppendFormat("<li>UserIP: {0}</li>\r\n", request.UserHostAddress);
            builder.AppendLine("</ul>");

            return builder.ToString();
        }

        public bool CheckDatabase(ref List<string> messages)
        {
            bool ret = true;

            var uniqueConnections = new Dictionary<string, List<string>>();

            for (var i = 0; i < ConfigurationManager.ConnectionStrings.Count; i++)
            {
                if (!uniqueConnections.ContainsKey(ConfigurationManager.ConnectionStrings[i].ConnectionString))
                {
                    uniqueConnections.Add(ConfigurationManager.ConnectionStrings[i].ConnectionString, new List<string>());
                    uniqueConnections[ConfigurationManager.ConnectionStrings[i].ConnectionString].Add(ConfigurationManager.ConnectionStrings[i].Name);
                }
                else
                {
                    uniqueConnections[ConfigurationManager.ConnectionStrings[i].ConnectionString].Add(ConfigurationManager.ConnectionStrings[i].Name);
                }
            }

            messages.Add(string.Format("Conn strings to ignore: {0}", _connectionstringsToIgnore));

            

            foreach (var entry in uniqueConnections)
            {
                if (ConnectionStringsToIgnore.Intersect(entry.Value).Any())
                {
                    messages.Add(string.Format("IGNORING: {0}", string.Join(",", entry.Value.ToArray())));
                    continue;
                }

                string exceptionMessage = string.Empty;
                DbConnection conn;
                //special case to handle legacy ODBC conn strings
                if (entry.Key.StartsWith("Driver={SQL Server}"))
                {
                    conn = new OdbcConnection(entry.Key);
                }
                else
                {
                    conn = new SqlConnection(entry.Key);
                }

                var probeStartTimeStamp = DateTime.Now;
                var dbAlive = ProbeDatabase(conn, ref exceptionMessage);
                var probeEndTimeStamp = DateTime.Now;
                var connectionNames = new StringBuilder();
                for (var i = 0; i < entry.Value.Count; i++)
                {
                    connectionNames.AppendFormat("{0}{1}", entry.Value[i], i == entry.Value.Count - 1 ? "" : ", ");
                }

                messages.Add(string.Format("Start Probing Time: {0}", probeStartTimeStamp.ToString("o")));
                messages.Add(string.Format("Probing: {0} : {1} [{2}] ... {3}", connectionNames, conn.DataSource, conn.Database, dbAlive ? "ok" : "error"));
                if (!dbAlive && !string.IsNullOrEmpty(exceptionMessage))
                {
                    messages.Add(string.Format("Exception Message: {0}", exceptionMessage));
                }
                messages.Add(string.Format("End Probing Time: {0}", probeEndTimeStamp.ToString("o")));

                ret = ret && dbAlive;
            }
            return ret;
        }

        #region Databases

        private static bool ProbeDatabase(DbConnection conn, ref string exceptionMessage)
        {
            exceptionMessage = string.Empty;
            var ret = false;
            DbDataReader reader = null;
            try
            {
                conn.Open();
                DbCommand command = null;
                
                if (conn is SqlConnection)
                {
                    command = new SqlCommand("SELECT GETDATE()", (SqlConnection)conn);
                }
                else if (conn is OdbcConnection)
                {
                    command = new OdbcCommand("SELECT GETDATE()", (OdbcConnection)conn);
                }
                
                if (command != null)
                {
                    command.CommandType = System.Data.CommandType.Text;
                    reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        ret = true;
                    }
                }
            }
            catch (Exception ex)
            {
                exceptionMessage = ex.Message;
            }
            finally
            {
                if (reader != null)
                {
                    if (!reader.IsClosed)
                    {
                        reader.Close();
                    }
                }
                conn.Close();

            }
            return ret;
        }

        #endregion

        public enum RequestType
        {
            Invalid,
            DbCheck,
            Verbose,
            Check,
            Create,
            Update,
            Remove,
            QuickWash,
            UploadList,
            SOAP
        }
    }
}
