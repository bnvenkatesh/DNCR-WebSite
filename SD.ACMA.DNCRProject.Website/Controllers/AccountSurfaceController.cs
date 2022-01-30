using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using SD.ACMA.BusinessLogic.Avanade;
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
using SD.ACMA.POCO.Base;

namespace SD.ACMA.DNCRProject.Website.Controllers
{
    public class AccountSurfaceController : SurfaceController
    {
        private IIndustryDataInterchange _industryDataInterchange;
        private IErrorMessageHelper _errorMessageHelper;
        private IUserContextHelper _userContextHelper;

        private readonly string LastPurchasedDateParseExactFormat = ConfigurationManager.AppSettings["SubscriptionHistory.LastPurchasedDate.ParseExactFormat"];
        private readonly string SubscriptionExpiryDateParseExactFormat = ConfigurationManager.AppSettings["SubscriptionHistory.SubscriptionExpiryDate.ParseExactFormat"];

        public AccountSurfaceController(IIndustryDataInterchange industryDataInterchange, IErrorMessageHelper errorMessageHelper, IUserContextHelper userContextHelper)
        {
            _industryDataInterchange = industryDataInterchange;
            _errorMessageHelper = errorMessageHelper;
            _userContextHelper = userContextHelper;
        }

        #region Create

        [ChildActionOnly]
        public ActionResult Create()
        {
            GetViewBagProperties();
            ViewBag.SiteKey = ConfigurationManager.AppSettings["RecaptchaSiteKey"];
            ViewBag.Recaptcha = ConfigurationManager.AppSettings["RecaptchaEnabled"] == "true";

            var home = CurrentPage.AncestorOrSelf(1);
            IPublishedContent industryLodgeEnquiryNode = Umbraco.Content(home.GetPropertyValue("industryLodgeEnquiryNode"));
            ViewBag.LodgeEnquiryUrl = industryLodgeEnquiryNode.Url;
            var model = new CreateAccountViewModel() { Country = "AU", IsSubmitted = false, HasABN = false };
            model.CountryList = CountryDropDownHelper.BuildCountryDropDownList(_industryDataInterchange.GetCountries().Countries);
            return PartialView("Create", model);
        }

        [NotChildAction]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CreateAccountViewModel model)
        {
            ModelState.Remove("MinimumIndustries");
            if (model.Country != "AU")
            {
                ModelState.Remove("State");
            }

            if (model.HasABN == false)
            {
                ModelState.Remove("ABN");
            }

            if (ModelState.IsValid && RecaptchaHelper.ValidateRecaptcha(ConfigurationManager.AppSettings["RecaptchaSecretKey"]))
            {
                var result = _industryDataInterchange.CreateAccount(new CreateAccountModel
                {
                    Account_Model = new AccountModel
                    {
                        ABN = model.ABN,
                        AddressLine1 = model.AddressLine1,
                        AddressLine2 = !string.IsNullOrEmpty(model.AddressLine2) ? model.AddressLine2 : string.Empty,
                        City = model.City,
                        Country = model.Country,
                        Email = model.Email,
                        FirstName = model.FirstName,
                        HasABN = model.HasABN,
                        HasACN = false,
                        Industry = model.Industry,
                        IsDailySummary = false, //25/05/2015 - Lemuel: hardcoded to false, value is not in ViewModel
                        LastName = model.LastName,
                        OrganisationName = model.OrganisationName,
                        PhoneNumber = model.OrganisationPhone.FixPhoneNumber(),
                        Postcode = model.Postcode,
                        PrincipalActivity = model.Activity,
                        State = model.State,
                    },
                    Account_User_Model = new AccountUserModel
                    {
                        EmailAddress = model.Email,
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        PhoneNumber = model.PhoneNumber.FixPhoneNumber()
                    },
                    WashFormat = new WashingFormat
                    {
                        WashResultFormatFileWithIndicators = true,
                        WashResultFormatInvalidNumbers = false,
                        WashResultFormatRegisteredNumbers = false,
                        WashResultFormatUnregisteredNumbers = false
                    }
                });

                if (result.Errors != null)
                {
                    if (result.Errors.FaultReasons != null && result.Errors.FaultReasons.Any(x => x.Message.Contains("The username is already in-use")))
                    {
                        //ViewBag.ErrorMessage = "There is already an account associated with this email address, please <a class='openSignIn'>sign in</a> using your username/password or contact your administrator.";
                        ViewBag.ErrorMessage = "There is already an account associated with the details you have provided. Please login using your username/password. For further assistance, please call 1300 785 749.";
                    }
                    else ViewBag.ErrorMessage = _errorMessageHelper.GenerateErrorMessage(result.Errors);
                }
                if (result.IsDuplicateAccount || result.IsAccountPendingApproval)
                {
                    model.AccessSeekerAccountId = result.AccountId.ToString();
                    ViewBag.IsDuplicateAccount = true;
                }
                if(result.IsSuccessful)
                {
                    model.AccessSeekerAccountId = result.AccountId.ToString();
                    model.IsSubmitted = true;
                }
            }
            model.CountryList = CountryDropDownHelper.BuildCountryDropDownList(_industryDataInterchange.GetCountries().Countries);
            TempData.Add("AccountViewModel", model);
            return CurrentUmbracoPage();
        }

        #endregion

        #region Edit

        [ChildActionOnly]
        public ActionResult Edit()
        {
            GetViewBagProperties();
            var home = CurrentPage.AncestorOrSelf(1);
            IPublishedContent industryLodgeEnquiryNode = Umbraco.Content(home.GetPropertyValue("industryLodgeEnquiryNode"));
            ViewBag.IndustryLodgeEnquiryNodeUrl = industryLodgeEnquiryNode.Url;

            var ucm = _userContextHelper.CreateUserContextObject(SessionHelper.AccountUserId, SessionHelper.AgentId);

            var model = new EditAccountViewModel { Country = "AU", IsSubmitted = false };

            model.CountryList = CountryDropDownHelper.BuildCountryDropDownList(_industryDataInterchange.GetCountries().Countries);

            var result = _industryDataInterchange.GetAccount(SessionHelper.AccountId, ucm);

            if (result.Errors != null)
            {
                ViewBag.ErrorMessage = _errorMessageHelper.GenerateErrorMessage(result.Errors);
            }
            else
            {
                if (result.Account_Model != null && result.AdminModel != null)
                {
                    model.Industry = result.Account_Model.Industry;
                    model.Activity = result.Account_Model.PrincipalActivity;
                    model.FirstName = result.AdminModel.FirstName;
                    model.LastName = result.AdminModel.LastName;
                    model.PhoneNumber = result.Account_Model.PhoneNumber;
                    model.Email = result.Account_Model.Email;
                    model.ConfirmEmail = result.Account_Model.Email;                 
                    model.OrganisationName = result.Account_Model.OrganisationName;
                    model.OrganisationPhone = result.Account_Model.PhoneNumber;
                    model.HasABN = result.Account_Model.HasABN;
                    model.ABN = result.Account_Model.ABN;
                    model.AddressLine1 = result.Account_Model.AddressLine1;
                    model.AddressLine2 = !string.IsNullOrEmpty(result.Account_Model.AddressLine2) ? result.Account_Model.AddressLine2 : string.Empty;
                    model.City = result.Account_Model.City;
                    model.Postcode = result.Account_Model.Postcode;
                    model.State = result.Account_Model.State;
                    model.WashingResultOption = result.WashFormat.WashResultFormatFileWithIndicators;
                    model.FileOfInvalidNumbers = result.WashFormat.WashResultFormatInvalidNumbers;
                    model.FileOfRegisteredNumbers = result.WashFormat.WashResultFormatRegisteredNumbers;
                    model.FileOfUnregisteredNumbers = result.WashFormat.WashResultFormatUnregisteredNumbers;
                    model.AccessSeekerAccountId = result.Account_Model.AccountUserID.ToString();
                    model.Country = new CultureInfo("en-US", false).TextInfo.ToTitleCase(result.Account_Model.Country.ToLower());
                    model.DailySummaryEmail = result.Account_Model.IsDailySummary;
                    model.PhoneNumber = result.AdminModel.PhoneNumber;
                    model.Email = result.AdminModel.EmailAddress;
                    model.ConfirmEmail = result.AdminModel.EmailAddress;
                }
            }

            return PartialView("Edit", model);
        }

        [NotChildAction]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EditAccountViewModel model)
        {
            //model.Industries.RemoveRange(model.Industries.Count - 1, 1);
            ModelState.Remove("MinimumResultFile");
            ModelState.Remove("MinimumOrganisationType");
            ModelState.Remove("MinimumIndustries");
            if (model.Country != "AU")
            {
                ModelState.Remove("State");
            }

            if (model.HasABN == false)
            {
                ModelState.Remove("ABN");
            }

            if (!model.ChangePassword)
            {
                ModelState.Remove("CurrentPassword");
                ModelState.Remove("NewPassword");
                ModelState.Remove("ConfirmPassword");
            }

            if (ModelState.IsValid)
            {
                var accountId = SessionHelper.AccountId;
                var accountUserId = SessionHelper.AccountUserId;
                var ucm = _userContextHelper.CreateUserContextObject(accountUserId, SessionHelper.AgentId);

                model.AccessSeekerAccountId = accountUserId.ToString();
                var currentDetails = _industryDataInterchange.GetAccount(accountId, ucm);
                if (currentDetails.Errors == null)
                {
                    if (currentDetails.AdminModel.EmailAddress != model.Email)
                    {
                        SessionHelper.RequireLogout = true;
                    }
                    if (!string.IsNullOrEmpty(model.NewPassword) && !string.IsNullOrEmpty(model.CurrentPassword))
                    {
                        var changePasswordResult = _industryDataInterchange.ChangePassword(accountUserId,
                            model.NewPassword, model.CurrentPassword);

                        if (changePasswordResult.Errors != null)
                        {
                            StringBuilder sb = new StringBuilder();

                            if (changePasswordResult.Errors != null)
                                sb.Append(_errorMessageHelper.GenerateErrorMessage(changePasswordResult.Errors));

                            ViewBag.ErrorMessage = sb.ToString();

                        }
                        else if (!changePasswordResult.IsSuccessful)
                        {
                            ViewBag.ErrorMessage = changePasswordResult.Message;
                        }
                        else
                        {
                            var editResponse = EditAccount(model, accountId, accountUserId, ucm);

                            if (editResponse.UpdateAccountResult.Errors == null &&
                                editResponse.UpdateAccountResult.IsSuccessful
                                && editResponse.UpdateUserResult.Errors == null &&
                                editResponse.UpdateUserResult.IsSuccessful)
                            {
                                model.IsSubmitted = DetermineIsSubmittedValue(editResponse);
                                ViewBag.ErrorMessage = editResponse.ErrorMessage;
                            }
                        }
                    }
                    else
                    {
                        var editResponse = EditAccount(model, accountId, accountUserId, ucm);
                        model.IsSubmitted = DetermineIsSubmittedValue(editResponse);
                        ViewBag.ErrorMessage = editResponse.ErrorMessage;
                    }
                }
            }
            model.CountryList = CountryDropDownHelper.BuildCountryDropDownList(_industryDataInterchange.GetCountries().Countries);
            TempData.Add("AccountViewModel", model);
            return CurrentUmbracoPage();
        }

        private bool DetermineIsSubmittedValue(EditAccountResponse editResponse)
        {
            if (editResponse.UpdateAccountResult.Errors == null && editResponse.UpdateAccountResult.IsSuccessful
                && editResponse.UpdateUserResult.Errors == null && editResponse.UpdateUserResult.IsSuccessful)
            {
                return true;
            }

            return false;
        }

        private EditAccountResponse EditAccount(EditAccountViewModel model, int accountId, int accountUserId, UserContextModel ucm)
        {
            var accountDetails = CreateAccountModel(model, accountId, accountUserId);
            var wf = CreateWashingFormatModel(model);

            var accountUser = new UpdateAccountUserModel
            {
                WashFormat = wf,
                AccountUserModel = accountDetails
            };

            var uam = new UpdateAccountModel
            {
                AccounDetails = accountDetails
            };

            var updateAccountResult = _industryDataInterchange.UpdateAccount(ucm, uam);
            var updateUserResult = _industryDataInterchange.UpdateAccountUser(ucm, accountUser);

            var response = new EditAccountResponse
            {
                UpdateAccountResult = updateAccountResult,
                UpdateUserResult = updateUserResult,
            };

            if (updateAccountResult.Errors != null || updateUserResult.Errors != null)
            {
                StringBuilder sb = new StringBuilder();

                if (updateAccountResult.Errors != null)
                    sb.Append(_errorMessageHelper.GenerateErrorMessage(updateAccountResult.Errors));

                if (updateUserResult.Errors != null)
                    sb.Append(_errorMessageHelper.GenerateErrorMessage(updateUserResult.Errors));

                response.ErrorMessage = sb.ToString();

            }
            else if (!updateAccountResult.IsSuccessful || !updateUserResult.IsSuccessful)
            {
                StringBuilder sb = new StringBuilder();

                if (!updateAccountResult.IsSuccessful)
                    sb.Append(updateAccountResult.Message);

                if (!updateUserResult.IsSuccessful)
                    sb.Append(updateUserResult.Message);

                response.ErrorMessage = string.Format("{0} {1}", response.ErrorMessage, sb.ToString());
            }

            return response;
        }

        private AccountModel CreateAccountModel(EditAccountViewModel model, int accountId, int accountUserId)
        {
            return new AccountModel
            {
                HasABN = model.HasABN,
                ABN = model.ABN,
                AccountID = accountId,
                AccountUserID = accountUserId,
                AddressLine1 = model.AddressLine1,
                AddressLine2 = !string.IsNullOrEmpty(model.AddressLine2) ? model.AddressLine2 : string.Empty,
                City = model.City.Trim(),
                Country = model.Country,
                Email = model.Email,
                FirstName = model.FirstName,
                HasACN = false,
                Industry = model.Industry,
                IsDailySummary = model.DailySummaryEmail,
                LastName = model.LastName,
                OrganisationName = model.OrganisationName,
                //BusinessType = ActivityToWebServiceBusinessTypeMapper(model.Activity),
                //TODO: Get from model BusinessSector once WS has its enums matching to wireframe. BusinessSector & Activity too
                PhoneNumber = model.PhoneNumber.FixPhoneNumber(),
                OrganisationPhoneNumber = model.OrganisationPhone.FixPhoneNumber(),
                Postcode = model.Postcode,
                PrincipalActivity = model.Activity,
                State = model.State,
                CanSeeWashQuote = true,
                //BusinessSector = IndustryToWebServiceBusinessSectorMapper(model.Industries.FirstOrDefault())
            };
        }

        private WashingFormat CreateWashingFormatModel(EditAccountViewModel model)
        {
            return new WashingFormat
            {
                WashResultFormatFileWithIndicators = model.WashingResultOption,
                WashResultFormatInvalidNumbers = model.FileOfInvalidNumbers,
                WashResultFormatRegisteredNumbers = model.FileOfRegisteredNumbers,
                WashResultFormatUnregisteredNumbers = model.FileOfUnregisteredNumbers
            };
        }

        public string WebServiceBusinessTypeToActivityMapper(Enums.BusinessTypeEnum selectedEnumValue)
        {
            switch ((int)selectedEnumValue)
            {
                case 1:
                    return "telephoning";
                case 2:
                    return "sending faxes";
                case 3:
                    return "telephoning and sending faxes";
                default:
                    return "telephoning and sending faxes";
            }
        }

        public SD.ACMA.DNCR.Infrastructure.Enums.BusinessTypeEnum ActivityToWebServiceBusinessTypeMapper(string selectedValue)
        {
            if (selectedValue == "telephoning")
            {
                return Enums.BusinessTypeEnum.Telemarketing;
            }
            else if (selectedValue == "sending faxes")
            {
                return Enums.BusinessTypeEnum.FaxMarketing;
            }
            else
            {
                return Enums.BusinessTypeEnum.Both;
            }
        }

        public string WebServiceBusinessSectorToIndustryMapper(SD.ACMA.DNCR.Infrastructure.Enums.BusinessSectorEnum selectedEnumValue)
        {
            switch ((int)selectedEnumValue)
            {
                case 6:
                    return "Community and Voluntary Groups";

                default:
                    return "Community and Voluntary Groups";
            }
        }

        public SD.ACMA.DNCR.Infrastructure.Enums.BusinessSectorEnum IndustryToWebServiceBusinessSectorMapper(string selectedValue)
        {
            if (selectedValue == "Community and Voluntary Groups")
            {
                return Enums.BusinessSectorEnum.CommunityorVolunteerGroup;
            }
            else
            {
                return Enums.BusinessSectorEnum.CommunityorVolunteerGroup;
            }
        }

        #endregion

        #region Dashboard

        public ActionResult Dashboard()
        {
            var model = new DashboardAccountViewModel();
            var accountId = SessionHelper.AccountId;
            var accountUserId = SessionHelper.AccountUserId;
            var ucm = _userContextHelper.CreateUserContextObject(accountUserId, SessionHelper.AgentId);

            var getAccountResult = _industryDataInterchange.GetAccount(accountId, ucm);
            var getSubscriptionSummaryDetailsResult = _industryDataInterchange.GetSubscriptionSummaryDetailsByAccountID(accountId);
            var headerResult = _industryDataInterchange.GetAccountUserHeaderInformationByAccountUserID(accountUserId);


            if (getAccountResult.Errors != null || getSubscriptionSummaryDetailsResult.Errors != null || headerResult.Errors != null)
            {
                //TODO: Error getting results - what do we do here?
            }
            else
            {
                model.AccountId = accountId;
                model.AccessSeekerId = accountUserId;

                if (SessionHelper.IsAdmin)
                {
                    var getAllAccountUsersResult = _industryDataInterchange.GetAllAccountUsers(accountId, ucm);

                    model.NumOfUsers = (getAllAccountUsersResult.Errors == null &&
                                        getAllAccountUsersResult.ResponseModel.IsSuccessful)
                        ? getAllAccountUsersResult.Accounts.Where(a => a.Account_Model.Status != Enums.AccountUserStatusEnum.Deleted).Count()
                        : 0;
                }
                else
                {
                    model.NumOfUsers = 0;
                }

                model.Status = (getAccountResult.Account_Model.Status != null)
                    ? getAccountResult.Account_Model.Status.ToString()
                    : "Active";

                if (String.IsNullOrEmpty(getSubscriptionSummaryDetailsResult.SubscriptionSummary.LastPurchaseDate))
                {
                    model.DatePurchased = null;
                }
                else
                {
                    model.DatePurchased = DateTime.ParseExact(
                        getSubscriptionSummaryDetailsResult.SubscriptionSummary.LastPurchaseDate,
                        LastPurchasedDateParseExactFormat,
                        null
                    );
                }

                model.AvailableCredit = getSubscriptionSummaryDetailsResult.SubscriptionSummary.AvailableWashCredits;

                if (String.IsNullOrEmpty(getSubscriptionSummaryDetailsResult.SubscriptionSummary.SubscriptionExpiryDate))
                {
                    model.DateExpires = null;
                }
                else
                {
                    model.DateExpires = DateTime.ParseExact(
                        getSubscriptionSummaryDetailsResult.SubscriptionSummary.SubscriptionExpiryDate,
                        SubscriptionExpiryDateParseExactFormat, 
                        null
                    );
                }

                model.ReservedCredit = getSubscriptionSummaryDetailsResult.SubscriptionSummary.ReservedWashCredits;
                model.ReservedAccountBalance = getSubscriptionSummaryDetailsResult.SubscriptionSummary.ReservedCreditNotes;
                model.AccountBalance = getSubscriptionSummaryDetailsResult.SubscriptionSummary.CreditNotes;

                model.CanSeeWashQuote = headerResult.CanSeeWashQuote;
            }

            return PartialView("Dashboard", model);
        }

        #endregion

        #region Activate

        [ChildActionOnly]
        public ActionResult Activate(string token, string email)
        {
            var model = new ActivateViewModel() { IsExpired = false, IsInvalid = false, IsEmailSent = false };
            model.Token = token;
            model.Email = email;

            return PartialView("Activate", model);
        }

        [NotChildAction]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Activate(ActivateViewModel model)
        {
            if (model.IsExpired)
            {
                var result = _industryDataInterchange.SendAccountActivationEmail(model.Email);
                if (!result.IsSuccessful)
                {
                    ViewBag.ErrorMessage = result.Message;
                }
                else
                {
                    model.IsEmailSent = true;
                }
            }
            else
            if (ModelState.IsValid)
            {
                {
                    var result = _industryDataInterchange.ActivateAccount(model.Token, model.NewPassword, model.Email);

                    if (result.Errors != null)
                    {
                        ViewBag.ErrorMessage = result.Errors.Message;
                    }
                    else
                    {
                        if (result.IsExpired)
                        {
                            model.IsExpired = result.IsExpired;
                            ViewBag.ErrorMessage = "Activation Token has expired";
                        }
                        else if (result.CannotActivate)
                        {
                            model.CannotActivate = true;
                            ViewBag.ErrorMessage = "Cannot activate Account";
                        }
                        else
                        {
                            if (!result.GenericResponse.IsSuccessful)
                            {
                                model.IsInvalid = true;
                                ViewBag.ErrorMessage = result.GenericResponse.Message;
                            }
                            else
                            {
                                //Successfully activated
                                //Log user in automatically
                                var loginResult = _industryDataInterchange.Login(model.Email, model.NewPassword);

                                if (loginResult.Errors != null)
                                {
                                    model.IsInvalid = true;
                                    ViewBag.ErrorMessage = loginResult.Errors.Message;
                                }
                                else
                                {
                                    if (loginResult.IsLocked)
                                    {
                                        model.IsInvalid = true;
                                        ViewBag.ErrorMessage = "Account is locked";
                                    }
                                    else if (!loginResult.IsSuccessful)
                                    {
                                        model.IsInvalid = true;
                                        ViewBag.ErrorMessage = loginResult.LoginMessage;
                                    }
                                    else
                                    {
                                        //Successfully logged in
                                        SessionHelper.SetLoginSessions(loginResult.AccountId, loginResult.AccountUserId, loginResult.IsAdmin, model.Email);
                                        var home = CurrentPage.AncestorOrSelf(1);
                                        IPublishedContent accountDashboardNode = Umbraco.Content(home.GetPropertyValue("accountDashboardNode"));
                                        return new RedirectResult(accountDashboardNode.Url);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            TempData.Add("ActivateViewModel", model);
            return CurrentUmbracoPage();
        }

        #endregion

        private void GetViewBagProperties()
        {
            ViewBag.PageTitle = CurrentPage.GetPropertyValue("pageTitle");
            ViewBag.PageSummary = CurrentPage.GetPropertyValue("pageSummary");
            ViewBag.Faqs = CurrentPage.GetPropertyValue("faqs");
            ViewBag.Downloads = CurrentPage.GetPropertyValue("downloads");
            ViewBag.RelatedLinks = CurrentPage.GetPropertyValue("relatedLinks").ToString();
            var home = CurrentPage.AncestorOrSelf(1);
            ViewBag.DataCollectionNode = Umbraco.Content(home.GetPropertyValue("dataCollectionNoticeNode")).Url;
            ViewBag.AustralianPrivacyUrl = home.GetPropertyValue("australianPrivacyPrinciplesUrl");
        }
    }
}