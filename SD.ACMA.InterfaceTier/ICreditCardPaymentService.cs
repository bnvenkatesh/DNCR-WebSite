using SD.ACMA.DNCR.Infrastructure;
using SD.ACMA.POCO.PetaPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SD.ACMA.InterfaceTier
{
    public interface ICreditCardPaymentService
    {
        CreditCardPayment[] GetPendingPayments();
        CreditCardPayment[] GetPendingPayments(int accountId);
        CreditCardPayment[] GetUpdatable();
        CreditCardPayment[] GetUpdatable(int accountId);
        CreditCardPayment GetCreditCardPayment(string transactionId);

        bool CreatePayment(int orderId, string orderNumber, int accountId, int amountInCents, out string transactionId);
        bool UpdateStatus(string transactionId, string receiptData, string responseCode, string message, string receiptNo, DateTime? settlementDate, Enums.PaymentStatusEnum paymentStatus, string creditCardReference, long? transactionNo, int? transactionAmountInCents, string authorizeId, string transactionType, string cardType, out int? orderId);
        bool SetTransactionAsProcessed(string transactionId, bool? portalResponseSuccess, string portalResponseError);
    }
}
