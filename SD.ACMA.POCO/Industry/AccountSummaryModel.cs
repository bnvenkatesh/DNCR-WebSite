using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SD.ACMA.POCO.Industry
{
    public class AccountSummaryModel
    {
        public string AgentUserDisplayName { get; set; }
        public string Amount { get; set; }
        public string CreatedTimeStampt { get; set; }
        public string Description { get; set; }
        public string EnquiryId { get; set; }
        public string Expires { get; set; }
        public string OrderNumber { get; set; }
        public string PaymentReference { get; set; }
        public string TransactionType { get; set; }
        public string WashCredits { get; set; }
    }
}
