using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using SD.ACMA.DNCRProject.Website.Helpers;

namespace SD.ACMA.DNCRProject.Website.Models
{
    public class EditAccountViewModel : BaseAccountViewModel
    {
        [Display(Name = "Change Password?")]
        public bool ChangePassword { get; set; }

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

        [Display(Name = "Please choose how you want to receive the washing result")]
        [Required(ErrorMessage = "Please select an option")]
        public bool WashingResultOption { get; set; }

        [MinimumElements(1, ".separateFiles input[type=checkbox]:checked", ErrorMessage = "Please select at least one result file")]
        public string MinimumResultFile { get; set; }

        [Display(Name = "a file of registered numbers")]
        public bool FileOfRegisteredNumbers { get; set; }

        [Display(Name = "a file of unregistered numbers")]
        public bool FileOfUnregisteredNumbers { get; set; }

        [Display(Name = "a file of invalid numbers")]
        public bool FileOfInvalidNumbers { get; set; }

        [Display(Name = "I want daily summary email")]
        public bool DailySummaryEmail { get; set; }

        public SD.ACMA.DNCR.Infrastructure.Enums.BusinessSectorEnum? BusinessSector { get; set; }
    }
}