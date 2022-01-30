using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SD.ACMA.DNCR.Infrastructure;
using SD.ACMA.POCO.Base;

namespace SD.ACMA.POCO.Industry
{
    public class ImpersonateResponse : GenericResponseModel
    {
        public int AccountId { get; set; }
        public int AccountUserId { get; set; }
        public int AgentId { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsLocked { get; set; }
        public Enums.OrganisationEnum Organisation { get; set; }
        public Enums.OrganisationGroupEnum Role { get; set; }
        public Enums.AccountUserStatusEnum? AccountUserStatus { get; set; }
        public Enums.AccountStatusEnum? AccountStatus { get; set; }
    }
}
