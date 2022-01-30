using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SD.ACMA.DNCRProject.FinancialReports.Startup))]
namespace SD.ACMA.DNCRProject.FinancialReports
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
