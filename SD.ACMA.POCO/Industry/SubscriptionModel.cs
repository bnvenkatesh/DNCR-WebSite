using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SD.ACMA.POCO.Industry
{
    public class SubscriptionModel
    {
        public int ExpirityInDays { get; set; }
        public bool IsFreeSubscription { get; set; }
        public DateTime LastUpdatedTimeStamp { get; set; }
        public decimal Price { get; set; }
        public string SubscriptionName { get; set; }
        public int SubscriptionRequestID { get; set; }
        public int WashCredits { get; set; }
    }
}
