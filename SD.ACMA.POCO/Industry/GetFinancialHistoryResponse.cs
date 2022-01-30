using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SD.ACMA.POCO.Base;

namespace SD.ACMA.POCO.Industry
{
    public class GetFinancialHistoryResponse : BaseWebServiceResponse
    {
        public AccountAdjustmentModel AccountAdjustment { get; set; }
        public List<AccountAdjustmentTypesModel> AccountAdjustmentTypes { get; set; }
        public List<OrderModel> Orders { get; set; }
        public string ResultMessage { get; set; }
        public List<AccountTransactionModel> TransactionHistory { get; set; }
    }
}
