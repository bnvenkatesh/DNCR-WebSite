using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using SD.ACMA.POCO.Industry;

namespace SD.ACMA.DNCRProject.Website.Models
{
    public class SubscriptionRefundViewModel
    {
        [Display(Name = "Available wash numbers for a refund")]
        [Required]
        public string RefundReservedWashCredits { get; set; }

        [Display(Name = "Account BSB")]
        [Required(ErrorMessage = "Please enter Account BSB number e.g. 123-456")]
        [RegularExpression(@"^([0-9]-*){6}$", ErrorMessage = "The number you have provided is not valid. Numbers must be six digits and can contain an optional hyphen (-) e.g. 123-456")]
        public string AccountBSB { get; set; }

        [Display(Name = "Account Name")]
        [Required(ErrorMessage = "Please enter name as it appears on the bank account e.g. John Smith")]
        public string AccountName { get; set; }

        [Display(Name = "Account Number")]
        [Required(ErrorMessage = "Please enter Account Number e.g. 1234567890")]
        [RegularExpression(@"^([0-9](-|\s)*){6,10}$", ErrorMessage = "The number you have provided is not valid. Numbers must be between 6 to 10 digits and can contain an optional hyphen (-), spaces e.g. 1234567890")]
        public string AccountNumber { get; set; }

        [Display(Name = "Other account details")]
        public string OtherAccountDetails { get; set; }

        [Display(Name = "Reason for refund")]
        [Required(ErrorMessage = "Please provide a reason for refund so we can address your request appropriately")]
        public string Description { get; set; }

        public List<RefundSubscriptionModel> Subscriptions { get; set; }

        [Required(ErrorMessage = "Please select a subscription")]
        public int? RefundSubscriptionId { get; set; }

        [Required]
        public int RefundOrderId { get; set; }

        public int AvailableWash { get; set; }
    }

    public class RefundSubscriptionModel
    {
        [Display(Name = "Order No")]
        public string OrderNo { get; set; }

        [Display(Name = "Order Date")]
        public DateTime OrderDate { get; set; }

        [Display(Name = "Subscription Type")]
        public string SubscriptionType { get; set; }

        [Display(Name = "Subscription Limit")]
        public int SubscriptionLimit { get; set; }

        [Display(Name = "Amount inc GST")]
        public decimal AmountIncGst { get; set; }

        public int SubscriptionId { get; set; }
        public int OrderId { get; set; }
    }
}