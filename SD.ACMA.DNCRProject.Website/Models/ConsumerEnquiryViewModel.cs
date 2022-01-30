using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SD.ACMA.DNCRProject.Website.Helpers;

namespace SD.ACMA.DNCRProject.Website.Models
{
    public class ConsumerEnquiryViewModel
    {
        [Display(Name = "Please treat my enquiry as anonymous")]
        public bool IsAnonymous { get; set; }

        [Display(Name = "Title")]
        [Required(ErrorMessage = "Please select a Title")]
        public string Title { get; set; }

        [Display(Name = "First Name")]
        [Required(ErrorMessage = "Please enter your First Name e.g. John")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        [Required(ErrorMessage = "Please enter your Last Name e.g. Smith")]
        public string LastName { get; set; }

        [Display(Name = "Email<span class='visuallyhidden'> Please ensure you enter a valid email address e.g. john.smith@domain.com</span>")]
        [RequireAtLeastOneOfGroup("howToContactYou", ErrorMessage = " ")]
        [RegularExpression(@"\b[a-zA-Z0-9'._%+-]+@[a-zA-Z0-9.+-]+\.[a-zA-Z]{2,}\b", ErrorMessage = "This does not appear to be a valid email address e.g. john.smith@domain.com")]
        public string Email { get; set; }

        [Display(Name = "Confirm Email")]
        [System.ComponentModel.DataAnnotations.Compare("Email", ErrorMessage = "Email does not match, please re-enter your email address")]
        public string ConfirmEmail { get; set; }

        [Display(Name = "Phone")]
        [RequireAtLeastOneOfGroup("howToContactYou", ErrorMessage = "Please enter your email address e.g. john.smith@domain.com and/or your Contact Phone number e.g. 02 1234 5678 or 0412 345 678")]
        [StringLength(20, ErrorMessage = "Phone cannot be longer than 20 characters")]
        public string ContactNumber { get; set; }

        [Display(Name = "Organisation")]
        public string OrganisationName { get; set; }

        [Display(Name = "Address Line 1")]
        [Required(ErrorMessage = "Please enter your Address e.g. 1 Elizabeth Street")]
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
        [Required(ErrorMessage = "Please select or enter your State e.g. NSW")]
        public string State { get; set; }

        [Display(Name = "Country")]
        [Required(ErrorMessage = "Please select Country")]
        public string Country { get; set; }

        [Display(Name = "What does your enquiry relate to?")]
        public string Subject { get; set; }

        [Display(Name = "Please enter your enquiry")]
        [Required(ErrorMessage = "Please enter your Enquiry Details")]
        [RegularExpression(@"[^<>]*", ErrorMessage = "< and > are not allowed")]
        public string Details { get; set; }

        [Display(Name = "Channel")]
        [Required(ErrorMessage = "Please select a channel")]
        public string Channel { get; set; }

        public bool IsSubmitted { get; set; }

        public string RefCode { get; set; }

        public List<SelectListItem> CountryList { get; set; }
    }
}