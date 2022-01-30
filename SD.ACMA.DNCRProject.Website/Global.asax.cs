using SD.ACMA.DNCRProject.Website.App_Start;
using System.Net;
using System.Web.Optimization;
using Umbraco.Web;

namespace SD.ACMA.DNCRProject.Website
{
    public partial class Global : UmbracoApplication
    {
        

        protected override void OnApplicationStarting(object sender, System.EventArgs e)
        {
            Bootstrapper.Initialise();
            base.OnApplicationStarting(sender, e);
        }

        protected override void OnApplicationStarted(object sender, System.EventArgs e)
        {
            base.OnApplicationStarted(sender, e);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            //Below line will upgrade current TLS 1.0 to TLS 1.1 or higher on whole application level
            ServicePointManager.SecurityProtocol =
                SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3;
        }
    }

}