using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SD.ACMA.DNCRProject.Website.Models
{
    public class PostViewModel
    {
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

        public List<AttachmentViewModel> Attachments { get; set; }
    }
}