using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SD.ACMA.DNCRProject.Website.HealthCheck;

namespace SD.ACMA.DNCRProject.Website.SDSupport
{
    public partial class HealthCheck : BaseHealthCheckPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            BaseHealthCheck healthCheck = new ACMAHealthCheck();
            this.litHealthcheck.Text = DoHealthCheck(healthCheck);
        }
    }
}