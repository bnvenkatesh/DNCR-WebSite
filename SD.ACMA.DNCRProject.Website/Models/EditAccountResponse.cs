using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SD.ACMA.POCO.Base;

namespace SD.ACMA.DNCRProject.Website.Models
{
    public class EditAccountResponse
    {
        public GenericResponseModel UpdateAccountResult { get; set; }
        public GenericResponseModel UpdateUserResult { get; set; }
        public string ErrorMessage { get; set; }
    }
}