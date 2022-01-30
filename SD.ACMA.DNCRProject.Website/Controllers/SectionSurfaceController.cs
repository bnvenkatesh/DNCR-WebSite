using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SD.ACMA.DNCRProject.Website.Helpers;
using SD.ACMA.DNCRProject.Website.Models;
using Umbraco.Core.Models;
using Umbraco.Web.Models;
using Umbraco.Web.Mvc;
using Umbraco.Web;

namespace SD.ACMA.DNCRProject.Website.Controllers
{
    public class SectionSurfaceController : SurfaceController
    {
        [ChildActionOnly]
        public ActionResult SectionList(int itemsPerPage, string itemDocType, string colourClass, int currentPage)
        {
            var model = GetViewModel(0, itemsPerPage, itemDocType, colourClass, currentPage);

            ViewBag.MobileOnly = false;
            ViewBag.ItemDocType = itemDocType;
            ViewBag.CurrentNodeId = CurrentPage.Id;

            return PartialView("SectionList", model);
        }

        public ActionResult LoadItems(int currentNodeId, int itemsPerPage, string itemDocType, string colourClass, int currentPage)
        {
            var model = GetViewModel(currentNodeId, itemsPerPage, itemDocType, colourClass, currentPage);
            
            ViewBag.MobileOnly = true;

            return PartialView("_LoadItems", model);
        }

        private SectionViewModel GetViewModel(int currentNodeId, int itemsPerPage, string itemDocType, string colourClass, int currentPage)
        {
            var model = new SectionViewModel();
            List<IPublishedContent> items;

            if (currentNodeId != 0)
            {
                IPublishedContent currentNode = Umbraco.Content(currentNodeId);
                items = currentNode.Siblings().Where(x => x.DocumentTypeAlias == itemDocType).ToList();
            }
            else items = CurrentPage.Siblings().Where(x => x.DocumentTypeAlias == itemDocType).ToList();

            var pager = model.Pager = PagerHelper.GetPager(itemsPerPage, items.Count(), currentPage);
            model.SectionItems = items.OrderByDescending(x => x.GetPropertyValue("articleDate")).Skip((pager.CurrentPage - 1) * pager.ItemsPerPage).Take(pager.ItemsPerPage);
            model.ColourClass = colourClass;

            return model;
        }
    }

}