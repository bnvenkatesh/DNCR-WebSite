using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SD.ACMA.POCO.Industry
{
    public class OrderLineModel
    {
        public string Activated { get; set; }
        public int ExpirityInDays { get; set; }
        public int OrderID { get; set; }
        public int OrderLineID { get; set; }
        public int OrderLineNumber { get; set; }
        public SD.ACMA.DNCR.Infrastructure.Enums.OrderLineStatusEnum OrderLineStatus { get; set; }
        public decimal Price { get; set; }
        public string SubscriptionName { get; set; }
        public int WashCredits { get; set; }
    }
}
