using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using SD.ACMA.BusinessLogic;
using SD.ACMA.BusinessLogic.Avanade;
using SD.ACMA.DNCRProject.Website.Extensions;
using SD.ACMA.DNCRProject.Website.Helpers;
using SD.ACMA.DNCRProject.Website.Models;
using SD.ACMA.InterfaceTier;
using SD.ACMA.POCO.Consumer;
using Umbraco.Core.Models;
using Umbraco.Web;
using Umbraco.Web.Mvc;
using SD.ACMA.POCO.Base;

namespace SD.ACMA.DNCRProject.Website.Controllers
{
    public class RegistrationSurfaceController : SurfaceController
    {
        private IConsumerDataInterchange _consumerDataInterchange;
        private IErrorMessageHelper _errorMessageHelper;

        public RegistrationSurfaceController(IConsumerDataInterchange consumerDataInterchange, IErrorMessageHelper errorMessageHelper)
        {
            _consumerDataInterchange = consumerDataInterchange;
            _errorMessageHelper = errorMessageHelper;
        }

        #region Healthchecks

        public string CheckHealthCheck(string id)
        {
            var checkKey = ConfigurationManager.AppSettings["HealthCheck.Methods.CheckKey"] ?? "pleaseCheckMe";
            if (id == checkKey)
            {
                var numbers = new List<string> { ConfigurationManager.AppSettings["HealthCheck.NumberOnRegister"] ?? "0433319146" };
                var email = ConfigurationManager.AppSettings["HealthCheck.Email"] ?? "no-reply@salmat.com.au";

                var result = _consumerDataInterchange.CheckRegistration(email, numbers);

                if (result.Errors != null)
                {
                    return _errorMessageHelper.GenerateErrorMessage(result.Errors);
                }
                return "PASS";
            }

            return "Invalid key";
        }

        public string CreateHealthCheck(string id)
        {
            var createKey = ConfigurationManager.AppSettings["HealthCheck.Methods.CreateKey"] ?? "pleaseAddMe";
            if (id == createKey)
            {
                var r = new Registration
                {
                    ContactNumber = ConfigurationManager.AppSettings["HealthCheck.NumberNotOnRegister"] ?? "0287832973",
                    Email = ConfigurationManager.AppSettings["HealthCheck.Email"] ?? "no-reply@salmat.com.au",
                    FirstName = "Health",
                    LastName = "Check",
                    Numbers = new List<string> { ConfigurationManager.AppSettings["HealthCheck.NumberNotOnRegister"] ?? "0287832973" },
                };
                var result = _consumerDataInterchange.Register(r);

                if (result.Errors != null)
                {
                    return _errorMessageHelper.GenerateErrorMessage(result.Errors);
                }
                return "PASS";
            }
            return "Invalid key";
        }

        public string UpdateHealthCheck(string id)
        {
            var updateKey = ConfigurationManager.AppSettings["HealthCheck.Methods.UpdateKey"] ?? "pleaseUpdateMe";
            if (id == updateKey)
            {
                var r = new Registration
                {
                    ContactNumber = ConfigurationManager.AppSettings["HealthCheck.NumberOnRegister"] ?? "0433319146",
                    Email = ConfigurationManager.AppSettings["HealthCheck.Email"] ?? "no-reply@salmat.com.au",
                    FirstName = "Health",
                    LastName = "Check",
                    Numbers = new List<string> { ConfigurationManager.AppSettings["HealthCheck.NumberOnRegister"] ?? "0433319146" }
                };
                var result = _consumerDataInterchange.Register(r);

                if (result.Errors != null)
                {
                    return _errorMessageHelper.GenerateErrorMessage(result.Errors);
                }
                return "PASS";
            }
            return "Invalid key";
        }

        /*public string RemoveHealthCheck(string id)
        {
            var removeKey = ConfigurationManager.AppSettings["HealthCheck.Methods.RemoveKey"] ?? "pleaseRemoveMe";
            if (id == removeKey)
            {
                var bulkReg = new BulkRegistration
                {
                    AddressLine1 = "116 Miller Street",
                    AddressLine2 = string.Empty,
                    City = "North Sydney",
                    Country = "Australia",
                    Email = ConfigurationManager.AppSettings["HealthCheck.Email"] ?? "no-reply@salmat.com.au",
                    FirstName = "Health",
                    LastName = "Check",
                    OperationType = DNCR.Infrastructure.Enums.OperationTypeEnum.Deregister,
                    OrganisationName = string.Empty,
                    Postcode = "2060",
                    State = "NSW",
                    PreferredContactMethod = DNCR.Infrastructure.Enums.PreferredContactMethodEnum.Email,
                    PhoneNumbersFile = Encoding.ASCII.GetBytes(ConfigurationManager.AppSettings["HealthCheck.NumberOnRegister"] ?? "0433319146"),
                    EvidenceFile = new byte[] { 0 },
                };

                var result = _consumerDataInterchange.BulkRegistration(bulkReg);

                if (result.Errors != null)
                {
                    return _errorMessageHelper.GenerateErrorMessage(result.Errors);
                }
                if (result.HasErrorsInFax)
                {
                    return "There are errors in the fax numbers file";
                }
                if (result.HasErrorsInPhone)
                {
                    return "There are errors in the phone numbers file";
                }
                if (result.IsSuccessful)
                {
                    return "PASS";
                }
            }
            return "Invalid key";
        }*/

        #endregion

        #region Check Numbers

        [ChildActionOnly]
        public ActionResult Check()
        {
            GetViewBagProperties();

            var home = CurrentPage.AncestorOrSelf(1);
            var offlineForm = Umbraco.Media(home.GetPropertyValue("offlineCheckNumberForm"));
            ViewBag.OfflineForm = offlineForm.Url;
            ViewBag.OfflineFormFormat = offlineForm.UmbracoExtension;
            ViewBag.OfflineFormSize = Int32.Parse(offlineForm.UmbracoBytes.ToString()) / 1024;

            var model = new CheckRegistrationViewModel() { IsSubmitted = false };
            return PartialView("Check", model);
        }

        [NotChildAction]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Check(CheckRegistrationViewModel model)
        {
            ModelState.Remove("MinimumNumbers");
            if (ModelState.IsValid && RecaptchaHelper.ValidateRecaptcha(ConfigurationManager.AppSettings["RecaptchaSecretKey"]))
            {
                var numbers = new List<string>();

                foreach (var item in model.Numbers)
                {
                    if (!String.IsNullOrEmpty(item.Number))
                        numbers.Add(item.Number.FixPhoneNumber());
                }

                var result = _consumerDataInterchange.CheckRegistration(model.Email, numbers);

                if (result.Errors != null)
                {
                    ViewBag.ErrorMessage = _errorMessageHelper.GenerateErrorMessage(result.Errors);
                }
                else
                    model.IsSubmitted = true;
            }
            TempData.Add("CheckRegistrationViewModel", model);
            return CurrentUmbracoPage();
        }

        #endregion

        #region Register Numbers

        [ChildActionOnly]
        public ActionResult Create()
        {
            GetViewBagProperties();
            var home = CurrentPage.AncestorOrSelf(1);
            IPublishedContent bulkRegisterNode = Umbraco.Content(home.GetPropertyValue("bulkRegistrationNode"));
            ViewBag.BulkRegisterUrl = bulkRegisterNode.Url;

            var offlineForm = Umbraco.Media(home.GetPropertyValue("offlineRegistrationForm"));
            ViewBag.OfflineFormFormat = offlineForm.UmbracoExtension;
            ViewBag.OfflineFormSize = Int32.Parse(offlineForm.UmbracoBytes.ToString()) / 1024;
            ViewBag.OfflineForm = offlineForm.Url;

            var model = new RegistrationViewModel() { IsSubmitted = false };
            model.RegisterType = "my own number/s";
            return PartialView("Create", model);
        }

        [NotChildAction]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(RegistrationViewModel model)
        {
            ModelState.Remove("MinimumNumbers");
            if (model.RegisterType == "my own number/s")
            {
                ModelState.Remove("OrganisationName");
            }
            bool faxExist = false;
            foreach (var regNumber in model.Numbers)
            {
                if (regNumber.IsFax)
                {
                    faxExist = true;
                }
            }
            if (!faxExist)
            {
                ModelState.Remove("ContactNumber");
            }
            if (ModelState.IsValid && RecaptchaHelper.ValidateRecaptcha(ConfigurationManager.AppSettings["RecaptchaSecretKey"]))
            {
                var r = new Registration
                {
                    ContactNumber = model.ContactNumber.FixPhoneNumber(),
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName
                };

                var numbers = new List<string>();
                var faxes = new List<string>();
                foreach (var item in model.Numbers)
                {
                    if (!String.IsNullOrEmpty(item.Number))
                    {
                        var number = item.Number.FixPhoneNumber();
                        if (item.IsFax)
                            faxes.Add(number);
                        else
                            numbers.Add(number);
                    }
                }

                r.Numbers = numbers;
                r.FaxNumbers = faxes;

                if (!string.IsNullOrEmpty(model.ContactNumber))
                    r.ContactNumber = model.ContactNumber.FixPhoneNumber();

                var result = _consumerDataInterchange.Register(r);

                if (result.Errors != null)
                {
                    ViewBag.ErrorMessage = _errorMessageHelper.GenerateErrorMessage(result.Errors);
                }
                else model.IsSubmitted = true;
            }
            TempData.Add("RegistrationViewModel", model);
            return CurrentUmbracoPage();
        }

        #endregion

        #region Bulk Register Numbers

        [ChildActionOnly]
        public ActionResult BulkCreate()
        {
            GetViewBagProperties();
            var model = new BulkRegistrationViewModel { Country = "AU", IsSubmitted = false };
            model.CountryList = CountryDropDownHelper.BuildCountryDropDownList(_consumerDataInterchange.GetCountries().Countries);
            model.TransactionType = "Register numbers";
            return PartialView("BulkCreate", model);
        }

        [NotChildAction]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult BulkCreate(BulkRegistrationViewModel model)
        {
            if (model.TransactionType == "Register numbers")
            {
                ModelState.Remove("PhoneFaxFileUpload");
                if (model.PhoneFileUpload == null && model.FaxFileUpload == null)
                {
                    ModelState.AddModelError("FaxFileUpload", "At least one file must be uploaded");
                }
            }
            if (model.Country != "AU")
            {
                ModelState.Remove("State");
            }

            if (ModelState.IsValid && RecaptchaHelper.ValidateRecaptcha(ConfigurationManager.AppSettings["RecaptchaSecretKey"]))
            {
                var bulkReg = new BulkRegistration
                {
                    AddressLine1 = model.AddressLine1,
                    AddressLine2 = !string.IsNullOrEmpty(model.AddressLine2) ? model.AddressLine2 : string.Empty,
                    City = model.City,
                    Country = model.Country,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    OperationType = GetRegistrationTransactionType(model.TransactionType),
                    OrganisationName = !string.IsNullOrEmpty(model.OrganisationName) ? model.OrganisationName : string.Empty,
                    Postcode = model.Postcode,
                    State = model.State,
                    PreferredContactMethod = DNCR.Infrastructure.Enums.PreferredContactMethodEnum.Email
                };

                if (model.PhoneFaxFileUpload != null)
                {
                    var phoneByteArray = new byte[model.PhoneFaxFileUpload.ContentLength];
                    model.PhoneFaxFileUpload.InputStream.Read(phoneByteArray, 0, model.PhoneFaxFileUpload.ContentLength);
                    bulkReg.PhoneNumbersFile = phoneByteArray;
                }

                if (model.PhoneFileUpload != null)
                {
                    var phoneByteArray = new byte[model.PhoneFileUpload.ContentLength];
                    model.PhoneFileUpload.InputStream.Read(phoneByteArray, 0, model.PhoneFileUpload.ContentLength);
                    bulkReg.PhoneNumbersFile = phoneByteArray;
                }

                if (model.FaxFileUpload != null)
                {
                    var faxByteArray = new byte[model.FaxFileUpload.ContentLength];
                    model.FaxFileUpload.InputStream.Read(faxByteArray, 0, model.FaxFileUpload.ContentLength);
                    bulkReg.FaxNumbersFile = faxByteArray;
                }

                if (model.ValidationPDF != null)
                {
                    var validationFileArray = new byte[model.ValidationPDF.ContentLength];
                    model.ValidationPDF.InputStream.Read(validationFileArray, 0, model.ValidationPDF.ContentLength);
                    bulkReg.EvidenceFile = validationFileArray;
                }

                var result = _consumerDataInterchange.BulkRegistration(bulkReg);

                if (result.Errors != null)
                {
                    ViewBag.ErrorMessage = _errorMessageHelper.GenerateErrorMessage(result.Errors);
                }
                else if (result.HasErrorsInFax || result.HasErrorsInPhone)
                {
                    if (result.HasErrorsInFax)
                    {
                        ViewBag.ErrorMessage = "There are errors in the fax numbers file";
                        model.FaxErrorFileLocation = result.FaxErrorFileLocation;
                        model.FaxErrorFileName = result.FaxErrorFileName;
                        model.IsSubmitted = true;
                        model.HasErrorsInFax = result.HasErrorsInFax;
                    }
                    
                    if (result.HasErrorsInPhone)
                    {
                        ViewBag.ErrorMessage = "There are errors in the phone numbers file";
                        model.PhoneErrorFileLocation = result.PhoneErrorFileLocation;
                        model.PhoneErrorFileName = result.PhoneErrorFileName;
                        model.IsSubmitted = true;
                        model.HasErrorsInPhone = result.HasErrorsInPhone;
                    }
                }
                else if (result.IsSuccessful)
                    model.IsSubmitted = true;
            }
            model.CountryList = CountryDropDownHelper.BuildCountryDropDownList(_consumerDataInterchange.GetCountries().Countries);
            TempData.Add("BulkRegistrationViewModel", model);
            return CurrentUmbracoPage();
        }

        public FileResult DownloadErrorFile(string fileLocation, string fileName)
        {
            return File(fileLocation, System.Net.Mime.MediaTypeNames.Text.Plain, string.Format("{0}.txt", fileName));
        }

        private DNCR.Infrastructure.Enums.OperationTypeEnum GetRegistrationTransactionType(string selectedValue)
        {
            if (selectedValue == "Register numbers")
                return DNCR.Infrastructure.Enums.OperationTypeEnum.Register;
            else if (selectedValue == "Deregister numbers")
                return DNCR.Infrastructure.Enums.OperationTypeEnum.Deregister;
            else
                return DNCR.Infrastructure.Enums.OperationTypeEnum.Check;
        }

        #endregion

        #region Activate

        [ChildActionOnly]
        public ActionResult Activate(string token)
        {
            var success = false;
            var result = _consumerDataInterchange.ConfirmRegistration(token);

            if (result.Errors != null)
            {
                //ViewBag.Header = "Registration incomplete";
                ViewBag.Message = result.Errors.Message;
            }
            else
            {
                success = result.IsSuccessful;
                if (success)
                {
                    ViewBag.Header = "Registration complete!";
                    ViewBag.Message = String.Format("You have successfully registered {0} number/s on the Do Not Call Register. Please note that registration is permanent. However, you can remove your number/s or update your details at any time.", result.RegisteredNumberCount);
                }
                else
                {
                    /*if (result.IsTokenExpired)
                        ViewBag.Message = "It appears that the validation link has expired. Please call 1300 792 958 for assistance";
                    else ViewBag.Message = "It appears that the validation link has expired. Please call 1300 792 958 for assistance";
                    */

                    var home = CurrentPage.AncestorOrSelf(1);
                    IPublishedContent checkNumbersNode = Umbraco.Content(home.GetPropertyValue("checkNumbersNode"));

                    ViewBag.Message = String.Format(@"It appears your registration has already been validated or that you have attempted to validate outside of the validation period.
                    <br/><br/>
                    If you are unsure if your registration attempt was successful, please check the registration status of your number/s by clicking <a href='{0}'>here</a>.
                    <br/><br/>
                    Please call the Do Not Call Register contact centre on 1300 792 958 during business hours (Monday – Friday 8:30am – 5pm) for further support.", checkNumbersNode.Url);
                }
            }

            return PartialView("Activate");
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
        }
    }
}