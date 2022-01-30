using SD.ACMA.DatabaseIntermediary;
using SD.ACMA.InterfaceTier;
using SD.ACMA.POCO.PetaPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace SD.ACMA.BusinessLogic.FinancialReport
{
    public class FinancialReportService : IFinancialReportService
    {        
        private IFinancialReportDataRepository _financialReportDataRepository;

        public FinancialReportService(IFinancialReportDataRepository financialReportDataRepository)
        {
            _financialReportDataRepository = financialReportDataRepository;
        }

        public List<POCO.PetaPoco.FinancialReport> GetAllFinancialReports()
        {
            return _financialReportDataRepository.GetAllFinancialReports();
        }
        public int InsertNewFinancialReport(DateTime? fromDate, DateTime toDate, double totalOutstandingExceptionsAmount, double totalCreditNotesAmount, double creditCardTimingDifference, string status, string markedFinalBy)
        {
            POCO.PetaPoco.FinancialReport financialReport = new POCO.PetaPoco.FinancialReport()
            {
                FromDate = fromDate,
                ToDate = toDate,
                TotalOutstandingExceptionsAmount = totalOutstandingExceptionsAmount,
                TotalCreditNotesAmount = totalCreditNotesAmount,
                CreditCardTimingDifference = creditCardTimingDifference,
                Status = status,
                MarkedFinalBy = ToDisplayUserName(markedFinalBy)
            };

            return _financialReportDataRepository.InsertFinancialReport(financialReport);
        }
        public void UpdateFinancialReport(int id, double totalOutstandingExceptionsAmount, double totalCreditNotesAmount, double creditCardTimingDifference, string status)
        {
            var financialReport = _financialReportDataRepository.GetFinancialReport(id);

            if (financialReport != null)
            {
                financialReport.TotalOutstandingExceptionsAmount = totalOutstandingExceptionsAmount;
                financialReport.TotalCreditNotesAmount = totalCreditNotesAmount;
                financialReport.CreditCardTimingDifference = creditCardTimingDifference;
                financialReport.Status = status;

                _financialReportDataRepository.UpdateFinancialReport(financialReport);
            }
        }
        public POCO.PetaPoco.FinancialReport GetLastFinancialReport()
        {
            return _financialReportDataRepository.GetLastFinancialReport();
        }

        public FinancialReport_PaymentsReconReport RunPaymentsReconReport(DateTime? fromDate, DateTime toDate)
        {
            FinancialReport_PaymentsReconReport reconReport = new FinancialReport_PaymentsReconReport();

            POCO.PetaPoco.FinancialReport previousFinancialReport = _financialReportDataRepository.GetPreviousFinancialReport(fromDate.HasValue ? fromDate.Value : DateTime.MinValue);

            if (previousFinancialReport != null)
            {
                reconReport.CreditCardTimingDifferenceBroughtForward =
                    previousFinancialReport.CreditCardTimingDifference;
            }

            reconReport.CreditCardSettlements_SUM_A = _financialReportDataRepository.GetCreditCardSettlements(fromDate, toDate);

            //reconReport.TotalCashReceipts_EFT_SUM_P1 = _financialReportDataRepository.GetTotalCashReceipts_EFT(fromDate, toDate);
            //reconReport.TotalCashReceipts_BPAY_SUM_P2 = _financialReportDataRepository.GetTotalCashReceipts_BPAY(fromDate, toDate);
            reconReport.TotalCashReceipts_UNKNOWN_SUM_P3 = _financialReportDataRepository.GetTotalCashReceipts_UNKNOWN(fromDate, toDate);
            reconReport.TotalCashReceipts_EFT_BPAY_SUM_P1_P2 = _financialReportDataRepository.GetTotalCashReceipts_BPAY_EFT(fromDate, toDate);

            reconReport.TotalReceipts_Sub1 = reconReport.CreditCardSettlements_SUM_A +
                                                (reconReport.TotalCashReceipts_EFT_BPAY_SUM_P1_P2 +
                                                //reconReport.TotalCashReceipts_EFT_SUM_P1 +
                                                //reconReport.TotalCashReceipts_BPAY_SUM_P2 +
                                                reconReport.TotalCashReceipts_UNKNOWN_SUM_P3);

            reconReport.Exception_UnmatchedPayments_U1 = _financialReportDataRepository.GetExceptionUnmatchedPayments(fromDate, toDate);
            reconReport.TotalExceptionUnmatchedPaymentAmount_U1 = reconReport.Exception_UnmatchedPayments_U1.Sum(p => p.Amount);
            
            reconReport.Exception_TransferToCreditNotes_U2 = _financialReportDataRepository.GetExceptionTransferToCreditNotes(fromDate, toDate);
            reconReport.TotalExceptionTransferToCreditNoteAmount_U2 = reconReport.Exception_TransferToCreditNotes_U2.Sum(p => p.Amount);
            
            reconReport.Exception_Underpayments_U3 = _financialReportDataRepository.GetExceptionUnderpayments(fromDate, toDate);
            reconReport.TotalExceptionUnderpaymentAmount_U3 = reconReport.Exception_Underpayments_U3.Sum(p => p.Amount);

            reconReport.Exception_ReversedDishonouredPayments_U4 = _financialReportDataRepository.GetExceptionReversedDishonouredPayments(fromDate, toDate);
            reconReport.TotalExceptionReversedDishonouredPaymentAmount_U4 = reconReport.Exception_ReversedDishonouredPayments_U4.Sum(p => p.Amount);

            reconReport.Exception_ErroneousPayments_U5 = _financialReportDataRepository.GetExceptionErroneousPayments(fromDate, toDate);
            reconReport.TotalExceptionErroneousPaymentAmount_U5 = reconReport.Exception_ErroneousPayments_U5.Sum(p => p.Amount);

            reconReport.Exception_CreditCardDeferredReceivedPayments_F = _financialReportDataRepository.GetExceptionCreditCardDeferredReceivedPayments(fromDate, toDate);
            reconReport.TotalExceptionCreditCardDeferredReceivedPaymentAmount_F = reconReport.Exception_CreditCardDeferredReceivedPayments_F.Sum(p => p.Amount);


            reconReport.TotalDeductions_Sub2 = reconReport.TotalExceptionUnmatchedPaymentAmount_U1 +
                                               reconReport.TotalExceptionTransferToCreditNoteAmount_U2 +
                                               reconReport.TotalExceptionUnderpaymentAmount_U3 +
                                               reconReport.TotalExceptionReversedDishonouredPaymentAmount_U4 +
                                               reconReport.TotalExceptionErroneousPaymentAmount_U5 +
                                               reconReport.TotalExceptionCreditCardDeferredReceivedPaymentAmount_F + 
                                               reconReport.CurrentPeriodTimingDifference;

            reconReport.NewCreditCardDeferralPayments_C = _financialReportDataRepository.GetNewCreditCardDeferralPayments(fromDate, toDate);
            reconReport.TotalNewCreditCardDeferralPaymentAmount_C = reconReport.NewCreditCardDeferralPayments_C.Sum(p => p.Amount);

            reconReport.DishonouredPayments_D1 = _financialReportDataRepository.GetDishonouredPayments(fromDate, toDate);
            reconReport.TotalDishonouredPayments_D1 = reconReport.DishonouredPayments_D1.Sum(p => p.Amount);

            reconReport.CreditNotesAppliedToSalesPayments_D2 = _financialReportDataRepository.GetCreditNotesAppliedToSalesPayments(fromDate, toDate);
            reconReport.TotalCreditNotesAppliedToSalesPayments_D2 = reconReport.CreditNotesAppliedToSalesPayments_D2.Sum(p => p.Amount);

            reconReport.ClearedPreviousException_MatchedPayments_E1 = _financialReportDataRepository.GetClearedPreviousException_MatchedPayments(fromDate, toDate);
            reconReport.TotalClearedPreviousException_MatchedPayments_E1 = reconReport.ClearedPreviousException_MatchedPayments_E1.Sum(p => p.Amount);

            reconReport.ClearedPreviousException_Underpayments_E2 = _financialReportDataRepository.GetClearedPreviousException_Underpayments(fromDate, toDate);
            reconReport.TotalClearedPreviousException_Underpayments_E2 = reconReport.ClearedPreviousException_Underpayments_E2.Sum(p => p.Amount);

            reconReport.TotalClearedPreviousExceptionPayments_D3 = reconReport.TotalClearedPreviousException_MatchedPayments_E1 +
                                                                        reconReport.TotalClearedPreviousException_Underpayments_E2;

            reconReport.TotalAdditions_Sub3 = reconReport.TotalNewCreditCardDeferralPaymentAmount_C +
                                        (reconReport.TotalDishonouredPayments_D1 +
                                        reconReport.TotalCreditNotesAppliedToSalesPayments_D2 +
                                        reconReport.TotalClearedPreviousExceptionPayments_D3);

            reconReport.TotalMatchedReceipts_Z = reconReport.TotalReceipts_Sub1 -
                                                    reconReport.TotalDeductions_Sub2 +
                                                    reconReport.TotalAdditions_Sub3;



            reconReport.AllActivatedSubscriptions_S1toS5 = _financialReportDataRepository.GetAllActivationsPayments(fromDate, toDate);
            reconReport.TotalActivatedSubscriptionsPayments_S = _financialReportDataRepository.GetTotalActivationsPayments(fromDate, toDate);

            reconReport.TotalReconciled = Math.Round(reconReport.TotalMatchedReceipts_Z - reconReport.TotalActivatedSubscriptionsPayments_S, 2, MidpointRounding.AwayFromZero);

            reconReport.RefundedPayments = _financialReportDataRepository.GetRefundedPayments(fromDate, toDate);

            return reconReport;
        }

        public FinancialReport_ExceptionsReport RunExceptionsReport(DateTime? fromDate, DateTime toDate)
        {
            FinancialReport_ExceptionsReport exceptionReport = new FinancialReport_ExceptionsReport();
            POCO.PetaPoco.FinancialReport previousFinancialReport = _financialReportDataRepository.GetPreviousFinancialReport(fromDate.HasValue ? fromDate.Value : DateTime.MinValue);
            
            exceptionReport.ExceptionUnmatchedAmounts = _financialReportDataRepository.GetExceptionUnmatchedAmounts(fromDate, toDate);

            // In the Day 0 report, which closes off history prior to the migration from previous vendor, we will ignore cleared exceptions
            if (previousFinancialReport == null)
            {
                exceptionReport.ExceptionUnmatchedAmounts = exceptionReport.ExceptionUnmatchedAmounts.Where(ex => ex.Cleared == 0).ToList();
            }


            //var underpayments = _financialReportDataRepository.GetExceptionUnderpayments(fromDate, toDate);

            //foreach (var item in exceptionReport.ExceptionUnmatchedAmounts)
            //{
            //    if (underpayments.Any(p => p.OrderNumber == item.OrderNumber))
            //    {
            //        item.PaymentStatus = "UnderPayment";
            //    }
            //}

            exceptionReport.ExceptionTransferredFromToCreditNotes = _financialReportDataRepository.GetExceptionTransferredFromToCreditNotes(fromDate, toDate);
            exceptionReport.ExceptionDeferredCreditCardReceipts = _financialReportDataRepository.GetExceptionDeferredCreditCardReceipts(fromDate, toDate);

            foreach (var item in exceptionReport.ExceptionDeferredCreditCardReceipts)
            {
                if (item.SettlementDate.HasValue && item.SettlementDate.Value <= toDate)
                {
                    item.Cleared = item.ExceptionAmount;                    
                    item.PaymentStatus = "Payment Settled";
                }
                
                if (item.DateReceived > toDate)
                {
                    item.ExceptionAmount = 0;
                }
            }

            if (previousFinancialReport != null)
                exceptionReport.TotalOutstandingExceptionBroughtForward_Sub4 = previousFinancialReport.TotalOutstandingExceptionsAmount;
            else
                exceptionReport.TotalOutstandingExceptionBroughtForward_Sub4 = 0;

            exceptionReport.TotalExceptionsTransferredToCreditNotes = exceptionReport.ExceptionTransferredFromToCreditNotes.Where(p => p.TransactionType == "CreditAccountBalance").Sum(p => p.ExceptionAmount);
            exceptionReport.TotalExceptionsTransferredFromCreditNotes = exceptionReport.ExceptionTransferredFromToCreditNotes.Where(p => p.TransactionType == "DebitAccountBalance").Sum(p => p.ExceptionAmount);

            //exceptionReport.TotalExceptionsTransferredFromToCreditNotes_Sub5 = exceptionReport.ExceptionTransferredFromToCreditNotes.Sum(p => p.ExceptionAmount);
            exceptionReport.TotalExceptionsTransferredFromToCreditNotes_Sub5 = exceptionReport.TotalExceptionsTransferredToCreditNotes + exceptionReport.TotalExceptionsTransferredFromCreditNotes;

            //exceptionReport.TotalNewExceptions_Sub6 = (exceptionReport.ExceptionUnmatchedAmounts.Where(p => p.Cleared <= 0 && (!fromDate.HasValue || p.DateReceived >= fromDate.Value) && p.DateReceived <= toDate).Sum(p => p.ExceptionAmount) +
            //                                            exceptionReport.ExceptionTransferredFromToCreditNotes.Where(p => p.Cleared <= 0 && (!fromDate.HasValue || p.DateReceived >= fromDate.Value) && p.DateReceived <= toDate).Sum(p => p.ExceptionAmount) +
            //                                            exceptionReport.ExceptionDeferredCreditCardReceipts.Where(p => p.Cleared <= 0 && (!fromDate.HasValue || p.DateReceived >= fromDate.Value) && p.DateReceived <= toDate).Sum(p => p.ExceptionAmount));

            exceptionReport.TotalNewExceptions_Sub6 = (exceptionReport.ExceptionUnmatchedAmounts.Where(p => (!fromDate.HasValue || p.DateReceived >= fromDate.Value) && p.DateReceived <= toDate).Sum(p => p.ExceptionAmount) +
                                                        exceptionReport.ExceptionTransferredFromToCreditNotes.Where(p => (!fromDate.HasValue || p.DateReceived >= fromDate.Value) && p.DateReceived <= toDate).Sum(p => p.ExceptionAmount) +
                                                        exceptionReport.ExceptionDeferredCreditCardReceipts.Where(p => (!fromDate.HasValue || p.DateReceived >= fromDate.Value) && p.DateReceived <= toDate).Sum(p => p.ExceptionAmount));

            exceptionReport.TotalExceptionsCleared_Sub7 = exceptionReport.ExceptionUnmatchedAmounts.Sum(p => p.Cleared) +
                                                            exceptionReport.ExceptionDeferredCreditCardReceipts.Sum(p => p.Cleared);

            exceptionReport.TotalOutstandingExceptions = (exceptionReport.TotalOutstandingExceptionBroughtForward_Sub4 + exceptionReport.TotalNewExceptions_Sub6) -
                                                            (exceptionReport.TotalExceptionsTransferredFromToCreditNotes_Sub5);

            // If this is the first report ever, then we do not want to track the cleared exceptions prior to cutover.
            // If it is *not* the first report, then we do need to track the cleared exceptions
            // UPDATE (26/10/2015) No, Ralf has changed his mind - https://jira.sd.salmat.com.au/browse/SDADS-1845?focusedCommentId=651690&page=com.atlassian.jira.plugin.system.issuetabpanels:comment-tabpanel#comment-651690
            
            //if (previousFinancialReport != null) 
            //{
                exceptionReport.TotalOutstandingExceptions -= exceptionReport.TotalExceptionsCleared_Sub7;
            //}

            List<FinancialReport_CreditNote> NewCreditNoteTransactions = _financialReportDataRepository.GetNewCreditNoteTransactions(fromDate, toDate);

            if (previousFinancialReport != null)
                exceptionReport.TotalCreditNotesBroughtForward_B = previousFinancialReport.TotalCreditNotesAmount;
            else
                exceptionReport.TotalCreditNotesBroughtForward_B = 0;

            exceptionReport.TotalCreditNotesAppliedToPurchasesToday_T = _financialReportDataRepository.GetTotalCreditNotesAppliedToPurchasesToday(fromDate, toDate);
            exceptionReport.TotalCreditNotesRefundedToday_R = _financialReportDataRepository.GetTotalCreditNotesRefundedToday(fromDate, toDate);
            exceptionReport.TotalCreditNotesCreatedToday_N = _financialReportDataRepository.GetTotalCreditNotesCreatedToday(fromDate, toDate);

            exceptionReport.TotalCreditNotes = exceptionReport.TotalCreditNotesBroughtForward_B -
                                                exceptionReport.TotalCreditNotesAppliedToPurchasesToday_T -
                                                exceptionReport.TotalCreditNotesRefundedToday_R +
                                                exceptionReport.TotalCreditNotesCreatedToday_N;

            return exceptionReport;
        }

        public FinancialReport_CreditNotesReport RunCreditNotesReport(DateTime? fromDate, DateTime toDate)
        {
            FinancialReport_CreditNotesReport creditNoteReport = new FinancialReport_CreditNotesReport();

            creditNoteReport.NewCreditNoteTransactions = _financialReportDataRepository.GetNewCreditNoteTransactions(fromDate, toDate);

            POCO.PetaPoco.FinancialReport previousFinancialReport = _financialReportDataRepository.GetPreviousFinancialReport(fromDate.HasValue ? fromDate.Value : DateTime.MinValue);

            if (previousFinancialReport != null)
                creditNoteReport.TotalCreditNotesBroughtForward_B = previousFinancialReport.TotalCreditNotesAmount;
            else
                creditNoteReport.TotalCreditNotesBroughtForward_B = 0;

            creditNoteReport.TotalCreditNotesAppliedToPurchasesToday_T = _financialReportDataRepository.GetTotalCreditNotesAppliedToPurchasesToday(fromDate, toDate);
            creditNoteReport.TotalCreditNotesRefundedToday_R = _financialReportDataRepository.GetTotalCreditNotesRefundedToday(fromDate, toDate);

            /*TODO - if the Credit Note listings are ever fixed such that instances of CN-00000 are replaced with the actual 
             credit notes they are impersonating, the below calculation will break and need to be re-examined*/
            creditNoteReport.TotalCreditNotesCreatedToday_N = _financialReportDataRepository.GetTotalCreditNotesCreatedToday(fromDate, toDate);

            creditNoteReport.TotalCreditNotes = creditNoteReport.TotalCreditNotesBroughtForward_B -
                                                creditNoteReport.TotalCreditNotesAppliedToPurchasesToday_T -
                                                creditNoteReport.TotalCreditNotesRefundedToday_R +
                                                creditNoteReport.TotalCreditNotesCreatedToday_N;

            creditNoteReport.CreditNoteRegister = _financialReportDataRepository.GetCreditNoteRegister(toDate);

            return creditNoteReport;
        }

        public void UpdateAllFinancialReports()
        {
            var allFinancialReports = GetAllFinancialReports().OrderBy(p => p.Id);

            foreach (var item in allFinancialReports)
            {
                FinancialReport_ExceptionsReport exceptionReport = RunExceptionsReport(item.FromDate, item.ToDate);
                FinancialReport_CreditNotesReport creditNotesReport = RunCreditNotesReport(item.FromDate, item.ToDate);
                FinancialReport_PaymentsReconReport paymentReconReport = RunPaymentsReconReport(item.FromDate, item.ToDate);
                string status = paymentReconReport.TotalReconciled == 0 ? "Reconciled" : (item.FromDate != null ? "Error" : "Pre-Transition Data");

                UpdateFinancialReport(item.Id, exceptionReport.TotalOutstandingExceptions, exceptionReport.TotalCreditNotes, paymentReconReport.ClosingCreditCardTimingDifference, status);
            }
        }

        private string SplitCamelCase(string input)
        {
            if (!String.IsNullOrWhiteSpace(input))
            {
                var cleanInput = input.Replace("ID", "Id");
                if (cleanInput.Length <= 4)
                    return cleanInput;

                return Regex.Replace(cleanInput, "(?<=[a-z])(?=[A-Z])|(?<=[A-Z])(?=[A-Z][a-z])", " ", System.Text.RegularExpressions.RegexOptions.Compiled).Trim();
            }
            return String.Empty;
        }

        private string ToDisplayUserName(string input)
        {
            if (input.Contains("\\"))
                input = input.Substring(input.LastIndexOf("\\") + 1);

            input = input.Replace('.', ' ');
            input = SplitCamelCase(input);
            input = Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(input.ToLower());

            return input.Trim();
        }
    }
}
