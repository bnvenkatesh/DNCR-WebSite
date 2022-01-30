using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SD.ACMA.POCO.Base;

namespace SD.ACMA.POCO.Consumer
{
    public class RegistrationResult : BaseWebServiceResponse
    {
        public bool HasEmailSent { get; set; }
        public int RegistrationRequestID { get; set; }
    }
}
