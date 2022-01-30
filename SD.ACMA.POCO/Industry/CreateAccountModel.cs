using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SD.ACMA.POCO.Industry
{
    public class CreateAccountModel
    {
        public AccountUserModel Account_User_Model { get; set; }
        public AccountModel Account_Model { get; set; }
        public WashingFormat WashFormat { get; set; }
    }

}
