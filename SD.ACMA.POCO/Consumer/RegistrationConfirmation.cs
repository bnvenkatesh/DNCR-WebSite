using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SD.ACMA.POCO.Base;

namespace SD.ACMA.POCO.Consumer
{
    public class RegistrationConfirmation : BaseWebServiceResponse
    {
        public bool IsSuccessful { get; set; }
        public bool IsTokenConsumed { get; set; }
        public bool IsTokenExpired { get; set; }
        public int RegisteredNumberCount { get; set; }
    }
}
