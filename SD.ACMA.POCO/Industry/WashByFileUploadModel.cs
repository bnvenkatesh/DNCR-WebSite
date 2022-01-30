using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SD.ACMA.POCO.Industry
{
    public class WashByFileUploadModel
    {
        public int AccountId { get; set; }
        public int WashingRequestId { get; set; }
        public bool WashResultFormatFileWithIndicators { get; set; }
        public bool WashResultFormatInvalidNumbers { get; set; }
        public bool WashResultFormatRegisteredNumbers { get; set; }
        public bool WashResultFormatUnregisteredNumbers { get; set; }
    }
}
