using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SD.ACMA.POCO.PetaPoco
{
    public class FinancialReport_ExceptionsReport
    {
        public List<FinancialReport_Exception> ExceptionUnmatchedAmounts { get; set; }
        public List<FinancialReport_Exception> ExceptionTransferredFromToCreditNotes { get; set; }
        public List<FinancialReport_Exception> ExceptionDeferredCreditCardReceipts { get; set; }

        public List<FinancialReport_Exception> PreviousExceptionUnmatchedAmounts { get; set; }
        public List<FinancialReport_Exception> PreviousExceptionTransferredFromToCreditNotes { get; set; }
        public List<FinancialReport_Exception> PreviousExceptionDeferredCreditCardReceipts { get; set; }

        public double TotalOutstandingExceptionBroughtForward_Sub4 { get; set; }
        public double TotalExceptionsTransferredFromCreditNotes { get; set; }
        public double TotalExceptionsTransferredToCreditNotes { get; set; }
        public double TotalExceptionsTransferredFromToCreditNotes_Sub5 { get; set; }
        public double TotalNewExceptions_Sub6 { get; set; }
        public double TotalExceptionsCleared_Sub7 { get; set; }
        public double TotalOutstandingExceptions { get; set; }

        public double TotalCreditNotesBroughtForward_B { get; set; }
        public double TotalCreditNotesAppliedToPurchasesToday_T { get; set; }
        public double TotalCreditNotesCreatedToday_N { get; set; }
        public double TotalCreditNotesRefundedToday_R { get; set; }
        public double TotalCreditNotes { get; set; }
    }

    public class FinancialReport_Exception
    {
        public int ID { get; set; }
        public DateTime DateReceived { get; set; }
        public string PaymentType { get; set; }
        public string Reference { get; set; }
        public int AccountID { get; set; }
        public string OrderNumber { get; set; }
        public double ExceptionAmount { get; set; }
        public double Cleared { get; set; }
        public string PaymentStatus { get; set; }
        public DateTime? SettlementDate { get; set; }
        public string TransactionType { get; set; }
        public int CreditNoteNumber { get; set; }
    }
}
