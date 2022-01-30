using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SD.ACMA.POCO.PetaPoco
{
    public class FinancialReport_PaymentsReconReport
    {
        public double CreditCardSettlements_SUM_A { get; set; }
        public double TotalCashReceipts_EFT_SUM_P1 { get; set; }
        public double TotalCashReceipts_BPAY_SUM_P2 { get; set; }
        public double TotalCashReceipts_UNKNOWN_SUM_P3 { get; set; }
        public double TotalCashReceipts_EFT_BPAY_SUM_P1_P2 { get; set; }
        public double TotalReceipts_Sub1 { get; set; }

        public List<FinancialReport_Payment> Exception_UnmatchedPayments_U1 { get; set; }
        public double TotalExceptionUnmatchedPaymentAmount_U1 { get; set; }

        public List<FinancialReport_Payment> Exception_TransferToCreditNotes_U2 { get; set; }
        public double TotalExceptionTransferToCreditNoteAmount_U2 { get; set; }

        public List<FinancialReport_Payment> Exception_Underpayments_U3 { get; set; }
        public double TotalExceptionUnderpaymentAmount_U3 { get; set; }

        public List<FinancialReport_Payment> Exception_ReversedDishonouredPayments_U4 { get; set; }
        public double TotalExceptionReversedDishonouredPaymentAmount_U4 { get; set; }

        public List<FinancialReport_Payment> Exception_ErroneousPayments_U5 { get; set; }
        public double TotalExceptionErroneousPaymentAmount_U5 { get; set; }

        public List<FinancialReport_Payment> Exception_CreditCardDeferredReceivedPayments_F { get; set; }
        public double TotalExceptionCreditCardDeferredReceivedPaymentAmount_F { get; set; }

        public double CreditCardTimingDifferenceBroughtForward { get; set; }

        public double CurrentPeriodTimingDifference
        {
            get { return CreditCardSettlements_SUM_A - Exception_CreditCardDeferredReceivedPayments_F.Sum(item => item.Amount); }
        }

        public double ClosingCreditCardTimingDifference
        {
            get { return CurrentPeriodTimingDifference + CreditCardTimingDifferenceBroughtForward; }
        }

        public double TotalDeductions_Sub2 { get; set; }

        public List<FinancialReport_Payment> NewCreditCardDeferralPayments_C { get; set; }
        public double TotalNewCreditCardDeferralPaymentAmount_C { get; set; }

        public List<FinancialReport_Payment> DishonouredPayments_D1 { get; set; }
        public double TotalDishonouredPayments_D1 { get; set; }

        public List<FinancialReport_Payment> CreditNotesAppliedToSalesPayments_D2 { get; set; }
        public double TotalCreditNotesAppliedToSalesPayments_D2 { get; set; }

        public List<FinancialReport_Payment> ClearedPreviousException_MatchedPayments_E1 { get; set; }
        public double TotalClearedPreviousException_MatchedPayments_E1 { get; set; }

        public List<FinancialReport_Payment> ClearedPreviousException_Underpayments_E2 { get; set; }
        public double TotalClearedPreviousException_Underpayments_E2 { get; set; }

        public double TotalClearedPreviousExceptionPayments_D3 { get; set; }

        public double TotalAdditions_Sub3 { get; set; }

        public double TotalMatchedReceipts_Z { get; set; }


        //public double TotalCreditCardActivationsPayments_S1 { get; set; }
        //public double TotalEFTActivationsPayments_S2 { get; set; }
        //public double TotalBPayActivationsPayments_S3 { get; set; }
        //public double TotalFundTransferActivationsPayments_S4 { get; set; }
        //public double TotalOtherActivationsPayments_S5 { get; set; }
        public IList<ActivatedSubscription> AllActivatedSubscriptions_S1toS5 { get; set; }
        public double TotalActivatedSubscriptionsPayments_S { get; set; }

        public double TotalReconciled { get; set; }

        public List<FinancialReport_Payment> RefundedPayments { get; set; }
    }

    public class FinancialReport_Payment
    {
        public int OrderTransactionID { get; set; }
        public string TransactionType { get; set; }
        public DateTime PaymentDate { get; set; }
        public string PaymentReference { get; set; }
        public int AccountID { get; set; }
        public double Amount { get; set; }
        public string OrderNumber { get; set; }
        public int CreditNoteNumber { get; set; }
        public DateTime? UnderPaymentClearedTimeStamp { get; set; }
    }

    public class ActivatedSubscription
    {
        public string OrderNumber { get; set; }
        public string SubscriptionName { get; set; }
        public string BusinessName { get; set; }
        public double Price { get; set; }
    }
}
