using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SD.ACMA.POCO.Base;

namespace SD.ACMA.POCO.Industry
{
    public class GetAvailableSubscriptionsResponse : BaseWebServiceResponse
    {
        public List<SubscriptionModel> Subscriptions { get; set; }
        public string Message { get; set; }
    }
}
