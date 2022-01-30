using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SD.ACMA.DNCRProject.Website.Models
{
    public class BalanceRefundViewModel
    {
        [Display(Name = "Available account balance to refund")]
        [Required]
        public decimal RefundReservedAccountBalance { get; set; }

        [Display(Name = "Account BSB")]
        [Required(ErrorMessage = "Please enter Account BSB number e.g. 123-456")]
        [RegularExpression(@"^([0-9]-*){6}$", ErrorMessage = "The number you have provided is not valid. Numbers must be six digits and can contain an optional hyphen (-) e.g. 123-456")]
        public string AccountBSB { get; set; }

        [Display(Name = "Account Name")]
        [Required(ErrorMessage = "Please enter name as it appears on the bank account e.g. John Smith")]
        public string AccountName { get; set; }

        [Display(Name = "Account Number")]
        [Required(ErrorMessage = "Please enter Account Number e.g. 1234567890")]
        [RegularExpression(@"^([0-9](-|\s)*){6,10}$", ErrorMessage = "The number you have provided is not valid. Numbers must be between 6 to 10 digits and can contain an optional hyphen (-), spaces e.g. 1234567890")]
        public string AccountNumber { get; set; }

        [Display(Name = "Other Account Details")]
        public string OtherAccountDetails { get; set; }

        [Display(Name = "Additional Information")]
        public string AdditionalInformation { get; set; }
    }
}