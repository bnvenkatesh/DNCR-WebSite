using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SD.ACMA.POCO.Base;

namespace SD.ACMA.POCO.Industry
{
    public class PreWashResponse : BaseWebServiceResponse
    {
        public bool FileNameAlreadyExists { get; set; }
        public bool HasInValidNumbers { get; set; }
        public bool HasSufficientWashingCredits { get; set; }
        public bool HasValidSubscription { get; set; }
        public bool IsFileFormatNotValid { get; set; }
        public bool IsFileNameNotValid { get; set; }
        public bool IsFileSizeExceedLimit { get; set; }
        public bool IsSuccessful { get; set; }
        public string RelativeInputFilePath { get; set; }
        public int RequiredWashingCredits { get; set; }
        public bool ShowWashQuote { get; set; }
        public DateTime Timestamp { get; set; }
        public int WashingRequestId { get; set; }
        public int AccountWashingCreditsBalance { get; set; }
    }
}
