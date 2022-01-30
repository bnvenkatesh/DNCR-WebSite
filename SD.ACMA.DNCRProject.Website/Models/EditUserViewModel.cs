using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SD.ACMA.DNCRProject.Website.Models
{
    public class EditUserViewModel : UserViewModel
    {
        [Display(Name = "Yes")]
        public bool ResetPassword { get; set; }

        public string OriginalEmail { get; set; }
    }
}