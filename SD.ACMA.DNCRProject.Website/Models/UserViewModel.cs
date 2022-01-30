using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using SD.ACMA.DNCRProject.Website.Helpers;

namespace SD.ACMA.DNCRProject.Website.Models
{
    public class UserViewModel
    {
        [Display(Name = "First Name")]
        [Required(ErrorMessage = "Please enter First Name e.g. John")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        [Required(ErrorMessage = "Please enter Last Name e.g. Smith")]
        public string LastName { get; set; }

        [Display(Name = "Company Department")]
        public string CompanyDepartment { get; set; }

        [Display(Name = "Phone Number 1")]
        [Required(ErrorMessage = "Please enter Phone Number e.g. 02 1234 5678 or 0412 345 678")]
        public string PhoneNumber1 { get; set; }

        [Display(Name = "Phone Number 2")]
        public string PhoneNumber2 { get; set; }

        [Display(Name = "Email<span class='visuallyhidden'> Please ensure you enter a valid email address e.g. john.smith@domain.com</span>")]
        [Required(ErrorMessage = "Please enter an Email address e.g. john.smith@domain.com")]
        [RegularExpression(@"\b[a-zA-Z0-9'._%+-]+@[a-zA-Z0-9.+-]+\.[a-zA-Z]{2,}\b", ErrorMessage = "This does not appear to be a valid email address e.g. john.smith@domain.com")]
        public string Email { get; set; }

        [Display(Name = "Confirm Email")]
        [Required(ErrorMessage = "Please enter your email again to confirm")]
        [Compare("Email", ErrorMessage = "Email does not match, please re-enter your email address")]
        public string ConfirmEmail { get; set; }

        [Display(Name = "Wash account user will be able to see the wash quote")]
        public bool CanSeeWashQuote { get; set; }

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

        public bool IsSubmitted { get; set; }

        public int AccountUserId { get; set; } //TODO: add this as a hidden field in the view
    }
}