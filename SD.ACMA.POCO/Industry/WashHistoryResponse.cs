using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SD.ACMA.POCO.Base;

namespace SD.ACMA.POCO.Industry
{
    public class WashHistoryResponse : BaseWebServiceResponse
    {
        public List<WashHistoryResponseObject> WashHistoryResponseObjects { get; set; }
    }

    public class WashHistoryResponseObject
    {
        public int AccountUserId { get; set; }
        public string AllInOneFile { get; set; }
        public int AllInOneNumbersCount { get; set; }
        public bool CanDownload { get; set; }
        public string FileName { get; set; }
        public bool HasRefunded { get; set; }
        public int InvalidNumbersCount { get; set; }
        public string InvalidNumbersFile { get; set; }
        public int NumbersWashed { get; set; }
        public string OriginalFile { get; set; }
        public int RegisteredNumbersCount { get; set; }
        public string RegisteredNumbersFile { get; set; }
        public DateTime RequestDate { get; set; }
        public int UnregisteredNumbersCount { get; set; }
        public string UnregisteredNumbersFile { get; set; }
        public SD.ACMA.DNCR.Infrastructure.Enums.WashingChannelEnum WashingChannel { get; set; }
        public int WashingRequestId { get; set; }
        public string AccountUserName { get; set; }
        public string ClientReference { get; set; }
    }
}
