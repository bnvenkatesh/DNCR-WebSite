using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using SD.ACMA.DNCRProject.Website.Helpers;

namespace SD.ACMA.DNCRProject.Website.Models
{
    public class SubscriptionViewModel
    {
        [Display(Name = "Please select one or more subscriptions to add to your account")]
        [MinimumElements(1, ".subscription select", ErrorMessage = "Please select a subscription type")]
        public List<SubscriptionModel> SubscriptionModels { get; set; }

        [Display(Name = "Please select one or more subscriptions to add to your account")]
        [MinimumElements(1, ".subscription select", ErrorMessage = "Please select a subscription type")]
        public List<SubscriptionModel> MinimumSubscriptionModels { get; set; }

        [Display(Name = "Apply account balance to this order")]
        public bool UseAccountBalance { get; set; }

        public decimal AccountBalanceToUse { get; set; }

        /*[Display(Name = "Name")]
        [Required(ErrorMessage = "Please enter your name")]
        [RegularExpression(@"^[a-zA-Z ,.'-]+$", ErrorMessage = "This does not appear to be a valid name")]
        public string Name { get; set; }
        */

        [Display(Name = "Email<span class='visuallyhidden'> Please ensure you enter a valid email address e.g. john.smith@domain.com</span>")]
        [Required(ErrorMessage = "Please enter your email address e.g. john.smith@domain.com")]
        [RegularExpression(@"\b[a-zA-Z0-9'._%+-]+@[a-zA-Z0-9.+-]+\.[a-zA-Z]{2,}\b", ErrorMessage = "This does not appear to be a valid email address e.g. john.smith@domain.com")]
        public string Email { get; set; }

        [Display(Name = "Confirm Email")]
        [Required(ErrorMessage = "Please enter your email again to confirm")]
        [Compare("Email", ErrorMessage = "Email does not match, please re-enter your email address")]
        public string ConfirmEmail { get; set; }

        [Display(Name = "I agree to these requirements")]
        [Mandatory(ErrorMessage = "Please read and agree to the requirements")]
        public bool AcceptTerms { get; set; }

        [Required(ErrorMessage = "Please select a payment method")]
        public string PaymentMethod { get; set; }

        public decimal PaymentDue { get; set; }

        public bool IsSubmitted { get; set; }

        public bool IsFreeSubscription { get; set; }

        public string EnquiryId { get; set; }
    }

    public class SubscriptionModel
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public int Limit { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public bool IsFree { get; set; }
    }
}