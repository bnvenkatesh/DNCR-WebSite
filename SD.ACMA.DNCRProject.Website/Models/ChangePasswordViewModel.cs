using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SD.ACMA.DNCRProject.Website.Models
{
    public class ChangePasswordViewModel
    {
        [Display(Name = "Current Password<span class='visuallyhidden'> Password must be at least 8 characters, must include at least one of uppercase letters, lowercase letters, and numbers</span>")]
        [Required(ErrorMessage = "Please enter your current password")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[a-zA-Z\d\!\$\&\+\-\\_\~]{8,}$", ErrorMessage = "This does not appear to be a valid current password")]
        public string CurrentPassword { get; set; }

        [Display(Name = "New Password<span class='visuallyhidden'> Password must be at least 8 characters, must include at least one of uppercase letters, lowercase letters, and numbers</span>")]
        [Required(ErrorMessage = "Please enter a new password")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[a-zA-Z\d\!\$\&\+\-\\_\~]{8,}$", ErrorMessage = "Invalid Password - Must be at least 8 characters, must include at least one of uppercase letters, lowercase letters, and numbers; can contain ! $ & + - _ ~")]
        public string NewPassword { get; set; }

        [Display(Name = "Confirm Password")]
        [Required(ErrorMessage = "Please enter your new password again to confirm")]
        [Compare("NewPassword", ErrorMessage = "Password does not match, please re-enter password")]
        public string ConfirmPassword { get; set; }

        public bool IsSubmitted { get; set; }
    }
}