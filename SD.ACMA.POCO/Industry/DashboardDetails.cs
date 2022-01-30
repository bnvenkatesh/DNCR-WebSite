using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SD.ACMA.POCO.Industry
{
    public class DashboardDetails
    {
        public string Email { get; set; }
        public string SubscriptionType { get; set; }
        public DateTime ExpiryDate { get; set; }
        public int SubscriptionLimit { get; set; }
        public string SubscriptionRollover { get; set; }
        public int QuantityOfNumbersWashed { get; set; }
        public int QuantityOfNumbersAvailable { get; set; }
        public int NumberOfUsers { get; set; }
    }
}
