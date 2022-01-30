using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SD.ACMA.InterfaceTier;
using SD.ACMA.POCO.Consumer;

namespace SD.ACMA.BusinessLogic.Helpers
{
    public class ErrorMessageHelper : IErrorMessageHelper
    {
        public string GenerateErrorMessage(WebServiceFault wsFault)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(wsFault.Message);

            if (wsFault.FaultReasons != null && wsFault.FaultReasons.Count > 0)
            {
                sb.Append("\n");

                foreach (var item in wsFault.FaultReasons)
                {
                    sb.Append(string.Format("{0}: {1}", item.PropertyName, item.Message));
                    sb.Append("\n");
                }
            }

            return sb.ToString();
        }

        public List<string> GenerateErrorMessages(WebServiceFault wsFault)
        {
            var errorMessages = new List<string>();
            errorMessages.Add(wsFault.Message);

            if (wsFault.FaultReasons != null && wsFault.FaultReasons.Count > 0)
            {
                foreach (var item in wsFault.FaultReasons)
                {
                    errorMessages.Add(string.Format("{0}: {1}", item.PropertyName, item.Message));
                }
            }

            return errorMessages;
        }

        public List<string> GenerateErrorMessages(params string[] errorMessages)
        {
            var errorMessagesList = new List<string>();

            foreach (var item in errorMessages)
            {
                errorMessagesList.Add(item);
            }

            return errorMessagesList;
        }
    }
}
