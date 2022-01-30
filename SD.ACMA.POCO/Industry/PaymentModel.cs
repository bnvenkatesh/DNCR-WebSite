using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SD.ACMA.POCO.Industry
{
    public class PaymentModel
    {
        public DateTime CreatedTimeStamp { get; set; }
        public string CreditCardReference { get; set; }
        public int? CreditCardTransactionCode { get; set; }
        public int OrderID { get; set; }
        public decimal PaidAmount { get; set; }
        public int PaymentFeedEntryID { get; set; }
        public int PaymentID { get; set; }
        public SD.ACMA.DNCR.Infrastructure.Enums.PaymentMethodEnum PaymentMethod { get; set; }
        public int PaymentMethodID { get; set; }
    }
}
