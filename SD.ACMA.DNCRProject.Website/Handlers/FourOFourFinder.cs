using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Web.Routing;

namespace SD.ACMA.DNCRProject.Website.Handlers
{
    public class FourOFourFinder : IContentFinder
    {
        public bool TryFindContent(PublishedContentRequest contentRequest)
        {
            if (contentRequest.Is404)
            {
                var home = contentRequest.RoutingContext.UmbracoContext.ContentCache.GetAtRoot().First(x => x.DocumentTypeAlias == "HomePage");
                var fourOFourNode = home.Children.First(x => x.DocumentTypeAlias == "FourOFourPage");
                contentRequest.SetResponseStatus(404, "404 Page Not Found");
                contentRequest.PublishedContent = fourOFourNode;
            }

            return contentRequest.PublishedContent != null;
        }
    }
}