using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SD.ACMA.DNCRProject.FinancialReports.Models
{
    public class ReportViewModel
    {
        public DateTime? FromDate { get; set; }
        public DateTime ToDate { get; set; }

        public string FromDateString { get; set; }
        public string ToDateString { get; set; }

        public List<ReportViewModel_FinancialReport> HistoricalFinancialReports { get; set; }

        public bool IsCurrent { get; set; }
    }

    public class ReportViewModel_FinancialReport
    {
        public int ReportPeriodID { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string FromDateString { get; set; }
        public string ToDateString { get; set; }
        public string MarkedFinalBy { get; set; }
        public string Status { get; set; }
    }

    public class PaymentReconReportViewModel
    {
        public string FromDateString { get; set; }
        public string ToDateString { get; set; }
        public bool IsCurrent { get; set; }

        public string CreditCardSettlements_SUM_A { get; set; }
        public string TotalCashReceipts_EFT_SUM_P1 { get; set; }
        public string TotalCashReceipts_BPAY_SUM_P2 { get; set; }
        public string TotalCashReceipts_UNKNOWN_SUM_P3 { get; set; }
        public string TotalCashReceipts_EFT_BPAY_SUM_P1_P2 { get; set; }
        public string TotalReceipts_Sub1 { get; set; }

        public List<PaymentReconReportViewModel_Payment> ExceptionUnmatchedPayments_U1 { get; set; }
        public string TotalExceptionUnmatchedPaymentAmount_U1 { get; set; }

        public List<PaymentReconReportViewModel_Payment> ExceptionTransferToCreditNotes_U2 { get; set; }
        public string TotalExceptionTransferToCreditNoteAmount_U2 { get; set; }

        public List<PaymentReconReportViewModel_Payment> ExceptionUnderpayments_U3 { get; set; }
        public string TotalExceptionUnderpaymentAmount_U3 { get; set; }

        public List<PaymentReconReportViewModel_Payment> ExceptionReversedDishonouredPayments_U4 { get; set; }
        public string TotalExceptionReversedDishonouredPaymentAmount_U4 { get; set; }

        public List<PaymentReconReportViewModel_Payment> ExceptionErroneousPayments_U5 { get; set; }
        public string TotalExceptionErroneousPaymentAmount_U5 { get; set; }

        public List<PaymentReconReportViewModel_Payment> ExceptionCreditCardDeferredReceivedPayments_F { get; set; }
        public string TotalExceptionCreditCardDeferredReceivedPaymentAmount_F { get; set; }

        public string TotalDeductions_Sub2 { get; set; }

        public List<PaymentReconReportViewModel_Payment> NewCreditCardDeferralPayments_C { get; set; }
        public string TotalNewCreditCardDeferralPaymentAmount_C { get; set; }

        public List<PaymentReconReportViewModel_Payment> DishonouredPayments_D1 { get; set; }
        public string TotalDishonouredPayments_D1 { get; set; }

        public List<PaymentReconReportViewModel_Payment> CreditNotesAppliedToSalesPayments_D2 { get; set; }
        public string TotalCreditNotesAppliedToSalesPayments_D2 { get; set; }

        public List<PaymentReconReportViewModel_Payment> ClearedPreviousException_MatchedPayments_E1 { get; set; }
        public string TotalClearedPreviousException_MatchedPayments_E1 { get; set; }

        public List<PaymentReconReportViewModel_Payment> ClearedPreviousException_Underpayments_E2 { get; set; }
        public string TotalClearedPreviousException_Underpayments_E2 { get; set; }

        public List<PaymentReconReportViewModel_Payment> ClearedPreviousExceptionPayments_D3 { get; set; }
        public string TotalClearedPreviousExceptionPayments_D3 { get; set; }

        public string TotalAdditions_Sub3 { get; set; }

        public string TotalMatchedReceipts_Z { get; set; }

        public string TotalCreditCardActivationsPayments_S1 { get; set; }
        public string TotalEFTActivationsPayments_S2 { get; set; }
        public string TotalBPayActivationsPayments_S3 { get; set; }
        public string TotalFundTransferActivationsPayments_S4 { get; set; }
        public string TotalOtherActivationsPayments_S5 { get; set; }

        public IList<ActivatedSubscription> AllActivatedSubscriptions_S1to5 { get; set; }
        public string TotalActivatedSubscriptionsPayments_S { get; set; }

        public double TotalReconciled { get; set; }

        public List<PaymentReconReportViewModel_Payment> RefundedPayments { get; set; }

        public string CreditCardTimingDifferenceBroughtForward { get; set; }
        public string CurrentPeriodTimingDifference { get; set; }
        public string ClosingCreditCardTimingDifference { get; set; }
    
    }

    public class ActivatedSubscription
    {
        public string Description { get; set; }
        public string Amount { get; set; }
    }

    public class PaymentReconReportViewModel_Payment
    {
        public string PaymentDate { get; set; }
        public string PaymentReference { get; set; }
        public int AccountID { get; set; }
        public string Amount { get; set; }
    }

    public class ExceptionReportViewModel
    {
        public string FromDateString { get; set; }
        public string ToDateString { get; set; }
        public bool IsCurrent { get; set; }

        public List<ExceptionReportViewModel_Exception> ExceptionUnmatchedAmounts { get; set; }
        public List<ExceptionReportViewModel_Exception> ExceptionTransferredFromToCreditNotes { get; set; }
        public List<ExceptionReportViewModel_Exception> ExceptionDeferredCreditCardReceipts { get; set; }

        public string TotalOutstandingExceptionBroughtForward_Sub4 { get; set; }
        public string TotalExceptionsTransferredFromCreditNotes { get; set; }
        public string TotalExceptionsTransferredToCreditNotes { get; set; }
        public string TotalExceptionsTransferredFromToCreditNotes_Sub5 { get; set; }
        public string TotalNewExceptions_Sub6 { get; set; }
        public string TotalExceptionsCleared_Sub7 { get; set; }
        public string TotalOutstandingExceptions { get; set; }

        public string TotalCreditNotesBroughtForward_B { get; set; }
        public string TotalCreditNotesAppliedToPurchasesToday_T { get; set; }
        public string TotalCreditNotesCreatedToday_N { get; set; }
        public string TotalCreditNotesRefundedToday_R { get; set; }
        public string TotalCreditNotes { get; set; }
    }

    public class ExceptionReportViewModel_Exception
    {
        public string DateReceived { get; set; }
        public string PaymentType { get; set; }
        public string Reference { get; set; }
        public int AccountID { get; set; }
        public string OrderNumber { get; set; }
        public string ExceptionAmount { get; set; }
        public string Cleared { get; set; }
        public string PaymentStatus { get; set; }
    }

    public class CreditNoteReportViewModel
    {
        public string FromDateString { get; set; }
        public string ToDateString { get; set; }
        public bool IsCurrent { get; set; }

        public List<CreditNoteReportViewModel_CreditNote> NewCreditNoteTransactions { get; set; }

        public string TotalCreditNotesBroughtForward_B { get; set; }
        public string TotalCreditNotesAppliedToPurchasesToday_T { get; set; }
        public string TotalCreditNotesCreatedToday_N { get; set; }
        public string TotalCreditNotesRefundedToday_R { get; set; }
        public string TotalCreditNotes { get; set; }

        public List<CreditNoteReportViewModel_CreditNote> CreditNoteRegister { get; set; }
    }

    public class CreditNoteReportViewModel_CreditNote
    {
        public string DateReceived { get; set; }
        public string PaymentType { get; set; }
        public int AccountID { get; set; }
        public string CreditNoteNumber { get; set; }
        public string CreditNoteAmount { get; set; }
        public string AccountBalance { get; set; }
        public string PaymentNote { get; set; }
        public string OrderNumber { get; set; }
    }
}