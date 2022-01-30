using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Examine;
using Examine.LuceneEngine.SearchCriteria;
using SD.ACMA.DNCRProject.Website.Helpers;
using SD.ACMA.DNCRProject.Website.Models;
using Umbraco.Web;
using Umbraco.Web.Mvc;

namespace SD.ACMA.DNCRProject.Website.Controllers
{
    public class SearchSurfaceController : SurfaceController
    {
        [ChildActionOnly]
        public ActionResult Search()
        {
            var model = new SearchViewModel();
            //model.Keywords = "Search";
            return PartialView("Search", model);
        }

        [NotChildAction]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Search(SearchViewModel model)
        {
            if (ModelState.IsValid)
            {
                var searchResultsNode = CurrentPage.AncestorOrSelf("HomePage").Children.First(x => x.DocumentTypeAlias == "SearchResultsPage");
                return new RedirectResult(String.Format("{0}?keywords={1}", searchResultsNode.Url, model.Keywords));
            }
            return CurrentUmbracoPage();
        }

        [ChildActionOnly]
        public ActionResult SearchAgain(string keyword)
        {
            var model = new SearchViewModel();
            model.Keywords = keyword;
            return PartialView("SearchAgain", model);
        }

        [ChildActionOnly]
        public ActionResult SearchAgainMobile(string keyword)
        {
            var model = new SearchViewModel();
            model.Keywords = keyword;
            return PartialView("SearchAgainMobile", model);
        }

        [ChildActionOnly]
        public ActionResult SearchResults(string keywords, int itemsPerPage, int currentPage)
        {
            var model = GetViewModel(keywords.Trim(), itemsPerPage, currentPage);

            ViewBag.MobileOnly = false;
           
            return PartialView("SearchResults", model);
        }

        public ActionResult LoadSearchResults(string keywords, int itemsPerPage, int currentPage)
        {
            var model = GetViewModel(keywords.Trim(), itemsPerPage, currentPage);

            ViewBag.MobileOnly = true;

            return PartialView("_LoadSearchResults", model);
        }

        private SearchResultsViewModel GetViewModel(string keywords, int itemsPerPage, int currentPage)
        {
            List<SearchResult> searchResults = new List<SearchResult>();
            if (String.IsNullOrWhiteSpace(keywords))
            {
                keywords = String.Empty;
            }
            else
            {
                var searcher = ExamineManager.Instance;
                var searchCriteria = searcher.CreateSearchCriteria();
                //var query = searchCriteria.GroupedOr(new[] { "nodeName", "name", "pageTitle", "pageSummary", "pageContent", "seo" }, keywords).Compile();

                var strongSearch = "";
                var weakSearch = "";
                foreach (var keyword in keywords.Split(' '))
                {
                    strongSearch = strongSearch + "+" + keyword + "* ";
                    weakSearch = weakSearch + keyword + "* ";
                }

                var luceneString = "pageTitle:(" + strongSearch + ")^5 pageTitle:(" + weakSearch + ") pageSummary:(" + strongSearch + ")^4 pageSummary:(" + weakSearch + ") pageContent:(" + strongSearch + ")^3 pageContent:(" + weakSearch + ")";

                var query = searchCriteria.RawQuery(luceneString);
                var allResults = searcher.Search(query).Where(r => r["__IndexType"] == "content" && r["template"] != "0").ToList();
                foreach (var result in allResults)
                {
                    var node = Umbraco.TypedContent(result.Id);
                    if (node != null)
                    {
                        searchResults.Add(result);
                    }
                }
            }

            var model = new SearchResultsViewModel();
            model.SearchTerm = keywords;
            var pager = model.Pager = PagerHelper.GetPager(itemsPerPage, searchResults.Count(), currentPage);
            model.SearchResults = searchResults.Skip((pager.CurrentPage - 1) * pager.ItemsPerPage).Take(pager.ItemsPerPage);

            return model;
        }
    }
}