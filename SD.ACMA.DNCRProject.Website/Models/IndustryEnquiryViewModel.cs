using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using SD.ACMA.DNCR.Infrastructure;
using SD.ACMA.DNCRProject.Website.Helpers;

namespace SD.ACMA.DNCRProject.Website.Models
{
    public class IndustryEnquiryViewModel
    {
        [Display(Name = "Please treat my enquiry as anonymous")]
        public bool IsAnonymous { get; set; }

        [Display(Name = "First Name")]
        [Required(ErrorMessage = "Please enter your First Name e.g. John")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        [Required(ErrorMessage = "Please enter your Last Name e.g. Smith")]
        public string LastName { get; set; }

        [Display(Name = "Organisation")]
        public string OrganisationName { get; set; }

        [Display(Name="Access-seeker ID")]
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
        [Required(ErrorMessage = "Please select Country")]
        public string Country { get; set; }

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

        [Display(Name = "What is your enquiry regarding?")]
        [Required(ErrorMessage = "You need to select one, that is appropriate, to progress the application")]
        public string Subject { get; set; }

        [Display(Name = "Please enter your enquiry")]
        [Required(ErrorMessage = "You need to provide some details so we can address your request appropriately")]
        public string Details { get; set; }

        [Display(Name = "Channel")]
        [Required(ErrorMessage = "Please select a channel")]
        public string Channel { get; set; }

        public List<SelectListItem> EnquiryTypes { get; set; }

        public List<SelectListItem> CountryList { get; set; }

        #region Refund section

        //Mandatory for: BalanceRefund, SubscriptionRefund
        //Optional for: WashCreditRollover, WashReversal, AccountBalanceAdjustment, WashNumberAdjustment
        [Required(ErrorMessage = "Please enter name as it appears on the bank account e.g. John Smith")]
        public string AccountName { get; set; }

        [Required(ErrorMessage = "Please enter Account Number e.g. 1234567890")]
        [RegularExpression(@"^([0-9](-|\s)*){6,10}$", ErrorMessage = "The number you have provided is not valid. Numbers must be between 6 to 10 digits and can contain an optional hyphen (-), spaces e.g. 1234567890")]
        public string AccountNumber { get; set; }

        [Required(ErrorMessage = "Please enter Account BSB number e.g. 123-456")]
        [RegularExpression(@"^([0-9]-*){6}$", ErrorMessage = "The number you have provided is not valid. Numbers must be six digits and can contain an optional hyphen (-) e.g. 123-456")]
        public string AccountBSB { get; set; }

        public string OtherAccountDetails { get; set; }

        //Mandatory for: SubscriptionRefund, WashCreditRollover, WashReversal, AccountBalanceAdjustment, WashNumberAdjustment
        //Optional for: BalanceRefund
        [Required(ErrorMessage = "Please provide a reason so we can address your request appropriately")]
        public string Description { get; set; }

        //BalanceRefund
        [Required]
        public decimal RefundReservedAccountBalance { get; set; }
        public string AdditionalInformation { get; set; }

        //SubscriptionRefund
        [Required]
        public int RefundReservedWashCredits { get; set; }
        [Required(ErrorMessage = "Please select a subscription")]
        public int RefundSubscriptionId { get; set; }
        [Required]
        public int RefundOrderId { get; set; }

        //WashCreditRollover
        [Required]
        public int WashCreditsRolloverAmount { get; set; }
        [Required]
        public int WashCreditsRolloverTransactionId { get; set; }

        //WashReversal
        [Required(ErrorMessage = "Please select a wash")]
        public int RefundWashTransactionId { get; set; }

        //AccountBalanceAdjustment
        [Required(ErrorMessage = "Please enter adjustment amount e.g. +100")]
        public string RefundAcmaIncrementAccountBalance { get; set; }

        //WashNumberAdjustment
        [Required(ErrorMessage = "Please enter adjustment amount e.g. +1000")]
        public string RefundAcmaIncrementWashCredits { get; set; }

        #endregion

        public bool IsSubmitted { get; set; }

        public string RefCode { get; set; }
    }
}