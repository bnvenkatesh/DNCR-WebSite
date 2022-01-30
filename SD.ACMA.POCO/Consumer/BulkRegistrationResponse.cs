using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SD.ACMA.POCO.Base;

namespace SD.ACMA.POCO.Consumer
{
    public class BulkRegistrationResponse : BaseWebServiceResponse
    {
        public int RegistrationRequestID { get; set; }
        public byte[] ErrorsInPhoneFile { get; set; }
        public byte[] ErrorsInFaxFile { get; set; }
        public bool HasErrorsInPhone { get; set; }
        public bool HasErrorsInFax { get; set; }
        public bool IsSuccessful { get; set; }

        public string PhoneErrorFileLocation { get; set; }
        public string FaxErrorFileLocation { get; set; }

        public string PhoneErrorFileName { get; set; }
        public string FaxErrorFileName { get; set; }
    }
}
