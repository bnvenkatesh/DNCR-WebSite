using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SD.ACMA.POCO.Industry
{
    public class AccountTransactionModel
    {
        public int AccountID { get; set; }
        public int AccountTransactionID { get; set; }
        public SD.ACMA.DNCR.Infrastructure.Enums.AccountTransactionTypeEnum AccountTransactionType { get; set; }
        public decimal? Amount { get; set; }
        public DateTime CreatedTimeStamp { get; set; }
        public int? WashCredits { get; set; }
        public string Reference { get; set; }
    }
}
