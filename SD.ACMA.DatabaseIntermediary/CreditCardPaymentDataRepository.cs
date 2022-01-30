using SD.ACMA.POCO.PetaPoco;
using SD.Core.Data.Repository.PetaPoco.Business.Interface;
using SD.Core.Data.Repository.PetaPoco.UnityOfWorkContainer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SD.ACMA.DatabaseIntermediary
{
    public interface ICreditCardPaymentDataRepository
    {
        CreditCardPayment[] GetPendingCreditCardPayments();
        CreditCardPayment[] GetPendingCreditCardPayments(int accountId);
        CreditCardPayment[] GetUpdatableCreditCardPayments();
        CreditCardPayment[] GetUpdatableCreditCardPayments(int accountId);
        CreditCardPayment[] GetCreditCardPayments(string orderNumber);
        CreditCardPayment GetCreditCardPayment(string transactionId);
        int InsertCreditCardPayment(CreditCardPayment creditCardPayment);
        int UpdateCreditCardPayment(CreditCardPayment creditCardPayment);
    }

    public class CreditCardPaymentDataRepository : ICreditCardPaymentDataRepository
    {
        private IRepository _repository;
        private IUnitOfWorkProvider _unitOfWorkProvider;

        public CreditCardPaymentDataRepository(IRepository repository, IUnitOfWorkProvider unitOfWorkProvider)
        {
            _repository = repository;
            _unitOfWorkProvider = unitOfWorkProvider;
        }

        public CreditCardPayment[] GetCreditCardPayments(string orderNumber)
        {
            return _repository.Fetch<CreditCardPayment>("WHERE OrderNumber = @0", orderNumber).ToArray();
        }
        public CreditCardPayment GetCreditCardPayment(string transactionId)
        {
            return _repository.Fetch<CreditCardPayment>("WHERE TransactionId = @0", transactionId).FirstOrDefault();
        }

        public CreditCardPayment[] GetPendingCreditCardPayments()
        {
            return _repository.Fetch<CreditCardPayment>("WHERE IsPaymentProcessed = 0").ToArray();
        }
        public CreditCardPayment[] GetPendingCreditCardPayments(int accountId)
        {
            return _repository.Fetch<CreditCardPayment>("WHERE IsPaymentProcessed = 0 AND AccountId = @0", accountId).ToArray();
        }
        public CreditCardPayment[] GetUpdatableCreditCardPayments()
        {
            return _repository.Fetch<CreditCardPayment>("WHERE IsPaymentProcessed = 1 AND IsProcessed = 0").ToArray();
        }
        public CreditCardPayment[] GetUpdatableCreditCardPayments(int accountId)
        {
            return _repository.Fetch<CreditCardPayment>("WHERE IsPaymentProcessed = 1 AND IsProcessed = 0 AND AccountId = @0", accountId).ToArray();
        }

        public int InsertCreditCardPayment(CreditCardPayment creditCardPayment)
        {
            using (var uow = _unitOfWorkProvider.GetUnitOfWork())
            {
                int PaymentAttempt = 0;

                CreditCardPayment[] existingCreditCardPayments = GetCreditCardPayments(creditCardPayment.OrderNumber);

                if (existingCreditCardPayments != null && existingCreditCardPayments.Length > 0)
                {
                    PaymentAttempt = existingCreditCardPayments.Max(p => p.PaymentAttempt);
                }

                creditCardPayment.PaymentAttempt = PaymentAttempt + 1;
                creditCardPayment.TransactionId = string.Format("{0}/{1}", creditCardPayment.OrderNumber, creditCardPayment.PaymentAttempt);
                creditCardPayment.IsPaymentProcessed = false;
                creditCardPayment.IsProcessed = false;
                creditCardPayment.CreatedAt = DateTime.Now;

                var result = _repository.Insert(uow, creditCardPayment);
                uow.Commit();

                return result;
            }
        }
        public int UpdateCreditCardPayment(CreditCardPayment creditCardPayment)
        {
            using (var uow = _unitOfWorkProvider.GetUnitOfWork())
            {
                creditCardPayment.UpdatedAt = DateTime.Now;
                var result = _repository.Update(uow, creditCardPayment, creditCardPayment.Id);
                uow.Commit();

                return result;
            }
        }
    }
}
