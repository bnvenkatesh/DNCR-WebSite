using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SD.ACMA.DNCRProject.Website.Models
{
    public class SearchViewModel
    {
        [Display(Name = "Search")]
        [Required]
        public string Keywords { get; set; }
    }
}