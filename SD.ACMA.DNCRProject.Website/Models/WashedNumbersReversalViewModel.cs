using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using SD.ACMA.POCO.Industry;

namespace SD.ACMA.DNCRProject.Website.Models
{
    public class WashedNumbersReversalViewModel
    {
        [Display(Name = "Reason for reversal")]
        [Required(ErrorMessage = "Please provide a reason for reversal so we can address your request appropriately")]
        public string Description { get; set; }

        public List<RefundWashModel> Washes { get; set; }

        [Required(ErrorMessage = "Please select a wash")]
        public int? RefundWashTransactionId { get; set; }
    }

    public class RefundWashModel
    {
        [Display(Name = "Wash Reference")]
        public string WashReference { get; set; }

        [Display(Name = "Date Washed")]
        public DateTime DateWashed { get; set; }

        [Display(Name = "User")]
        public string User { get; set; }

        [Display(Name = "Wash Source")]
        public string WashSource { get; set; }

        [Display(Name = "Washed Numbers")]
        public int WashedNumbers { get; set; }
    }
}