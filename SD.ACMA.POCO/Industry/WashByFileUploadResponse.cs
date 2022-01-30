using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SD.ACMA.POCO.Base;

namespace SD.ACMA.POCO.Industry
{
    public class WashByFileUploadResponse : BaseWebServiceResponse
    {
        public string AllInOneFile { get; set; }
        public bool HasSufficientWashingCredits { get; set; }
        public bool HasValidSubscription { get; set; }
        public string InvalidNumbersFile { get; set; }
        public bool IsSuccessful { get; set; }
        public string OriginalFile { get; set; }
        public string RegisteredNumbersFile { get; set; }
        public SD.ACMA.DNCR.Infrastructure.Enums.WashingStatusEnum Status { get; set; }
        public string UnregisteredNumbersFile { get; set; }
        public int WashCredits { get; set; }
        public int WashingRequestId { get; set; }
    }
}
