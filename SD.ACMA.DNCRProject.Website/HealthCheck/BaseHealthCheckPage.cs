using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace SD.ACMA.DNCRProject.Website.HealthCheck
{
    public class BaseHealthCheckPage : System.Web.UI.Page
    {
   
        public string DoHealthCheck(BaseHealthCheck healthCheck)
        {
             var requestType = healthCheck.AuthorizeRequest(Request, Response);
            healthCheck.DisableCaching(Response);
            
            string result = string.Empty;
            var client = new WebClient();
            var host = String.Format("{0}://{1}", HttpContext.Current.Request.Url.Scheme, HttpContext.Current.Request.Url.Host);
            StringBuilder builder = new StringBuilder();

            if (requestType == BaseHealthCheck.RequestType.Check)
            {
                result = client.DownloadString(String.Format("{0}/umbraco/Surface/RegistrationSurface/CheckHealthCheck/{1}", host, Request["key"]));
            }
            else if(requestType == BaseHealthCheck.RequestType.Create)
            {
                result = client.DownloadString(String.Format("{0}/umbraco/Surface/RegistrationSurface/CreateHealthCheck/{1}", host, Request["key"]));
            }
            else if (requestType == BaseHealthCheck.RequestType.Update)
            {
                result = client.DownloadString(String.Format("{0}/umbraco/Surface/RegistrationSurface/UpdateHealthCheck/{1}", host, Request["key"]));
            }
            else if (requestType == BaseHealthCheck.RequestType.Remove)
            {
                result = client.DownloadString(String.Format("{0}/umbraco/Surface/RegistrationSurface/RemoveHealthCheck/{1}", host, Request["key"]));
            }
            else if (requestType == BaseHealthCheck.RequestType.QuickWash)
            {
                result = client.DownloadString(String.Format("{0}/umbraco/Surface/WashSurface/QuickWashHealthCheck/{1}", host, Request["key"]));
            }
            else if (requestType == BaseHealthCheck.RequestType.UploadList)
            {
                result = client.DownloadString(String.Format("{0}/umbraco/Surface/WashSurface/UploadListHealthCheck/{1}", host, Request["key"]));
            }
            else if (requestType == BaseHealthCheck.RequestType.SOAP)
            {
                result = CheckSOAPWash() ? "PASS" : "FAIL";
            }
            else
            {
                if (requestType == BaseHealthCheck.RequestType.Verbose)
                {
                    #region Useful stuff for debugging

                    string generalInfo = healthCheck.GetGeneralInfo(Request, Page.Server);
                    builder.Append(generalInfo);

                    #endregion
                }

                #region Health of Database

                builder.AppendLine("<h2>Database:</h2>");
                builder.AppendLine("<ul>");

                var messages = new List<string>();
                bool dbOk = healthCheck.CheckDatabase(ref messages);

                builder.AppendFormat("<li>CheckDatabase: {0}</li>\r\n", (dbOk) ? "PASS" : "FAIL");
                foreach (string message in messages)
                {
                    builder.AppendFormat("<li>{0}</li>\r\n", message);
                }

                builder.AppendLine("</ul>");

                #endregion
                    
                result = builder.ToString().Contains("FAIL") ? "FAIL" : "PASS";
            }

            if (requestType != BaseHealthCheck.RequestType.Verbose)
            {
                if (result != "PASS")
                {
                    result = "FAIL";
                    Response.StatusCode = 500;
                    Response.StatusDescription = "Healthcheck Failed";
                }
            }
            else result = builder.ToString();

            return result;
        }

        bool CheckSOAPWash()
        {
            var host = string.Format("{0}://{1}", HttpContext.Current.Request.Url.Scheme, HttpContext.Current.Request.Url.Host);
            var request = (HttpWebRequest)WebRequest.Create(string.Format("{0}/dncrtelem/rtw/washing.cfc", host));
           
            string postDataString = string.Format(washSOAPPayload, 
                ConfigurationManager.AppSettings["HealthCheck.AccountId"],
                ConfigurationManager.AppSettings["HealthCheck.AccountsSerId"],
                "NOPASSWORD",
                Guid.NewGuid(),
                ConfigurationManager.AppSettings["HealthCheck.NumberNotOnRegister"]
                );

            byte[] postData = Encoding.ASCII.GetBytes(postDataString);

            request.Method = "POST";
            request.ContentType = "text/xml";
            request.ContentLength = postData.Length;

            using (var stream = request.GetRequestStream())
            {
                stream.Write(postData, 0, postData.Length);
            }

            var response = (HttpWebResponse)request.GetResponse();

            return response != null && response.StatusCode == HttpStatusCode.OK;
        }

        readonly string washSOAPPayload = @"<SOAP-ENV:Envelope xmlns:SOAP-ENV=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:ns1=""http://rtw.dncrtelem"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:SOAP-ENC=""http://schemas.xmlsoap.org/soap/encoding/"" SOAP-ENV:encodingStyle=""http://schemas.xmlsoap.org/soap/encoding/"">
    <SOAP-ENV:Body>
        <ns1:WashNumbers>
         <TelemarketerId xsi:type=""xsd:string"">{0}</TelemarketerId>
         <WashOnlyUserId xsi:type=""xsd:string"">{1}</WashOnlyUserId>
         <TelemarketerPassword xsi:type=""xsd:string"">{2}</TelemarketerPassword>
         <ClientReferenceId xsi:type=""xsd:string"">{3}</ClientReferenceId>
         <NumbersToWash SOAP-ENC:arrayType=""xsd:ur-type[1]"" xsi:type=""ns1:ArrayOf_xsd_anyType""><Number xsi:type=""xsd:string"">{4}</Number></NumbersToWash>
        </ns1:WashNumbers>
    </SOAP-ENV:Body>
</SOAP-ENV:Envelope>";
    }
}
