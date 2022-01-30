using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SD.ACMA.POCO.Industry
{
    public class FinancialDetails
    {
        public DateTime OrderDate { get; set; }
        public string OrderNumber { get; set; }
        public string Description { get; set; }
        public string OrderStatus { get; set; }
        public DateTime Expires { get; set; }
        public decimal OrderAmount { get; set; }
        public decimal OutstandingAmount { get; set; }
        public decimal CreditBalance { get; set; }
        public bool AllowCreditCardPament { get; set; }
    }
}
