using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SD.ACMA.POCO.Industry
{
    public class AccountAdjustmentModel
    {
        public SD.ACMA.DNCR.Infrastructure.Enums.AccountAdjustmentTypeEnum AccountAdjustmentType { get; set; }
        public int AccountAdjustmentTypeID { get; set; }
        public int AccountID { get; set; }
        public string AdjustmentNote { get; set; }
        public int AgentUserID { get; set; }
        public DateTime CreatedTimeStamp { get; set; }
        public string CreditNotes { get; set; }
        public int EnquiryID { get; set; }
        public string ExpiryDate { get; set; }
        public string RefundOrderID { get; set; }
        public string RefundSubscriptionID { get; set; }
        public string ReservedAccountBalance { get; set; }
        public string ReservedWashingCredits { get; set; }
        public string WashReferenceID { get; set; }
        public string WashingCredits { get; set; }
    }
}
