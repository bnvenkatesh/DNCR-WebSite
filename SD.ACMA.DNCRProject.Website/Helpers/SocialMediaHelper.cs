using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using SD.ACMA.DatabaseIntermediary;
using SD.ACMA.DNCR.Infrastructure;
using SD.ACMA.DNCRProject.Website.Models;

namespace SD.ACMA.DNCRProject.Website.Helpers
{
    public interface ISocialMediaHelper
    {
        List<SocialMediaViewModel> GetLatestPosts(int twitterNumberOfPosts, int twitterMediaId, string twitterSearchTerm, int facebookNumberOfPosts, int facebookMediaId, string facebookSearchTerm,
            int youTubeNumberOfPosts, int youTubeMediaId, string youTubeSearchTerm);
    }

    public class SocialMediaHelper : ISocialMediaHelper
    {
        private IPostService _postService;

        public SocialMediaHelper(IPostService postService)
        {
            _postService = postService;
        }

        public List<SocialMediaViewModel> GetLatestPosts(int twitterNumberOfPosts, int twitterMediaId, string twitterSearchTerm, int facebookNumberOfPosts, int facebookMediaId, string facebookSearchTerm,
            int youTubeNumberOfPosts, int youTubeMediaId, string youTubeSearchTerm)
        {
            var posts = _postService.SearchPosts(twitterNumberOfPosts, twitterMediaId, twitterSearchTerm,
                facebookNumberOfPosts, facebookMediaId, facebookSearchTerm,
                youTubeNumberOfPosts, youTubeMediaId, youTubeSearchTerm).OrderByDescending(x => x.CreatedOn);

            if (posts == null)
                return null;

            var model = new List<SocialMediaViewModel>();

            foreach (var item in posts)
            {
                var p = new SocialMediaViewModel
                {
                    CreatedOn = item.CreatedOn,
                    Id = item.Id,
                    InsertedOn = item.InsertedOn,
                    MediaId = item.MediaId,
                    Name = item.Name,
                    ProfilePic = item.ProfilePic,
                    Source = item.Source,
                    Text = item.Text,
                    Title = item.Title,
                    UniqueId = item.UniqueId,
                    UserId = item.UserId,
                    Username = item.Username,
                    Thumbnail = item.Thumbnail
                };

                //Convert time to PST for youtube
                if (item.MediaId == (int) Enums.Media.YouTube)
                {
                    item.CreatedOn = DateTime.SpecifyKind(item.CreatedOn, DateTimeKind.Unspecified);
                    item.CreatedOn = TimeZoneInfo.ConvertTime(item.CreatedOn, TimeZoneInfo.FindSystemTimeZoneById("AUS Eastern Standard Time"), TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time"));
                }

                var span = DateTime.Now.Subtract(item.CreatedOn);
                if (span.Days > 0)
                {
                    if (span.Hours > 12)
                    {
                        p.HowLongAgo = String.Format("{0} days ago", span.Days + 1);
                    }
                    else p.HowLongAgo = span.Days == 1 ? String.Format("{0} day ago", span.Days) : String.Format("{0} days ago", span.Days);
                } else if (span.Hours > 0)
                {
                    if (span.Minutes > 30)
                    {
                        p.HowLongAgo = String.Format("{0} hours ago", span.Hours + 1);
                    }
                    else p.HowLongAgo = span.Hours == 1 ? String.Format("{0} hour ago", span.Hours) : String.Format("{0} hours ago", span.Hours);
                } else if (span.Minutes > 0)
                {
                    p.HowLongAgo = span.Minutes == 1 ? String.Format("{0} minute ago", span.Minutes) : String.Format("{0} minutes ago", span.Minutes);
                } else if (span.Seconds > 0)
                {
                    p.HowLongAgo = span.Seconds == 1 ? String.Format("{0} second ago", span.Seconds) : String.Format("{0} seconds ago", span.Seconds);
                }

                //for http
                p.Text = Regex.Replace(item.Text, @"((http|ftp|https):\/\/[\w\-_]+(\.[\w\-_]+)+([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])?)", "<a target='_blank' href='$&'>$&</a>");

                //for www
                p.Text = Regex.Replace(p.Text, @"([\w\-_]+(\.[\w\-_]+)+([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])? )", "<a target='_blank' href='http://$&'>$&</a>");

                //for hashtag
                if (item.MediaId == (int) Enums.Media.Facebook)
                {
                    p.Text = Regex.Replace(p.Text, @"#([A-Za-z0-9_]*)", "<a target='_blank' href='http://www.facebook.com/hashtag/$&'>$&</a>");
                }
                else if (item.MediaId == (int) Enums.Media.Twitter)
                {
                    p.Text = Regex.Replace(p.Text, @"#([A-Za-z0-9_]*)", "<a target='_blank' href='http://twitter.com/hashtag/$&'>$&</a>");
                }
                p.Text = p.Text.Replace("/#", "/");
                
                if (item.Attachments != null)
                {
                    var attachmentList = new List<AttachmentViewModel>();

                    foreach (var x in item.Attachments)
                    {
                        var y = new AttachmentViewModel
                        {
                            Id = x.Id,                            
                            LowResImage = x.LowResImage,
                            PostId = x.PostId,
                            StandardImage = x.StandardImage,
                            Thumbnail = x.Thumbnail,
                            Type = x.Type
                        };

                        if (item.MediaId == (int)Enums.Media.YouTube)
                        {
                            y.Link = string.Format("https://www.youtube.com/embed/{0}", x.Link.Substring(x.Link.IndexOf("?v=") + 3));
                        }
                        else
                            y.Link = x.Link;

                        attachmentList.Add(y);
                    }

                    p.Attachments = attachmentList;
                }

                model.Add(p);
            }

            return model;
        }
    }
}