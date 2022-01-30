using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SD.ACMA.DNCR.Infrastructure;

namespace SD.ACMA.POCO.Industry
{
    public class OnlineRefundRequestModel
    {
        public int AccountID { get; set; }
        public string BankDetailsAccountName { get; set; }
        public string BankDetailsAccountNumber { get; set; }
        public string BankDetailsBSB { get; set; }
        public string BankDetailsOther { get; set; }
        public string Description { get; set; }
        public int? DishonouredPaymentRefundID { get; set; }
        public string EnquiryRefundPaymentType { get; set; }
        public Enums.RefundTypeEnum EnquiryRefundType { get; set; }
        public decimal? RefundAcmaIncrementAccountBalance { get; set; } // Account Balance Adjustment
        public int? RefundAcmaIncrementWashCredits { get; set; } // Wash Numbers Adjustment
        public decimal? RefundAmountFromSubscription { get; set; }
        public int? RefundOrderID { get; set; } // Refund Subscription - Order Id 
        public decimal? RefundReservedAccountBalance { get; set; } // Refund Account Balance
        public int? RefundReservedWashCredits { get; set; } // Refund Subscription Reserved Wash numbers
        public int? RefundSubscriptionID { get; set; } // Refund Subcription - OrderLine Id
        public int? RefundWashCreditsRemainingFromSubscription { get; set; } 
        public int? RefundWashTransactionID { get; set; } // Reversal Wash number
        public int? WashCreditsRolloverAmount { get; set; } // Extension/Rollover expiry
        public int? WashCreditsRolloverTransactionID { get; set; } // Extension/Rollover expiry
    }
}
