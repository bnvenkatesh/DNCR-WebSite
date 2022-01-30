using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using SD.ACMA.BusinessLogic.Avanade;
using SD.ACMA.DNCR.Infrastructure;
using SD.ACMA.DNCR.Infrastructure.Configuration;
using SD.ACMA.DNCRProject.Website.Extensions;
using SD.ACMA.DNCRProject.Website.Helpers;
using SD.ACMA.DNCRProject.Website.Models;
using SD.ACMA.InterfaceTier;
using SD.ACMA.POCO.Base;
using SD.ACMA.POCO.Industry;
using Umbraco.Core.Models;
using Umbraco.Web;
using Umbraco.Web.Models;
using Umbraco.Web.Mvc;

namespace SD.ACMA.DNCRProject.Website.Controllers
{
    public class UserSurfaceController : SurfaceController
    {
        private IIndustryDataInterchange _industryDataInterchange;
        private IErrorMessageHelper _errorMessageHelper;
        private IUserContextHelper _userContextHelper;

        public UserSurfaceController(IIndustryDataInterchange industryDataInterchange, IErrorMessageHelper errorMessageHelper, IUserContextHelper userContextHelper)
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

            var model = new UserViewModel() { IsSubmitted = false, WashingResultOption = true };
            return PartialView("Create", model);
        }

        [NotChildAction]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(UserViewModel model)
        {
            ModelState.Remove("MinimumResultFile");
            if (ModelState.IsValid)
            {
                var ucm = _userContextHelper.CreateUserContextObject(SessionHelper.AccountUserId, SessionHelper.AgentId);

                var washingFormat = new WashingFormat()
                {
                    WashResultFormatFileWithIndicators = model.WashingResultOption
                };

                // Options for seperate files are only valid if the user selected seperate files
                if (!model.WashingResultOption)
                {
                    washingFormat.WashResultFormatInvalidNumbers = model.FileOfInvalidNumbers;
                    washingFormat.WashResultFormatRegisteredNumbers = model.FileOfRegisteredNumbers;
                    washingFormat.WashResultFormatUnregisteredNumbers = model.FileOfUnregisteredNumbers;
                }
                
                var washOnlyUser = new CreateWashOnlyUserModel
                {
                    WashFormat = washingFormat,
                    AccountUserModel = new AccountModel
                    {
                        CanSeeWashQuote = model.CanSeeWashQuote,
                        CompanyDepartment =
                            !string.IsNullOrEmpty(model.CompanyDepartment) ? model.CompanyDepartment : string.Empty,
                        Email = model.Email,
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        PhoneNumber = model.PhoneNumber1.FixPhoneNumber(),
                        PhoneNumber2 = model.PhoneNumber2.FixPhoneNumber(),
                        AccountID = SessionHelper.AccountId
                    }
                };

                var result = _industryDataInterchange.CreateWashOnlyUser(washOnlyUser, ucm);

                if (result.Errors != null)
                {
                    ViewBag.ErrorMessage = _errorMessageHelper.GenerateErrorMessage(result.Errors);
                }
                else if (!result.IsSuccessful)
                {
                    ViewBag.ErrorMessage = result.Message;
                }
                else if (result.Errors == null && result.IsSuccessful)
                {
                    model.IsSubmitted = true;
                }
            }
            TempData.Add("UserViewModel", model);
            return CurrentUmbracoPage();
        }

        #endregion

        #region Edit

        [ChildActionOnly]
        public ActionResult Edit(int? accountUserId)
        {
            if (accountUserId == null)
            {
                var home = CurrentPage.AncestorOrSelf(1);
                IPublishedContent accountDashboardNode = Umbraco.Content(home.GetPropertyValue("accountDashboardNode"));
                ControllerContext.HttpContext.Response.Redirect(accountDashboardNode.Url);
                return null;
            }
            GetViewBagProperties();

            var ucm = _userContextHelper.CreateUserContextObject(SessionHelper.AccountUserId, SessionHelper.AgentId);

            var model = new EditUserViewModel() {IsSubmitted = false};
            var result = _industryDataInterchange.GetAccountUser(accountUserId.Value, ucm);

            if (result != null)
            {
                model.FirstName = result.FirstName;
                model.LastName = result.LastName;
                model.CompanyDepartment = result.Department;
                model.PhoneNumber1 = result.PhoneNumber;
                model.PhoneNumber2 = result.MobileNumber;
                model.Email = result.EmailAddress;
                model.ConfirmEmail = result.EmailAddress;
                model.CanSeeWashQuote = result.CanSeeWashQuote;
                model.WashingResultOption = result.WashFormat.WashResultFormatFileWithIndicators;
                model.FileOfInvalidNumbers = result.WashFormat.WashResultFormatInvalidNumbers;
                model.FileOfRegisteredNumbers = result.WashFormat.WashResultFormatRegisteredNumbers;
                model.FileOfUnregisteredNumbers = result.WashFormat.WashResultFormatUnregisteredNumbers;
                model.AccountUserId = accountUserId.Value;
                model.OriginalEmail = result.EmailAddress;
            }
            return PartialView("Edit", model);
        }

        [NotChildAction]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EditUserViewModel model)
        {
            ModelState.Remove("MinimumResultFile");
            if (ModelState.IsValid)
            {
                var ucm = _userContextHelper.CreateUserContextObject(SessionHelper.AccountUserId, SessionHelper.AgentId);

                var account = new UpdateAccountUserModel
                {
                    WashFormat = new WashingFormat
                    {
                        WashResultFormatFileWithIndicators = model.WashingResultOption,
                        WashResultFormatInvalidNumbers = model.FileOfInvalidNumbers,
                        WashResultFormatRegisteredNumbers = model.FileOfRegisteredNumbers,
                        WashResultFormatUnregisteredNumbers = model.FileOfUnregisteredNumbers
                    },
                    AccountUserModel = new AccountModel
                    {
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        CompanyDepartment = model.CompanyDepartment,
                        CanSeeWashQuote = model.CanSeeWashQuote,
                        PhoneNumber = model.PhoneNumber1.FixPhoneNumber(),
                        PhoneNumber2 = model.PhoneNumber2.FixPhoneNumber(),
                        Email = model.Email,
                        AccountUserID = model.AccountUserId
                    }
                };

                var resetResult = new GenericResponseModel { IsSuccessful = true };
                var decativateResult = new GenericResponseModel { IsSuccessful = true };
                StringBuilder sb = new StringBuilder();

                if (model.ResetPassword && (model.OriginalEmail.ToLower() == model.Email.ToLower()))
                {
                    resetResult = _industryDataInterchange.ForgotPassword(account.AccountUserModel.Email);

                    if (resetResult.Errors != null)
                    {
                        ViewBag.ErrorMessage = _errorMessageHelper.GenerateErrorMessage(resetResult.Errors);
                    }
                    else if (!resetResult.IsSuccessful)
                    {
                        ViewBag.ErrorMessage = resetResult.Message;
                    }
                    //else
                    //{
                    //    decativateResult = _industryDataInterchange.DeActivateAccountUser(ucm, model.AccountUserId);

                    //    if (decativateResult.Errors != null || !decativateResult.IsSuccessful)
                    //    {
                    //        resetResult.IsSuccessful = false;
                    //        resetResult.Message = "Deactivate user process failed";
                    //    }
                    //}
                }

                var result = _industryDataInterchange.UpdateAccountUser(ucm, account);

                if (result.Errors != null || resetResult.Errors != null)
                {
                    if (result.Errors != null)
                        sb.Append(_errorMessageHelper.GenerateErrorMessage(result.Errors));

                    if (resetResult.Errors != null)
                        sb.Append(_errorMessageHelper.GenerateErrorMessage(resetResult.Errors));

                    ViewBag.ErrorMessage = sb.ToString();
                }
                else if (!result.IsSuccessful || !resetResult.IsSuccessful)
                {
                    if (!result.IsSuccessful)
                        sb.Append(result.Message);

                    if (!resetResult.IsSuccessful)
                        sb.Append(resetResult.Message);

                    ViewBag.ErrorMessage = sb.ToString();
                }
                else if (result.Errors == null && result.IsSuccessful && resetResult.Errors == null && resetResult.IsSuccessful)
                {
                    model.IsSubmitted = true;
                }
            }
            TempData.Add("UserViewModel", model);
            return CurrentUmbracoPage();
        }

        #endregion

        #region ActivateWashOnlyUser

        [ChildActionOnly]
        public ActionResult ActivateWashOnlyUser(string token, string email)
        {
            var model = new ActivateViewModel() { IsExpired = false, IsInvalid = false };
            model.Token = token;
            model.Email = email;

            return PartialView("ActivateWashOnlyUser", model);
        }

        [NotChildAction]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ActivateWashOnlyUser(ActivateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = _industryDataInterchange.ActivateWashOnlyUser(model.Email, model.NewPassword, model.Token);

                if (result.Errors != null)
                {
                    ViewBag.ErrorMessage = result.Errors.Message;
                }
                else
                {
                    if (result.IsExpired)
                    {
                        model.IsExpired = result.IsExpired;
                    }
                    else
                    {
                        if (!result.IsSuccessful)
                        {
                            model.IsInvalid = true;
                            ViewBag.ErrorMessage = result.Message;
                        }
                        else
                        {
                            //Successfully activated
                            //Log user in automatically
                            var loginResult = _industryDataInterchange.Login(model.Email, model.NewPassword);

                            if (!loginResult.IsSuccessful)
                            {
                                model.CannotLogin = true;
                                if (loginResult.IsLocked)
                                {
                                    ViewBag.ErrorMessage = "The user has been activated but there was an error logging you in.<br/><br/>The user is locked. Please try to sign in and reset your password. If you continue to experience this error, please contact us on 1300 785 749.";
                                }
                                else if (loginResult.LoginMessage.IndexOf("not active") >= 0)
                                {
                                    ViewBag.ErrorMessage = "There was an error logging you in.<br/><br/>Your account has been suspended or closed. Please contact us on 1300 785 749.";
                                }
                                else
                                {
                                    ViewBag.ErrorMessage = "There was an error logging you in.<br/><br/>" + loginResult.LoginMessage;
                                }
                            }
                            else
                            {
                                //Successfully logged in
                                SessionHelper.SetLoginSessions(loginResult.AccountId, loginResult.AccountUserId,
                                    loginResult.IsAdmin, model.Email);
                                var home = CurrentPage.AncestorOrSelf(1);
                                IPublishedContent accountDashboardNode =
                                    Umbraco.Content(home.GetPropertyValue("accountDashboardNode"));
                                return new RedirectResult(accountDashboardNode.Url);
                            }
                        }
                    }
                }
            }
            TempData.Add("ActivateViewModel", model);
            return CurrentUmbracoPage();
        }

        #endregion

        #region Dashboard

        public ActionResult Dashboard(int currentPage, string editUserNodeUrl)
        {
            var model = getViewModel(currentPage, editUserNodeUrl);

            return PartialView("Dashboard", model);
        }

        private DashboardUserViewModel getViewModel(int currentPage, string editUserNodeUrl)
        {
            var accountUserId = SessionHelper.AccountUserId;
            var ucm = _userContextHelper.CreateUserContextObject(accountUserId, SessionHelper.AgentId);
            var itemsPerPage = 10;
            var users = new List<DashboardUserModel>();
            var model = new DashboardUserViewModel();
            model.Users = users;

            var result = _industryDataInterchange.GetAllAccountUsers(SessionHelper.AccountId, ucm);

            if (result.Errors != null)
            {
                ViewBag.ErrorMessage = result.Errors.Message;
            }
            else
            {
                users = GetAllUsers(result.Accounts, accountUserId);
                var pager = model.Pager = PagerHelper.GetPager(itemsPerPage, users.Count(), currentPage);
                model.Users = users.Skip((pager.CurrentPage - 1) * pager.ItemsPerPage).Take(pager.ItemsPerPage);
                ViewBag.EditUserNodeUrl = editUserNodeUrl;
            }
            return model;
        }

        private List<DashboardUserModel> GetAllUsers(List<GenericAccountModel> accounts, int adminId)
        {
            var models = new List<DashboardUserModel>();

            foreach (var item in accounts)
            {
                if (item.Account_Model.Status != Enums.AccountUserStatusEnum.Deleted && item.Account_Model.AccountUserID != adminId)
                {
                    var model = new DashboardUserModel()
                                {
                                    User =
                                        String.Format("{0} {1}", item.Account_Model.FirstName,
                                            item.Account_Model.LastName),
                                    Phone = item.Account_Model.PhoneNumber,
                                    Email = item.Account_Model.Email,
                                    CanSeeQuotes = item.Account_Model.CanSeeWashQuote,
                                    ReturnFormats = GenerateReturnFormatString(item.WashFormat),
                                    Status = item.Account_Model.Status.ToString(),
                                    AccountUserId = item.Account_Model.AccountUserID
                                };
                    models.Add(model);
                }
            }

            return models;
        }

        private string GenerateReturnFormatString(WashingFormat formats)
        {
            StringBuilder sb = new StringBuilder();

            if (formats.WashResultFormatRegisteredNumbers)
                sb.Append(string.Format("{0}, ", ConfigurationHelper.WASH_FORMAT_REGISTERED_NUMBERS));

            if (formats.WashResultFormatUnregisteredNumbers)
                sb.Append(string.Format("{0}, ", ConfigurationHelper.WASH_FORMAT_UNREGISTERED_NUMBERS));

            if (formats.WashResultFormatInvalidNumbers)
                sb.Append(string.Format("{0}, ", ConfigurationHelper.WASH_FORMAT_INVALID_NUMBERS));

            if (formats.WashResultFormatFileWithIndicators)
                sb.Append(string.Format("{0}, ", ConfigurationHelper.WASH_FORMAT_FILE_WITH_INDICATORS));

            return sb.ToString();
        }

        #endregion

        #region Delete Popup

        [ChildActionOnly]
        public ActionResult Delete(string editUserNodeUrl)
        {
            ViewBag.EditUserNodeUrl = editUserNodeUrl;
            return PartialView("Delete");
        }

        [NotChildAction]
        public ActionResult Delete(int userId, int currentPage, string editUserNodeUrl)
        {
            var ucm = _userContextHelper.CreateUserContextObject(SessionHelper.AccountUserId, SessionHelper.AgentId);

            var result = _industryDataInterchange.DeleteAccountUser(ucm, userId);

            if (result.Errors != null)
            {
                ViewBag.ErrorMessage = _errorMessageHelper.GenerateErrorMessage(result.Errors);
            }
            else if (!result.IsSuccessful)
            {
                ViewBag.ErrorMessage = result.Message;
            }

            var model = getViewModel(currentPage, editUserNodeUrl);
            return PartialView("Dashboard", model);
        }

        #endregion

        #region Activate Popup

        [ChildActionOnly]
        public ActionResult Activate(string editUserNodeUrl)
        {
            ViewBag.EditUserNodeUrl = editUserNodeUrl;
            return PartialView("Activate");
        }

        [NotChildAction]
        public ActionResult Activate(int userId, int currentPage, string editUserNodeUrl)
        {
            var ucm = _userContextHelper.CreateUserContextObject(SessionHelper.AccountUserId, SessionHelper.AgentId);

            var result = _industryDataInterchange.ActivateAccountUser(ucm, userId);
            string error = string.Empty;

            if (result.Errors != null)
            {
                error = _errorMessageHelper.GenerateErrorMessage(result.Errors);
            }
            else if (!result.IsSuccessful)
            {
                error = result.Message;
            }

            if (!String.IsNullOrEmpty(error) && error.IndexOf("Account has to be activated through activation link") >= 0)
            {
                Response.StatusCode = 403;
            }
            var model = getViewModel(currentPage, editUserNodeUrl);
            return PartialView("Dashboard", model);
        }

        #endregion

        #region Deactivate Popup

        [ChildActionOnly]
        public ActionResult Deactivate(string editUserNodeUrl)
        {
            ViewBag.EditUserNodeUrl = editUserNodeUrl;
            return PartialView("Deactivate");
        }

        [NotChildAction]
        public ActionResult Deactivate(int userId, int currentPage, string editUserNodeUrl)
        {
            var ucm = _userContextHelper.CreateUserContextObject(SessionHelper.AccountUserId, SessionHelper.AgentId);

            var result = _industryDataInterchange.DeActivateAccountUser(ucm, userId);

            if (result.Errors != null)
            {
                ViewBag.ErrorMessage = _errorMessageHelper.GenerateErrorMessage(result.Errors);
            }
            else if (!result.IsSuccessful)
            {
                ViewBag.ErrorMessage = result.Message;
            }

            var model = getViewModel(currentPage, editUserNodeUrl);
            return PartialView("Dashboard", model);
        }

        #endregion

        #region Change Password Popup

        [ChildActionOnly]
        public ActionResult ChangePassword()
        {
            var model = new ChangePasswordViewModel() { IsSubmitted = false };

            return PartialView("ChangePassword", model);
        }

        [NotChildAction]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = _industryDataInterchange.ChangePassword(SessionHelper.AccountUserId, model.NewPassword, model.CurrentPassword);

                if (result.Errors != null)
                {
                    ViewBag.ChangePasswordErrorMessage = _errorMessageHelper.GenerateErrorMessage(result.Errors);
                }
                else if (!result.IsSuccessful)
                {
                    ViewBag.ChangePasswordErrorMessage = result.Message;
                }

                if (result.IsSuccessful)
                {
                    model.IsSubmitted = true;
                }
            }
            TempData.Add("ChangePasswordViewModel", model);
            return CurrentUmbracoPage();
        }

        #endregion

        #region Reset Password

        [ChildActionOnly]
        public ActionResult ResetPassword(string token, string email)
        {
            var model = new ResetPasswordViewModel { IsInvalidOrExpired = false, Token = token, Email = email };

            return PartialView("ResetPassword", model);
        }

        [NotChildAction]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = _industryDataInterchange.ResetPassword(model.Email, model.NewPassword, model.Token);
                if (result.Errors != null)
                {
                    ViewBag.ErrorMessage = result.Errors.Message;
                }
                else
                {
                    if (!result.IsSuccessful)
                    {
                        /*if (result.Message == "Invalid token.")
                        {
                            ViewBag.ErrorMessage = "The reset password you have entered is invalid or expired, please try again";
                        }
                        else ViewBag.ErrorMessage = result.Message;*/

                        model.IsInvalidOrExpired = true;
                    }
                    else
                    {
                        //Add 500 ms delay
                        Thread.Sleep(500);
                        //Log user in automatically
                        var loginResult = _industryDataInterchange.Login(model.Email, model.NewPassword);

                        if (loginResult.Errors != null)
                        {
                            ViewBag.ErrorMessage = loginResult.Errors.Message;
                        }
                        else
                        {
                            if (loginResult.IsLocked)
                            {
                                ViewBag.ErrorMessage = "Account is locked. Please contact us on 1300 785 749.";
                            }
                            else if (!loginResult.IsSuccessful)
                            {
                                ViewBag.ErrorMessage = loginResult.LoginMessage;
                            }
                            else
                            {
                                //Successfully logged in
                                SessionHelper.SetLoginSessions(loginResult.AccountId, loginResult.AccountUserId, loginResult.IsAdmin, model.Email);
                                //redirect to account dashboard
                                var home = CurrentPage.AncestorOrSelf(1);
                                IPublishedContent accountDashboardNode = Umbraco.Content(home.GetPropertyValue("accountDashboardNode"));
                                return new RedirectResult(accountDashboardNode.Url);
                            }
                        }
                    }
                }
            }
            TempData.Add("ResetPasswordViewModel", model);
            return CurrentUmbracoPage();
        }

        #endregion

        #region Sign In

        [ChildActionOnly]
        public ActionResult SignIn()
        {
            var home = CurrentPage.AncestorOrSelf(1);
            IPublishedContent createAccountNode = Umbraco.Content(home.GetPropertyValue("createAccountNode"));
            var model = new SignInViewModel() { CreateAccountNodeUrl = createAccountNode.Url, IsLocked = false };

            return PartialView("SignIn", model);
        }

        [NotChildAction]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SignIn(SignInViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = _industryDataInterchange.Login(model.SignInEmailAddress, model.Password);

                if (result.Errors != null)
                {
                    ViewBag.SignInErrorMessage = result.Errors.Message;
                }
                else
                {
                    if (!result.IsSuccessful)
                    {
                        if (result.IsMigrated)
                        {
                            model.IsLocked = true;
                            model.IsMigrated = result.IsMigrated;
                        }
                        else if (result.IsLocked)
                        {
                            model.IsLocked = true;
                        }
                        else if (result.LoginMessage.IndexOf("not active") >= 0)
                        {
                            ViewBag.SignInErrorMessage = "Your account has been suspended or closed. Please contact us on 1300 785 749.";
                        }
                        else
                        {
                            ViewBag.SignInErrorMessage = "Your username or password may be incorrect.<br/><br/>Please use the correct login details assigned to operate this account. You have a limited number of attempts to login before your account will be locked.<br/><br/>If you continue to experience difficulties, please contact us on 1300 785 749.";
                        }
                    }
                    else
                    {
                        SessionHelper.SetLoginSessions(result.AccountId, result.AccountUserId, result.IsAdmin, model.SignInEmailAddress);

                        var headerResult = _industryDataInterchange.GetAccountUserHeaderInformationByAccountUserID(result.AccountUserId);

                        //Credit Alerts
                        if (headerResult.WashCreditsCount == 0 && headerResult.WashCredits50Count == 0 && headerResult.WashCredits80Count == 0)
                        {
                            SessionHelper.Below50Credit = false;
                            SessionHelper.Below20Credit = false;
                        }
                        else
                        {
                            if (headerResult.WashCreditsCount == 0)
                            {
                                SessionHelper.Below50Credit = false;
                                SessionHelper.Below20Credit = false;
                            }
                            else
                            {
                                SessionHelper.Below50Credit = headerResult.WashCreditsCount <= headerResult.WashCredits50Count;
                                SessionHelper.Below20Credit = headerResult.WashCreditsCount <= headerResult.WashCredits80Count;
                            }
                        }

                        if (CurrentPage != null)
                        {
                            var home = CurrentPage.AncestorOrSelf(1);
                            IPublishedContent accountDashboardNode =
                                Umbraco.Content(home.GetPropertyValue("accountDashboardNode"));
                            return new RedirectResult(accountDashboardNode.Url);
                        }
                    }
                }
            }
            TempData.Add("SignInViewModel", model);
            return CurrentUmbracoPage();
        }

        private enum SignInErrors
        {
            None,
            InvalidCredentials,
            InvalidAccount,
            TooManyAttemptAdmin,
            TooManyAttemptUser
        }

        #endregion

        #region Sign In Banner

        [ChildActionOnly]
        public ActionResult SignInBanner()
        {
            var accountUserId = SessionHelper.AccountUserId;
            if (SessionHelper.AccountId > 0 && accountUserId > 0)
            {
                var headerResult = _industryDataInterchange.GetAccountUserHeaderInformationByAccountUserID(accountUserId);

                if (headerResult.Errors != null || headerResult.AccountUserStatus != Enums.AccountUserStatusEnum.Active || headerResult.AccountStatus != Enums.AccountStatusEnum.Active)
                {
                    return SignOut();
                }

                SessionHelper.CompanyName = headerResult.CompanyName;
                ViewBag.Credit = headerResult.WashCreditsCount;
                ViewBag.CanSeeWashQuote = headerResult.CanSeeWashQuote;

                //Credit Alerts
                if (headerResult.WashCreditsCount == 0 && headerResult.WashCredits50Count == 0 && headerResult.WashCredits80Count == 0)
                {
                    SessionHelper.Below50Credit = false;
                    SessionHelper.Below20Credit = false;
                }
                else
                {
                    if (headerResult.WashCreditsCount == 0)
                    {
                        SessionHelper.Below50Credit = false;
                        SessionHelper.Below20Credit = false;
                    }
                    else
                    {
                        SessionHelper.Below50Credit = headerResult.WashCreditsCount <= headerResult.WashCredits50Count;
                        SessionHelper.Below20Credit = headerResult.WashCreditsCount <= headerResult.WashCredits80Count;
                    }
                }

                var home = CurrentPage.AncestorOrSelf(1);
                IPublishedContent accountDashboardNode = Umbraco.Content(home.GetPropertyValue("accountDashboardNode"));
                ViewBag.AccountDashboardURL = accountDashboardNode.Url;
                var timeoutPage = home.Children.Where(x => x.DocumentTypeAlias == "TimeoutPage");
                if (timeoutPage.Any())
                {
                    ViewBag.TimeoutPageUrl = timeoutPage.FirstOrDefault().Url;
                }

                return PartialView("SignInBanner");
            }
            return null;
        }

        public int SignOutCheck()
        {
            if (SessionHelper.AccountId == 0 || SessionHelper.AccountUserId == 0)
            {
                return 0;
            }
            return 1;
        }

        #endregion

        #region Sign Out

        public ActionResult SignOut(string redirectUrl = null)
        {
            SessionHelper.ClearSessions();

            if (String.IsNullOrEmpty(redirectUrl))
            {
                ControllerContext.HttpContext.Response.Redirect("/");
            }
            else
            {
                ControllerContext.HttpContext.Response.Redirect(redirectUrl);
            }

            return null;
        }

        #endregion

        #region Forgot Password

        [ChildActionOnly]
        public ActionResult ForgotPassword()
        {
            var model = new ForgotPasswordViewModel() { EmailSent = false };

            return PartialView("ForgotPassword", model);
        }

        [NotChildAction]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = _industryDataInterchange.ForgotPassword(model.ForgotPasswordEmailAddress);

                if (result.Errors != null)
                {
                    if (result.Errors.Message == "The account does not exist.")
                    {
                        ViewBag.ForgotPasswordErrorMessage = "There is no account linked to the email address that you have entered.";
                    }
                    else
                    {
                        ViewBag.ForgotPasswordErrorMessage = result.Errors.Message;
                    }
                }
                else
                {
                    if (!result.IsSuccessful)
                    {
                        ViewBag.ForgotPasswordErrorMessage = result.Message;
                    }
                    else
                    {
                        model.EmailSent = true;
                    }
                }
            }
            TempData.Add("SignInViewModel", model);
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
            ViewBag.AccountDashboardNode = Umbraco.Content(home.GetPropertyValue("accountDashboardNode")).Url;
        }
    }
}