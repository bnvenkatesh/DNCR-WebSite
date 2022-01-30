using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using umbraco.dialogs;

namespace SD.ACMA.DNCRProject.Website.Models
{
    public class ForgotPasswordViewModel
    {
        [Display(Name = "Email Address<span class='visuallyhidden'> Please ensure you enter a valid email address e.g. john.smith@domain.com</span>")]
        [Required(ErrorMessage = "Please enter your user name e.g. john.smith@domain.com")]
        [RegularExpression(@"\b[a-zA-Z0-9'._%+-]+@[a-zA-Z0-9.+-]+\.[a-zA-Z]{2,}\b", ErrorMessage = "This does not appear to be a valid email address e.g. john.smith@domain.com")]
        public string ForgotPasswordEmailAddress { get; set; }

        public bool EmailSent { get; set; }
    }
}