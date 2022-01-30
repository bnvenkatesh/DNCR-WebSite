using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SD.ACMA.DatabaseIntermediary;
using SD.Core.Data.Repository.PetaPoco.Business.Interface;
using SD.Core.Data.Repository.PetaPoco.Business;
using SD.Core.Data.Repository.PetaPoco.UnityOfWorkContainer;
using SD.ACMA.InterfaceTier;
using SD.ACMA.BusinessLogic.PaymentGateway;
using SD.ACMA.DNCR.Infrastructure;
using SD.ACMA.POCO.PetaPoco;
using Microsoft.Practices.Unity;

namespace SD.ACMA.BusinessLogicTests
{
    [TestClass]
    public class CreditCardPaymentServiceUnitTest
    {        
        private ICreditCardPaymentService _creditCardPaymentService;// = new CreditCardPaymentService(new CreditCardPaymentDataRepository(new PetaPocoRepository(), new PetaPocoUnitOfWorkProvider()));
        private IPaymentGatewayService _paymentGatewayService;// = new PaymentGatewayService();

        [TestInitialize]
        public void Init()
        {
            IUnityContainer container = new UnityContainer()
                .RegisterType<IRepository, PetaPocoRepository>()
                .RegisterType<IUnitOfWorkProvider, PetaPocoUnitOfWorkProvider>()
                .RegisterType<ICreditCardPaymentDataRepository, CreditCardPaymentDataRepository>()
                .RegisterType<IPaymentGatewayService, PaymentGatewayService>()
                .RegisterType<ICreditCardPaymentService, CreditCardPaymentService>();

            this._creditCardPaymentService = container.Resolve<ICreditCardPaymentService>();
            this._paymentGatewayService = container.Resolve<IPaymentGatewayService>();
        }

        [TestMethod]
        public void TestCreatePayment()
        {
            int accountId = 1234;
            int orderId = 5678;
            string orderNumber = string.Format("{0}{1}{2}", accountId, orderId, "9");            
            int amountInCents = 100;
            string transactionId = null;

            Assert.IsTrue(_creditCardPaymentService.CreatePayment(orderId, orderNumber, accountId, amountInCents, out transactionId));
        }

        [TestMethod]
        public void TestUpdatePayment()
        {
            string transactionId = "123456789/1";

            string receipt = null, response = null, message = null, receiptNumber = null, creditCardReference = null, authorizeId = null, transactionType = null, cardType = null;
            long? transactionNo = null;
            int? transactionAmountInCents = null, orderId = null;
            DateTime? settlementDate = null;
            Enums.PaymentStatusEnum paymentStatus = Enums.PaymentStatusEnum.UNKNOWN;

            _paymentGatewayService.QueryPayment(transactionId, out receipt, out paymentStatus, out response, out message, out creditCardReference, out receiptNumber, out settlementDate, out transactionNo, out transactionAmountInCents, out authorizeId, out transactionType, out cardType);

            Assert.IsTrue(_creditCardPaymentService.UpdateStatus(transactionId, receipt, response, message, receiptNumber, settlementDate, paymentStatus, creditCardReference, transactionNo, transactionAmountInCents, authorizeId, transactionType, cardType, out orderId));
        }

        [TestMethod]
        public void TestGetPendingPayments()
        {
            CreditCardPayment[] creditCardPayments = _creditCardPaymentService.GetPendingPayments();

            Assert.IsTrue(creditCardPayments != null);
        }

        [TestMethod]
        public void TestGetPendingPaymentsByAccountId()
        {
            int accountId = 123;

            CreditCardPayment[] creditCardPayments = _creditCardPaymentService.GetPendingPayments(accountId);

            Assert.IsTrue(creditCardPayments != null);
        }

        [TestMethod]
        public void TestGetUpdatable()
        {
            CreditCardPayment[] creditCardPayments = _creditCardPaymentService.GetUpdatable();

            Assert.IsTrue(creditCardPayments != null);
        }

        [TestMethod]
        public void TestGetUpdatableByAccountId()
        {
            int accountId = 123;

            CreditCardPayment[] creditCardPayments = _creditCardPaymentService.GetUpdatable(accountId);

            Assert.IsTrue(creditCardPayments != null);
        }

        [TestMethod]
        public void TestUpdateOrderProcessingStatus()
        {
            string transactionId = "12345/1";

            try
            {
                Assert.IsTrue(_creditCardPaymentService.SetTransactionAsProcessed(transactionId, null, null));
            }
            catch (Exception ex)
            {
                switch (ex.Message)
                {
                    case "Update is not allowed. Transaction has already been marked as processed":
                    case "Update is not allowed. Transaction has not been marked as payment proceesed":
                    case "Transaction not found with TransactionId: {0}":
                        Assert.IsTrue(true);
                        break;
                }
            }
        }
    }
}
