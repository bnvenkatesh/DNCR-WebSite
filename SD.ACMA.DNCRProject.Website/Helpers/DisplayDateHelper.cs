using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SD.ACMA.DNCRProject.Website.Helpers
{
    public static class DisplayDateHelper
    {
        readonly static string dashboardDateFormat = "dd MMM yyyy";
        public static MvcHtmlString DashboardDate(this HtmlHelper html, DateTime? date, string placeholder = "-")
        {
            if (date.HasValue)
            {
                return html.DashboardDate(date.Value);
            }

            return new MvcHtmlString(placeholder);
        }

        public static MvcHtmlString DashboardDate(this HtmlHelper html, DateTime date)
        {
            return new MvcHtmlString(date.ToString(dashboardDateFormat));
        }
    }
}