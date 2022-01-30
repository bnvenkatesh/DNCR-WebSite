using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using SD.ACMA.DNCRProject.Website.Helpers;

namespace SD.ACMA.DNCRProject.Website.Models
{
    public class CheckRegistrationViewModel
    {
        [MinimumElements(1, ".phoneNumber input.phoneInput", ErrorMessage = "Please enter the number you wish to check e.g. 02 1234 5678 or 0412 345 678")]
        public List<CheckNumber> Numbers { get; set; }

        [MinimumElements(1, ".phoneNumber input.phoneInput", ErrorMessage = "Please enter the number you wish to check e.g. 02 1234 5678 or 0412 345 678")]
        public List<CheckNumber> MinimumNumbers { get; set; }

        [Display(Name = "Email<span class='visuallyhidden'> Please ensure you enter a valid email address e.g. john.smith@domain.com</span>")]
        [Required(ErrorMessage = "Please enter an email address e.g. john.smith@domain.com")]
        [RegularExpression(@"\b[a-zA-Z0-9'._%+-]+@[a-zA-Z0-9.+-]+\.[a-zA-Z]{2,}\b", ErrorMessage = "This does not appear to be a valid email address e.g. john.smith@domain.com")]
        public string Email { get; set; }

        [Display(Name = "Confirm Email")]
        [Required(ErrorMessage = "Please enter your email again to confirm")]
        [Compare("Email", ErrorMessage = "Email does not match, please re-enter your email address")]
        public string ConfirmEmail { get; set; }

        [Display(Name = "I have read and agree with these statements")]
        [Mandatory(ErrorMessage = "Please read and accept these conditions")]
        public bool AcceptTerms { get; set; }

        public bool IsSubmitted { get; set; }
    }

    public class CheckNumber
    {
        [RegularExpression(@"^(([ ().-]*(0)([ ().-]*[0-9][ ().-]*){9})|([ ().-]*(18)([ ().-]*[0-9][ ().-]*){5}(([ ().-]*[0-9][ ().-]*){3})?)|([ ().-]*(13)([ ().-]*[0-9][ ().-]*){4}(([ ()-]*[0-9][ ()-]*){2})?(([ ()-]*[0-9][ ()-]*){2})?))$", ErrorMessage = "The number you have provided is not valid. <br/>Numbers must start with 0, 13 or 18. <br/>Numbers starting with 0 must be 10 digits. <br/>Numbers starting with 18 must be 7 or 10 digits. <br/>Numbers starting with 13 must be 6, 8 or 10 digits. <br/>Optional spaces or - dashes or ( ) brackets are allowed. <br/>Letters (a-z), plus (+) and other characters are not accepted. E.g. 02 1234 5678 or 0412 345 678")]
        public string Number { get; set; }
    }
}