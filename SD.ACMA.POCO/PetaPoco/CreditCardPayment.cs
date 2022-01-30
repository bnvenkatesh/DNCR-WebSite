using PetaPoco;
using SD.ACMA.DNCR.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SD.ACMA.POCO.PetaPoco
{
    [TableName("CreditCardPayment")]
    [PrimaryKey("Id")]
    public class CreditCardPayment
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public int OrderId { get; set; }
        public string OrderNumber { get; set; }
        public int PaymentAttempt { get; set; }
        public string TransactionId { get; set; }
        public int AmountInCents { get; set; }
        public bool IsPaymentProcessed { get; set; }
        public string ReceiptData { get; set; }
        public string ResponseCode { get; set; }
        public string CreditCardReference { get; set; }
        public string Message { get; set; }
        public string ReceiptNo { get; set; }
        public DateTime? SettlementDate { get; set; }
        public string TransactionStatus { get; set; }
        public long? TransactionNo { get; set; }
        public int? TransactionAmountInCents { get; set; }
        public string AuthorizeID { get; set; }
        public string TransactionType { get; set; }
        public string CardType { get; set; }
        public bool IsProcessed { get; set; }
        public bool? PortalResponseSuccess { get; set; }
        public string PortalResponseError { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
