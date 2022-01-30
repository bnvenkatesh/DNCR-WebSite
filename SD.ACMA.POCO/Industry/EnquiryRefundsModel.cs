using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SD.ACMA.POCO.Industry
{
    public class EnquiryRefundsModel
    {
        public int EnquiryID { get; set; }
        public SD.ACMA.DNCR.Infrastructure.Enums.RefundDecisionEnum RefundDecision { get; set; }
        public SD.ACMA.DNCR.Infrastructure.Enums.RefundTypeEnum RefundType { get; set; }
    }
}
