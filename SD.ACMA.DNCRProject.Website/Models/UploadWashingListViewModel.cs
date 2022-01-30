using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using SD.ACMA.DNCRProject.Website.Helpers;

namespace SD.ACMA.DNCRProject.Website.Models
{
    public class UploadWashingListViewModel
    {
        [Display(Name = "File to wash")]
        [Required(ErrorMessage = "You need to upload a file (list of number/s) to progress to the next step/stage")]
        [FileUploadExtensions("csv|txt|zip|gz|dat", ErrorMessage = "The file to be uploaded is in an invalid file format, please check the file and try again")]
        [MaxFileSize(10 * 1024 * 1024, ErrorMessage = "Maximum allowed file size is 10 MB")]
        [RegularExpression(@"^(((?:[a-zA-Z]\:|\\\\[\w\.]+\\[\w\s.$]+)\\(?:[\w\s.!@#$%^&()_+=;'[\]{}`~-]+\\)*\w([\w-._])+)|(\w([\w-._])+))$", ErrorMessage = "No spaces allowed in file name.")]
        public HttpPostedFileBase FileToWash { get; set; }

        [Display(Name = "Please choose how you want to receive the washing result")]
        [Required(ErrorMessage = "Please select an option")]
        public bool WashingResultOption { get; set; }

        [MinimumElements(1, ".separateFiles input[type=checkbox]:checked", ErrorMessage = "Please select at least one result file")]
        public string MinimumResultFile { get; set; }

        [Display(Name = "a file of registered numbers")]
        public bool FileOfRegisteredNumbers { get; set; }

        [Display(Name = "a file of unregistered numbers")]
        public bool FileOfUnregisteredNumbers { get; set; }

        [Display(Name = "a file of invalid numbers")]
        public bool FileOfInvalidNumbers { get; set; }

        public bool IsSubmitted { get; set; }

        public string FileToWashCopyPath { get; set; }

        public string WashHistoryURL { get; set; }

        public int? WashingRequestId { get; set; }

        public string AllInOneFile { get; set; }
        public bool HasSufficientWashingCredits { get; set; }
        public bool HasValidSubscription { get; set; }
        public string InvalidNumbersFile { get; set; }
        public bool IsSuccessful { get; set; }
        public string OriginalFile { get; set; }
        public string RegisteredNumbersFile { get; set; }
        public SD.ACMA.DNCR.Infrastructure.Enums.WashingStatusEnum Status { get; set; }
        public string UnregisteredNumbersFile { get; set; }
        public int WashCredits { get; set; }
    }
}