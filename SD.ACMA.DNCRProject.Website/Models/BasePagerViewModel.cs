using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SD.ACMA.DNCRProject.Website.Models
{
    public class BasePagerViewModel
    {
        public Pager Pager { get; set; }
    }

    public class Pager
    {
        public int NumberOfItems { get; set; }
        public int ItemsPerPage { get; set; }
        public int CurrentPage { get; set; }
        public IEnumerable<int> Pages { get; set; }

        public bool IsFirstPage
        {
            get
            {
                return CurrentPage <= 1;
            }
        }

        public bool IsLastPage
        {
            get
            {
                return (CurrentPage * ItemsPerPage) >= NumberOfItems;
            }
        }
    }
}