using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using SD.ACMA.DNCRProject.Website.Helpers;

namespace SD.ACMA.DNCRProject.Website.Models
{
    public class CreateAccountViewModel : BaseAccountViewModel
    {
        [Display(Name = "I agree to the preceding terms of use")]
        [Mandatory(ErrorMessage = "Please read and accept these conditions")]
        public bool AcceptTerms { get; set; }
    }
}