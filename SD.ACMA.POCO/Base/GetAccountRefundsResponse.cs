using System.Collections.Generic;
using SD.ACMA.POCO.Industry;
namespace SD.ACMA.POCO.Base
{
    public class GetAccountRefundsResponse : BaseWebServiceResponse
    {
        public List<AccountRefundsModel> Refunds { get; set; }
        public int AccountID { get; set; }
    }
}
