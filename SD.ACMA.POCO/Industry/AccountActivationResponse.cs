using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SD.ACMA.POCO.Industry
{
    public class AccountActivationResponse
    {
        public string Message { get; set; }
        public bool IsTokenValid { get; set; }
        public bool IsValidated { get; set; }
    }
}
