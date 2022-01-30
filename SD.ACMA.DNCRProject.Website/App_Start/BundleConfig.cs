using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;

namespace SD.ACMA.DNCRProject.Website.App_Start
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle("~/bundles/css")
                .Include("~/css/jquery-ui.css")
        .Include("~/css/jquery.bxslider.css")
        .Include("~/css/responsive-tables.css")
        .Include("~/css/style.css")
        .Include("~/css/accordion.css")
        .Include("~/css/new.css"));

            bundles.Add(new ScriptBundle("~/bundles/js")
                .Include("~/scripts/jquery-1.11.2.min.js")
        .Include("~/scripts/jquery.cookie.js")
        .Include("~/scripts/jquery.validate.min.js")
        .Include("~/scripts/jquery.validate.unobtrusive.min.js")
        .Include("~/scripts/jquery-ui.js")
        .Include("~/scripts/jquery.bxslider.min.js")
        .Include("~/scripts/jquery.nanoscroller.js")
        .Include("~/scripts/icheck.min.js")
        .Include("~/scripts/modernizr-latest.js")
        .Include("~/scripts/handlebars-v3.0.0.js")
        .Include("~/scripts/responsive-tables.js")
        .Include("~/scripts/widgets/sd.addmorenumbers.js")
        .Include("~/scripts/widgets/sd.registertype.js")
        .Include("~/scripts/widgets/sd.bulktransactiontype.js")
        .Include("~/scripts/widgets/sd.otheroptionfield.js")
        .Include("~/scripts/widgets/sd.checkboxsubfield.js")
        .Include("~/scripts/custom.js")
        .Include("~/scripts/new.js"));

           
            //Comment this out to control this setting via web.config compilation debug attribute
            BundleTable.EnableOptimizations = true;
        }
    }
}