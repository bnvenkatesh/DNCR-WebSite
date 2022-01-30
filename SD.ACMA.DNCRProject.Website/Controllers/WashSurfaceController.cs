using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using SD.ACMA.DNCR.Infrastructure;
using SD.ACMA.DNCR.Infrastructure.Configuration;
using SD.ACMA.DNCRProject.Website.Extensions;
using SD.ACMA.DNCRProject.Website.Helpers;
using SD.ACMA.DNCRProject.Website.Models;
using SD.ACMA.InterfaceTier;
using SD.ACMA.POCO.Industry;
using Umbraco.Core.Models;
using Umbraco.Web;
using Umbraco.Web.Mvc;

namespace SD.ACMA.DNCRProject.Website.Controllers
{
    public class WashSurfaceController : SurfaceController
    {
        private IIndustryDataInterchange _industryDataInterchange;
        private IErrorMessageHelper _errorMessageHelper;
        private IWebServiceResultFormatter _webServiceResultFormatter;
        private IUserContextHelper _userContextHelper;

        public WashSurfaceController(IIndustryDataInterchange industryDataInterchange, IErrorMessageHelper errorMessageHelper, IWebServiceResultFormatter webServiceResultFormatter,
            IUserContextHelper userContextHelper)
        {
            _industryDataInterchange = industryDataInterchange;
            _errorMessageHelper = errorMessageHelper;
            _webServiceResultFormatter = webServiceResultFormatter;
            _userContextHelper = userContextHelper;
        }

        #region Healthchecks

        public string QuickWashHealthCheck(string id)
        {
            var quickWashKey = ConfigurationManager.AppSettings["HealthCheck.Methods.QuickWashKey"] ?? "pleaseWashMe";

            if (id == quickWashKey)
            {
                var accountId = int.Parse(ConfigurationManager.AppSettings["HealthCheck.AccountId"] ?? "1");
                var accountUserId = int.Parse(ConfigurationManager.AppSettings["HealthCheck.AccountUserId"] ?? "1");
                var ucm = _userContextHelper.CreateUserContextObject(accountUserId, null);
                var numbers = new List<string> { ConfigurationManager.AppSettings["HealthCheck.NumberOnRegister"] ?? "0433319146" };

                var result = _industryDataInterchange.QuickWash(accountId, string.Empty, numbers, ucm);

                if (result.IsSuccessful || !result.HasSufficientWashingCredits)
                {
                    return "PASS";
                }

                StringBuilder sb = new StringBuilder();

                //if (!result.HasSufficientWashingCredits)
                //sb.Append("Your account currently has insufficient credit to process this request. Please purchase a new subscription and try again.");

                if (result.HasInValidNumbers)
                    sb.Append(" Has invalid numbers.");

                if (!result.HasValidSubscription)
                    sb.Append(" Invalid Subscription.");

                if (result.Errors != null)
                    sb.Append(_errorMessageHelper.GenerateErrorMessage(result.Errors));

                return sb.ToString();
            }
            return "Invalid key";
        }

        public string UploadListHealthCheck(string id)
        {
            var uploadListKey = ConfigurationManager.AppSettings["HealthCheck.Methods.UploadListKey"] ?? "pleaseUploadMe";

            if (id == uploadListKey)
            {
                var accountId = int.Parse(ConfigurationManager.AppSettings["HealthCheck.AccountId"] ?? "1");
                var accountUserId = int.Parse(ConfigurationManager.AppSettings["HealthCheck.AccountUserId"] ?? "1");
                var washingRequestId = int.Parse(ConfigurationManager.AppSettings["HealthCheck.WashingRequestId"] ?? "1");
                //var fileToWashPath = String.Format("{0}\\{1}", ConfigurationManager.AppSettings["PreWashUploadLocation"],ConfigurationManager.AppSettings["HealthCheck.Methods.UploadListFileName"] ?? "numbersHealthcheck.txt");
                var ucm = _userContextHelper.CreateUserContextObject(accountUserId, null);

                var washResult = _industryDataInterchange.WashByFileUpload(new WashByFileUploadModel
                {
                    AccountId = accountId,
                    WashingRequestId = washingRequestId,
                    WashResultFormatFileWithIndicators = true,
                    WashResultFormatRegisteredNumbers = false,
                    WashResultFormatUnregisteredNumbers = false,
                    WashResultFormatInvalidNumbers = false
                },
                    ucm);
                if (washResult.Errors != null)
                {
                    return _errorMessageHelper.GenerateErrorMessage(washResult.Errors);
                }
                if (washResult.IsSuccessful || !washResult.HasSufficientWashingCredits)
                {
                    return "PASS";
                }
                return "Error";
            }
            return "Invalid key";
        }

        #endregion

        #region Wash History

        [ChildActionOnly]
        public ActionResult History(int currentPage, string from, string to)
        {
            GetViewBagProperties();
            ViewBag.MobileOnly = false;

            DateTime fromDate;
            DateTime toDate;
            if (!DateTime.TryParseExact(from, "dd-MM-yyyy", CultureInfo.CreateSpecificCulture("en-AU"), DateTimeStyles.None, out fromDate) || !DateTime.TryParseExact(to, "dd-MM-yyyy", CultureInfo.CreateSpecificCulture("en-AU"), DateTimeStyles.None, out toDate))
            {
                //use default if parsing failed
                fromDate = DateTime.Now.AddDays(-30);
                toDate = DateTime.Now;
            }
            var channel = DNCR.Infrastructure.Enums.WashingChannelEnum.OnlineQuickCheck; //TODO: How do we set this on load

            var model = GetViewModel(currentPage, fromDate, toDate, SessionHelper.AccountUserId, SessionHelper.AccountId, channel, null, null, 20);

            return PartialView("History", model);
        }

        public ActionResult LoadHistory(int currentPage, string from, string to)
        {
            DateTime fromDate;
            DateTime toDate;
            var channel = DNCR.Infrastructure.Enums.WashingChannelEnum.OnlineQuickCheck; //TODO: How do we set this on load

            if (!DateTime.TryParseExact(from, "dd/MM/yyyy", CultureInfo.CreateSpecificCulture("en-AU"), DateTimeStyles.None, out fromDate) || !DateTime.TryParseExact(to, "dd/MM/yyyy", CultureInfo.CreateSpecificCulture("en-AU"), DateTimeStyles.None, out toDate))
            {
                //use default if parsing failed
                fromDate = DateTime.Now.AddDays(-30);
                toDate = DateTime.Now;
            }

            var model = GetViewModel(currentPage, fromDate, toDate, SessionHelper.AccountUserId, SessionHelper.AccountId, channel, null, null, 20);

            ViewBag.MobileOnly = true;

            return PartialView("_LoadHistory", model);
        }

        [NotChildAction]
        [HttpPost]
        public ActionResult History(WashHistoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                DateTime fromDate;
                DateTime toDate;
                if (DateTime.TryParseExact(model.FromDate, "dd/MM/yyyy", CultureInfo.CreateSpecificCulture("en-AU"), DateTimeStyles.None, out fromDate) && DateTime.TryParseExact(model.ToDate, "dd/MM/yyyy", CultureInfo.CreateSpecificCulture("en-AU"), DateTimeStyles.None, out toDate))
                {
                    return new RedirectResult(String.Format("{0}?from={1}&to={2}", Request.UrlReferrer.AbsolutePath, fromDate.ToString("dd-MM-yyyy"), toDate.ToString("dd-MM-yyyy")));
                }
            }
            
            TempData.Add("WashHistoryViewModel", model);
            return CurrentUmbracoPage();         
                
        }

        private WashHistoryViewModel GetViewModel(int currentPage, DateTime fromDate, DateTime toDate, int accountUserId, int accountId, SD.ACMA.DNCR.Infrastructure.Enums.WashingChannelEnum channel,
            bool? excludeWashWithOneNumber, int? washReferenceId, int itemsPerPage)
        {
            var model = new WashHistoryViewModel();
            model.FromDate = fromDate.ToString("dd/MM/yyyy");
            model.ToDate = toDate.ToString("dd/MM/yyyy");

            var washes = GetWashes(fromDate, toDate, accountUserId, accountId, channel, excludeWashWithOneNumber, washReferenceId);

            if (washes != null)
            {
                model.WashCount = washes.Count();
                var pager = model.Pager = PagerHelper.GetPager(itemsPerPage, washes.Count(), currentPage);
                model.Washes = washes.OrderByDescending(x => x.DateOfWash).Skip((pager.CurrentPage - 1) * pager.ItemsPerPage).Take(pager.ItemsPerPage);
                
            }
            else
            {
                model.WashCount = 0;
                model.Pager = PagerHelper.GetPager(itemsPerPage, 0, currentPage);
            }

            return model;            
        }

        private IList<WashHistoryModel> GetWashes(DateTime? fromDate, DateTime? toDate, int accountUserId, int accountId, SD.ACMA.DNCR.Infrastructure.Enums.WashingChannelEnum channel,
            bool? excludeWashWithOneNumber, int? washReferenceId)
        {
            var currentDate = DateTime.Now;
            var ucm = _userContextHelper.CreateUserContextObject(accountUserId, SessionHelper.AgentId);

            var washHistoryRequestModel = new WashHistoryRequestModel
            {
                AccountId = accountId,
                Channel = channel,
                StartDateTime = fromDate ?? currentDate.AddDays(-30),
                EndDateTime = toDate ?? currentDate,
                ExcludeWashWithOneNumber = excludeWashWithOneNumber != null ? excludeWashWithOneNumber.Value : false,
                WashReferenceId = washReferenceId != null ? washReferenceId : null
            };

            var listWashHistoryModel = new List<WashHistoryModel>();
            var result = _industryDataInterchange.WashHistory(washHistoryRequestModel, ucm);

            if (result.WashHistoryResponseObjects != null && result.WashHistoryResponseObjects.Count > 0)
            {
                foreach (var item in result.WashHistoryResponseObjects)
                {
                    var w = new WashHistoryModel
                    {
                        Combined = item.AllInOneNumbersCount,
                        CombinedFile = item.AllInOneFile,
                        DateOfWash = item.RequestDate,
                        Invalid = item.InvalidNumbersCount,
                        InvalidFile = item.InvalidNumbersFile,
                        ReferenceNumber = item.WashingRequestId.ToString(),
                        ClientReference = item.ClientReference,
                        Registered = item.RegisteredNumbersCount,
                        RegisteredFile = item.RegisteredNumbersFile,
                        Unregistered = item.UnregisteredNumbersCount,
                        UnregisteredFile = item.UnregisteredNumbersFile,
                        Username = item.AccountUserName,
                        Numbers = item.NumbersWashed,
                        WashSource = item.WashingChannel == Enums.WashingChannelEnum.OnlineQuickCheck ? Enum.GetName(typeof(Enums.WashingChannelEnum), item.WashingChannel).SplitCamelCase() : String.Format("{0} - {1}", Enum.GetName(typeof(Enums.WashingChannelEnum), item.WashingChannel).SplitCamelCase(), item.FileName),
                        CanDownload = item.CanDownload,
                    };

                    listWashHistoryModel.Add(w);
                }
            }
            else return null;

            return listWashHistoryModel;
        }

        public FileResult DownloadHistory(int year, int month, int day, string filename, bool isRTA)
        {
            string path;
            if (isRTA)
            {
                path = String.Format(@"{0}\RTA\{1}\{2}\{3}\{4}", ConfigurationManager.AppSettings["WashHistoryFileLocation"], year, month, day, filename);
            }
            else path = String.Format(@"{0}\{1}\{2}\{3}", ConfigurationManager.AppSettings["WashHistoryFileLocation"], year, month, filename);

            var bytes = System.IO.File.ReadAllBytes(path);
            return File(bytes, System.Net.Mime.MediaTypeNames.Application.Octet, filename);
        }

        #endregion

        #region Quick Wash

        [ChildActionOnly]
        public ActionResult QuickWash()
        {
            GetViewBagProperties();

            var model = new QuickWashViewModel();
            model.Numbers = new List<WashNumber>();
            for (int i = 0; i < 10; i++)
            {
                model.Numbers.Add(new WashNumber() { Registered = null });
            }

            return PartialView("QuickWash", model);
        }

        [NotChildAction]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult QuickWash(QuickWashViewModel model)
        {
            ModelState.Remove("MinimumNumbers");
            if (ModelState.IsValid)
            {
                var ucm = _userContextHelper.CreateUserContextObject(SessionHelper.AccountUserId, SessionHelper.AgentId);

                //Call WS
                var result = _industryDataInterchange.QuickWash(SessionHelper.AccountId, string.Empty, model.Numbers.Where(x => !string.IsNullOrEmpty(x.Number)).Select(y => y.Number).ToList(),
                    ucm);

                if (result.IsSuccessful)
                {
                    foreach (var number in model.Numbers)
                    {
                        if (!String.IsNullOrEmpty(number.Number))
                        {
                            var flag = result.WashNumbers.Where(x => x.Number == number.Number).FirstOrDefault().Flag;

                            if (flag == ConfigurationHelper.NUMBER_REGISTERED)
                            {
                                number.Registered = true;
                            }
                            else
                            {
                                number.Registered = false;
                            }
                        }
                    }
                }
                else
                {
                    StringBuilder sb = new StringBuilder();

                    if (!result.HasSufficientWashingCredits)
                        sb.Append("Your account currently has insufficient credit to process this request. Please purchase a new subscription and try again.");

                    if (result.HasInValidNumbers)
                        sb.Append(" Has invalid numbers.");

                    if (!result.HasValidSubscription)
                        sb.Append(" Invalid Subscription.");

                    if (result.Errors != null)
                        sb.Append(_errorMessageHelper.GenerateErrorMessage(result.Errors));

                    ViewBag.ErrorMessage = sb.ToString();
                }
            }

            TempData.Add("QuickWashViewModel", model);
            return CurrentUmbracoPage();
        }

        #endregion

        #region Upload List

        [ChildActionOnly]
        public ActionResult UploadList()
        {
            GetViewBagProperties();

            var accountUserId = SessionHelper.AccountUserId;
            var ucm = _userContextHelper.CreateUserContextObject(accountUserId, SessionHelper.AgentId);

            var accountUserResult = _industryDataInterchange.GetAccountUser(accountUserId, ucm);

            var model = new UploadWashingListViewModel() { IsSubmitted = false, FileToWashCopyPath = ""};
            if (accountUserResult != null && accountUserResult.Errors == null)
            {
                model.WashingResultOption = accountUserResult.WashFormat.WashResultFormatFileWithIndicators;
                model.FileOfRegisteredNumbers = accountUserResult.WashFormat.WashResultFormatRegisteredNumbers;
                model.FileOfUnregisteredNumbers = accountUserResult.WashFormat.WashResultFormatUnregisteredNumbers;
                model.FileOfInvalidNumbers = accountUserResult.WashFormat.WashResultFormatInvalidNumbers;
            }
            else
            {
                model.WashingResultOption = true;
            }
            return PartialView("UploadList", model);
        }

        [NotChildAction]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UploadList(UploadWashingListViewModel model)
        {
            ModelState.Remove("MinimumResultFile");
            try
            {
                if (!model.WashingResultOption && !model.FileOfRegisteredNumbers && !model.FileOfUnregisteredNumbers && !model.FileOfInvalidNumbers)
                {
                    ViewBag.ErrorMessage = "Result file is not selected. Please try again.";
                }
                else
                {
                var ucm = _userContextHelper.CreateUserContextObject(SessionHelper.AccountUserId, SessionHelper.AgentId);

                //Perform Wash happens here
                if (!String.IsNullOrEmpty(model.FileToWashCopyPath))
                {
                    ModelState.Remove("FileToWash");

                    var home = CurrentPage.AncestorOrSelf(1);
                    IPublishedContent washHistoryNode = Umbraco.Content(home.GetPropertyValue("washHistoryNode"));
                    model.WashHistoryURL = washHistoryNode.Url;

                    var washResult = _industryDataInterchange.WashByFileUpload(new WashByFileUploadModel
                    {
                        AccountId = SessionHelper.AccountId,
                        WashingRequestId = model.WashingRequestId.Value,
                        WashResultFormatFileWithIndicators = model.WashingResultOption,
                        WashResultFormatRegisteredNumbers = model.FileOfRegisteredNumbers,
                        WashResultFormatUnregisteredNumbers = model.FileOfUnregisteredNumbers,
                        WashResultFormatInvalidNumbers = model.FileOfInvalidNumbers
                    },
                        ucm);

                    if (washResult.Errors != null)
                    {
                        if (washResult.Errors.Message.IndexOf("request channel timed out") >= 0)
                        {
                            model.IsSubmitted = true;
                            ViewBag.WashSuccessful = false;
                            ViewBag.IsTimeout = true;
                        }
                            else
                            {
                                ViewBag.ErrorMessage = _errorMessageHelper.GenerateErrorMessage(washResult.Errors);
                            }
                        model.FileToWashCopyPath = string.Empty;
                    }
                    else
                    {
                        model.IsSuccessful = washResult.IsSuccessful;
                        model.AllInOneFile = washResult.AllInOneFile;
                        model.HasSufficientWashingCredits = washResult.HasSufficientWashingCredits;
                        model.HasValidSubscription = washResult.HasValidSubscription;
                        model.InvalidNumbersFile = washResult.InvalidNumbersFile;
                        model.OriginalFile = washResult.OriginalFile;
                        model.RegisteredNumbersFile = washResult.RegisteredNumbersFile;
                        model.Status = washResult.Status;
                        model.UnregisteredNumbersFile = washResult.UnregisteredNumbersFile;
                        model.WashCredits = washResult.WashCredits;
                        model.IsSubmitted = true;
                        ViewBag.WashSuccessful = washResult.IsSuccessful;

                        if(!washResult.IsSuccessful)
                        {
                            if (!washResult.HasSufficientWashingCredits)
                            {
                                if (SessionHelper.IsAdmin)
                                    ViewBag.WashUnSuccessMessage = "Your account currently has insufficient credit to process this request. Please purchase a new subscription and try again.";
                                else
                                    ViewBag.WashUnSuccessMessage = "Your account currently has insufficient credit to process this request. Please contact your account administrator.";
                            }
                            else if (!washResult.HasValidSubscription)
                            {
                                ViewBag.WashUnSuccessMessage = "You do not have a valid subscription";
                            }
                        }
                    }
                }

                //Pre-Wash happens here
                if (String.IsNullOrEmpty(model.FileToWashCopyPath) && ModelState.IsValid && !model.IsSubmitted)
                {
                    var fileName = Path.GetFileName(model.FileToWash.FileName);
                    var preWashUploadLocation = ConfigurationManager.AppSettings["PreWashUploadLocation"];

                    Guid g = Guid.NewGuid();
                    var myGuid = g.ToString("N");

                    Directory.CreateDirectory(preWashUploadLocation.StartsWith("~")
                        ? string.Format("{0}\\{1}", Server.MapPath(preWashUploadLocation), myGuid)
                        : string.Format("{0}\\{1}", preWashUploadLocation, myGuid));
                    var path =
                        Path.Combine(
                            preWashUploadLocation.StartsWith("~")
                                ? Server.MapPath(preWashUploadLocation)
                                : preWashUploadLocation, myGuid, fileName);
                    model.FileToWash.SaveAs(path);
                    model.FileToWashCopyPath = path;

                    var pathForWebService = string.Format("{0}\\{1}", myGuid, fileName);

                    var preWashResult = _industryDataInterchange.PreWash(string.Empty, pathForWebService, ucm);
                    
                    if (preWashResult.Errors != null)
                    {
                        if (preWashResult.Errors.Message.IndexOf("request channel timed out") >= 0)
                        {
                            ViewBag.ErrorMessage = "The file upload is taking too long. Please split the numbers into smaller files and retry.";
                        }
                        else ViewBag.ErrorMessage = _errorMessageHelper.GenerateErrorMessage(preWashResult.Errors);
                        model.FileToWashCopyPath = string.Empty;
                    }
                    else
                    {
                        ViewBag.NumberCount = preWashResult.RequiredWashingCredits;
                        ViewBag.AvailableCredit = preWashResult.AccountWashingCreditsBalance;
                        ViewBag.FileNameAlreadyExists = preWashResult.FileNameAlreadyExists;
                        ViewBag.WashingRequestId = preWashResult.WashingRequestId;

                        ViewBag.PreWashResult = GeneratePreWashAnalysisResponse(preWashResult);
                        ViewBag.PreWashSuccessful = preWashResult.IsSuccessful;
                        ViewBag.ShowWashQuote = preWashResult.ShowWashQuote;
                    }

                    //cleanup
                    try
                    {
                        Directory.Delete(preWashUploadLocation.StartsWith("~")
                            ? string.Format("{0}\\{1}", Server.MapPath(preWashUploadLocation), myGuid)
                            : string.Format("{0}\\{1}", preWashUploadLocation, myGuid), true);
                    }
                    catch (Exception ex)
                    {
                        // ignored
                    }
                }
                }

                TempData.Add("UploadWashingListViewModel", model);
                return CurrentUmbracoPage();
            }
            catch (Exception ex)
            {
                return CurrentUmbracoPage();
            }
        }

        public List<string> GeneratePreWashAnalysisResponse(PreWashResponse preWashResult)
        {
            var results = new List<string>();

            if (preWashResult.HasInValidNumbers)
                results.Add("File has invalid numbers");

            if (!preWashResult.HasSufficientWashingCredits)
            {
                results.Add(SessionHelper.IsAdmin
                    ? "Your account currently has insufficient credit to process this request. Please purchase a new subscription and try again."
                    : "Your account currently has insufficient credit to process this request. Please contact your account administrator.");
            }

            if (!preWashResult.HasValidSubscription)
                results.Add("You do not have a valid subscription");

            if (preWashResult.IsFileFormatNotValid)
                results.Add("File format is not valid");

            if (preWashResult.IsFileNameNotValid)
                results.Add("File name is not valid");

            if (preWashResult.IsFileSizeExceedLimit)
                results.Add("File size has exceeded limit");

            if (!preWashResult.IsSuccessful)
                results.Add("Prewash is unsuccessful");

            return results;
        }

        public FileResult DownloadError(int washingRequestId)
        {
            var ucm = _userContextHelper.CreateUserContextObject(SessionHelper.AccountUserId, SessionHelper.AgentId);

            var result = _industryDataInterchange.DownloadErrorLog(SessionHelper.AccountId, washingRequestId, ucm);

            return File(result.Content, System.Net.Mime.MediaTypeNames.Application.Octet, "wash errors.txt");
        }

        #endregion

        #region Dashboard

        public ActionResult Dashboard(int currentPage, string from, string to)
        {
            DateTime fromDate;
            DateTime toDate;
            if (!DateTime.TryParseExact(from, "dd/MM/yyyy", CultureInfo.CreateSpecificCulture("en-AU"), DateTimeStyles.None, out fromDate) || !DateTime.TryParseExact(to, "dd/MM/yyyy", CultureInfo.CreateSpecificCulture("en-AU"), DateTimeStyles.None, out toDate))
            {
                //use default if parsing failed
                fromDate = DateTime.Now.AddDays(-30);
                toDate = DateTime.Now;
            }
            var channel = DNCR.Infrastructure.Enums.WashingChannelEnum.OnlineQuickCheck; //TODO: How do we set this on load

            var model = GetViewModel(currentPage, fromDate, toDate, SessionHelper.AccountUserId, SessionHelper.AccountId, channel, null, null, 10);

            return PartialView("Dashboard", model);
        }

        public ActionResult LoadDashboardHistory(int currentPage, string from, string to)
        {
            DateTime fromDate;
            DateTime toDate;

            if (!DateTime.TryParseExact(from, "dd/MM/yyyy", CultureInfo.CreateSpecificCulture("en-AU"), DateTimeStyles.None, out fromDate) || !DateTime.TryParseExact(to, "dd/MM/yyyy", CultureInfo.CreateSpecificCulture("en-AU"), DateTimeStyles.None, out toDate))
            {
                //use default if parsing failed
                fromDate = DateTime.Now.AddDays(-30);
                toDate = DateTime.Now;
            }

            var channel = DNCR.Infrastructure.Enums.WashingChannelEnum.OnlineQuickCheck; //TODO: How do we set this on load

            var model = GetViewModel(currentPage, fromDate, toDate, SessionHelper.AccountUserId, SessionHelper.AccountId, channel, null, null, 10);

            return PartialView("_LoadDashboardHistory", model);
        }

        #endregion

        private void GetViewBagProperties()
        {
            ViewBag.PageTitle = CurrentPage.GetPropertyValue("pageTitle");
            ViewBag.PageSummary = CurrentPage.GetPropertyValue("pageSummary");
            ViewBag.Faqs = CurrentPage.GetPropertyValue("faqs");
            ViewBag.Downloads = CurrentPage.GetPropertyValue("downloads");
            ViewBag.RelatedLinks = CurrentPage.GetPropertyValue("relatedLinks").ToString();
        }
    }
}