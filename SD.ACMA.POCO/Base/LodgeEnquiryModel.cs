using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SD.ACMA.DNCR.Infrastructure;
using SD.ACMA.POCO.Industry;

namespace SD.ACMA.POCO.Base
{
    public class LodgeEnquiryModel
    {
        public string Addressline1 { get; set; }
        public string Addressline2 { get; set; }
        public string Company { get; set; }
        public string Country { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Suburb { get; set; }
        public string Postcode { get; set; }
        public string State { get; set; }
        public string Title { get; set; }
        public Enums.ChannelsEnum ChannelID { get; set; }
        public string CompanyName { get; set; }
        public string Details { get; set; }
        public string Type { get; set; }
        public string TelemarketerType { get; set; }
        public string Subject { get; set; }

        public bool IsConsumer { get; set; }
        public bool? IsRefund { get; set; }

        public bool IsAnonymous { get; set; }

        public OnlineRefundRequestModel RefundRequestModel { get; set; }
    }
}
