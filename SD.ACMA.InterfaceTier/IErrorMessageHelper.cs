using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SD.ACMA.POCO.Consumer;

namespace SD.ACMA.InterfaceTier
{
    public interface IErrorMessageHelper
    {
        string GenerateErrorMessage(WebServiceFault wsFault);

        List<string> GenerateErrorMessages(WebServiceFault wsFault);

        List<string> GenerateErrorMessages(params string[] errorMessages);
    }
}
