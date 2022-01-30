using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SD.ACMA.POCO.Industry
{
    public class AccountUserModel
    {
        public string EmailAddress { get; set; }
        public string FirstName { get; set; }        
        public string LastName { get; set; }
        public string MobileNumber { get; set; }
        public string PhoneNumber { get; set; }
        public bool CanSeeWashQuote { get; set; }
        public int AccountUserId { get; set; }
        public string Department { get; set; }
    }
}
