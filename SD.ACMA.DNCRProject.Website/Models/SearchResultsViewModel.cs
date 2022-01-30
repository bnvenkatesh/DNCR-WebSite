using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Examine;
using Umbraco.Core.Models;

namespace SD.ACMA.DNCRProject.Website.Models
{
    public class SearchResultsViewModel : BasePagerViewModel
    {
        public string SearchTerm { get; set; }
        public IEnumerable<SearchResult> SearchResults { get; set; }
    }
}