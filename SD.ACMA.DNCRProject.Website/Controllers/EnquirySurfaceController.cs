using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using SD.ACMA.BusinessLogic;
using SD.ACMA.BusinessLogic.Avanade;
using SD.ACMA.BusinessLogic.DNCRAccessSeekerServices;
using SD.ACMA.DNCR.Infrastructure;
using SD.ACMA.DNCR.Infrastructure.Configuration;
using SD.ACMA.DNCRProject.Website.Extensions;
using SD.ACMA.DNCRProject.Website.Helpers;
using SD.ACMA.DNCRProject.Website.Models;
using SD.ACMA.InterfaceTier;
using SD.ACMA.POCO.Base;
using SD.ACMA.POCO.Consumer;
using SD.ACMA.POCO.Industry;
using Umbraco.Web;
using Umbraco.Web.Mvc;

namespace SD.ACMA.DNCRProject.Website.Controllers
{
    public class EnquirySurfaceController : SurfaceController
    {
        private IConsumerDataInterchange _consumerDataInterchange;
        private IIndustryDataInterchange _industryDataInterchange;
        private IErrorMessageHelper _errorMessageHelper;
        private IUserContextHelper _userContextHelper;

        private const string BalanceRefundType = "Balance Refund";
        private const string SubscriptionRefundType = "Subscription Refund";
        private const string WashCreditsRolloverType = "Wash Credits Rollover";
        private const string WashReversalType = "Wash Reversal";
        private const string AccountBalanceAdjustmentType = "Account Balance Adjustment";
        private const string WashNumberAdjustmentType = "Wash Number Adjustment";
        private const string OtherType = "Other";
        
        public EnquirySurfaceController(IConsumerDataInterchange consumerDataInterchange, IIndustryDataInterchange industryDataInterchange, IErrorMessageHelper errorMessageHelper, IUserContextHelper userContextHelper)
        {
            _consumerDataInterchange = consumerDataInterchange;
            _industryDataInterchange = industryDataInterchange;
            _errorMessageHelper = errorMessageHelper;
            _userContextHelper = userContextHelper;
        }

        #region Consumer

        [ChildActionOnly]
        public ActionResult Consumer()
        {
            GetViewBagProperties();

            var model = new ConsumerEnquiryViewModel { Country = "AU", IsSubmitted = false, Channel = "Webform" };

            if (SessionHelper.AgentId != null)
            {
                if (SessionHelper.IsAcma)
                    model.Channel = "ACMA";
                else
                    model.Channel = "Agent";
            }

            model.CountryList = CountryDropDownHelper.BuildCountryDropDownList(_consumerDataInterchange.GetCountries().Countries);

            return PartialView("Consumer", model);
        }

        [NotChildAction]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Consumer(ConsumerEnquiryViewModel model)
        {
            if (model.IsAnonymous)
            {
                ModelState.Remove("Title");
                ModelState.Remove("FirstName");
                ModelState.Remove("LastName");
                ModelState.Remove("AddressLine1");
                ModelState.Remove("City");
                ModelState.Remove("Postcode");
                ModelState.Remove("State");
                ModelState.Remove("Country");
            }
            else if (model.Country != "AU")
            {
                ModelState.Remove("State");
            }

            if (ModelState.IsValid && RecaptchaHelper.ValidateRecaptcha(ConfigurationManager.AppSettings["RecaptchaSecretKey"]))
            {
                var lodgeEnquiryModel = new LodgeEnquiryModel
                    {
                        Email = !string.IsNullOrEmpty(model.Email) ? model.Email : string.Empty,
                        PhoneNumber = model.ContactNumber.FixPhoneNumber(),
                        IsConsumer = true,
                        Type = "Consumer",
                        ChannelID = ChannelToWebServiceChannelTypeMapper(model.Channel),
                        Country = !string.IsNullOrEmpty(model.Country) ? model.Country : string.Empty,
                        Details = !string.IsNullOrEmpty(model.Details) ? model.Details : string.Empty,
                        Subject = !string.IsNullOrEmpty(model.Subject) ? model.Subject : string.Empty,
                        IsAnonymous = model.IsAnonymous,
                    };

                if (!model.IsAnonymous)
                {
                    lodgeEnquiryModel.Addressline1 = !string.IsNullOrEmpty(model.AddressLine1) ? model.AddressLine1 : string.Empty;
                    lodgeEnquiryModel.Addressline2 = !string.IsNullOrEmpty(model.AddressLine2) ? model.AddressLine2 : string.Empty;
                    lodgeEnquiryModel.CompanyName = !string.IsNullOrEmpty(model.OrganisationName) ? model.OrganisationName : string.Empty;
                    lodgeEnquiryModel.FirstName = !string.IsNullOrEmpty(model.FirstName) ? model.FirstName : string.Empty;
                    lodgeEnquiryModel.LastName = !string.IsNullOrEmpty(model.LastName) ? model.LastName : string.Empty;
                    lodgeEnquiryModel.State = !string.IsNullOrEmpty(model.State) ? model.State : string.Empty;
                    lodgeEnquiryModel.Suburb = !string.IsNullOrEmpty(model.City) ? model.City : string.Empty;
                    lodgeEnquiryModel.Title = !string.IsNullOrEmpty(model.Title) ? model.Title : null;
                    lodgeEnquiryModel.Postcode = !string.IsNullOrEmpty(model.Postcode) ? model.Postcode : string.Empty;
                }
                else
                {
                    lodgeEnquiryModel.Addressline1 = string.Empty;
                    lodgeEnquiryModel.Addressline2 = string.Empty;
                    lodgeEnquiryModel.CompanyName = string.Empty;
                    lodgeEnquiryModel.FirstName = string.Empty;
                    lodgeEnquiryModel.LastName = string.Empty;
                    lodgeEnquiryModel.State = string.Empty;
                    lodgeEnquiryModel.Suburb = string.Empty;
                    lodgeEnquiryModel.Title = null;
                    lodgeEnquiryModel.Postcode = string.Empty;
                }

                LodgeEnquiryResponse result = new LodgeEnquiryResponse();

                result = _consumerDataInterchange.LodgeEnquiry(lodgeEnquiryModel, SessionHelper.AgentId);

                if (!result.IsSuccessful)
                {
                    if (result.Errors != null)
                        ViewBag.ErrorMessage = _errorMessageHelper.GenerateErrorMessage(result.Errors);
                    else
                        ViewBag.ErrorMessage = "Lodge Enquiry was not successful";
                }
                else
                {
                    model.RefCode = result.EnquiryID != null ? result.EnquiryID.ToString() : null;
                    model.IsSubmitted = true;
                }
            }
            model.CountryList = CountryDropDownHelper.BuildCountryDropDownList(_consumerDataInterchange.GetCountries().Countries);
            TempData.Add("ConsumerEnquiryViewModel", model);
            return CurrentUmbracoPage();
        }

        #endregion

        #region Industry

        [ChildActionOnly]
        public ActionResult Industry()
        {
            GetViewBagProperties();

            var model = new IndustryEnquiryViewModel() { Country = "AU", IsSubmitted = false, Channel = "Webform" };

            if (SessionHelper.AgentId != null)
            {
                if (SessionHelper.IsAcma)
                    model.Channel = "ACMA";
                else
                    model.Channel = "Agent";
            }

            model.CountryList = CountryDropDownHelper.BuildCountryDropDownList(_consumerDataInterchange.GetCountries().Countries);
            var accountUserId = SessionHelper.AccountUserId;
            if (accountUserId > 0)
            {
                var ucm = _userContextHelper.CreateUserContextObject(accountUserId, SessionHelper.AgentId);

                var userResult = _industryDataInterchange.GetAccountUser(accountUserId, ucm);
                if (userResult != null)
                {
                    model.FirstName = userResult.FirstName;
                    model.LastName = userResult.LastName;
                    model.OrganisationName = SessionHelper.CompanyName;
                    model.AccessSeekerId = accountUserId.ToString();
                    model.Email = userResult.EmailAddress;
                    model.ConfirmEmail = userResult.EmailAddress;
                    model.ContactNumber = userResult.PhoneNumber;
                }

                var accountId = SessionHelper.AccountId;
                if (accountId > 0)
                {
                    var accountResult = _industryDataInterchange.GetAccount(accountId, ucm);
                    if (accountResult.Errors == null && accountResult.Account_Model != null)
                    {
                        model.AddressLine1 = accountResult.Account_Model.AddressLine1;
                        model.AddressLine2 = !string.IsNullOrEmpty(accountResult.Account_Model.AddressLine2) ? accountResult.Account_Model.AddressLine2 : string.Empty;
                        model.City = accountResult.Account_Model.City;
                        model.Postcode = accountResult.Account_Model.Postcode;
                        model.State = accountResult.Account_Model.State;
                        model.Country = new CultureInfo("en-US", false).TextInfo.ToTitleCase(accountResult.Account_Model.Country.ToLower());
                    }
                }
            }

            model.EnquiryTypes = BuildSubjectDropDownList();

            if (!SessionHelper.IsAdmin)
            {
                model.Subject = OtherType;
            }

            return PartialView("Industry", model);
        }

        [NotChildAction]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Industry(IndustryEnquiryViewModel model)
        {
            if (!model.IsSubmitted)
            {
                if (model.IsAnonymous)
                {
                    ModelState.Remove("Title");
                    ModelState.Remove("FirstName");
                    ModelState.Remove("LastName");
                    ModelState.Remove("AddressLine1");
                    ModelState.Remove("City");
                    ModelState.Remove("Postcode");
                    ModelState.Remove("State");
                    ModelState.Remove("Country");
                }
                else if (model.Country != "AU")
                {
                    ModelState.Remove("State");
                }

                //Removing model validation for certain refund types
                if (model.Subject != SubscriptionRefundType && model.Subject != WashCreditsRolloverType && model.Subject != WashReversalType && model.Subject != AccountBalanceAdjustmentType && model.Subject != WashNumberAdjustmentType)
                {
                    ModelState.Remove("Description");
                }

                if (model.Subject != BalanceRefundType && model.Subject != SubscriptionRefundType)
                {
                    ModelState.Remove("AccountName");
                    ModelState.Remove("AccountNumber");
                    ModelState.Remove("AccountBSB");
                }

                if (model.Subject != BalanceRefundType)
                {
                    ModelState.Remove("RefundReservedAccountBalance");
                }

                if (model.Subject != SubscriptionRefundType)
                {
                    ModelState.Remove("RefundReservedWashCredits");
                    ModelState.Remove("RefundSubscriptionId");
                    ModelState.Remove("RefundOrderId");
                }

                if (model.Subject != WashCreditsRolloverType)
                {
                    ModelState.Remove("WashCreditsRolloverAmount");
                    ModelState.Remove("WashCreditsRolloverTransactionId");
                }

                if (model.Subject != WashReversalType)
                {
                    ModelState.Remove("RefundWashTransactionId");
                }

                if (model.Subject != AccountBalanceAdjustmentType)
                {
                    ModelState.Remove("RefundAcmaIncrementAccountBalance");
                }

                if (model.Subject != WashNumberAdjustmentType)
                {
                    ModelState.Remove("RefundAcmaIncrementWashCredits");
                }

                if (model.Subject != OtherType)
                {
                    ModelState.Remove("Details");
                    model.Details = model.Description;
                }

                bool isValid;
                var accountId = SessionHelper.AccountId;
                if (accountId > 0 && SessionHelper.AccountUserId > 0)
                {
                    isValid = ModelState.IsValid;
                }
                else
                {
                    isValid = ModelState.IsValid && RecaptchaHelper.ValidateRecaptcha(ConfigurationManager.AppSettings["RecaptchaSecretKey"]);
                }

                //if (model.Subject == WashNumberAdjustmentType && accountId > 0)
                //{
                //    var limit = _industryDataInterchange.GetManualAdjustmentWashCreditsLimit(accountId);
                //    var washNumberAdjustment = !String.IsNullOrEmpty(model.RefundAcmaIncrementWashCredits) ? Int32.Parse(model.RefundAcmaIncrementWashCredits.Replace("+", String.Empty)) : (int?)null;
                //    if (limit.WashCreditsLimit > 0 && washNumberAdjustment > limit.WashCreditsLimit)
                //    {
                //        isValid = false;
                //        ViewBag.ErrorMessage = String.Format("The wash number entered exceeds the refund/adjustment limit for this account. The current limit is {0}.", limit.WashCreditsLimit);
                //    }
                //}

                if (isValid)
                {
                    var lem = new LodgeEnquiryModel
                    {
                        Email = !string.IsNullOrEmpty(model.Email) ? model.Email : string.Empty,
                        PhoneNumber = model.ContactNumber.FixPhoneNumber(),
                        IsConsumer = false,
                        IsRefund = model.Subject != OtherType,
                        ChannelID = ChannelToWebServiceChannelTypeMapper(model.Channel),
                        Country = !string.IsNullOrEmpty(model.Country) ? model.Country : string.Empty,
                        Details = !string.IsNullOrEmpty(model.Details) ? model.Details : string.Empty,
                        Subject = !string.IsNullOrEmpty(model.AccessSeekerId) ? string.Format("{0}##{1}", model.AccessSeekerId, model.Subject) : model.Subject,
                        IsAnonymous = model.IsAnonymous,
                        RefundRequestModel = model.Subject == OtherType
                            ? null
                            : new OnlineRefundRequestModel()
                            {
                                AccountID = accountId,
                                BankDetailsAccountName = model.AccountName,
                                BankDetailsAccountNumber = !string.IsNullOrEmpty(model.AccountNumber) ? model.AccountNumber.Replace("-", "") : string.Empty,
                                BankDetailsBSB = !string.IsNullOrEmpty(model.AccountBSB) ? model.AccountBSB.Replace("-", "") : string.Empty,
                                BankDetailsOther = model.OtherAccountDetails,
                                Description = !string.IsNullOrEmpty(model.AdditionalInformation) ? model.AdditionalInformation : model.Description,
                                EnquiryRefundType = SubjectToWebServiceRefundTypeMapper(model.Subject),
                                RefundAcmaIncrementAccountBalance = !String.IsNullOrEmpty(model.RefundAcmaIncrementAccountBalance) ? Decimal.Parse(model.RefundAcmaIncrementAccountBalance.Replace("+", String.Empty)) : (decimal?) null,
                                RefundAcmaIncrementWashCredits = !String.IsNullOrEmpty(model.RefundAcmaIncrementWashCredits) ? Int32.Parse(model.RefundAcmaIncrementWashCredits.Replace("+", String.Empty)) : (int?) null,
                                RefundOrderID = model.RefundOrderId != 0 ? model.RefundOrderId : (int?) null,
                                RefundReservedAccountBalance = model.RefundReservedAccountBalance != 0 ? model.RefundReservedAccountBalance : (decimal?) null,
                                RefundReservedWashCredits = model.RefundReservedWashCredits != 0 ? model.RefundReservedWashCredits : (int?) null,
                                RefundSubscriptionID = model.RefundSubscriptionId != 0 ? model.RefundSubscriptionId : (int?) null,
                                RefundWashTransactionID = model.RefundWashTransactionId != 0 ? model.RefundWashTransactionId : (int?) null,
                                WashCreditsRolloverAmount = model.WashCreditsRolloverAmount != 0 ? model.WashCreditsRolloverAmount : (int?) null,
                                WashCreditsRolloverTransactionID = model.WashCreditsRolloverTransactionId != 0 ? model.WashCreditsRolloverTransactionId : (int?) null,
                            }
                    };

                    if (!model.IsAnonymous)
                    {
                        lem.Addressline1 = !string.IsNullOrEmpty(model.AddressLine1) ? model.AddressLine1 : string.Empty;
                        lem.Addressline2 = !string.IsNullOrEmpty(model.AddressLine2) ? model.AddressLine2 : string.Empty;
                        lem.CompanyName = !string.IsNullOrEmpty(model.OrganisationName) ? model.OrganisationName : string.Empty;
                        lem.FirstName = !string.IsNullOrEmpty(model.FirstName) ? model.FirstName : string.Empty;
                        lem.LastName = !string.IsNullOrEmpty(model.LastName) ? model.LastName : string.Empty;
                        lem.State = !string.IsNullOrEmpty(model.State) ? model.State : string.Empty;
                        lem.Suburb = !string.IsNullOrEmpty(model.City) ? model.City : string.Empty;
                        lem.Title = null;
                        lem.Postcode = !string.IsNullOrEmpty(model.Postcode) ? model.Postcode : string.Empty;
                    }
                    else
                    {
                        lem.Addressline1 = string.Empty;
                        lem.Addressline2 = string.Empty;
                        lem.CompanyName = string.Empty;
                        lem.FirstName = string.Empty;
                        lem.LastName = string.Empty;
                        lem.State = string.Empty;
                        lem.Suburb = string.Empty;
                        lem.Title = null;
                        lem.Postcode = string.Empty;
                    }

                    if (model.Subject == BalanceRefundType)
                    {
                        var financialHistoryResult = _industryDataInterchange.GetFinancialHistory(accountId, null);

                        if (financialHistoryResult.Orders != null && financialHistoryResult.Orders.Count > 0)
                        {
                            var paymentMethods =
                            (from p in financialHistoryResult.Orders.Where(x => x.Payments != null).Select(x => x.Payments)
                                from item in p
                                select item.PaymentMethod).ToList().Distinct();

                            if (paymentMethods != null)
                            {
                                lem.RefundRequestModel.EnquiryRefundPaymentType = GeneratePaymentTypes(paymentMethods);
                            }
                        }
                    }
                    else if (model.Subject == SubscriptionRefundType)
                    {
                        var paymentMethods = _industryDataInterchange.GetFinancialHistory(accountId, model.RefundOrderId)
                            .Orders.FirstOrDefault().Payments.Select(x => x.PaymentMethod).Distinct();

                        if (paymentMethods != null)
                        {
                            lem.RefundRequestModel.EnquiryRefundPaymentType = GeneratePaymentTypes(paymentMethods);
                        }
                    }

                    var result = _industryDataInterchange.LodgeEnquiry(lem, SessionHelper.AgentId);

                    if (!result.IsSuccessful)
                    {
                        model.EnquiryTypes = BuildSubjectDropDownList();
                        ViewBag.ErrorMessage = result.Errors != null ? _errorMessageHelper.GenerateErrorMessage(result.Errors) : "Lodge Enquiry was not successful";
                    }
                    else
                    {
                        model.RefCode = result.EnquiryID != null ? result.EnquiryID : null;
                        model.IsSubmitted = true;
                    }
                }
            }
            else
            {
                model.IsSubmitted = false;
                if (model.IsAnonymous)
                {
                    ModelState.Remove("FirstName");
                    ModelState.Remove("LastName");
                    ModelState.Remove("AddressLine1");
                    ModelState.Remove("City");
                    ModelState.Remove("Postcode");
                    ModelState.Remove("State");
                    ModelState.Remove("Country");
                }
                ModelState.Remove("Subject");
                ModelState.Remove("Details");
                ModelState.Remove("Channel");
            }
            model.EnquiryTypes = BuildSubjectDropDownList();
            model.CountryList = CountryDropDownHelper.BuildCountryDropDownList(_consumerDataInterchange.GetCountries().Countries);
            TempData.Add("IndustryEnquiryViewModel", model);
            return CurrentUmbracoPage();
        }

        private string GeneratePaymentTypes(IEnumerable<Enums.PaymentMethodEnum> paymentMethods)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var paymentMethod in paymentMethods)
            {
                sb.Append(string.Format("{0},", paymentMethod.ToString()));
            }

            sb.Remove(sb.Length - 1, 1);
            return sb.ToString();
        }

        #endregion

        #region Refunds

        public ActionResult BalanceRefund()
        {
            var accountId = SessionHelper.AccountId;
            var accountRefunds = _industryDataInterchange.GetAccountRefunds(accountId);

            var openBalanceRefund = new List<AccountRefundsModel>();
            if (accountRefunds.Refunds != null)
            {
                openBalanceRefund = accountRefunds.Refunds.Where(x => x.EnquiryStatus != Enums.EnquryStatusEnum.Closed && x.RefundType == Enums.RefundTypeEnum.BalanceRefund).ToList();
            }
            var model = new BalanceRefundViewModel();
            if (!openBalanceRefund.Any())
            {
                var subscriptionSummary = _industryDataInterchange.GetSubscriptionSummaryDetailsByAccountID(accountId);

                model.RefundReservedAccountBalance = subscriptionSummary.SubscriptionSummary.CreditNotes;
            }
            else model.RefundReservedAccountBalance = 0;

            return View("_BalanceRefund", model);
        }

        public ActionResult SubscriptionRefund(string refundsInformationUrl)
        {
            var accountId = SessionHelper.AccountId;

            var refundableSubscriptions = new List<RefundSubscriptionModel>();

            var subscriptionSummary = _industryDataInterchange.GetSubscriptionSummaryDetailsByAccountID(accountId);
            if (subscriptionSummary.Errors != null)
            {
                //TODO: Error getting results - what do we do here?
            }
            else if (subscriptionSummary.SubscriptionSummary.AvailableWashCredits >= 20000)
            {
                var financialHistory = _industryDataInterchange.GetFinancialHistory(accountId, null);
                if (financialHistory.Orders != null && financialHistory.Orders.Count > 0)
                {
                    var orders =
                        financialHistory.Orders.Where(o => o.CreatedTimeStamp > DateTime.Now.AddYears(-1))
                            .OrderByDescending(o => o.CreatedTimeStamp);

                    var orderToBeRefunded =
                        orders.FirstOrDefault(
                            o =>
                                o.OrderLines.Count(
                                    s =>
                                        s.OrderLineStatus == Enums.OrderLineStatusEnum.Paid && s.SubscriptionName != "A") >
                                0);
                    if (orderToBeRefunded != null)
                    {
                        var accountRefunds = _industryDataInterchange.GetAccountRefunds(accountId);
                        var openSubscriptionRefund = new List<AccountRefundsModel>();
                        if (accountRefunds.Refunds != null)
                        {
                            openSubscriptionRefund =
                                accountRefunds.Refunds.Where(
                                    x =>
                                        x.EnquiryStatus != Enums.EnquryStatusEnum.Closed &&
                                        x.RefundType == Enums.RefundTypeEnum.SubscriptionRefund).ToList();
                        }

                        foreach (var subscription in orderToBeRefunded.OrderLines)
                        {
                            if (subscription.OrderLineStatus == Enums.OrderLineStatusEnum.Paid &&
                                openSubscriptionRefund.All(x => x.SubscriptionID != subscription.OrderLineID))
                            {
                                refundableSubscriptions.Add(new RefundSubscriptionModel()
                                                            {
                                                                OrderNo = orderToBeRefunded.OrderNumber,
                                                                AmountIncGst = subscription.Price,
                                                                OrderDate = orderToBeRefunded.CreatedTimeStamp,
                                                                SubscriptionLimit = subscription.WashCredits,
                                                                SubscriptionType = subscription.SubscriptionName,
                                                                SubscriptionId = subscription.OrderLineID,
                                                                OrderId = orderToBeRefunded.OrderID,
                                                            });
                            }
                        }
                    }
                }
            }

            var model = new SubscriptionRefundViewModel
            {
                RefundReservedWashCredits = "0",
                Subscriptions = refundableSubscriptions,
                AvailableWash = subscriptionSummary.SubscriptionSummary.AvailableWashCredits,
                RefundOrderId = refundableSubscriptions.Count > 0 ? refundableSubscriptions.FirstOrDefault().OrderId : 0,
            };

            ViewBag.RefundsInformationUrl = refundsInformationUrl;

            return View("_SubscriptionRefund", model);
        }

        public ActionResult WashCreditsRollover()
        {
            var accountId = SessionHelper.AccountId;

            var model = new WashCreditRolloverViewModel(){RequirePurchase = false};

            var financialHistory = _industryDataInterchange.GetFinancialHistory(accountId, null);
            if (financialHistory.TransactionHistory != null && financialHistory.TransactionHistory.Count > 0)
            {
                var washExpiredTransaction = financialHistory.TransactionHistory.Where(transaction => transaction.CreatedTimeStamp >= DateTime.Now.AddMonths(-1).Date)
                            .FirstOrDefault(transaction => transaction.AccountTransactionType == Enums.AccountTransactionTypeEnum.WashingCreditsExpired);

                if (washExpiredTransaction != null)
                {
                    var accountRefunds = _industryDataInterchange.GetAccountRefunds(accountId);
                    var existingExtensionRefund = new List<AccountRefundsModel>();
                    if (accountRefunds.Refunds != null)
                    {
                        existingExtensionRefund = accountRefunds.Refunds.Where(x => x.RefundType == Enums.RefundTypeEnum.WashCreditsRollOver && (x.EnquiryStatus != Enums.EnquryStatusEnum.Closed || (x.WashCreditsRolloverTrasactionID == washExpiredTransaction.AccountTransactionID && x.RefundDecision == Enums.RefundDecisionEnum.Approved))).ToList();
                    }
                    if (!existingExtensionRefund.Any())
                    {
                        var getAccountSummaryDetailResult = _industryDataInterchange.GetAccountSummaryDetailByAccountID(accountId);
                        var expiredSubscription = getAccountSummaryDetailResult.AccountSummaries.Where(x => DateTime.ParseExact(x.CreatedTimeStampt, ConfigurationHelper.Instance.AccountSummary_SubscriptionDate_ParseExactFormat, null) < washExpiredTransaction.CreatedTimeStamp).ToList();

                        if (expiredSubscription.Count > 1 ||
                            (expiredSubscription.Count == 1 && !String.IsNullOrEmpty(expiredSubscription.First().Amount)))
                        {
                            var newSubscriptions = getAccountSummaryDetailResult.AccountSummaries.Where(x => DateTime.ParseExact(x.CreatedTimeStampt, ConfigurationHelper.Instance.AccountSummary_SubscriptionDate_ParseExactFormat, null) >= washExpiredTransaction.CreatedTimeStamp).ToList();

                            if (!newSubscriptions.Any() || (newSubscriptions.Count == 1 && String.IsNullOrEmpty(newSubscriptions.First().Amount)))
                            {
                                model.RequirePurchase = true;
                            }
                            else
                            {
                                model.ExpiryDate = washExpiredTransaction.CreatedTimeStamp;
                                model.WashCreditsRolloverAmount = washExpiredTransaction.WashCredits ?? 0;
                                model.WashCreditsRolloverTransactionID = washExpiredTransaction.AccountTransactionID;
                            }
                        }
                        else model.WashCreditsRolloverAmount = 0;
                    }
                    else model.WashCreditsRolloverAmount = 0;
                }
                else model.WashCreditsRolloverAmount = 0;
            }
            else model.WashCreditsRolloverAmount = 0;

            return View("_WashCreditsRollover", model);
        }

        public ActionResult WashReversal()
        {
            var accountId = SessionHelper.AccountId;
            var ucm = _userContextHelper.CreateUserContextObject(SessionHelper.AccountUserId, SessionHelper.AgentId);

            var whrm = new WashHistoryRequestModel
            {
                AccountId = accountId,
                ExcludeWashWithOneNumber = false,
            };

            var model = new WashedNumbersReversalViewModel()
            {
                Washes = new List<RefundWashModel>()
            };

            var washHistory = _industryDataInterchange.WashHistory(whrm, ucm);

            if (washHistory.WashHistoryResponseObjects != null && washHistory.WashHistoryResponseObjects.Count > 0)
            {
                var accountRefunds = _industryDataInterchange.GetAccountRefunds(accountId);
                var openWashReversal = new List<AccountRefundsModel>();
                if (accountRefunds.Refunds != null)
                {
                    openWashReversal = accountRefunds.Refunds.Where(x => x.EnquiryStatus != Enums.EnquryStatusEnum.Closed && x.RefundType == Enums.RefundTypeEnum.ReverseWash).ToList();
                }

                var eligibleWashes = washHistory.WashHistoryResponseObjects.Where(w => w.RequestDate > DateTime.Now.AddMonths(-1) && 
                        w.RegisteredNumbersCount == 0 && w.UnregisteredNumbersCount == 0 && 
                        w.NumbersWashed >= 500 && 
                        !w.HasRefunded &&
                        openWashReversal.All(x => x.WashTransactionID != w.WashingRequestId));

                foreach (var wash in eligibleWashes)
                {
                    model.Washes.Add(new RefundWashModel()
                                     {
                                         DateWashed = wash.RequestDate,
                                         User = wash.AccountUserName,
                                         WashedNumbers = wash.NumbersWashed,
                                         WashReference = wash.WashingRequestId.ToString(),
                                         WashSource =
                                             Enum.GetName(typeof(DNCR.Infrastructure.Enums.WashingChannelEnum),
                                                 wash.WashingChannel).SplitCamelCase(),
                                     });
                }
            }

            return View("_WashReversal", model);
        }

        public ActionResult AccountBalanceAdjustment()
        {
            var result = _industryDataInterchange.GetSubscriptionSummaryDetailsByAccountID(SessionHelper.AccountId);

            var model = new AccountBalanceAdjustmentViewModel
            {
                AccountBalance = result.SubscriptionSummary.CreditNotes
            };

            return View("_AccountBalanceAdjustment", model);
        }

        public ActionResult WashNumberAdjustment()
        {
            var result = _industryDataInterchange.GetSubscriptionSummaryDetailsByAccountID(SessionHelper.AccountId);

            var model = new WashNumberAdjustmentViewModel
            {
                WashCredits = result.SubscriptionSummary.AvailableWashCredits
            };

            return View("_WashNumberAdjustment", model);
        }

        public ActionResult Other()
        {
            var model = new OtherEnquiriesViewModel();

            return View("_Other", model);
        }

        #endregion

        private void GetViewBagProperties()
        {
            ViewBag.SiteKey = ConfigurationManager.AppSettings["RecaptchaSiteKey"];
            ViewBag.Recaptcha = ConfigurationManager.AppSettings["RecaptchaEnabled"] == "true";
            ViewBag.PageTitle = CurrentPage.GetPropertyValue("pageTitle");
            ViewBag.PageSummary = CurrentPage.GetPropertyValue("pageSummary");
            ViewBag.Faqs = CurrentPage.GetPropertyValue("faqs");
            ViewBag.Downloads = CurrentPage.GetPropertyValue("downloads");
            ViewBag.RelatedLinks = CurrentPage.GetPropertyValue("relatedLinks").ToString();
            var home = CurrentPage.AncestorOrSelf(1);
            ViewBag.DataCollectionNode = Umbraco.Content(home.GetPropertyValue("dataCollectionNoticeNode")).Url;
            ViewBag.AustralianPrivacyUrl = home.GetPropertyValue("australianPrivacyPrinciplesUrl");
            ViewBag.RefundsInformationUrl = Umbraco.Content(home.GetPropertyValue("refundsInformationNode")).Url;
        }

        public List<SelectListItem> BuildSubjectDropDownList()
        {
            var enquiryTypes = new List<SelectListItem>();

            if (SessionHelper.IsAdmin)
            {
                enquiryTypes.Add(new SelectListItem { Text = "Request refund of account balance", Value = BalanceRefundType });
                enquiryTypes.Add(new SelectListItem { Text = "Request subscription refund", Value = SubscriptionRefundType });
                enquiryTypes.Add(new SelectListItem { Text = "Request extension to expiry of wash numbers", Value = WashCreditsRolloverType });
                enquiryTypes.Add(new SelectListItem { Text = "Request reversal of washed numbers", Value = WashReversalType });
            };

            if (SessionHelper.IsAcma)
            {
                enquiryTypes.Add(new SelectListItem { Text = "Manual Adjustment Of Account Balance", Value = AccountBalanceAdjustmentType });
                enquiryTypes.Add(new SelectListItem { Text = "Manual Adjustment Of Wash Number", Value = WashNumberAdjustmentType });
            }

            enquiryTypes.Add(new SelectListItem { Text = "Other", Value = OtherType });

            return enquiryTypes;
        }

        public ActionResult GetRefundModel(string typeOfRefundChosen, string refundsInformationUrl)
        {
            if (typeOfRefundChosen == BalanceRefundType)
                return BalanceRefund();

            if (typeOfRefundChosen == SubscriptionRefundType)
                return SubscriptionRefund(refundsInformationUrl);

            if (typeOfRefundChosen == WashCreditsRolloverType)
                return WashCreditsRollover();

            if (typeOfRefundChosen == WashReversalType)
                return WashReversal();

            if (typeOfRefundChosen == AccountBalanceAdjustmentType)
                return AccountBalanceAdjustment();

            if (typeOfRefundChosen == WashNumberAdjustmentType)
                return WashNumberAdjustment();

            if (typeOfRefundChosen == OtherType)
                return Other();

            return null;
        }

        public Enums.ChannelsEnum ChannelToWebServiceChannelTypeMapper(string selectedValue)
        {
            switch (selectedValue)
            {
                case "OasisIVR":
                    return Enums.ChannelsEnum.OasisIVR;
                case "Symposium":
                    return Enums.ChannelsEnum.Symposium;
                case "Agent":
                    return Enums.ChannelsEnum.Agent;
                case "Webform":
                    return Enums.ChannelsEnum.Webform;
                case "Other":
                    return Enums.ChannelsEnum.Other;
                case "Phone":
                    return Enums.ChannelsEnum.Phone;
                case "Email":
                    return Enums.ChannelsEnum.Email;
                case "Letter":
                    return Enums.ChannelsEnum.Letter;
                case "Fax":
                    return Enums.ChannelsEnum.Fax;
                case "ACMA":
                    return Enums.ChannelsEnum.ACMA;
                case "Split":
                    return Enums.ChannelsEnum.Split;
                default:
                    return Enums.ChannelsEnum.Webform;
            }
        }

        public Enums.RefundTypeEnum SubjectToWebServiceRefundTypeMapper(string selectedValue)
        {
            switch (selectedValue)
            {
                case BalanceRefundType:
                    return Enums.RefundTypeEnum.BalanceRefund;
                case SubscriptionRefundType:
                    return Enums.RefundTypeEnum.SubscriptionRefund;
                case WashCreditsRolloverType:
                    return Enums.RefundTypeEnum.WashCreditsRollOver;
                case WashReversalType:
                    return Enums.RefundTypeEnum.ReverseWash;
                case AccountBalanceAdjustmentType:
                case WashNumberAdjustmentType:
                    return Enums.RefundTypeEnum.ManualAdjustment;
                default:
                    return Enums.RefundTypeEnum.ManualAdjustment;
            }
        }
    }
}