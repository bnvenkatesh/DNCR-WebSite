using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SD.ACMA.DNCRProject.Website.Models
{
    public class DashboardFinancialViewModel : BasePagerViewModel
    {
        public IEnumerable<DashboardFinancialModel> Financials { get; set; }
    }

    public class DashboardFinancialModel
    {
        [Display(Name = "Order Date")]
        public DateTime OrderDate { get; set; }

        [Display(Name = "Order Number")]
        public string OrderNumber { get; set; }

        public int OrderId { get; set; }

        [Display(Name = "Status")]
        public string Status { get; set; }

        [Display(Name = "Order Expires In")]
        public string OrderExpires { get; set; }

        [Display(Name = "Order Amount")]
        public decimal OrderAmount { get; set; }

        [Display(Name = "Outstanding")]
        public decimal Outstanding { get; set; }

        public bool CanPay { get; set; }

        public bool CanCancel { get; set; }

        public bool CanDownload { get; set; }

        public bool CanClose { get; set; }
    }
}