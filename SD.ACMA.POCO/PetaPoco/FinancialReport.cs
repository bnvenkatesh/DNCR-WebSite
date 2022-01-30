using PetaPoco;
using SD.ACMA.DNCR.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SD.ACMA.POCO.PetaPoco
{
    [TableName("FinancialReport")]
    [PrimaryKey("Id")]
    public class FinancialReport
    {
        public int Id { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public double TotalOutstandingExceptionsAmount { get; set; }
        public double TotalCreditNotesAmount { get; set; }
        public double CreditCardTimingDifference { get; set; }
        public string Status { get; set; }
        public string MarkedFinalBy { get; set; }
    }
}
