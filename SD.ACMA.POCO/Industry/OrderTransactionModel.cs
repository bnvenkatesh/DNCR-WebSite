using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SD.ACMA.POCO.Industry
{
    public class OrderTransactionModel
    {
        public decimal Amount { get; set; }
        public SD.ACMA.DNCR.Infrastructure.Enums.OrderTransactionTypeEnum OrdeTransactionTypeName { get; set; }
        public DateTime CreatedTimeStamp { get; set; }
    }
}
