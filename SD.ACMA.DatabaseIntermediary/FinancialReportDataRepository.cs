using SD.ACMA.DNCR.Infrastructure.Configuration;
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
    public interface IFinancialReportDataRepository
    {
        List<FinancialReport> GetAllFinancialReports();
        int InsertFinancialReport(FinancialReport financialReport);
        int UpdateFinancialReport(FinancialReport financialReport);
        FinancialReport GetFinancialReport(int id);
        FinancialReport GetLastFinancialReport();
        FinancialReport GetPreviousFinancialReport(DateTime previousToDate);

        #region Payments Reconciliation Report

        double GetCreditCardSettlements(DateTime? fromDate, DateTime toDate);
        double GetTotalCashReceipts_UNKNOWN(DateTime? fromDate, DateTime toDate);
        double GetTotalCashReceipts_BPAY_EFT(DateTime? fromDate, DateTime toDate);

        List<FinancialReport_Payment> GetExceptionUnmatchedPayments(DateTime? fromDate, DateTime toDate);
        List<FinancialReport_Payment> GetExceptionTransferToCreditNotes(DateTime? fromDate, DateTime toDate);
        List<FinancialReport_Payment> GetExceptionUnderpayments(DateTime? fromDate, DateTime toDate);
        List<FinancialReport_Payment> GetExceptionReversedDishonouredPayments(DateTime? fromDate, DateTime toDate);
        List<FinancialReport_Payment> GetExceptionErroneousPayments(DateTime? fromDate, DateTime toDate);
        List<FinancialReport_Payment> GetExceptionCreditCardDeferredReceivedPayments(DateTime? fromDate, DateTime toDate);

        List<FinancialReport_Payment> GetNewCreditCardDeferralPayments(DateTime? fromDate, DateTime toDate);

        List<FinancialReport_Payment> GetDishonouredPayments(DateTime? fromDate, DateTime toDate);
        List<FinancialReport_Payment> GetCreditNotesAppliedToSalesPayments(DateTime? fromDate, DateTime toDate);
        List<FinancialReport_Payment> GetClearedPreviousException_MatchedPayments(DateTime? fromDate, DateTime toDate);
        List<FinancialReport_Payment> GetClearedPreviousException_Underpayments(DateTime? fromDate, DateTime toDate);

        double GetTotalCreditCardActivationsPayments(DateTime? fromDate, DateTime toDate);
        double GetTotalEFTActivationsPayments(DateTime? fromDate, DateTime toDate);
        double GetTotalBPayActivationsPayments(DateTime? fromDate, DateTime toDate);
        double GetTotalFundTransferActivationsPayments(DateTime? fromDate, DateTime toDate);
        double GetTotalOtherActivationsPayments(DateTime? fromDate, DateTime toDate);
        double GetTotalActivationsPayments(DateTime? fromDate, DateTime toDate);

        IList<ActivatedSubscription> GetAllActivationsPayments(DateTime? fromDate, DateTime toDate);

        List<FinancialReport_Payment> GetRefundedPayments(DateTime? fromDate, DateTime toDate);

        #endregion

        #region Exceptions Report

        List<FinancialReport_Exception> GetExceptionUnmatchedAmounts(DateTime? fromDate, DateTime toDate);
        List<FinancialReport_Exception> GetExceptionTransferredFromToCreditNotes(DateTime? fromDate, DateTime toDate);
        List<FinancialReport_Exception> GetExceptionDeferredCreditCardReceipts(DateTime? fromDate, DateTime toDate);

        #endregion

        #region Credit Notes Report

        List<FinancialReport_CreditNote> GetNewCreditNoteTransactions(DateTime? fromDate, DateTime toDate);
        double GetTotalCreditNotesAppliedToPurchasesToday(DateTime? fromDate, DateTime toDate);
        double GetTotalCreditNotesRefundedToday(DateTime? fromDate, DateTime toDate);
        double GetTotalCreditNotesCreatedToday(DateTime? fromDate, DateTime toDate);
        List<FinancialReport_CreditNote> GetCreditNoteRegister(DateTime toDate);

        #endregion
    }

    public partial class FinancialReportDataRepository : IFinancialReportDataRepository
    {
        private const string _dateTimeFormat = "yyyy/MM/dd hh:mm:ss tt";
        private const string _dateFormat = "yyyy/MM/dd";

        private IRepository _repository;
        private IUnitOfWorkProvider _unitOfWorkProvider;

        public FinancialReportDataRepository(IRepository repository, IUnitOfWorkProvider unitOfWorkProvider)
        {
            _repository = repository;
            _unitOfWorkProvider = unitOfWorkProvider;
        }

        public List<FinancialReport> GetAllFinancialReports()
        {
            _repository.ConnectToDb(ConfigurationHelper.Instance.ConnectionStringName);

            return _repository.Fetch<FinancialReport>().OrderByDescending(p => p.Id).ToList();
        }
        public int InsertFinancialReport(FinancialReport financialReport)
        {
            _repository.ConnectToDb(ConfigurationHelper.Instance.ConnectionStringName);

            using (var uow = _unitOfWorkProvider.GetUnitOfWork())
            {
                var result = _repository.Insert(uow, financialReport);
                uow.Commit();

                return result;
            }
        }
        public int UpdateFinancialReport(FinancialReport financialReport)
        {
            _repository.ConnectToDb(ConfigurationHelper.Instance.ConnectionStringName);

            using (var uow = _unitOfWorkProvider.GetUnitOfWork())
            {
                var result = _repository.Update(uow, financialReport, financialReport.Id);
                uow.Commit();

                return result;
            }
        }
        public FinancialReport GetFinancialReport(int id)
        {
            _repository.ConnectToDb(ConfigurationHelper.Instance.ConnectionStringName);

            return _repository.Fetch<FinancialReport>().Where(p => p.Id == id).FirstOrDefault();
        }
        public FinancialReport GetLastFinancialReport()
        {
            _repository.ConnectToDb(ConfigurationHelper.Instance.ConnectionStringName);

            return _repository.Fetch<FinancialReport>().OrderByDescending(p => p.Id).FirstOrDefault();
        }
        public FinancialReport GetPreviousFinancialReport(DateTime previousToDate)
        {
            _repository.ConnectToDb(ConfigurationHelper.Instance.ConnectionStringName);

            return _repository.Fetch<FinancialReport>().Where(p => p.ToDate == previousToDate).FirstOrDefault();
        }

        #region Payments Reconciliation Report

        public double GetCreditCardSettlements(DateTime? fromDate, DateTime toDate)
        {
            _repository.ConnectToDb(ConfigurationHelper.Instance.ConnectionStringName_Portal);

            StringBuilder sb = new StringBuilder();

            //sb.AppendLine("SELECT ISNULL(SUM(CASE WHEN PE.PaymentExceptionID IS NOT NULL THEN 0 ELSE P.PaidAmount END), 0)");
            //sb.AppendLine("FROM subscription.Payment P");
            //sb.AppendLine("INNER JOIN subscription.PaymentMethod PM");
            //sb.AppendLine("ON PM.PaymentMethodID = P.PaymentMethodID");
            //sb.AppendLine("AND PM.PaymentMethodName = 'CreditCard'");
            //sb.AppendLine("LEFT OUTER JOIN subscription.PaymentException PE");
            //sb.AppendLine("ON PE.PaymentFeedEntryID = P.PaymentFeedEntryID");
            //sb.AppendLine("WHERE P.ExpectedSettelmentDate BETWEEN @0 AND @1");
            //sb.AppendLine("AND P.IsPaymentSuccess = 1");

            sb.AppendLine("SELECT ISNULL(SUM(PFE.Amount), 0)");
            sb.AppendLine("FROM subscription.PaymentFeedEntry PFE");
            sb.AppendLine("INNER JOIN subscription.PaymentException PE");
            sb.AppendLine("ON PE.PaymentFeedEntryID = PFE.PaymentFeedEntryID");
            sb.AppendLine("INNER JOIN subscription.PaymentExceptionType PET");
            sb.AppendLine("ON PET.PaymentExceptionTypeID = PE.PaymentExceptionTypeID");
            sb.AppendLine("AND PET.TypeName = 'CreditCardPaymentSettled'");
            sb.AppendLine("INNER JOIN subscription.PaymentFeedRun PFR");
            sb.AppendLine("ON PFR.PaymentFeedRunID = PFE.PaymentFeedRunID");
            sb.AppendLine("WHERE PFR.CreatedTimeStamp BETWEEN @0 AND @1");

            return _repository.ExecuteScalar<double>(sb.ToString(), fromDate.HasValue ? fromDate.Value.ToString(_dateTimeFormat) : "", toDate.ToString(_dateTimeFormat));
        }
        /// <summary>
        /// Total $ value of payments received in the bank during the report 
        /// period that are not Credit Card Payments and Payment Method is
        /// UNKNOWN
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        public double GetTotalCashReceipts_UNKNOWN(DateTime? fromDate, DateTime toDate)
        {
            // In practice, 0 becasue there are no UNKNOWN Cash Payments
            
            //_repository.ConnectToDb(ConfigurationHelper.Instance.ConnectionStringName_Portal);

            //StringBuilder sb = new StringBuilder();
            //sb.AppendLine("SELECT ISNULL(SUM(P.PaidAmount), 0)");
            //sb.AppendLine("FROM subscription.Payment P");
            //sb.AppendLine("INNER JOIN subscription.PaymentMethod PM");
            //sb.AppendLine("ON PM.PaymentMethodID = P.PaymentMethodID");
            //sb.AppendLine("AND PM.PaymentMethodName <> 'BPay'");
            //sb.AppendLine("AND PM.PaymentMethodName <> 'BankTransfer'");
            //sb.AppendLine("AND PM.PaymentMethodName <> 'CreditCard'");
            //sb.AppendLine("AND PM.PaymentMethodName <> 'BalanceFundTransfer'");
            //sb.AppendLine("LEFT OUTER JOIN subscription.PaymentException PE");
            //sb.AppendLine("ON PE.PaymentFeedEntryID = P.PaymentFeedEntryID");
            //sb.AppendLine("WHERE P.CreatedTimeStamp BETWEEN @0 AND @1");

            //return _repository.ExecuteScalar<double>(sb.ToString(), fromDate.HasValue ? fromDate.Value.ToString(_dateFormat) : "", toDate.AddDays(1).ToString(_dateFormat));

            return 0;
        }
        /// <summary>
        /// Total $ value of payments received in the bank during the report 
        /// period that are not Credit Card Payments and Payment Method is 
        /// BPAY or EFT
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        public double GetTotalCashReceipts_BPAY_EFT(DateTime? fromDate, DateTime toDate)
        {
            // In practice, all Payments that do not have an exception or if 
            //they are an exception the exception is not a credit card payment 
            // settlement

            _repository.ConnectToDb(ConfigurationHelper.Instance.ConnectionStringName_Portal);

            StringBuilder sb = new StringBuilder();


            sb.AppendLine("SELECT ISNULL(SUM(CASE WHEN PE.PaymentFeedEntryID IS NOT NULL THEN CASE WHEN (PET.TypeName = 'CreditCardPaymentSettled' OR PET.TypeName = 'DailyFundsTransferByACMA') THEN 0 ELSE PFE.Amount END ELSE PFE.Amount END), 0)");
            sb.AppendLine("FROM subscription.PaymentFeedEntry PFE");
            sb.AppendLine("LEFT OUTER JOIN subscription.PaymentException PE");
            sb.AppendLine("ON PE.PaymentFeedEntryID = PFE.PaymentFeedEntryID");
            sb.AppendLine("LEFT OUTER JOIN subscription.PaymentExceptionType PET");
            sb.AppendLine("ON PET.PaymentExceptionTypeID = PE.PaymentExceptionTypeID");
            sb.AppendLine("INNER JOIN subscription.PaymentFeedRun PFR");
            sb.AppendLine("ON PFR.PaymentFeedRunID = PFE.PaymentFeedRunID");
            sb.AppendLine("WHERE PFR.CreatedTimeStamp BETWEEN @0 AND @1");
            sb.AppendLine("AND PFE.Narration not like '%A.F.T.%'");
            // WHERE PE.PaymentFeedEntryId is null or PET.TypeName <> 'CreditCardPaymentSettled'

            return _repository.ExecuteScalar<double>(sb.ToString(), fromDate.HasValue ? fromDate.Value.ToString(_dateTimeFormat) : "", toDate.ToString(_dateTimeFormat));
        }

        /// <summary>
        /// Total $ value of payments received during the report period that are not related to orders
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        public List<FinancialReport_Payment> GetExceptionUnmatchedPayments(DateTime? fromDate, DateTime toDate)
        {
            _repository.ConnectToDb(ConfigurationHelper.Instance.ConnectionStringName_Portal);

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("SELECT * FROM (");
            sb.AppendLine("SELECT DISTINCT PFE.PaymentFeedEntryID, PFR.CreatedTimeStamp AS PaymentDate, PFE.Reference AS PaymentReference, PFE.Amount AS Amount, NULL AS OrderNumber");
            sb.AppendLine("FROM subscription.PaymentExceptionAmendmentSnapshot PEAS");
            sb.AppendLine("INNER JOIN subscription.PaymentException PE");
            sb.AppendLine("ON PE.PaymentExceptionID = PEAS.PaymentExceptionID");
            sb.AppendLine("INNER JOIN subscription.PaymentExceptionType PET");
            sb.AppendLine("ON PET.PaymentExceptionTypeID = PEAS.PaymentExceptionTypeID");
            sb.AppendLine("AND PET.TypeName = 'UnmatchedPayment'");
            sb.AppendLine("INNER JOIN subscription.PaymentFeedEntry PFE");
            sb.AppendLine("ON PFE.PaymentFeedEntryID = PE.PaymentFeedEntryID");
            sb.AppendLine("INNER JOIN subscription.PaymentFeedRun PFR");
            sb.AppendLine("ON PFR.PaymentFeedRunID = PFE.PaymentFeedRunID");
            sb.AppendLine("LEFT OUTER JOIN subscription.Payment P");
            sb.AppendLine("ON P.PaymentFeedEntryID = PFE.PaymentFeedEntryID");
            sb.AppendLine("WHERE PFR.CreatedTimeStamp BETWEEN @0 AND @1");
            sb.AppendLine("AND NOT EXISTS (SELECT 1 FROM subscription.PaymentException PE1 INNER JOIN subscription.PaymentExceptionType PET1 ON PET1.PaymentExceptionTypeID = PE1.PaymentExceptionTypeID AND PET1.TypeName = 'InvalidPayment' WHERE PE1.PaymentExceptionID = PE.PaymentExceptionID AND PE1.ClearedTimeStamp BETWEEN @0 AND @1)");
            sb.AppendLine("UNION");
            sb.AppendLine("SELECT DISTINCT PFE.PaymentFeedEntryID, PFR.CreatedTimeStamp AS PaymentDate, PFE.Reference AS PaymentReference, PFE.Amount AS Amount, NULL AS OrderNumber");
            sb.AppendLine("FROM subscription.PaymentException PE");
            sb.AppendLine("INNER JOIN subscription.PaymentExceptionType PET");
            sb.AppendLine("ON PET.PaymentExceptionTypeID = PE.PaymentExceptionTypeID");
            sb.AppendLine("AND PET.TypeName = 'UnmatchedPayment'");
            sb.AppendLine("INNER JOIN subscription.PaymentFeedEntry PFE");
            sb.AppendLine("ON PFE.PaymentFeedEntryID = PE.PaymentFeedEntryID");
            sb.AppendLine("INNER JOIN subscription.PaymentFeedRun PFR");
            sb.AppendLine("ON PFR.PaymentFeedRunID = PFE.PaymentFeedRunID");
            sb.AppendLine("LEFT OUTER JOIN subscription.Payment P");
            sb.AppendLine("ON P.PaymentFeedEntryID = PFE.PaymentFeedEntryID");
            sb.AppendLine("WHERE PFR.CreatedTimeStamp BETWEEN @0 AND @1");
            sb.AppendLine("AND NOT EXISTS (SELECT 1 FROM subscription.PaymentException PE1 INNER JOIN subscription.PaymentExceptionType PET1 ON PET1.PaymentExceptionTypeID = PE1.PaymentExceptionTypeID AND PET1.TypeName = 'InvalidPayment' WHERE PE1.PaymentExceptionID = PE.PaymentExceptionID AND PE1.ClearedTimeStamp BETWEEN @0 AND @1)");
            sb.AppendLine(") AS RESULT");
            sb.AppendLine("ORDER BY PaymentDate DESC");


            return _repository.Fetch<FinancialReport_Payment>(sb.ToString(), fromDate.HasValue ? fromDate.Value.ToString(_dateTimeFormat) : "", toDate.ToString(_dateTimeFormat)).ToList();
        }

        /// <summary>
        /// Total $ value of payments received during the report period that 
        /// affects the Account Balance (Over payments, Duplicate Payments) 
        /// i.e. transferred to Credit Notes and did not result into activations
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        public List<FinancialReport_Payment> GetExceptionTransferToCreditNotes(DateTime? fromDate, DateTime toDate)
        {
            _repository.ConnectToDb(ConfigurationHelper.Instance.ConnectionStringName_Portal);

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("SELECT OT.CreatedTimeStampt AS PaymentDate, O.AccountID AS AccountID, OT.Amount AS Amount, ISNULL(PFE.Reference, P.PaymentReference) AS PaymentReference, O.OrderNumber, (SELECT CN.CreditNoteID FROM subscription.CreditNote AS CN WHERE CN.OrderTransactionID = OT.OrderTransactionID) AS CreditNoteNumber");
            sb.AppendLine("FROM subscription.OrderTransaction OT");
            sb.AppendLine("INNER JOIN subscription.OrderTransactionType OTT");
            sb.AppendLine("ON OTT.OrderTransactionTypeID = OT.OrderTransactionTypeID");
            sb.AppendLine("AND OTT.TransactionType IN ('CreditAccountBalance')");
            sb.AppendLine("INNER JOIN subscription.[Order] O");
            sb.AppendLine("ON O.OrderID = OT.OrderID");
            sb.AppendLine("LEFT OUTER JOIN subscription.Payment P");
            sb.AppendLine("ON OT.PaymentID = P.PaymentID");
            sb.AppendLine("LEFT OUTER JOIN subscription.PaymentFeedEntry PFE");
            sb.AppendLine("ON PFE.PaymentFeedEntryID = P.PaymentFeedEntryID");
            sb.AppendLine("WHERE OT.CreatedTimeStampt BETWEEN @0 AND @1");
            sb.AppendLine("UNION");
            sb.AppendLine("SELECT AT.CreatedTimeStampt AS PaymentDate, AT.AccountID AS AccountID, AT.Amount AS Amount, ISNULL(PFE.Reference, P.PaymentReference) AS PaymentReference, NULL AS OrderNumber, (SELECT CN.CreditNoteID FROM subscription.CreditNote AS CN WHERE CN.AccountTransactionID = AT.AccountTransactionID) AS CreditNoteNumber");
            sb.AppendLine("FROM subscription.AccountTransaction AT");
            sb.AppendLine("INNER JOIN subscription.AccountTransactionType ATT");
            sb.AppendLine("ON ATT.AccountTransactionTypeID = AT.AccountTransactionTypeID");
            sb.AppendLine("AND ATT.TransactionType IN ('CreditAccountBalance')");
            sb.AppendLine("INNER JOIN subscription.PaymentFeedEntry PFE");
            sb.AppendLine("ON PFE.PaymentFeedEntryID = AT.PaymentFeedEntryID");
            sb.AppendLine("LEFT OUTER JOIN subscription.Payment P");
            sb.AppendLine("ON P.PaymentFeedEntryID = PFE.PaymentFeedEntryID");
            sb.AppendLine("WHERE AT.CreatedTimeStampt BETWEEN @0 AND @1");
            sb.AppendLine("AND NOT EXISTS (SELECT 1 FROM subscription.OrderTransaction OT INNER JOIN subscription.OrderTransactionType OTT ON OTT.OrderTransactionTypeID = OT.OrderTransactionTypeID AND OTT.TransactionType IN ('CreditAccountBalance') INNER JOIN subscription.Payment P ON P.PaymentID = OT.PaymentID INNER JOIN subscription.PaymentFeedEntry PFE ON PFE.PaymentFeedEntryID = P.PaymentFeedEntryID WHERE PFE.PaymentFeedEntryID = AT.PaymentFeedEntryID)");

            return _repository.Fetch<FinancialReport_Payment>(sb.ToString(), fromDate.HasValue ? fromDate.Value.ToString(_dateTimeFormat) : "", toDate.ToString(_dateTimeFormat)).ToList();
        }

        /// <summary>
        /// Total $ value of payments received during the report period that 
        /// are related to orders and resulted into an increased Order Balance
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        public List<FinancialReport_Payment> GetExceptionUnderpayments(DateTime? fromDate, DateTime toDate)
        {
            _repository.ConnectToDb(ConfigurationHelper.Instance.ConnectionStringName_Portal);

            StringBuilder sb = new StringBuilder();
            //sb.AppendLine("SELECT P.CreatedTimeStamp AS PaymentDate, P.PaymentReference AS PaymentReference, O.AccountID AS AccountID, P.PaidAmount AS Amount, O.OrderNumber AS OrderNumber");
            //sb.AppendLine("FROM subscription.Payment P");
            //sb.AppendLine("INNER JOIN subscription.[Order] O");
            //sb.AppendLine("ON O.OrderID = P.OrderID");
            //sb.AppendLine("INNER JOIN subscription.OrderTransaction OT");
            //sb.AppendLine("ON OT.OrderID = O.OrderID");
            //sb.AppendLine("INNER JOIN subscription.OrderTransactionType OTT");
            //sb.AppendLine("ON OTT.OrderTransactionTypeID = OT.OrderTransactionTypeID");
            //sb.AppendLine("AND OTT.TransactionType = 'CreditOrderBalance'");
            //sb.AppendLine("WHERE P.CreatedTimeStamp BETWEEN @0 AND @1");
            //sb.AppendLine("ORDER BY PaymentDate");

            //sb.AppendLine("SELECT OT.CreatedTimeStampt AS PaymentDate, ");
            //sb.AppendLine("	(SELECT TOP 1 P.PaymentReference FROM subscription.Payment AS P WHERE P.OrderID = OT.OrderID) AS PaymentReference, ");
            //sb.AppendLine("	(SELECT O.AccountID FROM subscription.[Order] AS O WHERE O.OrderID = OT.OrderID) AS AccountID, ");
            //sb.AppendLine("	OT.Amount AS Amount, ");
            //sb.AppendLine("	(SELECT O.OrderNumber FROM subscription.[Order] AS O WHERE O.OrderID = OT.OrderID) AS OrderNumber");
            //sb.AppendLine("FROM subscription.OrderTransaction OT");
            //sb.AppendLine("INNER JOIN subscription.OrderTransactionType OTT");
            //sb.AppendLine("ON OTT.OrderTransactionTypeID = OT.OrderTransactionTypeID");
            //sb.AppendLine("AND OTT.TransactionType = 'CreditOrderBalance'");
            //sb.AppendLine("WHERE OT.CreatedTimeStampt BETWEEN @0 AND @1");

            sb.AppendLine("SELECT OT.OrderTransactionID, OT.CreatedTimeStampt AS PaymentDate, ");
            sb.AppendLine(" (SELECT TOP 1 ISNULL(PFE.Reference, P.PaymentReference) FROM subscription.Payment AS P LEFT OUTER JOIN subscription.PaymentFeedEntry PFE ON PFE.PaymentFeedEntryID = P.PaymentFeedEntryID WHERE P.OrderID = OT.OrderID) AS PaymentReference, ");
            sb.AppendLine("	(SELECT O.AccountID FROM subscription.[Order] AS O WHERE O.OrderID = OT.OrderID) AS AccountID, ");
            sb.AppendLine("	OT.Amount AS Amount, ");
            sb.AppendLine("	(SELECT O.OrderNumber FROM subscription.[Order] AS O WHERE O.OrderID = OT.OrderID) AS OrderNumber,");
            sb.AppendLine("	OTT.TransactionType");
            sb.AppendLine("FROM subscription.OrderTransaction OT");
            sb.AppendLine("INNER JOIN subscription.OrderTransactionType OTT");
            sb.AppendLine("ON OTT.OrderTransactionTypeID = OT.OrderTransactionTypeID");
            sb.AppendLine("AND OTT.TransactionType IN ('CreditOrderBalance', 'OrderActivated', 'SubscriptionActivated', 'DebitOrderBalance', 'OrderClosed')");
            sb.AppendLine("WHERE OT.CreatedTimeStampt BETWEEN @0 AND @1");

            var orderTransactions = _repository.Fetch<FinancialReport_Payment>(sb.ToString(), fromDate.HasValue ? fromDate.Value.ToString(_dateTimeFormat) : "", toDate.ToString(_dateTimeFormat)).ToList();
            List<string> orderNumbers = orderTransactions.Select(p => p.OrderNumber).Distinct().ToList();

            var underPayments = new List<FinancialReport_Payment>();

            int? lastSubscriptionActivatedTransactionID, lastOrderActivatedTransactionID, lastOrderClosedTransactionID;

            foreach (var orderNumber in orderNumbers)
            {
                //lastOrderClosedTransactionID = orderTransactions.Where(p => p.OrderNumber == orderNumber && p.TransactionType == "OrderClosed").Select(p => p.OrderTransactionID).FirstOrDefault();

                //if (!lastOrderClosedTransactionID.HasValue || lastOrderClosedTransactionID.Value == 0)
                //{
                //lastOrderActivatedTransactionID = orderTransactions.Where(p => p.OrderNumber == orderNumber && p.TransactionType == "OrderActivated").Select(p => p.OrderTransactionID).FirstOrDefault();

                //if (!lastOrderActivatedTransactionID.HasValue || lastOrderActivatedTransactionID.Value == 0)
                //{
                //    lastSubscriptionActivatedTransactionID = orderTransactions.Where(p => p.OrderNumber == orderNumber && p.TransactionType == "SubscriptionActivated").Select(p => p.OrderTransactionID).FirstOrDefault();

                //    if (lastSubscriptionActivatedTransactionID.HasValue)
                //    {
                //        underPayments.AddRange(orderTransactions.Where(p => p.OrderNumber == orderNumber && p.OrderTransactionID > lastSubscriptionActivatedTransactionID && p.TransactionType == "CreditOrderBalance").ToList());
                //    }
                //    else
                //    {
                underPayments.AddRange(orderTransactions.Where(p => p.OrderNumber == orderNumber && p.TransactionType == "CreditOrderBalance").ToList());
                //    }
                //}
                //}
            }

            return underPayments.OrderByDescending(p => p.PaymentDate).ToList();
        }

        /// <summary>
        /// Total $ value of payments received during the report period that is
        /// determined as reversed disputed payment.
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        public List<FinancialReport_Payment> GetExceptionReversedDishonouredPayments(DateTime? fromDate, DateTime toDate)
        {
            _repository.ConnectToDb(ConfigurationHelper.Instance.ConnectionStringName_Portal);

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("SELECT PFR.CreatedTimeStamp AS PaymentDate, PFE.Reference AS PaymentReference, PE.NewTelemarkerId AS AccountID, PFE.Amount AS Amount, NULL AS OrderNumber");
            sb.AppendLine("FROM subscription.PaymentFeedEntry PFE");
            sb.AppendLine("INNER JOIN subscription.PaymentFeedEntryOutcome PFEO");
            sb.AppendLine("ON PFEO.PaymentFeedEntryOutcomeID = PFE.PaymentFeedEntryOutcomeID");
            sb.AppendLine("AND PFEO.Outcome = 'Exception'");
            sb.AppendLine("INNER JOIN subscription.PaymentException PE");
            sb.AppendLine("ON PE.PaymentFeedEntryID = PFE.PaymentFeedEntryID");
            sb.AppendLine("INNER JOIN subscription.PaymentExceptionType PET");
            sb.AppendLine("ON PET.PaymentExceptionTypeID = PE.PaymentExceptionTypeID");
            sb.AppendLine("AND PET.TypeName IN ('ReverseDishonorPayment', 'ReverseDishonourPayment')");
            sb.AppendLine("INNER JOIN subscription.PaymentFeedRun PFR");
            sb.AppendLine("ON PFR.PaymentFeedRunID = PFE.PaymentFeedRunID");
            sb.AppendLine("WHERE PE.Cleared = 1 AND PE.ClearedTimeStamp BETWEEN @0 AND @1");
            sb.AppendLine("ORDER BY PaymentDate DESC");

            return _repository.Fetch<FinancialReport_Payment>(sb.ToString(), fromDate.HasValue ? fromDate.Value.ToString(_dateTimeFormat) : "", toDate.ToString(_dateTimeFormat)).ToList();
        }

        /// <summary>
        /// Total $ value of payments received during the report period that is
        /// determined as Invalid
        /// 
        /// Note: This is where an unknown payment was received as a result of 
        /// incorrect bank account number
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        public List<FinancialReport_Payment> GetExceptionErroneousPayments(DateTime? fromDate, DateTime toDate)
        {
            _repository.ConnectToDb(ConfigurationHelper.Instance.ConnectionStringName_Portal);

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("SELECT PFR.CreatedTimeStamp AS PaymentDate, PFE.Reference AS PaymentReference, PE.NewTelemarkerId AS AccountID, PFE.Amount AS Amount, NULL AS OrderNumber");
            sb.AppendLine("FROM subscription.PaymentFeedEntry PFE");
            sb.AppendLine("INNER JOIN subscription.PaymentFeedEntryOutcome PFEO");
            sb.AppendLine("ON PFEO.PaymentFeedEntryOutcomeID = PFE.PaymentFeedEntryOutcomeID");
            sb.AppendLine("AND PFEO.Outcome = 'Exception'");
            sb.AppendLine("INNER JOIN subscription.PaymentException PE");
            sb.AppendLine("ON PE.PaymentFeedEntryID = PFE.PaymentFeedEntryID");
            sb.AppendLine("INNER JOIN subscription.PaymentExceptionType PET");
            sb.AppendLine("ON PET.PaymentExceptionTypeID = PE.PaymentExceptionTypeID");
            sb.AppendLine("AND PET.TypeName = 'InvalidPayment'");
            sb.AppendLine("INNER JOIN subscription.PaymentFeedRun PFR");
            sb.AppendLine("ON PFR.PaymentFeedRunID = PFE.PaymentFeedRunID");
            sb.AppendLine("WHERE (PE.Cleared = 1 AND PE.ClearedTimeStamp BETWEEN @0 AND @1) AND PFR.CreatedTimeStamp BETWEEN @0 AND @1");
            sb.AppendLine("ORDER BY PaymentDate DESC");

            return _repository.Fetch<FinancialReport_Payment>(sb.ToString(), fromDate.HasValue ? fromDate.Value.ToString(_dateTimeFormat) : "", toDate.ToString(_dateTimeFormat)).ToList();
        }

        /// <summary>
        /// Total $ value of payments with the type Credit Card received 
        /// (cash in the bank account) during the report period
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        public List<FinancialReport_Payment> GetExceptionCreditCardDeferredReceivedPayments(DateTime? fromDate, DateTime toDate)
        {
            _repository.ConnectToDb(ConfigurationHelper.Instance.ConnectionStringName_Portal);

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("SELECT P.CreatedTimeStamp AS PaymentDate, P.PaymentReference + ' -> ' + O.OrderNumber AS PaymentReference, O.AccountID AS AccountID, P.PaidAmount AS Amount, O.OrderNumber AS OrderNumber");
            sb.AppendLine("FROM subscription.Payment P");
            sb.AppendLine("INNER JOIN subscription.[Order] O");
            sb.AppendLine("ON O.OrderID = P.OrderID");
            sb.AppendLine("INNER JOIN subscription.PaymentMethod PM");
            sb.AppendLine("ON PM.PaymentMethodID = P.PaymentMethodID");
            sb.AppendLine("AND PM.PaymentMethodName = 'CreditCard'");
            sb.AppendLine("WHERE P.ExpectedSettelmentDate BETWEEN @0 AND @1");
            sb.AppendLine("AND P.IsPaymentSuccess = 1");
            sb.AppendLine("ORDER BY PaymentDate DESC");

            return _repository.Fetch<FinancialReport_Payment>(sb.ToString(), fromDate.HasValue ? fromDate.Value.ToString(_dateTimeFormat) : "", toDate.ToString(_dateTimeFormat)).ToList();
        }

        /// <summary>
        /// Total $ value of subscriptions paid for with credit in the period 
        /// where cash is not yet received in the bank account during the period
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        public List<FinancialReport_Payment> GetNewCreditCardDeferralPayments(DateTime? fromDate, DateTime toDate)
        {
            _repository.ConnectToDb(ConfigurationHelper.Instance.ConnectionStringName_Portal);

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("SELECT P.CreatedTimeStamp AS PaymentDate, P.PaymentReference + ' -> ' + O.OrderNumber AS PaymentReference, O.AccountID AS AccountID, P.PaidAmount AS Amount, O.OrderNumber AS OrderNumber");
            sb.AppendLine("FROM subscription.Payment P");
            sb.AppendLine("INNER JOIN subscription.PaymentMethod PM");
            sb.AppendLine("ON PM.PaymentMethodID = P.PaymentMethodID");
            sb.AppendLine("AND PM.PaymentMethodName = 'CreditCard'");
            sb.AppendLine("INNER JOIN subscription.[Order] O");
            sb.AppendLine("ON O.OrderID = P.OrderID");
            sb.AppendLine("WHERE P.CreatedTimeStamp BETWEEN @0 AND @1");
            //sb.AppendLine("WHERE P.ExpectedSettelmentDate > @1 AND P.CreatedTimeStamp BETWEEN @0 AND @1");
            sb.AppendLine("AND P.IsPaymentSuccess = 1");
            sb.AppendLine("AND P.PaidAmount > 0"); // excluding Free Subscription which will be added with $0 credit card payment
            sb.AppendLine("ORDER BY PaymentDate DESC");

            return _repository.Fetch<FinancialReport_Payment>(sb.ToString(), fromDate.HasValue ? fromDate.Value.ToString(_dateTimeFormat) : "", toDate.ToString(_dateTimeFormat)).ToList();
        }

        /// <summary>
        /// Total $ value of payments received that are identified as 
        /// Dishonoured payments during the report period
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        public List<FinancialReport_Payment> GetDishonouredPayments(DateTime? fromDate, DateTime toDate)
        {
            _repository.ConnectToDb(ConfigurationHelper.Instance.ConnectionStringName_Portal);

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("SELECT PFR.CreatedTimeStamp AS PaymentDate, PFE.Reference AS PaymentReference, -1 * PFE.Amount AS Amount, NULL AS OrderNumber");
            sb.AppendLine("FROM subscription.PaymentFeedEntry PFE");
            sb.AppendLine("INNER JOIN subscription.PaymentFeedEntryOutcome PFEO");
            sb.AppendLine("ON PFEO.PaymentFeedEntryOutcomeID = PFE.PaymentFeedEntryOutcomeID");
            sb.AppendLine("AND PFEO.Outcome = 'Exception'");
            sb.AppendLine("INNER JOIN subscription.PaymentException PE");
            sb.AppendLine("ON PE.PaymentFeedEntryID = PFE.PaymentFeedEntryID");
            sb.AppendLine("INNER JOIN subscription.PaymentFeedRun PFR");
            sb.AppendLine("ON PFR.PaymentFeedRunID = PFE.PaymentFeedRunID");
            sb.AppendLine("WHERE PE.Cleared = 1 AND PE.ClearedTimeStamp BETWEEN @0 AND @1");
            sb.AppendLine("and PE.PaymentExceptionTypeID in (2) -- Dishonoured Payment");
            sb.AppendLine("ORDER BY PaymentDate DESC");

            return _repository.Fetch<FinancialReport_Payment>(sb.ToString(), fromDate.HasValue ? fromDate.Value.ToString(_dateTimeFormat) : "", toDate.ToString(_dateTimeFormat)).ToList();

            //sb.AppendLine("SELECT ISNULL(SUM(PFE.Amount), 0)");
            //sb.AppendLine("FROM subscription.PaymentFeedEntry PFE");
            //sb.AppendLine("INNER JOIN subscription.PaymentFeedEntryOutcome PFEO");
            //sb.AppendLine("ON PFEO.PaymentFeedEntryOutcomeID = PFE.PaymentFeedEntryOutcomeID");
            //sb.AppendLine("AND PFEO.Outcome = 'Exception'");
            //sb.AppendLine("INNER JOIN subscription.PaymentException PE");
            //sb.AppendLine("ON PE.PaymentFeedEntryID = PFE.PaymentFeedEntryID");
            //sb.AppendLine("INNER JOIN subscription.PaymentExceptionType PET");
            //sb.AppendLine("ON PET.PaymentExceptionTypeID = PE.PaymentExceptionTypeID");
            //sb.AppendLine("AND PET.TypeName IN ('DishonorPayment', 'DishonourPayment')");
            //sb.AppendLine("WHERE PE.Cleared = 1 AND PE.ClearedTimeStamp BETWEEN @0 AND @1");

            //return _repository.ExecuteScalar<double>(sb.ToString(), fromDate.HasValue ? fromDate.Value.ToString(_dateTimeFormat) : "", toDate.ToString(_dateTimeFormat));
        }

        /// <summary>
        /// Total $ value of previous payments (exceptions) where available 
        /// Credit Notes are applied to allow purchasing subscriptions
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        public List<FinancialReport_Payment> GetCreditNotesAppliedToSalesPayments(DateTime? fromDate, DateTime toDate)
        {
            _repository.ConnectToDb(ConfigurationHelper.Instance.ConnectionStringName_Portal);

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("SELECT P.CreatedTimeStamp AS PaymentDate, P.PaymentReference + ' -> ' + O.OrderNumber AS PaymentReference, O.AccountID AS AccountID, P.PaidAmount AS Amount, O.OrderNumber AS OrderNumber");
            sb.AppendLine("FROM subscription.Payment P");
            sb.AppendLine("INNER JOIN subscription.PaymentMethod PM");
            sb.AppendLine("ON PM.PaymentMethodID = P.PaymentMethodID");
            sb.AppendLine("AND PM.PaymentMethodName = 'BalanceFundTransfer'");
            sb.AppendLine("INNER JOIN subscription.[Order] O");
            sb.AppendLine("ON O.OrderID = P.OrderID");
            sb.AppendLine("WHERE P.CreatedTimeStamp BETWEEN @0 AND @1");
            //sb.AppendLine("AND P.OrderID NOT IN (SELECT OT.OrderID FROM subscription.OrderTransaction OT INNER JOIN subscription.OrderTransactionType OTT ON OTT.TransactionType IN ('OrderClosed', 'OrderCancelled', 'OrderExpired') WHERE OT.OrderID = O.OrderID AND OT.CreatedTimeStampt BETWEEN @0 AND @1)");

            return _repository.Fetch<FinancialReport_Payment>(sb.ToString(), fromDate.HasValue ? fromDate.Value.ToString(_dateTimeFormat) : "", toDate.ToString(_dateTimeFormat)).ToList();
        }

        /// <summary>
        /// Total $ value of payments that were previously flagged as an 
        /// exception that were cleared in the report period and are 
        /// cleared as Matched Payments
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        public List<FinancialReport_Payment> GetClearedPreviousException_MatchedPayments(DateTime? fromDate, DateTime toDate)
        {
            _repository.ConnectToDb(ConfigurationHelper.Instance.ConnectionStringName_Portal);

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("SELECT PFR.CreatedTimeStamp AS PaymentDate, PFE.Reference + ' -> ' + O.OrderNumber AS PaymentReference, PE.NewTelemarkerId AS AccountID, PFE.Amount AS Amount, O.OrderNumber AS OrderNumber");
            sb.AppendLine("FROM subscription.PaymentFeedEntry PFE");
            sb.AppendLine("INNER JOIN subscription.PaymentException PE");
            sb.AppendLine("ON PE.PaymentFeedEntryID = PFE.PaymentFeedEntryID");
            sb.AppendLine("INNER JOIN subscription.PaymentFeedRun PFR");
            sb.AppendLine("ON PFR.PaymentFeedRunID = PFE.PaymentFeedRunID");
            sb.AppendLine("LEFT OUTER JOIN subscription.Payment P");
            sb.AppendLine("ON P.PaymentFeedEntryID = PFE.PaymentFeedEntryID");
            sb.AppendLine("LEFT OUTER JOIN subscription.[Order] O");
            sb.AppendLine("ON O.OrderID = P.OrderID");
            sb.AppendLine("WHERE PE.ApprovedTimeStampt BETWEEN @0 AND @1");
            sb.AppendLine("and PE.PaymentExceptionTypeID in (1,2) -- Unmatched Payment & Dishonoured Payment");
            sb.AppendLine("AND PE.Cleared = 1");


            return _repository.Fetch<FinancialReport_Payment>(sb.ToString(), fromDate.HasValue ? fromDate.Value.ToString(_dateTimeFormat) : "", toDate.ToString(_dateTimeFormat)).ToList();
        }

        /// <summary>
        /// Total $ value of payments that were previously flagged as an 
        /// exception that were cleared in the report period and are 
        /// cleared as Underpayments
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        public List<FinancialReport_Payment> GetClearedPreviousException_Underpayments(DateTime? fromDate, DateTime toDate)
        {
            var totalClearedPreviousException_Underpayments = new List<FinancialReport_Payment>();
            var clearedUnderpayments = GetExceptionUnmatchedAmounts(fromDate, toDate);

            if (clearedUnderpayments != null)
            {
                clearedUnderpayments = clearedUnderpayments.Where(p => p.PaymentStatus == "Underpayment" && p.Cleared > 0).ToList();

                foreach (var item in clearedUnderpayments)
                {
                    totalClearedPreviousException_Underpayments.Add(new FinancialReport_Payment()
                        {
                            PaymentDate = item.DateReceived,
                            PaymentReference = string.Format("{0} -> {1}", item.Reference, item.OrderNumber),
                            AccountID = item.AccountID,
                            Amount = item.ExceptionAmount
                        });
                }
            }

            return totalClearedPreviousException_Underpayments;
        }

        /// <summary>
        /// Total $ value of subscriptions activated by credit card during the report period
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        public double GetTotalCreditCardActivationsPayments(DateTime? fromDate, DateTime toDate)
        {
            _repository.ConnectToDb(ConfigurationHelper.Instance.ConnectionStringName_Portal);

            StringBuilder sb = new StringBuilder();
            //sb.AppendLine("SELECT ISNULL(SUM(P.PaidAmount), 0)");
            //sb.AppendLine("FROM subscription.Payment P");
            //sb.AppendLine("INNER JOIN subscription.[Order] O");
            //sb.AppendLine("ON O.OrderID = P.OrderID");
            //sb.AppendLine("INNER JOIN subscription.OrderTransaction OT");
            //sb.AppendLine("ON OT.OrderID = O.OrderID");
            //sb.AppendLine("INNER JOIN subscription.OrderTransactionType OTT");
            //sb.AppendLine("ON OTT.OrderTransactionTypeID = OT.OrderTransactionTypeID");
            //sb.AppendLine("AND OTT.TransactionType = 'SubscriptionActivated'");
            //sb.AppendLine("INNER JOIN subscription.PaymentMethod PM");
            //sb.AppendLine("ON PM.PaymentMethodID = P.PaymentMethodID");
            //sb.AppendLine("AND PM.PaymentMethodName = 'CreditCard'");
            //sb.AppendLine("WHERE P.CreatedTimeStamp BETWEEN @0 AND @1");
            //sb.AppendLine("AND P.IsPaymentSuccess = 1");

            sb.AppendLine("SELECT ISNULL(SUM(OT.Amount), 0)");
            sb.AppendLine("FROM subscription.OrderTransaction OT");
            sb.AppendLine("INNER JOIN subscription.OrderTransactionType OTT");
            sb.AppendLine("ON OTT.OrderTransactionTypeID = OT.OrderTransactionTypeID");
            sb.AppendLine("AND OTT.TransactionType = 'SubscriptionActivated'");
            sb.AppendLine("INNER JOIN subscription.Payment P");
            sb.AppendLine("ON P.OrderID = OT.OrderID");
            sb.AppendLine("AND P.IsPaymentSuccess = 1");
            sb.AppendLine("INNER JOIN subscription.PaymentMethod PM");
            sb.AppendLine("ON PM.PaymentMethodID = P.PaymentMethodID");
            sb.AppendLine("AND PM.PaymentMethodName = 'CreditCard'");
            sb.AppendLine("WHERE OT.CreatedTimeStampt BETWEEN @0 AND @1");

            return _repository.ExecuteScalar<double>(sb.ToString(), fromDate.HasValue ? fromDate.Value.ToString(_dateTimeFormat) : "", toDate.ToString(_dateTimeFormat));
        }

        /// <summary>
        /// Total $ value of subscriptions activated by EFT during the report period.
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        public double GetTotalEFTActivationsPayments(DateTime? fromDate, DateTime toDate)
        {
            _repository.ConnectToDb(ConfigurationHelper.Instance.ConnectionStringName_Portal);

            StringBuilder sb = new StringBuilder();
            //sb.AppendLine("SELECT ISNULL(SUM(P.PaidAmount), 0)");
            //sb.AppendLine("FROM subscription.Payment P");
            //sb.AppendLine("INNER JOIN subscription.[Order] O");
            //sb.AppendLine("ON O.OrderID = P.OrderID");
            //sb.AppendLine("INNER JOIN subscription.OrderTransaction OT");
            //sb.AppendLine("ON OT.OrderID = O.OrderID");
            //sb.AppendLine("INNER JOIN subscription.OrderTransactionType OTT");
            //sb.AppendLine("ON OTT.OrderTransactionTypeID = OT.OrderTransactionTypeID");
            //sb.AppendLine("AND OTT.TransactionType = 'SubscriptionActivated'");
            //sb.AppendLine("INNER JOIN subscription.PaymentMethod PM");
            //sb.AppendLine("ON PM.PaymentMethodID = P.PaymentMethodID");
            //sb.AppendLine("AND PM.PaymentMethodName = 'BankTransfer'");
            //sb.AppendLine("WHERE P.CreatedTimeStamp BETWEEN @0 AND @1");

            sb.AppendLine("SELECT ISNULL(SUM(OT.Amount), 0)");
            sb.AppendLine("FROM subscription.OrderTransaction OT");
            sb.AppendLine("INNER JOIN subscription.OrderTransactionType OTT");
            sb.AppendLine("ON OTT.OrderTransactionTypeID = OT.OrderTransactionTypeID");
            sb.AppendLine("AND OTT.TransactionType = 'SubscriptionActivated'");
            sb.AppendLine("INNER JOIN subscription.Payment P");
            sb.AppendLine("ON P.OrderID = OT.OrderID");
            sb.AppendLine("INNER JOIN subscription.PaymentMethod PM");
            sb.AppendLine("ON PM.PaymentMethodID = P.PaymentMethodID");
            sb.AppendLine("AND PM.PaymentMethodName = 'BankTransfer'");
            sb.AppendLine("WHERE OT.CreatedTimeStampt BETWEEN @0 AND @1");

            return _repository.ExecuteScalar<double>(sb.ToString(), fromDate.HasValue ? fromDate.Value.ToString(_dateTimeFormat) : "", toDate.ToString(_dateTimeFormat));
        }

        /// <summary>
        /// Total $ value of subscriptions activated by Balance Fund Transfer during the report period.
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        public double GetTotalFundTransferActivationsPayments(DateTime? fromDate, DateTime toDate)
        {
            _repository.ConnectToDb(ConfigurationHelper.Instance.ConnectionStringName_Portal);

            StringBuilder sb = new StringBuilder();
            //sb.AppendLine("SELECT ISNULL(SUM(P.PaidAmount), 0)");
            //sb.AppendLine("FROM subscription.Payment P");
            //sb.AppendLine("INNER JOIN subscription.[Order] O");
            //sb.AppendLine("ON O.OrderID = P.OrderID");
            //sb.AppendLine("INNER JOIN subscription.OrderTransaction OT");
            //sb.AppendLine("ON OT.OrderID = O.OrderID");
            //sb.AppendLine("INNER JOIN subscription.OrderTransactionType OTT");
            //sb.AppendLine("ON OTT.OrderTransactionTypeID = OT.OrderTransactionTypeID");
            //sb.AppendLine("AND OTT.TransactionType = 'SubscriptionActivated'");
            //sb.AppendLine("INNER JOIN subscription.PaymentMethod PM");
            //sb.AppendLine("ON PM.PaymentMethodID = P.PaymentMethodID");
            //sb.AppendLine("AND PM.PaymentMethodName = 'BalanceFundTransfer'");
            //sb.AppendLine("WHERE P.CreatedTimeStamp BETWEEN @0 AND @1");

            sb.AppendLine("SELECT ISNULL(SUM(OT.Amount), 0)");
            sb.AppendLine("FROM subscription.OrderTransaction OT");
            sb.AppendLine("INNER JOIN subscription.OrderTransactionType OTT");
            sb.AppendLine("ON OTT.OrderTransactionTypeID = OT.OrderTransactionTypeID");
            sb.AppendLine("AND OTT.TransactionType = 'SubscriptionActivated'");
            sb.AppendLine("INNER JOIN subscription.Payment P");
            sb.AppendLine("ON P.OrderID = OT.OrderID");
            sb.AppendLine("INNER JOIN subscription.PaymentMethod PM");
            sb.AppendLine("ON PM.PaymentMethodID = P.PaymentMethodID");
            sb.AppendLine("AND PM.PaymentMethodName = 'BalanceFundTransfer'");
            sb.AppendLine("WHERE OT.CreatedTimeStampt BETWEEN @0 AND @1");

            return _repository.ExecuteScalar<double>(sb.ToString(), fromDate.HasValue ? fromDate.Value.ToString(_dateTimeFormat) : "", toDate.ToString(_dateTimeFormat));
        }

        /// <summary>
        /// Total $ value of subscriptions activated by BPay during the report period.
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        public double GetTotalBPayActivationsPayments(DateTime? fromDate, DateTime toDate)
        {
            _repository.ConnectToDb(ConfigurationHelper.Instance.ConnectionStringName_Portal);

            StringBuilder sb = new StringBuilder();
            //sb.AppendLine("SELECT ISNULL(SUM(P.PaidAmount), 0)");
            //sb.AppendLine("FROM subscription.Payment P");
            //sb.AppendLine("INNER JOIN subscription.[Order] O");
            //sb.AppendLine("ON O.OrderID = P.OrderID");
            //sb.AppendLine("INNER JOIN subscription.OrderTransaction OT");
            //sb.AppendLine("ON OT.OrderID = O.OrderID");
            //sb.AppendLine("INNER JOIN subscription.OrderTransactionType OTT");
            //sb.AppendLine("ON OTT.OrderTransactionTypeID = OT.OrderTransactionTypeID");
            //sb.AppendLine("AND OTT.TransactionType = 'SubscriptionActivated'");
            //sb.AppendLine("INNER JOIN subscription.PaymentMethod PM");
            //sb.AppendLine("ON PM.PaymentMethodID = P.PaymentMethodID");
            //sb.AppendLine("AND PM.PaymentMethodName = 'BPay'");
            //sb.AppendLine("WHERE P.CreatedTimeStamp BETWEEN @0 AND @1");

            sb.AppendLine("SELECT ISNULL(SUM(OT.Amount), 0)");
            sb.AppendLine("FROM subscription.OrderTransaction OT");
            sb.AppendLine("INNER JOIN subscription.OrderTransactionType OTT");
            sb.AppendLine("ON OTT.OrderTransactionTypeID = OT.OrderTransactionTypeID");
            sb.AppendLine("AND OTT.TransactionType = 'SubscriptionActivated'");
            sb.AppendLine("INNER JOIN subscription.Payment P");
            sb.AppendLine("ON P.OrderID = OT.OrderID");
            sb.AppendLine("INNER JOIN subscription.PaymentMethod PM");
            sb.AppendLine("ON PM.PaymentMethodID = P.PaymentMethodID");
            sb.AppendLine("AND PM.PaymentMethodName = 'BPay'");
            sb.AppendLine("WHERE OT.CreatedTimeStampt BETWEEN @0 AND @1");

            return _repository.ExecuteScalar<double>(sb.ToString(), fromDate.HasValue ? fromDate.Value.ToString(_dateTimeFormat) : "", toDate.ToString(_dateTimeFormat));
        }

        /// <summary>
        /// Total $ value of subscriptions activated by methods other than
        /// Credit Card, EFT, BPay, or Balance Transfer during the report 
        /// period.
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        public double GetTotalOtherActivationsPayments(DateTime? fromDate, DateTime toDate)
        {
            _repository.ConnectToDb(ConfigurationHelper.Instance.ConnectionStringName_Portal);

            StringBuilder sb = new StringBuilder();
            //sb.AppendLine("SELECT ISNULL(SUM(P.PaidAmount), 0)");
            //sb.AppendLine("FROM subscription.Payment P");
            //sb.AppendLine("INNER JOIN subscription.[Order] O");
            //sb.AppendLine("ON O.OrderID = P.OrderID");
            //sb.AppendLine("INNER JOIN subscription.OrderTransaction OT");
            //sb.AppendLine("ON OT.OrderID = O.OrderID");
            //sb.AppendLine("INNER JOIN subscription.OrderTransactionType OTT");
            //sb.AppendLine("ON OTT.OrderTransactionTypeID = OT.OrderTransactionTypeID");
            //sb.AppendLine("AND OTT.TransactionType = 'SubscriptionActivated'");
            //sb.AppendLine("INNER JOIN subscription.PaymentMethod PM");
            //sb.AppendLine("ON PM.PaymentMethodID = P.PaymentMethodID");
            //sb.AppendLine("AND PM.PaymentMethodName NOT IN ('CreditCard', 'BankTransfer', 'BPay', 'BalanceFundTransfer')");
            //sb.AppendLine("WHERE P.CreatedTimeStamp BETWEEN @0 AND @1");

            sb.AppendLine("SELECT ISNULL(SUM(OT.Amount), 0)");
            sb.AppendLine("FROM subscription.OrderTransaction OT");
            sb.AppendLine("INNER JOIN subscription.OrderTransactionType OTT");
            sb.AppendLine("ON OTT.OrderTransactionTypeID = OT.OrderTransactionTypeID");
            sb.AppendLine("AND OTT.TransactionType = 'SubscriptionActivated'");
            sb.AppendLine("INNER JOIN subscription.Payment P");
            sb.AppendLine("ON P.OrderID = OT.OrderID");
            sb.AppendLine("INNER JOIN subscription.PaymentMethod PM");
            sb.AppendLine("ON PM.PaymentMethodID = P.PaymentMethodID");
            sb.AppendLine("AND PM.PaymentMethodName NOT IN ('CreditCard', 'BankTransfer', 'BPay', 'BalanceFundTransfer')");
            sb.AppendLine("WHERE OT.CreatedTimeStampt BETWEEN @0 AND @1");

            return _repository.ExecuteScalar<double>(sb.ToString(), fromDate.HasValue ? fromDate.Value.ToString(_dateTimeFormat) : "", toDate.ToString(_dateTimeFormat));
        }

        public double GetTotalActivationsPayments(DateTime? fromDate, DateTime toDate)
        {
            _repository.ConnectToDb(ConfigurationHelper.Instance.ConnectionStringName_Portal);

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("SELECT ISNULL(SUM(OT.Amount), 0)");
            sb.AppendLine("FROM subscription.OrderTransaction OT");
            sb.AppendLine("INNER JOIN subscription.OrderTransactionType OTT");
            sb.AppendLine("ON OTT.OrderTransactionTypeID = OT.OrderTransactionTypeID");
            sb.AppendLine("AND OTT.TransactionType = 'SubscriptionActivated'");
            sb.AppendLine("WHERE OT.CreatedTimeStampt BETWEEN @0 AND @1");

            return _repository.ExecuteScalar<double>(sb.ToString(), fromDate.HasValue ? fromDate.Value.ToString(_dateTimeFormat) : "", toDate.ToString(_dateTimeFormat));
        }

        public IList<ActivatedSubscription> GetAllActivationsPayments(DateTime? fromDate, DateTime toDate)
        {
            _repository.ConnectToDb(ConfigurationHelper.Instance.ConnectionStringName_Portal);

            return _repository.Fetch<ActivatedSubscription>(GetAllActivatedSubscriptionsByPeriodQuery, fromDate.HasValue ? fromDate.Value.ToString(_dateTimeFormat) : "", toDate.ToString(_dateTimeFormat));
        }

        public List<FinancialReport_Payment> GetRefundedPayments(DateTime? fromDate, DateTime toDate)
        {
            _repository.ConnectToDb(ConfigurationHelper.Instance.ConnectionStringName_Portal);

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("SELECT * FROM (");
            sb.AppendLine("SELECT AJ.CreatedTimeStamp AS PaymentDate, ISNULL(PFE.Reference, P.PaymentReference) + ' -> ' + O.OrderNumber AS PaymentReference, ER.account_id AS AccountID, isnull(refund_amount_from_subscription, (SELECT OL.Price FROM subscription.OrderLine OL WHERE OL.OrderLineID = ER.refund_subscription_id)) AS Amount, O.OrderNumber AS OrderNumber");
            sb.AppendLine("FROM cms.enquiry_refunds ER");
            sb.AppendLine("INNER JOIN cms.enquiry_refund_types ERT");
            sb.AppendLine("ON ERT.enquiry_refund_type_id = ER.enquiry_refund_type_id");
            sb.AppendLine("AND ERT.refund_type = 'SubscriptionRefund'");
            sb.AppendLine("INNER JOIN subscription.AccountAdjustment AJ");
            sb.AppendLine("ON ER.decision_adjustment_id = AJ.AccountAdjustmentID");
            sb.AppendLine("INNER JOIN subscription.[Order] O");
            sb.AppendLine("ON O.OrderID = ER.refund_order_id");
            sb.AppendLine("INNER JOIN subscription.Payment P");
            sb.AppendLine("ON P.OrderID = O.OrderID");
            sb.AppendLine("LEFT OUTER JOIN subscription.PaymentFeedEntry PFE");
            sb.AppendLine("ON PFE.PaymentFeedEntryID = P.PaymentFeedEntryID");
            sb.AppendLine("WHERE AJ.CreatedTimeStamp BETWEEN @0 AND @1");
            sb.AppendLine("UNION");
            sb.AppendLine("SELECT AJ.CreatedTimeStamp AS PaymentDate, 'Account Balance Refund' AS PaymentReference, ER.account_id AS AccountID, ER.refund_reserved_account_balance AS Amount, NULL AS OrderNumber");
            sb.AppendLine("FROM cms.enquiry_refunds ER");
            sb.AppendLine("INNER JOIN cms.enquiry_refund_types ERT");
            sb.AppendLine("ON ERT.enquiry_refund_type_id = ER.enquiry_refund_type_id");
            sb.AppendLine("AND ERT.refund_type = 'BalanceRefund'");
            sb.AppendLine("INNER JOIN subscription.AccountAdjustment AJ");
            sb.AppendLine("ON ER.decision_adjustment_id = AJ.AccountAdjustmentID");
            sb.AppendLine("WHERE AJ.CreatedTimeStamp BETWEEN @0 AND @1");
            sb.AppendLine(") RESULT ORDER BY PaymentDate DESC");

            return _repository.Fetch<FinancialReport_Payment>(sb.ToString(), fromDate.HasValue ? fromDate.Value.ToString(_dateTimeFormat) : "", toDate.ToString(_dateTimeFormat)).ToList();
        }

        #endregion

        #region Exceptions Report

        public List<FinancialReport_Exception> GetExceptionUnmatchedAmounts(DateTime? fromDate, DateTime toDate)
        {
            _repository.ConnectToDb(ConfigurationHelper.Instance.ConnectionStringName_Portal);

            var unmatchedException = _repository.Fetch<FinancialReport_Exception>(Query_UnmatchedPaymentsExceptionReport, fromDate.HasValue ? fromDate.Value.ToString(_dateTimeFormat) : "", toDate.ToString(_dateTimeFormat)).ToList();

            // Get cleared underpayments from OrderTransactions
            var orderTransactions = _repository.Fetch<FinancialReport_Payment>(Query_UnmatchedPaymentsExceptionReport_OrderTransactions_, fromDate.HasValue ? fromDate.Value.ToString(_dateTimeFormat) : "", toDate.ToString(_dateTimeFormat)).ToList();

            List<string> orderNumbers = orderTransactions.Select(p => p.OrderNumber).Distinct().ToList();

            var underPayments = new List<FinancialReport_Payment>();

            int? lastSubscriptionActivatedTransactionID, lastOrderActivatedTransactionID, lastOrderClosedTransactionID;
            DateTime lastSubscriptionActivatedTimeStamp, lastOrderActivatedTimeStamp, lastOrderClosedTimeStamp;

            foreach (var orderNumber in orderNumbers)
            {
                lastOrderClosedTransactionID = orderTransactions.Where(p => p.OrderNumber == orderNumber && p.TransactionType == "OrderClosed").Select(p => p.OrderTransactionID).FirstOrDefault();

                // if Order is closed, all UnderPayments before the OrderClosed will be marked as cleared.
                if (lastOrderClosedTransactionID.HasValue && lastOrderClosedTransactionID.Value > 0)
                {
                    lastOrderClosedTimeStamp = orderTransactions.Where(p => p.OrderTransactionID == lastOrderClosedTransactionID).Select(p => p.PaymentDate).FirstOrDefault();

                    underPayments.AddRange(orderTransactions.Where(p => p.OrderNumber == orderNumber && p.OrderTransactionID < lastOrderClosedTransactionID && p.TransactionType == "CreditOrderBalance")
                        .Select(p => new FinancialReport_Payment()
                        {
                            PaymentDate = p.PaymentDate,
                            AccountID = p.AccountID,
                            Amount = p.Amount,
                            CreditNoteNumber = p.CreditNoteNumber,
                            OrderNumber = p.OrderNumber,
                            OrderTransactionID = p.OrderTransactionID,
                            PaymentReference = p.PaymentReference,
                            TransactionType = p.TransactionType,
                            UnderPaymentClearedTimeStamp = lastOrderClosedTimeStamp
                        }));
                }
                else
                {
                    lastOrderActivatedTransactionID = orderTransactions.Where(p => p.OrderNumber == orderNumber && p.TransactionType == "OrderActivated").Select(p => p.OrderTransactionID).FirstOrDefault();

                    // if Order is activated, all UnderPayments before the OrderActivated will be marked as cleared.
                    if (lastOrderActivatedTransactionID.HasValue && lastOrderActivatedTransactionID.Value > 0)
                    {
                        lastOrderActivatedTimeStamp = orderTransactions.Where(p => p.OrderTransactionID == lastOrderActivatedTransactionID).Select(p => p.PaymentDate).FirstOrDefault();

                        underPayments.AddRange(orderTransactions.Where(p => p.OrderNumber == orderNumber && p.OrderTransactionID < lastOrderActivatedTransactionID && p.TransactionType == "CreditOrderBalance")
                            .Select(p => new FinancialReport_Payment()
                            {
                                PaymentDate = p.PaymentDate,
                                AccountID = p.AccountID,
                                Amount = p.Amount,
                                CreditNoteNumber = p.CreditNoteNumber,
                                OrderNumber = p.OrderNumber,
                                OrderTransactionID = p.OrderTransactionID,
                                PaymentReference = p.PaymentReference,
                                TransactionType = p.TransactionType,
                                UnderPaymentClearedTimeStamp = lastOrderActivatedTimeStamp
                            }));
                    }
                    else
                    {
                        lastSubscriptionActivatedTransactionID = orderTransactions.Where(p => p.OrderNumber == orderNumber && p.TransactionType == "SubscriptionActivated").Select(p => p.OrderTransactionID).FirstOrDefault();

                        // Order has not been activated and if there is any Subscription in Order is activated, all UnderPayments before the last SubscriptionActivated will be marked as cleared.
                        if (lastSubscriptionActivatedTransactionID.HasValue && lastSubscriptionActivatedTransactionID.Value > 0)
                        {
                            lastSubscriptionActivatedTimeStamp = orderTransactions.Where(p => p.OrderTransactionID == lastSubscriptionActivatedTransactionID).Select(p => p.PaymentDate).FirstOrDefault();

                            underPayments.AddRange(orderTransactions.Where(p => p.OrderNumber == orderNumber && p.OrderTransactionID < lastSubscriptionActivatedTransactionID && p.TransactionType == "CreditOrderBalance")
                                .Select(p => new FinancialReport_Payment()
                                {
                                    PaymentDate = p.PaymentDate,
                                    AccountID = p.AccountID,
                                    Amount = p.Amount,
                                    CreditNoteNumber = p.CreditNoteNumber,
                                    OrderNumber = p.OrderNumber,
                                    OrderTransactionID = p.OrderTransactionID,
                                    PaymentReference = p.PaymentReference,
                                    TransactionType = p.TransactionType,
                                    UnderPaymentClearedTimeStamp = lastSubscriptionActivatedTimeStamp
                                }));
                        }
                    }
                }
            }

            foreach (var item in unmatchedException.Where(p => p.PaymentStatus == "Underpayment").ToList())
            {
                var underPayment = underPayments.Where(p => p.OrderTransactionID == item.ID).FirstOrDefault();

                if (underPayment != null)
                {
                    if (fromDate.HasValue && underPayment.UnderPaymentClearedTimeStamp < fromDate.Value)
                    {
                        unmatchedException.Remove(item);
                    }
                    else
                    {
                        item.Cleared = item.ExceptionAmount;
                    }
                }
            }

            return unmatchedException;
        }

        public List<FinancialReport_Exception> GetExceptionTransferredFromToCreditNotes(DateTime? fromDate, DateTime toDate)
        {
            //-- CreditAccountBalance
            //-- Closed Order Payment -> OrderClosed + (OT.Amount < O.Amount)
            //-- Cancelled Order Payment -> OrderCancelled + (OT.Amount < O.Amount)
            //-- Expired Order Payment -> OrderExpired + (OT.Amount <> O.Amount)
            //-- Over Payment -> (O.Status = FullyPaid) + (OT.Amount <> O.Amount)
            //-- Duplicate Payment -> (O.Status = FullyPaid) + (OT.Amount = O.Amount)

            //-- DebitAccountBalance = Applied to Sales

            _repository.ConnectToDb(ConfigurationHelper.Instance.ConnectionStringName_Portal);

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("SELECT * FROM (");
            sb.AppendLine("SELECT OT.CreatedTimeStampt AS DateReceived,");
            sb.AppendLine("(SELECT PM.PaymentMethodName FROM subscription.Payment P INNER JOIN subscription.PaymentMethod PM ON PM.PaymentMethodID = P.PaymentMethodID WHERE P.PaymentID = OT.PaymentID) AS PaymentType,");
            sb.AppendLine("O.AccountID AS AccountID, O.OrderNumber AS OrderNumber, ");
            sb.AppendLine("OT.Amount AS Cleared, ");
            sb.AppendLine("OT.Amount AS ExceptionAmount, OTT.TransactionType, ");
            sb.AppendLine("CASE ");
            sb.AppendLine("	WHEN OTT.TransactionType = 'CreditAccountBalance' ");
            sb.AppendLine("		THEN CASE ");
            sb.AppendLine("			WHEN EXISTS(SELECT 1 FROM subscription.OrderTransaction OT1 INNER JOIN subscription.OrderTransactionType OTT1 ON OTT1.OrderTransactionTypeID = OT1.OrderTransactionTypeID AND OTT1.TransactionType = 'OrderClosed' WHERE OT1.OrderID = OT.OrderID AND OT.Amount < O.Amount) ");
            sb.AppendLine("				THEN 'Closed Order Payment'");
            sb.AppendLine("			WHEN EXISTS(SELECT 1 FROM subscription.OrderTransaction OT1 INNER JOIN subscription.OrderTransactionType OTT1 ON OTT1.OrderTransactionTypeID = OT1.OrderTransactionTypeID AND OTT1.TransactionType = 'OrderCancelled' WHERE OT1.OrderID = OT.OrderID AND OT.Amount < O.Amount) ");
            sb.AppendLine("				THEN 'Cancelled Order Payment'");
            sb.AppendLine("			WHEN EXISTS(SELECT 1 FROM subscription.[Order] O1 INNER JOIN subscription.OrderStatus OS ON O1.OrderStatusID = OS.OrderStatusID AND OS.OrderStatusName = 'Expired' WHERE O1.OrderID = OT.OrderID AND OT.Amount != O.Amount) ");
            sb.AppendLine("				THEN 'Expired Order Payment'");
            sb.AppendLine("			WHEN EXISTS(SELECT 1 FROM subscription.[Order] O1 INNER JOIN subscription.OrderStatus OS ON O1.OrderStatusID = OS.OrderStatusID AND OS.OrderStatusName = 'FullyPaid' WHERE O1.OrderID = OT.OrderID AND OT.Amount != O.Amount) ");
            sb.AppendLine("				THEN 'Over Payment'");
            sb.AppendLine("			WHEN EXISTS(SELECT 1 FROM subscription.[Order] O1 INNER JOIN subscription.OrderStatus OS ON O1.OrderStatusID = OS.OrderStatusID AND OS.OrderStatusName = 'FullyPaid' WHERE O1.OrderID = OT.OrderID AND OT.Amount = O.Amount) ");
            sb.AppendLine("				THEN 'Duplicate Payment'");
            sb.AppendLine("			ELSE OTT.TransactionType");
            sb.AppendLine("		END");
            sb.AppendLine("	WHEN OTT.TransactionType = 'DebitAccountBalance' ");
            sb.AppendLine("		THEN 'Applied To Sales'");
            sb.AppendLine("	ELSE OTT.TransactionType");
            sb.AppendLine("END AS PaymentStatus, (SELECT CN.CreditNoteID from subscription.CreditNote CN WHERE CN.OrderTransactionID = OT.OrderTransactionID) AS CreditNoteNumber");
            sb.AppendLine("FROM subscription.OrderTransaction OT");
            sb.AppendLine("INNER JOIN subscription.OrderTransactionType OTT");
            sb.AppendLine("ON OTT.OrderTransactionTypeID = OT.OrderTransactionTypeID");
            sb.AppendLine("AND OTT.TransactionType IN ('CreditAccountBalance', 'DebitAccountBalance')");
            sb.AppendLine("LEFT OUTER JOIN subscription.[Order] O");
            sb.AppendLine("ON O.OrderID = OT.OrderID");
            sb.AppendLine("WHERE OT.CreatedTimeStampt BETWEEN @0 AND @1");
            sb.AppendLine("UNION");
            sb.AppendLine("SELECT AT.CreatedTimeStampt AS DateReceived, NULL AS PaymentType, AT.AccountID AS AccountID, NULL AS OrderNumber, AT.Amount AS Amount, (SELECT A.CreditNotes FROM accessseeker.Account AS A WHERE A.AccountID = AT.AccountID) AS AccountBalance, ATT.TransactionType, 'Matched Payment' AS PaymentNote, (SELECT CN.CreditNoteID from subscription.CreditNote CN WHERE CN.AccountTransactionID = AT.AccountTransactionID) AS CreditNoteNumber");
            sb.AppendLine("FROM subscription.AccountTransaction AT");
            sb.AppendLine("INNER JOIN subscription.AccountTransactionType ATT");
            sb.AppendLine("ON ATT.AccountTransactionTypeID = AT.AccountTransactionTypeID");
            sb.AppendLine("AND ATT.TransactionType IN ('CreditAccountBalance')");
            sb.AppendLine("INNER JOIN subscription.PaymentException PE");
            sb.AppendLine("ON AT.PaymentFeedEntryID = PE.PaymentFeedEntryID");
            sb.AppendLine("WHERE AT.CreatedTimeStampt BETWEEN @0 AND @1 AND PE.[NewTelemarkerId] > 0");
            sb.AppendLine(") RESULT");
            sb.AppendLine("ORDER BY DateReceived DESC");

            //sb.AppendLine("SELECT OT.CreatedTimeStampt AS DateReceived, ");
            //sb.AppendLine("CASE WHEN OTT.TransactionType = 'CreditAccountBalance' AND ( THEN AS PaymentType, ");
            //sb.AppendLine("O.AccountID AS AccountID, O.OrderNumber AS OrderNumber,");
            ////sb.AppendLine("CASE WHEN OTT.TransactionType = 'CreditAccountBalance' THEN OT.Amount ELSE 0 END AS Cleared, ");
            //sb.AppendLine("OT.Amount AS Cleared, ");
            //sb.AppendLine("OT.Amount AS ExceptionAmount, NULL AS PaymentStatus");
            //sb.AppendLine("FROM subscription.OrderTransaction OT");
            //sb.AppendLine("INNER JOIN subscription.OrderTransactionType OTT");
            //sb.AppendLine("ON OTT.OrderTransactionTypeID = OT.OrderTransactionTypeID");
            //sb.AppendLine("AND OTT.TransactionType IN ('CreditAccountBalance', 'DebitAccountBalance')");
            //sb.AppendLine("LEFT OUTER JOIN subscription.[Order] O");
            //sb.AppendLine("ON O.OrderID = OT.OrderID");
            //sb.AppendLine("WHERE OT.CreatedTimeStampt BETWEEN @0 AND @1");
            //sb.AppendLine("ORDER BY DateReceived DESC");

            //sb.AppendLine("SELECT OT.CreatedTimeStampt AS DateReceived, OTT.TransactionType AS PaymentType, O.AccountID AS AccountID, O.OrderNumber AS OrderNumber,");
            //sb.AppendLine("CASE WHEN OTT.TransactionType = 'CreditAccountBalance' THEN OT.Amount ELSE 0 END AS Cleared, ");
            //sb.AppendLine("OT.Amount AS ExceptionAmount, NULL AS PaymentStatus");
            //sb.AppendLine("FROM subscription.OrderTransaction OT");
            //sb.AppendLine("INNER JOIN subscription.OrderTransactionType OTT");
            //sb.AppendLine("ON OTT.OrderTransactionTypeID = OT.OrderTransactionTypeID");
            //sb.AppendLine("AND OTT.TransactionType IN ('CreditAccountBalance', 'DebitAccountBalance')");
            //sb.AppendLine("LEFT OUTER JOIN subscription.[Order] O");
            //sb.AppendLine("ON O.OrderID = OT.OrderID");
            //sb.AppendLine("WHERE OT.CreatedTimeStampt BETWEEN @0 AND @1");
            //sb.AppendLine("ORDER BY DateReceived");

            return _repository.Fetch<FinancialReport_Exception>(sb.ToString(), fromDate.HasValue ? fromDate.Value.ToString(_dateTimeFormat) : "", toDate.ToString(_dateTimeFormat)).ToList();
        }

        public List<FinancialReport_Exception> GetExceptionDeferredCreditCardReceipts(DateTime? fromDate, DateTime toDate)
        {
            _repository.ConnectToDb(ConfigurationHelper.Instance.ConnectionStringName_Portal);

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("SELECT DateReceived, PaymentType, Reference, AccountID, ExceptionAmount, SettlementDate, MAX(Cleared), MAX(PaymentStatus) FROM (");
            sb.AppendLine("SELECT P.CreatedTimeStamp AS DateReceived, PM.PaymentMethodName AS PaymentType, P.PaymentReference AS Reference, O.AccountID AS AccountID, P.PaidAmount AS ExceptionAmount, P.ExpectedSettelmentDate AS SettlementDate,");
            sb.AppendLine("NULL AS Cleared, NULL AS PaymentStatus");
            sb.AppendLine("FROM subscription.Payment P");
            sb.AppendLine("INNER JOIN subscription.PaymentMethod PM");
            sb.AppendLine("ON PM.PaymentMethodID = P.PaymentMethodID");
            sb.AppendLine("AND PM.PaymentMethodName = 'CreditCard'");
            sb.AppendLine("INNER JOIN subscription.[Order] O");
            sb.AppendLine("ON O.OrderID = P.OrderID");
            sb.AppendLine("WHERE P.CreatedTimeStamp BETWEEN @0 AND @1");
            sb.AppendLine("AND P.IsPaymentSuccess = 1");
            sb.AppendLine("UNION");
            sb.AppendLine("SELECT P.CreatedTimeStamp AS DateReceived, PM.PaymentMethodName AS PaymentType, P.PaymentReference AS Reference, O.AccountID AS AccountID, P.PaidAmount AS ExceptionAmount, P.ExpectedSettelmentDate AS SettlementDate,");
            sb.AppendLine("P.PaidAmount AS Cleared, 'Payment Settled' AS PaymentStatus");
            sb.AppendLine("FROM subscription.Payment P");
            sb.AppendLine("INNER JOIN subscription.PaymentMethod PM");
            sb.AppendLine("ON PM.PaymentMethodID = P.PaymentMethodID");
            sb.AppendLine("AND PM.PaymentMethodName = 'CreditCard'");
            sb.AppendLine("INNER JOIN subscription.[Order] O");
            sb.AppendLine("ON O.OrderID = P.OrderID");
            sb.AppendLine("WHERE P.ExpectedSettelmentDate BETWEEN @0 AND @1");
            sb.AppendLine("AND P.IsPaymentSuccess = 1");
            sb.AppendLine(") AS RESULT");
            sb.AppendLine("WHERE SettlementDate IS NOT NULL");
            sb.AppendLine("GROUP BY DateReceived, PaymentType, Reference, AccountID, ExceptionAmount, SettlementDate");
            sb.AppendLine("ORDER BY DateReceived DESC");

            return _repository.Fetch<FinancialReport_Exception>(sb.ToString(), fromDate.HasValue ? fromDate.Value.ToString(_dateTimeFormat) : "", toDate.ToString(_dateTimeFormat)).ToList();
        }

        #endregion

        #region Credit Notes Report

        public List<FinancialReport_CreditNote> GetNewCreditNoteTransactions(DateTime? fromDate, DateTime toDate)
        {
            //-- CreditAccountBalance
            //-- Closed Order Payment -> OrderClosed + (OT.Amount < O.Amount)
            //-- Cancelled Order Payment -> OrderCancelled + (OT.Amount < O.Amount)
            //-- Expired Order Payment -> OrderExpired + (OT.Amount <> O.Amount)
            //-- Over Payment -> (O.Status = FullyPaid) + (OT.Amount <> O.Amount)
            //-- Duplicate Payment -> (O.Status = FullyPaid) + (OT.Amount = O.Amount)

            _repository.ConnectToDb(ConfigurationHelper.Instance.ConnectionStringName_Portal);

            return _repository.Fetch<FinancialReport_CreditNote>(Query_GetNewCreditNoteTransactions, fromDate.HasValue ? fromDate.Value.ToString(_dateTimeFormat) : "", toDate.ToString(_dateTimeFormat)).ToList();
        }

        public double GetTotalCreditNotesAppliedToPurchasesToday(DateTime? fromDate, DateTime toDate)
        {
            _repository.ConnectToDb(ConfigurationHelper.Instance.ConnectionStringName_Portal);

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("SELECT ISNULL(SUM(OT.Amount), 0)");
            sb.AppendLine("FROM subscription.OrderTransaction OT");
            sb.AppendLine("INNER JOIN subscription.OrderTransactionType OTT");
            sb.AppendLine("ON OTT.OrderTransactionTypeID = OT.OrderTransactionTypeID");
            sb.AppendLine("AND OTT.TransactionType IN ('DebitAccountBalance')");
            sb.AppendLine("WHERE OT.CreatedTimeStampt BETWEEN @0 AND @1");

            return _repository.ExecuteScalar<double>(sb.ToString(), fromDate.HasValue ? fromDate.Value.ToString(_dateTimeFormat) : "", toDate.ToString(_dateTimeFormat));
        }

        public double GetTotalCreditNotesRefundedToday(DateTime? fromDate, DateTime toDate)
        {
            _repository.ConnectToDb(ConfigurationHelper.Instance.ConnectionStringName_Portal);

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("SELECT ISNULL(SUM(AT.Amount), 0)");
            sb.AppendLine("FROM subscription.AccountTransaction AT");
            sb.AppendLine("INNER JOIN subscription.AccountTransactionType ATT");
            sb.AppendLine("ON ATT.AccountTransactionTypeID = AT.AccountTransactionTypeID");
            sb.AppendLine("AND ATT.TransactionType IN ('BalanceRefund')");
            sb.AppendLine("WHERE AT.CreatedTimeStampt BETWEEN @0 AND @1");

            return _repository.ExecuteScalar<double>(sb.ToString(), fromDate.HasValue ? fromDate.Value.ToString(_dateTimeFormat) : "", toDate.ToString(_dateTimeFormat));
        }

        public double GetTotalCreditNotesCreatedToday(DateTime? fromDate, DateTime toDate)
        {
            _repository.ConnectToDb(ConfigurationHelper.Instance.ConnectionStringName_Portal);

           

            return _repository.ExecuteScalar<double>(Query_CreditNotesCreatedTodayExcludingBalanceRefunds, fromDate.HasValue ? fromDate.Value.ToString(_dateTimeFormat) : "", toDate.ToString(_dateTimeFormat));
        }

        public List<FinancialReport_CreditNote> GetCreditNoteRegister(DateTime toDate)
        {
            //-- CreditAccountBalance
            //-- Closed Order Payment -> OrderClosed + (OT.Amount < O.Amount)
            //-- Cancelled Order Payment -> OrderCancelled + (OT.Amount < O.Amount)
            //-- Expired Order Payment -> OrderExpired + (OT.Amount <> O.Amount)
            //-- Over Payment -> (O.Status = FullyPaid) + (OT.Amount <> O.Amount)
            //-- Duplicate Payment -> (O.Status = FullyPaid) + (OT.Amount = O.Amount)

            //-- DebitAccountBalance = Applied to Sales

            _repository.ConnectToDb(ConfigurationHelper.Instance.ConnectionStringName_Portal);



            return _repository.Fetch<FinancialReport_CreditNote>(Query_GetCreditNoteRegister, toDate.ToString(_dateTimeFormat)).ToList();
        }

        #endregion
    }
}
