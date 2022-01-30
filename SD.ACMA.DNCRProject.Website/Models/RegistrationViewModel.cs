using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using SD.ACMA.DNCRProject.Website.Helpers;

namespace SD.ACMA.DNCRProject.Website.Models
{
    public class RegistrationViewModel
    {
        [Display(Name = "I wish to register:")]
        [Required(ErrorMessage = "Please select what you want to register")]
        public string RegisterType { get; set; }

        [Display(Name = "First Name")]
        [Required(ErrorMessage = "Please enter your First Name e.g. John")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        [Required(ErrorMessage = "Please enter your Last Name e.g. Smith")]
        public string LastName { get; set; }

        [Display(Name = "Email<span class='visuallyhidden'> Please ensure you enter a valid email address e.g. john.smith@domain.com</span>")]
        [Required(ErrorMessage = "Please enter an email address e.g. john.smith@domain.com")]
        [RegularExpression(@"\b[a-zA-Z0-9'._%+-]+@[a-zA-Z0-9.+-]+\.[a-zA-Z]{2,}\b", ErrorMessage = "This does not appear to be a valid email address e.g. john.smith@domain.com")]
        public string Email { get; set; }

        [Display(Name = "Confirm Email")]
        [Required(ErrorMessage = "Please enter your email again to confirm")]
        [Compare("Email", ErrorMessage = "Email does not match, please re-enter your email address")]
        public string ConfirmEmail { get; set; }

        [MinimumElements(1, ".phoneNumber input.phoneInput", ErrorMessage = "Please enter the number you wish to register e.g. 02 1234 5678 or 0412 345 678")]
        public List<RegistrationNumber> Numbers { get; set; }

        [MinimumElements(1, ".phoneNumber input.phoneInput", ErrorMessage = "Please enter the number you wish to register e.g. 02 1234 5678 or 0412 345 678")]
        public List<RegistrationNumber> MinimumNumbers { get; set; }

        [Required(ErrorMessage = "Please enter your Contact Phone number e.g. 02 1234 5678 or 0412 345 678")]
        [Display(Name = "Contact Number<span class='visuallyhidden'> Numbers must be 10 digits beginning with 0. Numbers starting with 18 must be 7 or 10 digits. Numbers starting with 13 must be 6, 8 or 10 digits. E.g. 0212345678 or 0412345678</span>")]
        [RegularExpression(@"^(([ ().-]*(0)([ ().-]*[0-9][ ().-]*){9})|([ ().-]*(18)([ ().-]*[0-9][ ().-]*){5}(([ ().-]*[0-9][ ().-]*){3})?)|([ ().-]*(13)([ ().-]*[0-9][ ().-]*){4}(([ ()-]*[0-9][ ()-]*){2})?(([ ()-]*[0-9][ ()-]*){2})?))$", ErrorMessage = "The number you have provided is not valid. <br/>Numbers must start with 0, 13 or 18. <br/>Numbers starting with 0 must be 10 digits. <br/>Numbers starting with 18 must be 7 or 10 digits. <br/>Numbers starting with 13 must be 6, 8 or 10 digits. <br/>Optional spaces or - dashes or ( ) brackets are allowed. <br/>Letters (a-z), plus (+) and other characters are not accepted. E.g. 02 1234 5678 or 0412 345 678")]
        public string ContactNumber { get; set; }

        [Display(Name = "Organisation Name")]
        [Required(ErrorMessage = "Please enter Organisation Name e.g. XYZ Pty Ltd")]
        public string OrganisationName { get; set; }

        [Display(Name = "I have read and agree to these statements and understand that providing false or misleading information is a serious criminal offence")]
        [Mandatory(ErrorMessage = "Please acknowledge that you have read and agree to these statements")]
        public bool AcceptTerms { get; set; }

        
        [Display(Name = "I confirm that the above number/s are not used or maintained primarily for business purposes (except for fax numbers)")]
        [Mandatory(ErrorMessage = "Please confirm that the numbers are not business numbers")]
        public bool NotABusinessNumberAcceptTerms { get; set; }

        [Display(Name = "I am the account holder for the numbers submitted for registration")]
        [Mandatory(ErrorMessage = "Please confirm you are the account holder for the numbers submitted for registration")]
        public bool AmtheAccountHolder { get; set; }

        [Display(Name = "The numbers are used primarily for domestic purposes and are not used primarily for business purposes")]
        public bool NumberDomesticAcceptTerms { get; set; }

        [Display(Name = "The numbers are used exclusively for transmitting or receiving faxes")]
        public bool NumberFaxAcceptTerms { get; set; }

        [Display(Name = "The numbers are used or maintained exclusively for use by a government body")]
        public bool NumberGovtBodyAcceptTerms { get; set; }
             

        public bool IsSubmitted { get; set; }
    }

    public class RegistrationNumber
    {
        [RegularExpression(@"^(([ ().-]*(0)([ ().-]*[0-9][ ().-]*){9})|([ ().-]*(18)([ ().-]*[0-9][ ().-]*){5}(([ ().-]*[0-9][ ().-]*){3})?)|([ ().-]*(13)([ ().-]*[0-9][ ().-]*){4}(([ ()-]*[0-9][ ()-]*){2})?(([ ()-]*[0-9][ ()-]*){2})?))$", ErrorMessage = "The number you have provided is not valid. <br/>Numbers must start with 0, 13 or 18. <br/>Numbers starting with 0 must be 10 digits. <br/>Numbers starting with 18 must be 7 or 10 digits. <br/>Numbers starting with 13 must be 6, 8 or 10 digits. <br/>Optional spaces or - dashes or ( ) brackets are allowed. <br/>Letters (a-z), plus (+) and other characters are not accepted. E.g. 02 1234 5678 or 0412 345 678")]
        public string Number { get; set; }

        public bool IsFax { get; set; }
    }
}