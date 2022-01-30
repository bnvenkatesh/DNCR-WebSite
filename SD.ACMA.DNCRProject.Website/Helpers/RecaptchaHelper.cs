using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Script.Serialization;

namespace SD.ACMA.DNCRProject.Website.Helpers
{
    public static class RecaptchaHelper
    {
        public static bool ValidateRecaptcha(string secretKey)
        {
            if (ConfigurationManager.AppSettings["RecaptchaEnabled"] == "true")
            {
                string response = HttpContext.Current.Request["g-recaptcha-response"];
                bool valid = false;

                HttpWebRequest req = (HttpWebRequest)WebRequest.Create("https://www.google.com/recaptcha/api/siteverify?secret=" + secretKey + "&response=" + response);

                using (WebResponse wResponse = req.GetResponse())
                {
                    using (StreamReader stream = new StreamReader(wResponse.GetResponseStream()))
                    {
                        string json = stream.ReadToEnd();

                        JsonObject data = new JavaScriptSerializer().Deserialize<JsonObject>(json);

                        valid = Convert.ToBoolean(data.success);
                    }
                }

                return valid;
            }
            else
            {
                string response = HttpContext.Current.Request.Form["alternative-email"];
                return (!HttpContext.Current.Request.Form.AllKeys.Contains("alternative-email"))
                    || string.IsNullOrEmpty(response) || response == "Security check, do not replace this value";
            }
        }

        public class JsonObject
        {
            public string success { get; set; }
        }
    }
}