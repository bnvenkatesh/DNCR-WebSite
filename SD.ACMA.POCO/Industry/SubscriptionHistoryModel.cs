using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SD.ACMA.POCO.Industry
{
    public class SubscriptionHistoryModel
    {
        public int AccountTransactionID { get; set; }
        public string AccountUser { get; set; }
        public string AgentUser { get; set; }
        public decimal Amount { get; set; }
        public DateTime Created { get; set; }
        public decimal Credit { get; set; }
        public decimal Debit { get; set; }
        public string Description { get; set; }
        public DateTime ExpirationDate { get; set; }
        public string TransactionType { get; set; }
        public int? WashCreditBalance { get; set; }
    }
}
