using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SD.ACMA.DNCRProject.Website.Models
{
    public class DashboardSubscriptionViewModel : BasePagerViewModel
    {
        public IEnumerable<DashboardSubscriptionModel> Subscriptions { get; set; }
    }

    public class DashboardSubscriptionModel
    {
        [Display(Name = "Transaction")]
        public string Transaction { get; set; }

        [Display(Name = "Date")]
        public string Date { get; set; }

        [Display(Name = "Order Number")]
        public string OrderNumber { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Amount")]
        public string Amount { get; set; }

        [Display(Name = "Wash Balance")]
        public string WashBalance { get; set; }
    }
}