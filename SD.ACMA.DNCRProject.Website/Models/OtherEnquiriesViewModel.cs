using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SD.ACMA.DNCRProject.Website.Models
{
    public class OtherEnquiriesViewModel
    {
        [Display(Name = "Please enter your enquiry")]
        [Required(ErrorMessage = "You need to provide some details so we can address your request appropriately")]
        public string Details { get; set; }
    }
}