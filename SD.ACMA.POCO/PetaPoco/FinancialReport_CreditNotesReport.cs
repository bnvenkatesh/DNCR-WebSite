using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SD.ACMA.POCO.PetaPoco
{
    public class FinancialReport_CreditNotesReport
    {
        public List<FinancialReport_CreditNote> NewCreditNoteTransactions { get; set; }
        public List<FinancialReport_CreditNote> CreditNoteRegister { get; set; }

        public double TotalCreditNotesBroughtForward_B { get; set; }
        public double TotalCreditNotesAppliedToPurchasesToday_T { get; set; }
        public double TotalCreditNotesCreatedToday_N { get; set; }
        public double TotalCreditNotesRefundedToday_R { get; set; }
        public double TotalCreditNotes { get; set; }
    }

    public class FinancialReport_CreditNote
    {
        public DateTime DateReceived { get; set; }
        public string PaymentType { get; set; }        
        public int AccountID { get; set; }
        public int CreditNoteNumber { get; set; }
        public double CreditNoteAmount { get; set; }
        public double AccountBalance { get; set; }
        public string PaymentNote { get; set; }
        public string OrderNumber { get; set; }        
    }
}
