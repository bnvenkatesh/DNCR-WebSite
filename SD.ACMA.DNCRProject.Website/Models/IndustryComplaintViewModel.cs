using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SD.ACMA.DNCRProject.Website.Models
{
    public class IndustryComplaintViewModel
    {
        [Display(Name = "Please treat my complaint as anonymous")]
        public bool IsAnonymous { get; set; }

        [Display(Name = "Title")]
        [Required(ErrorMessage = "Please select a title")]
        public string Title { get; set; }

        [Display(Name = "First Name")]
        [Required(ErrorMessage = "Please enter your First Name e.g. John")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        [Required(ErrorMessage = "Please enter your Last Name e.g. Smith")]
        public string LastName { get; set; }

        [Display(Name = "Organisation")]
        public string OrganisationName { get; set; }

        [Display(Name = "Access-seeker ID")]
        public string AccessSeekerId { get; set; }

        [Display(Name = "Address Line 1")]
        [Required(ErrorMessage = "Please enter your address e.g. 1 Elizabeth Street")]
        public string AddressLine1 { get; set; }

        [Display(Name = "Address Line 2")]
        public string AddressLine2 { get; set; }

        [Display(Name = "City or Locality")]
        [Required(ErrorMessage = "Please enter your City or Locality e.g. North Sydney")]
        public string City { get; set; }

        [Display(Name = "Postcode")]
        //[RegularExpression(@"^[0-9]{4}$", ErrorMessage = "The number you have entered is not an Australian Postcode")]
        [Required(ErrorMessage = "Please enter your Postcode e.g. 2060")]
        public string Postcode { get; set; }

        [Display(Name = "State")]
        [Required(ErrorMessage = "Please enter or select your State")]
        public string State { get; set; }

        [Display(Name = "Country")]
        [Required(ErrorMessage = "Please select your Country")]
        public string Country { get; set; }

        [Display(Name = "Email<span class='visuallyhidden'> Please ensure you enter a valid email address e.g. john.smith@domain.com</span>")]
        [Required(ErrorMessage = "Please enter your email address e.g. john.smith@domain.com")]
        [RegularExpression(@"\b[a-zA-Z0-9'._%+-]+@[a-zA-Z0-9.+-]+\.[a-zA-Z]{2,}\b", ErrorMessage = "This does not appear to be a valid email address e.g. john.smith@domain.com")]
        public string Email { get; set; }

        [Display(Name = "Phone<span class='visuallyhidden'> Numbers must be 10 digits beginning with 0. Numbers starting with 18 must be 7 or 10 digits. Numbers starting with 13 must be 6, 8 or 10 digits. E.g. 0212345678 or 0412345678</span>")]
        [Required(ErrorMessage = "Please enter a contact phone number")]
        [RegularExpression(@"^(([ ().-]*(0)([ ().-]*[0-9][ ().-]*){9})|([ ().-]*(18)([ ().-]*[0-9][ ().-]*){5}(([ ().-]*[0-9][ ().-]*){3})?)|([ ().-]*(13)([ ().-]*[0-9][ ().-]*){4}(([ ()-]*[0-9][ ()-]*){2})?(([ ()-]*[0-9][ ()-]*){2})?))$", ErrorMessage = "The number you have provided is not valid. <br/>Numbers must start with 0, 13 or 18. <br/>Numbers starting with 0 must be 10 digits. <br/>Numbers starting with 18 must be 7 or 10 digits. <br/>Numbers starting with 13 must be 6, 8 or 10 digits. <br/>Optional spaces or - dashes or ( ) brackets are allowed. <br/>Letters (a-z), plus (+) and other characters are not accepted. E.g. 02 1234 5678 or 0412 345 678")]
        public string ContactNumber { get; set; }

        [Display(Name = "Your complaint details")]
        [Required(ErrorMessage = "You need to provide some details so we can address your complaint appropriately")]
        public string Details { get; set; }

        [Display(Name = "Channel")]
        [Required(ErrorMessage = "Please select a channel")]
        public string Channel { get; set; }

        public bool IsSubmitted { get; set; }
        public string RefCode { get; set; }

        public List<SelectListItem> CountryList { get; set; }
    }
}