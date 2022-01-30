using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SD.ACMA.InterfaceTier;
using SD.ACMA.BusinessLogic.PaymentGateway;
using SD.ACMA.DNCR.Infrastructure;
using Microsoft.Practices.Unity;

namespace SD.ACMA.BusinessLogicTests
{
    [TestClass]
    public class PaymentGatewayServiceUnitTest
    {
        private IPaymentGatewayService _paymentGatewayService; // = new PaymentGatewayService();

        [TestInitialize]
        public void Init()
        {
            IUnityContainer container = new UnityContainer()
                .RegisterType<IPaymentGatewayService, PaymentGatewayService>();

            this._paymentGatewayService = container.Resolve<IPaymentGatewayService>();
        }


        [TestMethod]
        public void TestGetPaymentGatewayUrl()
        {
            string transactionId = "123456789/1";
            string orderInfo = "Test Order";
            int amountInCents = 100;
            string paymentGatewayURL = "http://mvc.local/CS_VPC_3Party_DR.aspx";

            string url = _paymentGatewayService.GetPaymentGatewayUrl(transactionId, orderInfo, amountInCents, paymentGatewayURL);

            Assert.IsFalse(string.IsNullOrEmpty(url));
        }

        [TestMethod]
        public void TestVerifyReceipt()
        {
            //string receipt = "vpc_Amount=7900&vpc_BatchNo=0&vpc_Command=pay&vpc_Locale=en&vpc_MerchTxnRef=1414%2f1&vpc_Merchant=TESTANZACMA&vpc_Message=Cancelled&vpc_OrderInfo=1414&vpc_SecureHash=DDB3DF27B172F2C482DF33661132C534&vpc_TransactionNo=0&vpc_TxnResponseCode=C&vpc_Version=1";
           // string receipt = "vpc_3DSXID=qNGLbJUV%2b2iiFmcKsbl%2bCd9T%2b08%3d&vpc_3DSenrolled=N&vpc_AVSResultCode=Unsupported&vpc_AcqAVSRespCode=Unsupported&vpc_AcqCSCRespCode=Unsupported&vpc_AcqResponseCode=00&vpc_Amount=7900&vpc_AuthorizeId=372803&vpc_BatchNo=20150901&vpc_CSCResultCode=Unsupported&vpc_Card=VC&vpc_Command=pay&vpc_Locale=en&vpc_MerchTxnRef=18906316720%2f1&vpc_Merchant=TESTANZACMA&vpc_Message=Approved&vpc_OrderInfo=18906316720&vpc_ReceiptNo=150901372803&vpc_SecureHash=3BDD07B4C3A64D02887625B52ED3156C&vpc_TransactionNo=230&vpc_TxnResponseCode=0&vpc_VerSecurityLevel=06&vpc_VerStatus=E&vpc_VerType=3DS&vpc_Version=1";
            string receipt = "vpc_3DSXID=qNGLbJUV%2b2iiFmcKsbl%2bCd9T%2b08%3d&vpc_3DSenrolled=N&vpc_AVSResultCode=Unsupported&vpc_AcqAVSRespCode=Unsupported&vpc_AcqCSCRespCode=Unsupported&vpc_AcqResponseCode=00&vpc_Amount=7900&vpc_AuthorizeId=372803&vpc_BatchNo=20150901&vpc_CSCResultCode=Unsupported&vpc_Card=VC&vpc_Command=pay&vpc_Locale=en&vpc_MerchTxnRef=18906316720%2f1&vpc_Merchant=TESTANZACMA&vpc_Message=Approved&vpc_OrderInfo=18906316720&vpc_ReceiptNo=150901372803&vpc_TransactionNo=230&vpc_TxnResponseCode=0&vpc_VerSecurityLevel=06&vpc_VerStatus=E&vpc_VerType=3DS&vpc_Version=1";
            //"vpc_3DSXID=ACxoK2UvHIGfZQX6EWp0p1SNUq0%3D&vpc_3DSenrolled=N&vpc_AVSResultCode=Unsupported&vpc_AcqAVSRespCode=Unsupported&vpc_AcqCSCRespCode=Unsupported&vpc_AcqResponseCode=00&vpc_Amount=100&vpc_AuthorizeId=355034&vpc_BatchNo=20150630&vpc_CSCResultCode=Unsupported&vpc_Card=VC&vpc_Command=pay&vpc_Locale=en&vpc_MerchTxnRef=123456789%2F1&vpc_Merchant=TESTANZACMA&vpc_Message=Approved&vpc_OrderInfo=Test+Order&vpc_ReceiptNo=150630355034&vpc_SecureHash=AABE791C8430D8261DC5D60A67A3E2B6&vpc_TransactionNo=37&vpc_TxnResponseCode=0&vpc_VerSecurityLevel=06&vpc_VerStatus=E&vpc_VerType=3DS&vpc_Version=1";

            string response = null, message = null, receiptNumber = null, transactionId = null, creditCardReference = null, authorizeId = null, transactionType = null, cardType = null;
            int? transactionAmountInCents = null;
            long? transactionNo = null;
            DateTime? settlementDate = null;
            Enums.PaymentStatusEnum paymentStatus = Enums.PaymentStatusEnum.UNKNOWN;

            _paymentGatewayService.VerifyReceipt(receipt, out transactionId, out paymentStatus, out response, out message, out creditCardReference, out receiptNumber, out settlementDate, out transactionNo, out transactionAmountInCents, out authorizeId, out transactionType, out cardType);

            bool success = (paymentStatus == Enums.PaymentStatusEnum.SUCCESS);

            Assert.IsTrue(success);
        }

        [TestMethod]
        public void TestQueryPayment()
        {
            string transactionId = "123456789/1";

            string receipt = null, response = null, message = null, receiptNumber = null, creditCardReference = null, authorizeId = null, transactionType = null, cardType = null;
            long? transactionNo = null;
            int? transactionAmountInCents = null;
            DateTime? settlementDate = null;
            Enums.PaymentStatusEnum paymentStatus = Enums.PaymentStatusEnum.UNKNOWN;

            _paymentGatewayService.QueryPayment(transactionId, out receipt, out paymentStatus, out response, out message, out creditCardReference, out receiptNumber, out settlementDate, out transactionNo, out transactionAmountInCents, out authorizeId, out transactionType, out cardType);

            bool success = (paymentStatus != Enums.PaymentStatusEnum.SUCCESS);

            Assert.IsTrue(success);
        }
    }
}
