using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SD.ACMA.POCO.Base;

namespace SD.ACMA.POCO.Industry
{
    public class LoginResponse : BaseWebServiceResponse
    {
        public int AccountId { get; set; }
        public int AccountUserId { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsSuccessful { get; set; }
        public DateTime LastLogedInDateTime { get; set; }
        public string LoginMessage { get; set; }
        public bool IsLocked { get; set; }
        public bool IsMigrated { get; set; }
    }
}
