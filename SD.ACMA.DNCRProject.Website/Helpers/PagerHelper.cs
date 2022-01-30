using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SD.ACMA.DNCRProject.Website.Models;

namespace SD.ACMA.DNCRProject.Website.Helpers
{
    public static class PagerHelper
    {
        public static Pager GetPager(int itemsPerPage, int numberOfItems, int currentPage)
        {
            var numberOfPages = numberOfItems % itemsPerPage == 0 ? Math.Ceiling((decimal)(numberOfItems / itemsPerPage)) : Math.Ceiling((decimal)(numberOfItems / itemsPerPage)) + 1;
            var pages = Enumerable.Range(1, (int)numberOfPages);

            return new Pager()
            {
                NumberOfItems = numberOfItems,
                ItemsPerPage = itemsPerPage,
                CurrentPage = currentPage,
                Pages = pages
            };
        }
    }
}