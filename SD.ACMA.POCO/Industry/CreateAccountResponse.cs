using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SD.ACMA.POCO.Base;

namespace SD.ACMA.POCO.Industry
{
    public class CreateAccountResponse : BaseWebServiceResponse
    {
        public int? AccountId { get; set; }
        public int? AccountUserId { get; set; }
        public bool IsAccountPendingApproval { get; set; }
        public bool IsDuplicateAccount { get; set; }
        public bool IsSuccessful { get; set; }
    }
}
