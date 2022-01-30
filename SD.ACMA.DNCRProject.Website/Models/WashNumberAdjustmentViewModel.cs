using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SD.ACMA.DNCRProject.Website.Models
{
    public class WashNumberAdjustmentViewModel
    {
        [Display(Name = "Current wash number")]
        public decimal WashCredits { get; set; }

        [Display(Name = "Numbers to add or remove<span class='visuallyhidden'> Prefix + or – to the amount for addition or removal of wash numbers respectively</span>")]
        [RegularExpression(@"^[-+]?[0-9,]*$", ErrorMessage = "Please enter valid numbers with optional dots, commas and + or – e.g. +1000")]
        [Required(ErrorMessage = "Please enter adjustment amount e.g. +1000")]
        public string RefundAcmaIncrementWashCredits { get; set; }

        [Display(Name = "Reason for adjustment")]
        [Required(ErrorMessage = "Please provide a reason for adjustment so we can address your request appropriately")]
        public string Description { get; set; }
    }
}