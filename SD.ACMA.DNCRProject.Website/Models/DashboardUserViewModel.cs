using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SD.ACMA.DNCRProject.Website.Models
{
    public class DashboardUserViewModel : BasePagerViewModel
    {
        public IEnumerable<DashboardUserModel> Users { get; set; }
    }

    public class DashboardUserModel
    {
        [Display(Name = "Access-seeker ID")]
        public int AccountUserId { get; set; }

        [Display(Name = "Name")]
        public string User { get; set; }

        [Display(Name = "Phone")]
        public string Phone { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Can see quotes")]
        public bool CanSeeQuotes { get; set; }

        [Display(Name = "Return formats")]
        public string ReturnFormats { get; set; }

        [Display(Name = "Is active")]
        public string Status { get; set; }
    }
}