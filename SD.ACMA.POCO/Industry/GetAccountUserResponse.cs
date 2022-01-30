using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SD.ACMA.POCO.Base;

namespace SD.ACMA.POCO.Industry
{
    public class GetAccountUserResponse : BaseWebServiceResponse
    {
        public bool CanSeeWashQuote { get; set; }
        public string Department { get; set; }
        public string EmailAddress { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MobileNumber { get; set; }
        public string PhoneNumber { get; set; }
        public WashingFormat WashFormat { get; set; }
        public GenericResponseModel ResponseModel { get; set; }
    }
}
