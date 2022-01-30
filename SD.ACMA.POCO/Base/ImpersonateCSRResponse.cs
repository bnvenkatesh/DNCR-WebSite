using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SD.ACMA.POCO.Base
{
    public class ImpersonateCSRResponse : GenericResponseModel
    {
        public int AgentId { get; set; }
        public string DisplayName { get; set; }
        public string LoginName { get; set; }
        public bool IsAcma { get; set; }
    }
}
