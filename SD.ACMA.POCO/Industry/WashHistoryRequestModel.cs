using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SD.ACMA.POCO.Industry
{
    public class WashHistoryRequestModel
    {
        public int AccountId { get; set; }
        public SD.ACMA.DNCR.Infrastructure.Enums.WashingChannelEnum? Channel { get; set; }
        public DateTime? EndDateTime { get; set; }
        public bool ExcludeWashWithOneNumber { get; set; }
        public DateTime? StartDateTime { get; set; }
        public int? WashReferenceId { get; set; }
    }
}
