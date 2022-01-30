using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SD.ACMA.DNCR.Infrastructure;
using SD.ACMA.DNCR.Infrastructure.Configuration;
using SD.ACMA.DNCRProject.Website.Helpers;
using SD.ACMA.DNCRProject.Website.Models;
using SD.ACMA.InterfaceTier;
using umbraco.presentation.webservices;
using Umbraco.Core.Models;
using Umbraco.Web;
using Umbraco.Web.Mvc;

namespace SD.ACMA.DNCRProject.Website.Controllers
{
    public class AgentSurfaceController : SurfaceController
    {
        private IIndustryDataInterchange _industryDataInterchange;
        private IConsumerDataInterchange _consumerDataInterchange;
        private IUserContextHelper _userContextHelper;

        public AgentSurfaceController(IIndustryDataInterchange industryDataInterchange, IConsumerDataInterchange consumerDataInterchange, IUserContextHelper userContextHelper)
        {
            _industryDataInterchange = industryDataInterchange;
            _consumerDataInterchange = consumerDataInterchange;
            _userContextHelper = userContextHelper;
        }

        public ActionResult AccessSeeker(string accountUserId, string agentData, string token)
        {
            int userId;
            if (!String.IsNullOrEmpty(token) && Int32.TryParse(accountUserId, out userId) && !String.IsNullOrEmpty(agentData))
            {
                var result = _industryDataInterchange.Impersonate(userId, WebUtility.UrlEncode(agentData), WebUtility.UrlEncode(token));

                if (result.Errors == null && result.IsSuccessful)
                {
                    SessionHelper.ClearSessions();
                    SessionHelper.AgentId = result.AgentId;

                    var ucm = _userContextHelper.CreateUserContextObject(result.AccountUserId, result.AgentId);
                    var getUserResult = _industryDataInterchange.GetAccountUser(userId, ucm);
                    if (getUserResult != null)
                    {
                        SessionHelper.SetLoginSessions(result.AccountId, result.AccountUserId, result.IsAdmin, getUserResult.EmailAddress);
                    }
                    else SessionHelper.SetLoginSessions(result.AccountId, result.AccountUserId, result.IsAdmin, null);
                    
                    SessionHelper.IsAcma = result.Organisation == Enums.OrganisationEnum.ACMA;

                    var home = CurrentPage.AncestorOrSelf(1);
                    IPublishedContent accountDashboardNode = Umbraco.Content(home.GetPropertyValue("accountDashboardNode"));
                    ViewBag.RedirectUrl = accountDashboardNode.Url;
                    return PartialView("Redirect");
                }
            }
            else ControllerContext.HttpContext.Response.Redirect("/");
            return null;
        }

        public ActionResult LodgeEnquiry(string token)
        {
            if (!String.IsNullOrEmpty(token))
            {
                var result = _consumerDataInterchange.ImpersonateCSR(token);

                if (result.Errors == null && result.IsSuccessful)
                {
                    SessionHelper.ClearSessions();
                    SessionHelper.AgentId = result.AgentId;
                    SessionHelper.IsAcma = result.IsAcma;

                    var home = CurrentPage.AncestorOrSelf(1);
                    IPublishedContent consumerLodgeEnquiryNode = Umbraco.Content(home.GetPropertyValue("consumerLodgeEnquiryNode"));
                    ViewBag.RedirectUrl = consumerLodgeEnquiryNode.Url;
                    return PartialView("Redirect");
                }
            }
            else ControllerContext.HttpContext.Response.Redirect("/");
            return null;
        }

        public ActionResult LodgeComplaints(string token)
        {
            if (!String.IsNullOrEmpty(token))
            {
                var result = _consumerDataInterchange.ImpersonateCSR(token);

                if (result.Errors == null && result.IsSuccessful)
                {
                    SessionHelper.ClearSessions();
                    SessionHelper.AgentId = result.AgentId;
                    SessionHelper.IsAcma = result.IsAcma;

                    var home = CurrentPage.AncestorOrSelf(1);
                    IPublishedContent consumerLodgeComplaintNode = Umbraco.Content(home.GetPropertyValue("consumerLodgeComplaintNode"));
                    ViewBag.RedirectUrl = consumerLodgeComplaintNode.Url;
                    return PartialView("Redirect");
                }
            }
            else ControllerContext.HttpContext.Response.Redirect("/");
            return null;
        }
    }
}