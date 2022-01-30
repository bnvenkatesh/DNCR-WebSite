using SD.ACMA.DNCR.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SD.ACMA.InterfaceTier
{
    public interface IPaymentGatewayService
    {
        string GetPaymentGatewayUrl(string transactionId, string orderInfo, int amountInCents, string returnUrl);
        void VerifyReceipt(string receipt, out string transactionId, out Enums.PaymentStatusEnum paymentStatus, out string response, out string message, out string creditCardReference, out string receiptNumber, out DateTime? settlementDate, out long? transactionNo, out int? transactionAmountInCents, out string authorizeId, out string transactionType, out string cardType);
        void QueryPayment(string transactionId, out string receipt, out Enums.PaymentStatusEnum paymentStatus, out string response, out string message, out string creditCardReference, out string receiptNumber, out DateTime? settlementDate, out long? transactionNo, out int? transactionAmountInCents, out string authorizeId, out string transactionType, out string cardType);
    }
}
