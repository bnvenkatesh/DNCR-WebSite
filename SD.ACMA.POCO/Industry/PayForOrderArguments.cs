using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SD.ACMA.POCO.Industry
{
    public class PayForOrderArguments
    {
        public int AccountUserID { get; set; }
        public int? AgentUserID { get; set; }
        public decimal Amount { get; set; }
        public string AuthorizeID { get; set; }
        public string CardType { get; set; }
        public string CreditCardReference { get; set; }
        public string ExternalMessage { get; set; }
        public string ReceiptNo { get; set; }
        public bool IsCSR { get; set; }
        public int OrderID { get; set; }
        public string ResponseCode { get; set; }
        public string TransactionID { get; set; }
        public long TransactionNo { get; set; }
        public string TransactionType { get; set; }
        public DateTime? ExpectedSettelmentDate { get; set; }
    }
}
