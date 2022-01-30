using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SD.ACMA.DNCRProject.Website.Models
{
    public class DashboardAccountViewModel
    {
        [Display(Name = "Account ID")]
        public int AccountId { get; set; }

        [Display(Name = "Access-seeker ID")]
        public int AccessSeekerId { get; set; }

        [Display(Name = "Number of  Users")]
        public int NumOfUsers { get; set; }

        [Display(Name = "Status")]
        public string Status { get; set; }

        [Display(Name = "Purchased")]
        public DateTime? DatePurchased { get; set; }

        [Display(Name = "Available Wash Numbers")]
        public int AvailableCredit { get; set; }

        [Display(Name = "Expires")]
        public DateTime? DateExpires { get; set; }

        [Display(Name = "Reserved Washed Numbers")]
        public int ReservedCredit { get; set; }

        [Display(Name = "Available Account Balance")]
        public decimal AccountBalance { get; set; }

        [Display(Name = "Reserved Account Balance")]
        public decimal ReservedAccountBalance { get; set; }

        public bool CanSeeWashQuote { get; set; }
    }
}