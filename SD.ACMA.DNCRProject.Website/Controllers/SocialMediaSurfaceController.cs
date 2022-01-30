using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using System.Xml.Serialization;
using SD.ACMA.DNCR.Infrastructure;
using SD.ACMA.DNCRProject.Website.Helpers;
using SD.ACMA.DNCRProject.Website.Models;
using Umbraco.Web;
using Umbraco.Web.Models;
using Umbraco.Web.Mvc;

namespace SD.ACMA.DNCRProject.Website.Controllers
{
    public class SocialMediaSurfaceController : SurfaceController
    {
        private ISocialMediaHelper _socialMediaHelper;

        public SocialMediaSurfaceController(ISocialMediaHelper socialMediaHelper)
        {
            _socialMediaHelper = socialMediaHelper;
        }

        [ChildActionOnly]
        public ActionResult Settings(RenderModel model)
        {
            XElement root = new XElement("socialmedia");

            XElement facebook = new XElement("facebook");
            facebook.Add(new XAttribute("userid", CurrentPage.GetPropertyValue("facebookUserId")));
            facebook.Add(new XAttribute("clientId", CurrentPage.GetPropertyValue("facebookClientId")));
            facebook.Add(new XAttribute("clientSecret", CurrentPage.GetPropertyValue("facebookClientSecret")));
            root.Add(facebook);

            XElement twitter = new XElement("twitter");
            twitter.Add(new XAttribute("userid", CurrentPage.GetPropertyValue("twitterUserId")));
            twitter.Add(new XAttribute("consumerkey", CurrentPage.GetPropertyValue("twitterConsumerKey")));
            twitter.Add(new XAttribute("consumersecret", CurrentPage.GetPropertyValue("twitterConsumerSecret")));
            twitter.Add(new XAttribute("token", CurrentPage.GetPropertyValue("twitterToken")));
            twitter.Add(new XAttribute("tokensecret", CurrentPage.GetPropertyValue("twitterTokenSecret")));
            root.Add(twitter);

            XElement youtube = new XElement("youtube");
            youtube.Add(new XAttribute("username", CurrentPage.GetPropertyValue("youtubeUsername")));
            youtube.Add(new XAttribute("applicationname", CurrentPage.GetPropertyValue("youtubeApplicationName")));
            youtube.Add(new XAttribute("developerkey", CurrentPage.GetPropertyValue("youtubeDeveloperKey")));
            root.Add(youtube);

            return new XmlResult(root);
        }

        [ChildActionOnly]
        public ActionResult HomePage()
        {
            //var model = _socialMediaHelper.GetLatestPosts(Int32.Parse(CurrentPage.GetPropertyValue("twitterNumberOfFeeds").ToString()),
            //    (int)Enums.Media.Twitter, CurrentPage.GetPropertyValue("twitterSearchKeyword").ToString(),
            //    Int32.Parse(CurrentPage.GetPropertyValue("facebookNumberOfFeeds").ToString()),
            //    (int)Enums.Media.Facebook, CurrentPage.GetPropertyValue("facebookSearchKeyword").ToString(),
            //    Int32.Parse(CurrentPage.GetPropertyValue("youtubeNumberOfFeeds").ToString()),
            //    (int)Enums.Media.YouTube, CurrentPage.GetPropertyValue("youtubeSearchKeyword").ToString());

            //var twitterNumberOfPosts = Int32.Parse(CurrentPage.GetPropertyValue("twitterNumberOfFeeds").ToString()) == 0 ? 1 : Int32.Parse(CurrentPage.GetPropertyValue("twitterNumberOfFeeds").ToString());
            //var facebookNumberOfPosts = Int32.Parse(CurrentPage.GetPropertyValue("facebookNumberOfFeeds").ToString()) == 0 ? 1 : Int32.Parse(CurrentPage.GetPropertyValue("facebookNumberOfFeeds").ToString());
            //var youTubeNumberOfPosts = Int32.Parse(CurrentPage.GetPropertyValue("youtubeNumberOfFeeds").ToString()) == 0 ? 1 : Int32.Parse(CurrentPage.GetPropertyValue("youtubeNumberOfFeeds").ToString());

            //var model = _socialMediaHelper.GetLatestPosts(twitterNumberOfPosts,
            //    (int)Enums.Media.Twitter, CurrentPage.GetPropertyValue("twitterSearchKeyword").ToString(),
            //    facebookNumberOfPosts,
            //    (int)Enums.Media.Facebook, CurrentPage.GetPropertyValue("facebookSearchKeyword").ToString(),
            //    youTubeNumberOfPosts,
            //    (int)Enums.Media.YouTube, CurrentPage.GetPropertyValue("youtubeSearchKeyword").ToString());

            

            return PartialView("HomePage", null);
        }
    }

    public class XmlResult : ActionResult
    {
        private object _objectToSerialize;

        public XmlResult(object objectToSerialize)
        {
            _objectToSerialize = objectToSerialize;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            if (_objectToSerialize != null)
            {
                var xs = new XmlSerializer(_objectToSerialize.GetType());
                context.HttpContext.Response.ContentType = "text/xml";
                xs.Serialize(context.HttpContext.Response.Output, _objectToSerialize);
            }
        }
    }
}