using SD.ACMA.DatabaseIntermediary;
using SD.ACMA.DNCR.Infrastructure;
using SD.ACMA.InterfaceTier;
using SD.ACMA.POCO.PetaPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SD.ACMA.BusinessLogic.PaymentGateway
{
    public class CreditCardPaymentService : ICreditCardPaymentService
    {
        private ICreditCardPaymentDataRepository _creditCardPaymentDataRepository;

        public CreditCardPaymentService(ICreditCardPaymentDataRepository creditCardPaymentDataRepository)
        {
            _creditCardPaymentDataRepository = creditCardPaymentDataRepository;
        }

        public CreditCardPayment[] GetPendingPayments()
        {
            CreditCardPayment[] creditCardPayments = null;

            creditCardPayments = _creditCardPaymentDataRepository.GetPendingCreditCardPayments();

            return creditCardPayments;
        }

        public CreditCardPayment[] GetPendingPayments(int accountId)
        {
            CreditCardPayment[] creditCardPayments = null;

            creditCardPayments = _creditCardPaymentDataRepository.GetPendingCreditCardPayments(accountId);

            return creditCardPayments;
        }

        public CreditCardPayment[] GetUpdatable()
        {
            CreditCardPayment[] creditCardPayments = null;

            creditCardPayments = _creditCardPaymentDataRepository.GetUpdatableCreditCardPayments();

            return creditCardPayments;
        }

        public CreditCardPayment[] GetUpdatable(int accountId)
        {
            CreditCardPayment[] creditCardPayments = null;

            creditCardPayments = _creditCardPaymentDataRepository.GetUpdatableCreditCardPayments(accountId);

            return creditCardPayments;
        }

        public CreditCardPayment GetCreditCardPayment(string transactionId)
        {
            CreditCardPayment creditCardPayment = null;

            creditCardPayment = _creditCardPaymentDataRepository.GetCreditCardPayment(transactionId);

            return creditCardPayment;
        }

        public bool CreatePayment(int orderId, string orderNumber, int accountId, int amountInCents, out string transactionId)
        {
            bool success = false;

            transactionId = null;

            CreditCardPayment creditCardPayment = new CreditCardPayment();
            creditCardPayment.OrderId = orderId;
            creditCardPayment.OrderNumber = orderNumber;
            creditCardPayment.AccountId = accountId;
            creditCardPayment.AmountInCents = amountInCents;

            creditCardPayment.TransactionStatus = Enums.PaymentStatusEnum.UNKNOWN.ToString();
            creditCardPayment.IsPaymentProcessed = false;

            success = (_creditCardPaymentDataRepository.InsertCreditCardPayment(creditCardPayment) > 0);

            if (success)
            {
                transactionId = creditCardPayment.TransactionId;
            }

            return success;
        }

        public bool UpdateStatus(string transactionId, string receiptData, string responseCode, string message, string receiptNo, DateTime? settlementDate, Enums.PaymentStatusEnum paymentStatus, string creditCardReference, long? transactionNo, int? transactionAmountInCents, string authorizeId, string transactionType, string cardType, out int? orderId)
        {
            bool success = false;
            orderId = null;

            CreditCardPayment creditCardPayment = _creditCardPaymentDataRepository.GetCreditCardPayment(transactionId);

            if (creditCardPayment != null)
            {
                creditCardPayment.ReceiptData = receiptData;
                creditCardPayment.ResponseCode = responseCode;
                creditCardPayment.Message = message;
                creditCardPayment.ReceiptNo = receiptNo;
                creditCardPayment.SettlementDate = settlementDate;
                creditCardPayment.TransactionStatus = paymentStatus.ToString();
                creditCardPayment.CreditCardReference = creditCardReference;
                creditCardPayment.TransactionNo = transactionNo;
                creditCardPayment.TransactionAmountInCents = transactionAmountInCents;
                creditCardPayment.AuthorizeID = authorizeId;
                creditCardPayment.TransactionType = transactionType;
                creditCardPayment.CardType = cardType;

                switch (paymentStatus)
                {
                    case Enums.PaymentStatusEnum.UNKNOWN:
                        creditCardPayment.IsPaymentProcessed = false;
                        break;
                    case Enums.PaymentStatusEnum.TIMEOUT:
                    case Enums.PaymentStatusEnum.SUCCESS:
                    case Enums.PaymentStatusEnum.DECLINED:
                        creditCardPayment.IsPaymentProcessed = true;
                        break;
                }

                success = (_creditCardPaymentDataRepository.UpdateCreditCardPayment(creditCardPayment) > 0);

                orderId = creditCardPayment.OrderId;
            }
            else
            {
                throw new Exception(string.Format("Transaction not found with TransactionId: {0}", transactionId));
            }

            return success;
        }

        public bool SetTransactionAsProcessed(string transactionId, bool? portalResponseSuccess, string portalResponseError)
        {
            bool success = false;

            CreditCardPayment creditCardPayment = _creditCardPaymentDataRepository.GetCreditCardPayment(transactionId);

            if (creditCardPayment != null)
            {
                if (creditCardPayment.IsPaymentProcessed)
                {
                    if (!creditCardPayment.IsProcessed)
                    {
                        creditCardPayment.IsProcessed = true;
                        creditCardPayment.PortalResponseSuccess = portalResponseSuccess;
                        creditCardPayment.PortalResponseError = portalResponseError;

                        success = (_creditCardPaymentDataRepository.UpdateCreditCardPayment(creditCardPayment) > 0);
                    }
                    else
                    {
                        throw new Exception(string.Format("Update is not allowed. Transaction has already been marked as processed"));
                    }
                }
                else
                {
                    throw new Exception(string.Format("Update is not allowed. Transaction has not been marked as payment proceesed"));
                }
            }
            else
            {
                throw new Exception(string.Format("Transaction not found with TransactionId: {0}", transactionId));
            }

            return success;
        }
    }
}
