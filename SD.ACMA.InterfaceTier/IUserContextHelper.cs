using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SD.ACMA.POCO.Industry;

namespace SD.ACMA.InterfaceTier
{
    public interface IUserContextHelper
    {
        UserContextModel CreateUserContextObject(int accountUserID, int? agentID);
    }
}
