using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SD.ACMA.DNCRProject.Website.Models
{
    public class SocialMediaViewModel
    {
        //public string FacebookLink { get; set; }
        //public string FacebookSearchKeyword { get; set; }
        //public int FacebookNumberOfFeeds { get; set; }

        //public string TwitterLink { get; set; }
        //public string TwitterSearchKeyword { get; set; }
        //public int TwitterNumberOfFeeds { get; set; }

        //public string YouTubeLink { get; set; }
        //public string YouTubeSearchKeyword { get; set; }
        //public int YouTubeNumberOfFeeds { get; set; }

        public int Id { get; set; }
        public int MediaId { get; set; }
        public string UniqueId { get; set; }
        public string UserId { get; set; }
        public string Username { get; set; }
        public string Name { get; set; }
        public string ProfilePic { get; set; }
        public string Text { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime InsertedOn { get; set; }
        public string Source { get; set; }
        public string Title { get; set; }
        public string Thumbnail { get; set; }
        public string HowLongAgo { get; set; }

        public List<AttachmentViewModel> Attachments { get; set; }
    }
}