using System;
namespace SD.ACMA.POCO.Industry
{
    public class AccountRefundsModel
    {
        public int EnquiryID { get; set; }
        public int EnquiryRefundID { get; set; }
        public DNCR.Infrastructure.Enums.EnquryStatusEnum EnquiryStatus { get; set; }
        public DateTime? LodgedTimeStamp { get; set; }
        public int? OrderID { get; set; }
        public DNCR.Infrastructure.Enums.RefundDecisionEnum RefundDecision { get; set; }
        public DNCR.Infrastructure.Enums.RefundTypeEnum RefundType { get; set; }
        public int? SubscriptionID { get; set; }
        public int? WashCreditsRolloverTrasactionID { get; set; }
        public int? WashTransactionID { get; set; }
    }
}
