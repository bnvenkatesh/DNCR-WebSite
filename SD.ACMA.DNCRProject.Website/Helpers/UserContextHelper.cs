using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SD.ACMA.InterfaceTier;
using SD.ACMA.POCO.Industry;

namespace SD.ACMA.DNCRProject.Website.Helpers
{
    public class UserContextHelper : IUserContextHelper
    {
        public UserContextModel CreateUserContextObject(int accountUserID, int? agentID)
        {
            return new UserContextModel {
                AccountUserID = accountUserID,
                AgentID = agentID != null ? agentID : null
            };
        }
    }
}