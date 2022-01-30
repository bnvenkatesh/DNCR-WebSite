using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using SD.ACMA.DNCRProject.Website.Helpers;

namespace SD.ACMA.DNCRProject.Website.Models
{
    public class WashHistoryViewModel : BasePagerViewModel
    {
        [Display(Name = "From (DD/MM/YYYY)")]
        [Required(ErrorMessage="Please enter the From Date e.g. 15/01/2015")]
        [RegularExpression(@"^(((0?[1-9]|[12]\d|3[01])\/(0?[13578]|1[02])\/((1[6-9]|[2-9]\d)\d{2}))|((0?[1-9]|[12]\d|30)\/(0?[13456789]|1[012])\/((1[6-9]|[2-9]\d)\d{2}))|((0?[1-9]|1\d|2[0-8])\/0?2\/((1[6-9]|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0?[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "This does not appear to be a valid date e.g. 15/01/2015")]
        public string FromDate { get; set; }

        [Display(Name = "To (DD/MM/YYYY)")]
        [Required(ErrorMessage = "Please enter the To Date e.g. 31/01/2015")]
        [RegularExpression(@"^(((0?[1-9]|[12]\d|3[01])\/(0?[13578]|1[02])\/((1[6-9]|[2-9]\d)\d{2}))|((0?[1-9]|[12]\d|30)\/(0?[13456789]|1[012])\/((1[6-9]|[2-9]\d)\d{2}))|((0?[1-9]|1\d|2[0-8])\/0?2\/((1[6-9]|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0?[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "This does not appear to be a valid date e.g. 31/01/2015")]
        [DateGreaterThan("FromDate", ErrorMessage = "The date must be later than From date")]
        public string ToDate { get; set; }

        public IEnumerable<WashHistoryModel> Washes { get; set; }

        public int WashCount { get; set; }
    }

    public class WashHistoryModel
    {
        public string ReferenceNumber { get; set; }
        public DateTime DateOfWash { get; set; }
        public string Username { get; set; }
        public string WashSource { get; set; }
        public string ClientReference { get; set; }
        public int Numbers { get; set; }
        public int Registered { get; set; }
        public string RegisteredFile { get; set; }
        public int Unregistered { get; set; }
        public string UnregisteredFile { get; set; }
        public int Invalid { get; set; }
        public string InvalidFile { get; set; }
        public int Combined { get; set; }
        public string CombinedFile { get; set; }
        public bool CanDownload { get; set; }
    }
}