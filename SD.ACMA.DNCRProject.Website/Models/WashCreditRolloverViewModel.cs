using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SD.ACMA.DNCRProject.Website.Models
{
    public class WashCreditRolloverViewModel
    {
        [Display(Name = "Wash numbers recently expired")]
        [Required]
        public int WashCreditsRolloverAmount { get; set; }

        [Display(Name = "Expiry Date")]
        public DateTime ExpiryDate { get; set; }

        [Display(Name = "Reason for extension")]
        [Required(ErrorMessage = "Please provide a reason for extension so we can address your request appropriately")]
        public string Description { get; set; }

        [Required]
        public int WashCreditsRolloverTransactionID { get; set; }

        public bool RequirePurchase { get; set; }
    }
}