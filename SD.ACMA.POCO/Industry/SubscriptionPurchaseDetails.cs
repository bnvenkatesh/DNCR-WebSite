using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SD.ACMA.POCO.Industry
{
    public class SubscriptionPurchaseDetails
    {
        public string AccessSeekerID { get; set; }
        public string SubscriptionType { get; set; }
        public string SubscriptionLimit { get; set; }
        public string AnnualFee { get; set; }
        public int QuantityRequired { get; set; }
        public string Email { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentStatus { get; set; }
    }
}
