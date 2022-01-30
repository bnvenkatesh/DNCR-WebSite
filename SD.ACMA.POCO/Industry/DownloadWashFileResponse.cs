using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SD.ACMA.POCO.Base;

namespace SD.ACMA.POCO.Industry
{
    public class DownloadWashFileResponse : BaseWebServiceResponse
    {
        public string ContentType { get; set; }
        public string FileName { get; set; }
        public byte[] Content { get; set; }
        public bool IsExpired { get; set; }
    }
}
