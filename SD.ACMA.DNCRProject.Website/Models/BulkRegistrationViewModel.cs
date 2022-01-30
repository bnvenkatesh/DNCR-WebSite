using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using SD.ACMA.DNCRProject.Website.Helpers;

namespace SD.ACMA.DNCRProject.Website.Models
{
    public class BulkRegistrationViewModel
    {
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

        [Display(Name = "Phones and Faxes")]
        [Required(ErrorMessage = "You must at least upload 1 file")]
        [FileUploadExtensions("txt|csv", ErrorMessage = "The file that you have uploaded is in the incorrect format, please upload only .txt or .csv files")]
        public HttpPostedFileBase PhoneFaxFileUpload { get; set; }

        [Display(Name = "Phone numbers")]
        [FileUploadExtensions("txt|csv", ErrorMessage = "The file that you have uploaded is in the incorrect format, please upload only .txt or .csv files")]
        public HttpPostedFileBase PhoneFileUpload { get; set; }

        [Display(Name = "Fax numbers")]
        [FileUploadExtensions("txt|csv", ErrorMessage = "The file that you have uploaded is in the incorrect format, please upload only .txt or .csv files")]
        public HttpPostedFileBase FaxFileUpload { get; set; }

        [Display(Name = "Organisation")]
        public string OrganisationName { get; set; }

        [Display(Name = "Address Line 1")]
        [Required(ErrorMessage = "Please enter your Address e.g. 1 Elizabeth Street")]
        public string AddressLine1 { get; set; }

        [Display(Name = "Address Line 2")]
        public string AddressLine2 { get; set; }

        [Display(Name = "Suburb or Locality")]
        [Required(ErrorMessage = "Please enter your Suburb or Locality e.g. North Sydney")]
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

        [Display(Name = "Validation PDF")]
        [Required(ErrorMessage = "Please upload a file")]
        [FileUploadExtensions("pdf", ErrorMessage = "The file that you have uploaded is in the incorrect format, please upload only .pdf files")]
        [MaxFileSize(10 * 1024 * 1024, ErrorMessage = "Maximum allowed file size is 10 MB")]
        public HttpPostedFileBase ValidationPDF { get; set; }

        [Display(Name = "Select transaction")]
        [Required(ErrorMessage = "Please select transaction type")]
        public string TransactionType { get; set; }

        public bool FileValidationResult { get; set; }
        public string PhoneErrorFileLocation { get; set; }
        public string FaxErrorFileLocation { get; set; }
        public string PhoneErrorFileName { get; set; }
        public string FaxErrorFileName { get; set; }
        public bool HasErrorsInPhone { get; set; }
        public bool HasErrorsInFax { get; set; }

        [Display(Name = "I have read and agree with these statements")]
        [Mandatory(ErrorMessage = "Please read and accept these conditions")]
        public bool AcceptTerms { get; set; }

        public bool IsSubmitted { get; set; }

        public List<System.Web.Mvc.SelectListItem> CountryList { get; set; }
    }
}