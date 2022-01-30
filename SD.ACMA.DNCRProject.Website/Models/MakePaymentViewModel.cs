using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SD.ACMA.DNCRProject.Website.Models
{
    public class MakePaymentViewModel
    {
        public List<OrderSubscriptionModel> Subscriptions { get; set; }

        [Display(Name = "Order Total")]
        public decimal OrderTotal { get; set; }

        [Display(Name = "Amount Paid to Date")]
        public decimal PaidToDate { get; set; }

        [Display(Name = "Order Balance")]
        public decimal OrderBalance { get; set; }

        public int OrderId { get; set; }

        public string OrderNumber { get; set; }
    }

    public class OrderSubscriptionModel
    {
        public string Type { get; set; }
        public int Washes { get; set; }
        public decimal Price { get; set; }
    }
}