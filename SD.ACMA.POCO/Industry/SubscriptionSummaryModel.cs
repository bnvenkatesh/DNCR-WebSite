using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SD.ACMA.POCO.Base;

namespace SD.ACMA.POCO.Industry
{
    public class SubscriptionSummaryModel : BaseWebServiceResponse
    {
        public int AvailableWashCredits { get; set; }
        public decimal CreditNotes { get; set; }
        public string LastPurchaseDate { get; set; }
        public decimal ReservedCreditNotes { get; set; }
        public int ReservedWashCredits { get; set; }
        public string SubscriptionExpiryDate { get; set; }
        public string SubscriptionStatus { get; set; }

        public List<EnquiryRefundsModel> EnquiryRefundsModels { get; set; }
    }
}
