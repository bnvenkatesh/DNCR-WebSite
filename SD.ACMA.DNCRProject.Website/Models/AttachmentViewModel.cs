using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SD.ACMA.DNCRProject.Website.Models
{
    public class AttachmentViewModel
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public string LowResImage { get; set; }
        public string StandardImage { get; set; }
        public string Thumbnail { get; set; }
        public string Link { get; set; }
        public string Type { get; set; }
    }
}