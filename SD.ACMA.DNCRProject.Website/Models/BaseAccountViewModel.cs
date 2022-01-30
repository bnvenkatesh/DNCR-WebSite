using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using SD.ACMA.DNCRProject.Website.Helpers;


namespace SD.ACMA.DNCRProject.Website.Models
{
    public class BaseAccountViewModel
    {
        #region Admin

        [Display(Name = "First Name")]
        [Required(ErrorMessage = "Please enter your First Name e.g. John")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        [Required(ErrorMessage = "Please enter your Last Name e.g. Smith")]
        public string LastName { get; set; }

        [Display(Name = "Phone Number")]
        [Required(ErrorMessage = "Please enter your Phone Number e.g. 02 1234 5678 or 0412 345 678")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Email<span class='visuallyhidden'> Please ensure you enter a valid email address e.g. john.smith@domain.com</span>")]
        [Required(ErrorMessage = "Please enter an Email Address e.g. john.smith@domain.com")]
        [RegularExpression(@"\b[a-zA-Z0-9'._%+-]+@[a-zA-Z0-9.+-]+\.[a-zA-Z]{2,}\b", ErrorMessage = "This does not appear to be a valid email address e.g. john.smith@domain.com")]
        public string Email { get; set; }

        [Display(Name = "Confirm Email")]
        [Required(ErrorMessage = "Please enter your email again to confirm")]
        [Compare("Email", ErrorMessage = "Email does not match, please re-enter your email address")]
        public string ConfirmEmail { get; set; }

        #endregion

        #region Applicant
        
        [Display(Name = "What activities does the applicant primarily engage in?")]
        [Required(ErrorMessage = "Please select primary activity")]
        public string Activity { get; set; }

        [Display(Name = "Which industry sectors does the applicant service?")]
        [Required(ErrorMessage = "Please select an industry sector")]
        public string Industry { get; set; }

        [Display(Name = "Organisation Name")]
        [Required(ErrorMessage = "What is the name of the organisation e.g. XYZ Pty Ltd")]
        public string OrganisationName { get; set; }

        [Display(Name = "Phone")]
        public string OrganisationPhone { get; set; }

        [Display(Name = "Does this applicant have an ABN?")]
        [Required(ErrorMessage = "Please indicate whether you have an ABN")]
        public bool HasABN { get; set; }

        [Display(Name = "ABN")]
        [RegularExpression(@"^[0-9]{2}(\s)?[0-9]{3}(\s)?[0-9]{3}(\s)?[0-9]{3}$", ErrorMessage = "This does not appear to be a valid ABN e.g. 12 345 678 910")]
        [Required(ErrorMessage = "Please enter your ABN e.g. 12 345 678 910")]
        public string ABN { get; set; }

        [Display(Name = "Address Line 1")]
        [Required(ErrorMessage = "Please enter your address e.g. 1 Elizabeth Street")]
        public string AddressLine1 { get; set; }

        [Display(Name = "Address Line 2")]
        public string AddressLine2 { get; set; }

        [Display(Name = "Suburb or Locality")]
        [Required(ErrorMessage = "Please enter your suburb e.g. North Sydney")]
        public string City { get; set; }

        [Display(Name = "Postcode")]
        //[RegularExpression(@"^[0-9]{4}$", ErrorMessage = "The number you have entered is not an Australian Postcode")]
        [Required(ErrorMessage = "Please enter your Postcode e.g. 2060")]
        public string Postcode { get; set; }

        [Display(Name = "State")]
        [Required(ErrorMessage = "Please select or enter your State e.g. NSW")]
        public string State { get; set; }

        [Display(Name = "Country")]
        [Required(ErrorMessage = "Please select your Country")]
        public string Country { get; set; }

        public List<System.Web.Mvc.SelectListItem> CountryList { get; set; }

        #endregion

        public bool IsSubmitted { get; set; }

        public string AccessSeekerAccountId { get; set; }
    }
}