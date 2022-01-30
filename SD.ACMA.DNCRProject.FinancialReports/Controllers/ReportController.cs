using Ionic.Zip;
using SD.ACMA.BusinessLogic.Helpers;
using SD.ACMA.DNCR.Infrastructure;
using SD.ACMA.DNCR.Infrastructure.Configuration;
using SD.ACMA.DNCRProject.FinancialReports.Models;
using SD.ACMA.InterfaceTier;
using SD.ACMA.POCO.PetaPoco;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace SD.ACMA.DNCRProject.FinancialReports.Controllers
{
    public class ReportController : Controller
    {
        private IFinancialReportService _financialReportService;

        private const string dateTimeFormat = "dd/MM/yyyy hh:mm:ss tt";
        private const string dateFormat = "dd/MM/yyyy HH:mm";
        //private const string currencyFormat = "$###,###,##0.00";
        private const string currencyFormat = "C";
        private const string creditNoteFormat = "CN-00000";

        private DateTime? _fromDate = null;
        private DateTime _toDate;
        private bool IsCurrent = true;
        private NumberFormatInfo _nfi = new NumberFormatInfo();

        private readonly string _internalUsername = ConfigurationHelper.Instance.FinancialReportInternalUsername;
        private readonly string _internalPassword = ConfigurationHelper.Instance.FinancialReportInternalPassword;

        private string InternalHostFix(string url)
        {
            if (!string.IsNullOrEmpty(ConfigurationHelper.Instance.FinancialReportInternalHost))
            {
                return url.Replace(Request.Url.Host, ConfigurationHelper.Instance.FinancialReportInternalHost); 
            }

            return url;
        }

        public ReportController(IFinancialReportService financialReportService)
        {
            _financialReportService = financialReportService;

            POCO.PetaPoco.FinancialReport lastFinancialReport = _financialReportService.GetLastFinancialReport();

            if (lastFinancialReport != null)
            {
                _fromDate = lastFinancialReport.ToDate;
            }

            _toDate = DateTime.Now.AddDays(0);
            
            _nfi.CurrencySymbol = "$";
            _nfi.CurrencyNegativePattern = 0;            
        }

        public ActionResult Index()
        {
            if (TempData.ContainsKey("fromDate") && TempData.ContainsKey("toDate"))
            {
                if (TempData["fromDate"].ToString() != "")
                    _fromDate = Convert.ToDateTime(TempData["fromDate"]);
                else
                    _fromDate = null;

                _toDate = Convert.ToDateTime(TempData["toDate"]);

                IsCurrent = false;
            }

            ReportViewModel model = new ReportViewModel();
            model.FromDate = _fromDate;
            model.ToDate = _toDate;
            model.FromDateString = _fromDate.HasValue ? _fromDate.Value.ToString(dateTimeFormat) : "NOT SET";
            model.ToDateString = _toDate.ToString(dateTimeFormat);

            model.HistoricalFinancialReports = new List<ReportViewModel_FinancialReport>();

            List<POCO.PetaPoco.FinancialReport> allFinancialReports = _financialReportService.GetAllFinancialReports();

            foreach (var item in allFinancialReports)
            {                
                model.HistoricalFinancialReports.Add(new ReportViewModel_FinancialReport()
                {
                    ReportPeriodID = item.Id,
                    FromDate = item.FromDate,
                    ToDate = item.ToDate,
                    FromDateString = item.FromDate.HasValue ? item.FromDate.Value.ToString(dateTimeFormat) : "NOT SET",
                    ToDateString = item.ToDate.ToString(dateTimeFormat),
                    MarkedFinalBy = !String.IsNullOrEmpty(item.MarkedFinalBy) ? item.MarkedFinalBy : "N/A",
                    Status = item.Status
                });
            }

            model.IsCurrent = IsCurrent;

            return View(model);
        }

        public ActionResult RunCurrent()
        {
            return RedirectToAction("Index");
        }

        public ActionResult RunReport(DateTime? fromDate, DateTime toDate)
        {
            TempData["fromDate"] = fromDate.HasValue ? fromDate.Value.ToString() : "";
            TempData["toDate"] = toDate;

            return RedirectToAction("Index");
        }

        public FileResult DownloadReports(DateTime? fromDate, DateTime toDate, bool isCurrent)
        {
            byte[] paymentsReconReport = null;
            byte[] exceptionsReport = null;
            byte[] creditNotesReport = null;

            Uri uri;
            //Stream sHtml;

            using (PDFConverter converter = new PDFConverter(_internalUsername, _internalPassword))
            {
                uri = new Uri(InternalHostFix(Url.Action("PaymentsReconciliationReport", "Report", new { fromDate = fromDate, toDate = toDate, IsCurrent = isCurrent }, Request.Url.Scheme)), UriKind.Absolute);
                paymentsReconReport = converter.ConvertFromURL(uri.ToString());

                //using (WebClient wc = new WebClient())
                //{
                //    sHtml = wc.OpenRead(uri.ToString());
                //    paymentsReconReport = converter.ConvertFromStream(sHtml);
                //}

                uri = new Uri(InternalHostFix(Url.Action("ExceptionsReport", "Report", new { fromDate = fromDate, toDate = toDate, IsCurrent = isCurrent }, Request.Url.Scheme)), UriKind.Absolute);
                exceptionsReport = converter.ConvertFromURL(uri.ToString());

                //using (WebClient wc = new WebClient())
                //{
                //    sHtml = wc.OpenRead(uri.ToString());
                //    exceptionsReport = converter.ConvertFromStream(sHtml);
                //}

                uri = new Uri(InternalHostFix(Url.Action("CreditNotesReport", "Report", new { fromDate = fromDate, toDate = toDate, IsCurrent = isCurrent }, Request.Url.Scheme)), UriKind.Absolute);
                creditNotesReport = converter.ConvertFromURL(uri.ToString());

                //using (WebClient wc = new WebClient())
                //{
                //    sHtml = wc.OpenRead(uri.ToString());
                //    creditNotesReport = converter.ConvertFromStream(sHtml);
                //}
            }

            var outputStream = new MemoryStream();

            using (var zip = new ZipFile())
            {
                zip.AddEntry(string.Format("DNCR_FinancialReport_PaymentsReconReport_{0}_{1}.pdf", fromDate.HasValue ? fromDate.Value.ToString("yyyyMMddhhmmss") : "NotSet", toDate.ToString("yyyyMMddhhmmss")), paymentsReconReport);
                zip.AddEntry(string.Format("DNCR_FinancialReport_ExceptionsReport_{0}_{1}.pdf", fromDate.HasValue ? fromDate.Value.ToString("yyyyMMddhhmmss") : "NotSet", toDate.ToString("yyyyMMddhhmmss")), exceptionsReport);
                zip.AddEntry(string.Format("DNCR_FinancialReport_CreditNotesReport_{0}_{1}.pdf", fromDate.HasValue ? fromDate.Value.ToString("yyyyMMddhhmmss") : "NotSet", toDate.ToString("yyyyMMddhhmmss")), creditNotesReport);

                zip.Save(outputStream);
            }

            var cd = new System.Net.Mime.ContentDisposition
            {
                // for example foo.bak
                FileName = string.Format("DNCR_FinancialReport_{0}_{1}.zip", fromDate.HasValue ? fromDate.Value.ToString("yyyyMMddhhmmss") : "NotSet", toDate.ToString("yyyyMMddhhmmss")),

                // always prompt the user for downloading, set to true if you want 
                // the browser to try to show the file inline
                Inline = false
            };
            Response.AppendHeader("Content-Disposition", cd.ToString());

            outputStream.Position = 0;
            return File(outputStream, "application/zip");
        }

        public ActionResult MarkFinal(DateTime? fromDate, DateTime toDate)
        {


            FinancialReport_ExceptionsReport exceptionReport = _financialReportService.RunExceptionsReport(fromDate, toDate);
            FinancialReport_CreditNotesReport creditNoteReport = _financialReportService.RunCreditNotesReport(fromDate, toDate);
            FinancialReport_PaymentsReconReport paymentReport = _financialReportService.RunPaymentsReconReport(fromDate, toDate);
            string status = paymentReport.TotalReconciled == 0 ? "Reconciled" : "Error";
            string userName = System.Web.HttpContext.Current.User.Identity.Name;

            _financialReportService.InsertNewFinancialReport(fromDate, toDate, exceptionReport.TotalOutstandingExceptions, exceptionReport.TotalCreditNotes, paymentReport.ClosingCreditCardTimingDifference, status, userName);

            TempData["Message"] = "<strong>Success!</strong> Report has been marked as final.";

            return RedirectToAction("RunReport", new { fromDate = fromDate, toDate = toDate });
        }


        public ActionResult UpdateAllRunningTotals()
        {
            // Recalculate all outstanding exceptions and credit notes for each report.
            // Will not need to do this if the reports are running correctly.
            NLog.LogManager.GetCurrentClassLogger().Info("UpdateAllRunningTotals Started");
            _financialReportService.UpdateAllFinancialReports();
            NLog.LogManager.GetCurrentClassLogger().Info("UpdateAllRunningTotals Completed");

            return RedirectToAction("index");
        }

        public ActionResult PaymentsReconciliationReport(DateTime? fromDate, DateTime toDate, bool isCurrent)
        {
            PaymentReconReportViewModel model = new PaymentReconReportViewModel();

            model.FromDateString = fromDate.HasValue ? fromDate.Value.ToString(dateTimeFormat) : "NOT SET";
            model.ToDateString = toDate.ToString(dateTimeFormat);
            model.IsCurrent = isCurrent;

            FinancialReport_PaymentsReconReport paymentsReconReport = _financialReportService.RunPaymentsReconReport(fromDate, toDate);

            if (paymentsReconReport != null)
            {
                model.CreditCardSettlements_SUM_A = paymentsReconReport.CreditCardSettlements_SUM_A.ToString(currencyFormat, _nfi);
                //model.TotalCashReceipts_EFT_SUM_P1 = paymentsReconReport.TotalCashReceipts_EFT_SUM_P1.ToString(currencyFormat, _nfi);
                //model.TotalCashReceipts_BPAY_SUM_P2 = paymentsReconReport.TotalCashReceipts_BPAY_SUM_P2.ToString(currencyFormat, _nfi);
                model.TotalCashReceipts_EFT_BPAY_SUM_P1_P2 = paymentsReconReport.TotalCashReceipts_EFT_BPAY_SUM_P1_P2.ToString(currencyFormat, _nfi);
                model.TotalCashReceipts_UNKNOWN_SUM_P3 = paymentsReconReport.TotalCashReceipts_UNKNOWN_SUM_P3.ToString(currencyFormat, _nfi);
                model.TotalReceipts_Sub1 = paymentsReconReport.TotalReceipts_Sub1.ToString(currencyFormat, _nfi);

                model.ExceptionUnmatchedPayments_U1 = new List<PaymentReconReportViewModel_Payment>();
                foreach (var item in paymentsReconReport.Exception_UnmatchedPayments_U1)
                {
                    model.ExceptionUnmatchedPayments_U1.Add(new PaymentReconReportViewModel_Payment()
                        {
                            PaymentDate = item.PaymentDate.ToString(dateFormat),
                            PaymentReference = item.PaymentReference,
                            Amount = item.Amount.ToString(currencyFormat, _nfi)
                        });
                }
                model.TotalExceptionUnmatchedPaymentAmount_U1 = paymentsReconReport.TotalExceptionUnmatchedPaymentAmount_U1.ToString(currencyFormat, _nfi);

                model.ExceptionTransferToCreditNotes_U2 = new List<PaymentReconReportViewModel_Payment>();
                foreach (var item in paymentsReconReport.Exception_TransferToCreditNotes_U2)
                {
                    model.ExceptionTransferToCreditNotes_U2.Add(new PaymentReconReportViewModel_Payment()
                    {
                        PaymentDate = item.PaymentDate.ToString(dateFormat),
                        PaymentReference = string.Format("{0}; {1}", item.PaymentReference, item.CreditNoteNumber.ToString(creditNoteFormat)),
                        AccountID = item.AccountID,
                        Amount = item.Amount.ToString(currencyFormat, _nfi)
                    });
                }
                model.TotalExceptionTransferToCreditNoteAmount_U2 = paymentsReconReport.TotalExceptionTransferToCreditNoteAmount_U2.ToString(currencyFormat, _nfi);

                model.ExceptionUnderpayments_U3 = new List<PaymentReconReportViewModel_Payment>();
                foreach (var item in paymentsReconReport.Exception_Underpayments_U3)
                {
                    model.ExceptionUnderpayments_U3.Add(new PaymentReconReportViewModel_Payment()
                    {
                        PaymentDate = item.PaymentDate.ToString(dateFormat),
                        PaymentReference = item.PaymentReference,
                        AccountID = item.AccountID,
                        Amount = item.Amount.ToString(currencyFormat, _nfi)
                    });
                }
                model.TotalExceptionUnderpaymentAmount_U3 = paymentsReconReport.TotalExceptionUnderpaymentAmount_U3.ToString(currencyFormat, _nfi);

                model.ExceptionReversedDishonouredPayments_U4 = new List<PaymentReconReportViewModel_Payment>();
                foreach (var item in paymentsReconReport.Exception_ReversedDishonouredPayments_U4)
                {
                    model.ExceptionReversedDishonouredPayments_U4.Add(new PaymentReconReportViewModel_Payment()
                    {
                        PaymentDate = item.PaymentDate.ToString(dateFormat),
                        PaymentReference = item.PaymentReference,
                        AccountID = item.AccountID,
                        Amount = item.Amount.ToString(currencyFormat, _nfi)
                    });
                }
                model.TotalExceptionReversedDishonouredPaymentAmount_U4 = paymentsReconReport.TotalExceptionReversedDishonouredPaymentAmount_U4.ToString(currencyFormat, _nfi);

                model.ExceptionErroneousPayments_U5 = new List<PaymentReconReportViewModel_Payment>();
                foreach (var item in paymentsReconReport.Exception_ErroneousPayments_U5)
                {
                    model.ExceptionErroneousPayments_U5.Add(new PaymentReconReportViewModel_Payment()
                    {
                        PaymentDate = item.PaymentDate.ToString(dateFormat),
                        PaymentReference = item.PaymentReference,
                        AccountID = item.AccountID,
                        Amount = item.Amount.ToString(currencyFormat, _nfi)
                    });
                }
                model.TotalExceptionErroneousPaymentAmount_U5 = paymentsReconReport.TotalExceptionErroneousPaymentAmount_U5.ToString(currencyFormat, _nfi);

                model.ExceptionCreditCardDeferredReceivedPayments_F = new List<PaymentReconReportViewModel_Payment>();
                foreach (var item in paymentsReconReport.Exception_CreditCardDeferredReceivedPayments_F)
                {
                    model.ExceptionCreditCardDeferredReceivedPayments_F.Add(new PaymentReconReportViewModel_Payment()
                    {
                        PaymentDate = item.PaymentDate.ToString(dateFormat),
                        PaymentReference = item.PaymentReference,
                        AccountID = item.AccountID,
                        Amount = item.Amount.ToString(currencyFormat, _nfi)
                    });
                }
                model.TotalExceptionCreditCardDeferredReceivedPaymentAmount_F = paymentsReconReport.TotalExceptionCreditCardDeferredReceivedPaymentAmount_F.ToString(currencyFormat, _nfi);

                model.TotalDeductions_Sub2 = paymentsReconReport.TotalDeductions_Sub2.ToString(currencyFormat, _nfi);

                model.NewCreditCardDeferralPayments_C = new List<PaymentReconReportViewModel_Payment>();
                foreach (var item in paymentsReconReport.NewCreditCardDeferralPayments_C)
                {
                    model.NewCreditCardDeferralPayments_C.Add(new PaymentReconReportViewModel_Payment()
                    {
                        PaymentDate = item.PaymentDate.ToString(dateFormat),
                        PaymentReference = item.PaymentReference,
                        AccountID = item.AccountID,
                        Amount = item.Amount.ToString(currencyFormat, _nfi)
                    });
                }
                model.TotalNewCreditCardDeferralPaymentAmount_C = paymentsReconReport.TotalNewCreditCardDeferralPaymentAmount_C.ToString(currencyFormat, _nfi);

                model.DishonouredPayments_D1 = new List<PaymentReconReportViewModel_Payment>();
                foreach (var item in paymentsReconReport.DishonouredPayments_D1)
                {
                    model.DishonouredPayments_D1.Add(new PaymentReconReportViewModel_Payment()
                    {
                        PaymentDate = item.PaymentDate.ToString(dateFormat),
                        PaymentReference = item.PaymentReference,
                        AccountID = item.AccountID,
                        Amount = item.Amount.ToString(currencyFormat, _nfi)
                    });
                }
                model.TotalDishonouredPayments_D1 = paymentsReconReport.TotalDishonouredPayments_D1.ToString(currencyFormat, _nfi);

                model.CreditNotesAppliedToSalesPayments_D2 = new List<PaymentReconReportViewModel_Payment>();
                foreach (var item in paymentsReconReport.CreditNotesAppliedToSalesPayments_D2)
                {
                    model.CreditNotesAppliedToSalesPayments_D2.Add(new PaymentReconReportViewModel_Payment()
                    {
                        PaymentDate = item.PaymentDate.ToString(dateFormat),
                        PaymentReference = item.PaymentReference,
                        AccountID = item.AccountID,
                        Amount = item.Amount.ToString(currencyFormat, _nfi)
                    });
                }
                model.TotalCreditNotesAppliedToSalesPayments_D2 = paymentsReconReport.TotalCreditNotesAppliedToSalesPayments_D2.ToString(currencyFormat, _nfi);

                model.ClearedPreviousException_MatchedPayments_E1 = new List<PaymentReconReportViewModel_Payment>();
                foreach (var item in paymentsReconReport.ClearedPreviousException_MatchedPayments_E1)
                {
                    model.ClearedPreviousException_MatchedPayments_E1.Add(new PaymentReconReportViewModel_Payment()
                    {
                        PaymentDate = item.PaymentDate.ToString(dateFormat),
                        PaymentReference = item.PaymentReference,
                        AccountID = item.AccountID,
                        Amount = item.Amount.ToString(currencyFormat, _nfi)
                    });
                }
                model.TotalClearedPreviousException_MatchedPayments_E1 = paymentsReconReport.TotalClearedPreviousException_MatchedPayments_E1.ToString(currencyFormat, _nfi);

                model.ClearedPreviousException_Underpayments_E2 = new List<PaymentReconReportViewModel_Payment>();
                foreach (var item in paymentsReconReport.ClearedPreviousException_Underpayments_E2)
                {
                    model.ClearedPreviousException_Underpayments_E2.Add(new PaymentReconReportViewModel_Payment()
                    {
                        PaymentDate = item.PaymentDate.ToString(dateFormat),
                        PaymentReference = item.PaymentReference,
                        AccountID = item.AccountID,
                        Amount = item.Amount.ToString(currencyFormat, _nfi)
                    });
                }
                model.TotalClearedPreviousException_Underpayments_E2 = paymentsReconReport.TotalClearedPreviousException_Underpayments_E2.ToString(currencyFormat, _nfi);

                model.ClearedPreviousExceptionPayments_D3 = new List<PaymentReconReportViewModel_Payment>();
                model.ClearedPreviousExceptionPayments_D3.AddRange(model.ClearedPreviousException_MatchedPayments_E1);
                model.ClearedPreviousExceptionPayments_D3.AddRange(model.ClearedPreviousException_Underpayments_E2);
                model.ClearedPreviousExceptionPayments_D3 = model.ClearedPreviousExceptionPayments_D3.OrderByDescending(p => p.PaymentDate).ToList();
                model.TotalClearedPreviousExceptionPayments_D3 = paymentsReconReport.TotalClearedPreviousExceptionPayments_D3.ToString(currencyFormat, _nfi);

                model.TotalAdditions_Sub3 = paymentsReconReport.TotalAdditions_Sub3.ToString(currencyFormat, _nfi);

                model.TotalMatchedReceipts_Z = paymentsReconReport.TotalMatchedReceipts_Z.ToString(currencyFormat, _nfi);

                model.TotalActivatedSubscriptionsPayments_S = paymentsReconReport.TotalActivatedSubscriptionsPayments_S.ToString(currencyFormat, _nfi);
                model.AllActivatedSubscriptions_S1to5 = paymentsReconReport.AllActivatedSubscriptions_S1toS5.Select(s => new SD.ACMA.DNCRProject.FinancialReports.Models.ActivatedSubscription()
                {
                    Description = string.Format("{0}; 1 X {1}; {2}", s.OrderNumber, s.SubscriptionName, s.BusinessName),
                    Amount = s.Price.ToString(currencyFormat, _nfi)
                }).ToList();


                model.TotalReconciled = paymentsReconReport.TotalReconciled;

                model.RefundedPayments = new List<PaymentReconReportViewModel_Payment>();
                foreach (var item in paymentsReconReport.RefundedPayments)
                {
                    model.RefundedPayments.Add(new PaymentReconReportViewModel_Payment()
                    {
                        PaymentDate = item.PaymentDate.ToString(dateFormat),
                        PaymentReference = item.PaymentReference,
                        AccountID = item.AccountID,
                        Amount = item.Amount.ToString(currencyFormat, _nfi)
                    });
                }

                model.CreditCardTimingDifferenceBroughtForward =
                    paymentsReconReport.CreditCardTimingDifferenceBroughtForward.ToString(currencyFormat, _nfi);

                model.CurrentPeriodTimingDifference = paymentsReconReport.CurrentPeriodTimingDifference.ToString(currencyFormat, _nfi);
                model.ClosingCreditCardTimingDifference = paymentsReconReport.ClosingCreditCardTimingDifference.ToString(currencyFormat, _nfi);
            }

            return View(model);
        }

        public ActionResult ExceptionsReport(DateTime? fromDate, DateTime toDate, bool isCurrent)
        {
            ExceptionReportViewModel model = new ExceptionReportViewModel();

            model.FromDateString = fromDate.HasValue ? fromDate.Value.ToString(dateTimeFormat) : "NOT SET";
            model.ToDateString = toDate.ToString(dateTimeFormat);
            model.IsCurrent = isCurrent;

            FinancialReport_ExceptionsReport exceptionReport = _financialReportService.RunExceptionsReport(fromDate, toDate);

            if (exceptionReport != null)
            {
                model.ExceptionUnmatchedAmounts = new List<ExceptionReportViewModel_Exception>();
                foreach (var item in exceptionReport.ExceptionUnmatchedAmounts)
                {
                    model.ExceptionUnmatchedAmounts.Add(new ExceptionReportViewModel_Exception()
                    {
                        DateReceived = item.DateReceived.ToString(dateFormat),
                        PaymentType = item.PaymentType,
                        Reference = item.Reference,
                        OrderNumber = item.OrderNumber,
                        ExceptionAmount = item.ExceptionAmount.ToString(currencyFormat, _nfi),
                        Cleared = item.Cleared.ToString(currencyFormat, _nfi),
                        PaymentStatus = item.PaymentStatus 
                    });
                }

                model.ExceptionTransferredFromToCreditNotes = new List<ExceptionReportViewModel_Exception>();
                foreach (var item in exceptionReport.ExceptionTransferredFromToCreditNotes)
                {
                    model.ExceptionTransferredFromToCreditNotes.Add(new ExceptionReportViewModel_Exception()
                    {
                        DateReceived = item.DateReceived.ToString(dateFormat),
                        PaymentType = item.PaymentType,
                        AccountID = item.AccountID,
                        OrderNumber = item.OrderNumber,
                        ExceptionAmount = item.ExceptionAmount > 0 ? item.ExceptionAmount.ToString(currencyFormat, _nfi) : null,
                        Cleared = item.Cleared > 0 ? item.Cleared.ToString(currencyFormat, _nfi) : null,
                        PaymentStatus = string.Format("{0}, {1}", item.PaymentStatus, item.CreditNoteNumber.ToString(creditNoteFormat))
                    });
                }

                model.ExceptionDeferredCreditCardReceipts = new List<ExceptionReportViewModel_Exception>();
                foreach (var item in exceptionReport.ExceptionDeferredCreditCardReceipts)
                {
                    model.ExceptionDeferredCreditCardReceipts.Add(new ExceptionReportViewModel_Exception()
                    {
                        DateReceived = item.DateReceived.ToString(dateFormat),
                        PaymentType = item.PaymentType,
                        AccountID = item.AccountID,
                        Reference = item.Reference,
                        OrderNumber = item.OrderNumber,
                        ExceptionAmount = item.ExceptionAmount > 0 ? item.ExceptionAmount.ToString(currencyFormat, _nfi) : null,
                        Cleared = item.Cleared > 0 ? item.Cleared.ToString(currencyFormat, _nfi) : null,
                        PaymentStatus = item.PaymentStatus
                    });
                }

                model.TotalOutstandingExceptionBroughtForward_Sub4 = exceptionReport.TotalOutstandingExceptionBroughtForward_Sub4.ToString(currencyFormat, _nfi);
                model.TotalExceptionsTransferredFromCreditNotes = exceptionReport.TotalExceptionsTransferredFromCreditNotes.ToString(currencyFormat, _nfi);
                model.TotalExceptionsTransferredToCreditNotes = exceptionReport.TotalExceptionsTransferredToCreditNotes.ToString(currencyFormat, _nfi);
                //model.TotalExceptionsTransferredFromToCreditNotes_Sub5 = exceptionReport.TotalExceptionsTransferredFromToCreditNotes_Sub5.ToString(currencyFormat, _nfi);
                model.TotalNewExceptions_Sub6 = exceptionReport.TotalNewExceptions_Sub6.ToString(currencyFormat, _nfi);
                model.TotalExceptionsCleared_Sub7 = exceptionReport.TotalExceptionsCleared_Sub7.ToString(currencyFormat, _nfi);
                model.TotalOutstandingExceptions = exceptionReport.TotalOutstandingExceptions.ToString(currencyFormat, _nfi);

                model.TotalCreditNotesBroughtForward_B = exceptionReport.TotalCreditNotesBroughtForward_B.ToString(currencyFormat, _nfi);
                model.TotalCreditNotesAppliedToPurchasesToday_T = exceptionReport.TotalCreditNotesAppliedToPurchasesToday_T.ToString(currencyFormat, _nfi);
                model.TotalCreditNotesCreatedToday_N = exceptionReport.TotalCreditNotesCreatedToday_N.ToString(currencyFormat, _nfi);
                model.TotalCreditNotesRefundedToday_R = exceptionReport.TotalCreditNotesRefundedToday_R.ToString(currencyFormat, _nfi);
                model.TotalCreditNotes = exceptionReport.TotalCreditNotes.ToString(currencyFormat, _nfi);
            }

            return View(model);
        }

        public ActionResult CreditNotesReport(DateTime? fromDate, DateTime toDate, bool isCurrent)
        {
            CreditNoteReportViewModel model = new CreditNoteReportViewModel();

            model.FromDateString = fromDate.HasValue ? fromDate.Value.ToString(dateTimeFormat) : "NOT SET";
            model.ToDateString = toDate.ToString(dateTimeFormat);
            model.IsCurrent = isCurrent;

            FinancialReport_CreditNotesReport creditNoteReport = _financialReportService.RunCreditNotesReport(fromDate, toDate);

            if (creditNoteReport != null)
            {
                model.NewCreditNoteTransactions = new List<CreditNoteReportViewModel_CreditNote>();
                foreach (var item in creditNoteReport.NewCreditNoteTransactions)
                {
                    model.NewCreditNoteTransactions.Add(new CreditNoteReportViewModel_CreditNote()
                    {
                        DateReceived = item.DateReceived.ToString(dateFormat),
                        PaymentType = string.IsNullOrEmpty(item.PaymentType) ? "N/A" : item.PaymentType,
                        AccountID = item.AccountID,
                        CreditNoteNumber = item.CreditNoteNumber.ToString(creditNoteFormat),
                        CreditNoteAmount = item.CreditNoteAmount.ToString(currencyFormat, _nfi),
                        AccountBalance = item.AccountBalance.ToString(currencyFormat, _nfi),
                        PaymentNote = item.PaymentNote,
                        OrderNumber = string.IsNullOrEmpty(item.OrderNumber) ? "N/A" : item.OrderNumber
                    });
                }                

                model.TotalCreditNotesBroughtForward_B = creditNoteReport.TotalCreditNotesBroughtForward_B.ToString(currencyFormat, _nfi);
                model.TotalCreditNotesAppliedToPurchasesToday_T = creditNoteReport.TotalCreditNotesAppliedToPurchasesToday_T.ToString(currencyFormat, _nfi);
                model.TotalCreditNotesCreatedToday_N = creditNoteReport.TotalCreditNotesCreatedToday_N.ToString(currencyFormat, _nfi);
                model.TotalCreditNotesRefundedToday_R = creditNoteReport.TotalCreditNotesRefundedToday_R.ToString(currencyFormat, _nfi);
                model.TotalCreditNotes = creditNoteReport.TotalCreditNotes.ToString(currencyFormat, _nfi);

                model.CreditNoteRegister = new List<CreditNoteReportViewModel_CreditNote>();
                foreach (var item in creditNoteReport.CreditNoteRegister)
                {
                    model.CreditNoteRegister.Add(new CreditNoteReportViewModel_CreditNote()
                    {
                        DateReceived = item.DateReceived.ToString(dateFormat),
                        PaymentType = string.IsNullOrEmpty(item.PaymentType) ? "N/A" : item.PaymentType,
                        AccountID = item.AccountID,
                        CreditNoteNumber = item.CreditNoteNumber.ToString(creditNoteFormat),
                        CreditNoteAmount = item.CreditNoteAmount.ToString(currencyFormat, _nfi),
                        AccountBalance = item.AccountBalance.ToString(currencyFormat, _nfi),
                        PaymentNote = item.PaymentNote,
                        OrderNumber = string.IsNullOrEmpty(item.OrderNumber) ? "N/A" : item.OrderNumber
                    });
                }
            }

            return View(model);
        }
    }
}