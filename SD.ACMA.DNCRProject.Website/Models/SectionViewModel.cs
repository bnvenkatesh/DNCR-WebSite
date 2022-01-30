using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using Umbraco.Core.Models;
using Umbraco.Web;
using Umbraco.Web.Models;

namespace SD.ACMA.DNCRProject.Website.Models
{
    public class SectionViewModel : BasePagerViewModel
    {
        public IEnumerable<IPublishedContent> SectionItems { get; set; }
        public string ColourClass { get; set; }
    }
}