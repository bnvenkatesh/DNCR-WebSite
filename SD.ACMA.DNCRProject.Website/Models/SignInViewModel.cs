using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SD.ACMA.DNCRProject.Website.Models
{
    public class SignInViewModel
    {
        [Display(Name = "Email Address<span class='visuallyhidden'> Please ensure you enter a valid email address e.g. john.smith@domain.com</span>")]
        [Required(ErrorMessage = "Please enter your user name e.g. john.smith@domain.com. If you have forgotten your user name, please contact your administrator or contact us on 1300 785 749")]
        [RegularExpression(@"\b[a-zA-Z0-9'._%+-]+@[a-zA-Z0-9.+-]+\.[a-zA-Z]{2,}\b", ErrorMessage = "This does not appear to be a valid email address e.g. john.smith@domain.com")]
        public string SignInEmailAddress { get; set; }

        [Display(Name = "Password<span class='visuallyhidden'> Password must be at least 8 characters, must include at least one of uppercase letters, lowercase letters, and numbers</span>")]
        [Required(ErrorMessage = "Please enter your password")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[a-zA-Z\d\!\$\&\+\-\\_\~]{8,}$", ErrorMessage = "Invalid Password - Must be at least 8 characters, must include at least one of uppercase letters, lowercase letters, and numbers; can contain ! $ & + - _ ~")]
        public string Password { get; set; }

        public string CreateAccountNodeUrl { get; set; }

        public bool IsLocked { get; set; }
        public bool IsMigrated { get; set; }
    }
}