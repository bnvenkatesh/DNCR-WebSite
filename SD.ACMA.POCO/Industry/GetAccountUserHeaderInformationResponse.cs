using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SD.ACMA.POCO.Base;

namespace SD.ACMA.POCO.Industry
{
    public class GetAccountUserHeaderInformationResponse : BaseWebServiceResponse
    {
        public int AccountID { get; set; }
        public string AccountUserEmailAddress { get; set; }
        public int AccountUserID { get; set; }
        public bool CanSeeWashQuote { get; set; }
        public string CompanyName { get; set; }
        public bool IsSuccess { get; set; }
        public int WashCredits50Count { get; set; }
        public int WashCredits80Count { get; set; }
        public int WashCreditsCount { get; set; }
        public int ReservedWashCreditsCount { get; set; }
        public DNCR.Infrastructure.Enums.AccountUserStatusEnum? AccountUserStatus { get; set; }
        public DNCR.Infrastructure.Enums.AccountStatusEnum? AccountStatus { get; set; }
    }
}
