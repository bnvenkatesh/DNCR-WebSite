using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SD.ACMA.POCO.Consumer
{
    public class AttachMailResponseRequest
    {
        public string From { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string Token { get; set; }
        public byte[] EmailData { get; set; }
        public bool IsEnquiry { get; set; }
    }
}
