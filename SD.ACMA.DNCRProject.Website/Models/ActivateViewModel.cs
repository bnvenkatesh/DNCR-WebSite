using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SD.ACMA.DNCRProject.Website.Models
{
    public class ActivateViewModel
    {
        [Display(Name = "New Password<span class='visuallyhidden'> Password must be at least 8 characters, must include at least one of uppercase letters, lowercase letters, and numbers</span>")]
        [Required(ErrorMessage = "Please enter a new password")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[a-zA-Z\d\!\$\&\+\-\\_\~]{8,}$", ErrorMessage = "Invalid Password - Must be at least 8 characters, must include at least one of uppercase letters, lowercase letters, and numbers; can contain ! $ & + - _ ~")]
        public string NewPassword { get; set; }

        [Display(Name = "Confirm Password")]
        [Required(ErrorMessage = "Please confirm password")]
        [Compare("NewPassword", ErrorMessage = "Password does not match, please re-enter password")]
        public string ConfirmPassword { get; set; }

        public string Email { get; set; }
        public string Token { get; set; }

        public bool IsExpired { get; set; }
        public bool IsInvalid { get; set; }
        public bool IsEmailSent { get; set; }
        public bool CannotActivate { get; set; }
        public bool CannotLogin { get; set; }
    }
}