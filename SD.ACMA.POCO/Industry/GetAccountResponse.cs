using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SD.ACMA.POCO.Base;

namespace SD.ACMA.POCO.Industry
{
    public class GetAccountResponse : BaseWebServiceResponse
    {
        public AccountUserModel AdminModel { get; set; }
        public AccountModel Account_Model { get; set; }
        public WashingFormat WashFormat { get; set; }
    }
}
