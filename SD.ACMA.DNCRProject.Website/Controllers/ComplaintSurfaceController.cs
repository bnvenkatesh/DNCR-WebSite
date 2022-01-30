using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using SD.ACMA.BusinessLogic;
using SD.ACMA.BusinessLogic.Avanade;
using SD.ACMA.DNCR.Infrastructure;
using SD.ACMA.DNCR.Infrastructure.Configuration;
using SD.ACMA.DNCRProject.Website.Extensions;
using SD.ACMA.DNCRProject.Website.Helpers;
using SD.ACMA.DNCRProject.Website.Models;
using SD.ACMA.InterfaceTier;
using SD.ACMA.POCO.Base;
using SD.ACMA.POCO.Consumer;
using Umbraco.Core.Models;
using Umbraco.Web;
using Umbraco.Web.Mvc;

namespace SD.ACMA.DNCRProject.Website.Controllers
{
    public class ComplaintSurfaceController : SurfaceController
    {
        private IConsumerDataInterchange _consumerDataInterchange;
        private IIndustryDataInterchange _industryDataInterchange;
        private IErrorMessageHelper _errorMessageHelper;
        private IUserContextHelper _userContextHelper;

        public ComplaintSurfaceController(IConsumerDataInterchange consumerDataInterchange, IIndustryDataInterchange industryDataInterchange, IErrorMessageHelper errorMessageHelper, IUserContextHelper userContextHelper)
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

            var home = CurrentPage.AncestorOrSelf(1);
            ViewBag.LodgeEnquiryUrl = Umbraco.Content(home.GetPropertyValue("consumerLodgeEnquiryNode")).Url;
            ViewBag.RegistrationUrl = Umbraco.Content(home.GetPropertyValue("registrationNode")).Url;

            var model = new ConsumerComplaintViewModel { Country = "AU", IsSubmitted = false, IsNumberOnRegister = null, Channel = "Webform"};

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
        public ActionResult Consumer(ConsumerComplaintViewModel model)
        {
            if (!model.IsSubmitted)
            {
                if (!String.IsNullOrEmpty(model.RedirectUrl))
                {
                    return new RedirectResult(model.RedirectUrl);
                }

                if (model.ComplaintType == Enums.ComplaintTypeEnum.Call)
                {
                    ModelState.Remove("OtherComplaintType");
                    ModelState.Remove("HaveDestinationNumber");
                    ModelState.Remove("DestinationNumber");
                    RemoveContains(ModelState, "FaxIncidents");
                    RemoveContains(ModelState, "MinimumCallPurpose");
                    model.FaxIncidents = null;
                    for (int i = 0; i < model.CallIncidents.Count; i++)
                    {
                        if (!model.CallIncidents[i].CallPurposeOther)
                        {
                            ModelState.Remove("CallIncidents[" + i + "].OtherPurpose");
                        }
                    }
                }
                else if (model.ComplaintType == Enums.ComplaintTypeEnum.Fax)
                {
                    ModelState.Remove("CallType");
                    ModelState.Remove("CallOtherDescription");
                    ModelState.Remove("OtherComplaintType");
                    ModelState.Remove("IsBusinessNumber");
                    ModelState.Remove("OnlineSurvey");
                    RemoveContains(ModelState, "CallIncidents");
                    model.CallIncidents = null;
                    model.Consent = model.FaxConsent;
                    model.WithdrawnConsent = model.FaxWithdrawnConsent;
                    model.DateConsentWithdrawn = model.FaxDateConsentWithdrawn;
                    model.AdditionalWithdrawnConsent = model.FaxAdditionalWithdrawnConsent;
                }

                if (model.IsAnonymous)
                {
                    //ModelState.Remove("Title");
                    ModelState.Remove("FirstName");
                    ModelState.Remove("LastName");
                    //ModelState.Remove("AddressLine1");
                    //ModelState.Remove("City");
                    ModelState.Remove("Postcode");
                    //ModelState.Remove("State");
                    //ModelState.Remove("Country");
                }
                else if (model.Country != "AU")
                {
                    ModelState.Remove("State");
                }

                if (model.CallType != "Other")
                {
                    ModelState.Remove("CallOtherDescription");
                }

                if (ModelState.IsValid && RecaptchaHelper.ValidateRecaptcha(ConfigurationManager.AppSettings["RecaptchaSecretKey"]) &&
                    ((model.ComplaintType == Enums.ComplaintTypeEnum.Call && model.CallIncidents != null) || (model.ComplaintType == Enums.ComplaintTypeEnum.Fax && model.FaxIncidents != null)))
                {
                    var lcm = new LodgeComplaintModel();

                    var cdm = new ContactDetailsModel
                    {
                        Country = model.Country,
                        Email = model.Email,
                        PhoneNumber = model.ContactNumber.FixPhoneNumber(),
                    };

                    // Call/Fax Summary 
                    string summaryDetails = string.Empty;
                    if (model.CallIncidents != null)
                    {
                        summaryDetails = model.CallIncidents[0].CallSummary;
                    }
                    if (model.FaxIncidents != null)
                    {
                        summaryDetails = model.FaxIncidents[0].FaxSummary;
                    }

                    var crm = new ComplaintRequestModel
                    {
                        ComplaintType = model.ComplaintType.Value,
                        Anonymous = model.IsAnonymous,
                        ReceivingCallNumber = model.ComplainantNumber.FixPhoneNumber(),
                        ServiceProviderID = String.IsNullOrEmpty(model.PhoneServiceProvider) ? (int?) null : _consumerDataInterchange.GetServiceProviders().ServiceProviders.FirstOrDefault(x => x.ServiceProviderName.ToLower().Contains(model.PhoneServiceProvider.ToLower())).ServiceProviderID,
                        ContactingBusinessName = model.BusinessNameCalled,
                        ContactingBusinessDetails = model.DetailsBusinessCalled,
                        ProductProvider = model.ProductBusinessName,
                        ProductProviderDetails = model.DetailsProductBusinessName,
                        ProductOffered = model.ProductName,
                        DesignatedParty = ConvertCallPartiesStringToDesignatedPartyEnum(model.CallParties),
                        ConsentToBeContacted = ConvertConsentToConsentToBeContactedEnum(model.Consent),
                        ComplaintDetails = summaryDetails,
                        AdditionalDetailsConsentWithdrawn = !string.IsNullOrEmpty(model.AdditionalWithdrawnConsent) ? model.AdditionalWithdrawnConsent : string.Empty,
                        Channel = ChannelToWebServiceChannelTypeMapper(model.Channel),
                        IsNumberOnRegister = model.IsNumberOnRegister,
                        ContactBusiness = model.AgreePassedToBusiness,
                        ContactOtherParty = model.AgreeReferred,
                        ContactServiceProvider = model.AgreeTelephoneService,
                        IsConsumerComplaint = true,
                        AgentUserId = SessionHelper.AgentId,
                        HaveDestinationNumber = model.HaveDestinationNumber,
                        DestinationNumber = model.DestinationNumber,
                        OnlineSurveyCompletion = ConvertConsentToConsentToBeContactedEnum(model.OnlineSurvey),
                        OnlineSurveyAdditionalDetails = !string.IsNullOrEmpty(model.OnlineSurveyAddtionalDetails) ? model.OnlineSurveyAddtionalDetails : string.Empty,
                        IsBusinessNumber = model.IsBusinessNumber,
                    };

                    if (!string.IsNullOrEmpty(model.WithdrawnConsent))
                    {
                        crm.ConsentWithdrawn = model.WithdrawnConsent == "True";
                    }

                    if (!model.IsAnonymous)
                    {
                        cdm.Addressline1 = !string.IsNullOrEmpty(model.AddressLine1) ? model.AddressLine1 : string.Empty;
                        cdm.Addressline2 = !string.IsNullOrEmpty(model.AddressLine2) ? model.AddressLine2 : string.Empty;
                        cdm.FirstName = !string.IsNullOrEmpty(model.FirstName) ? model.FirstName : string.Empty;
                        cdm.LastName = !string.IsNullOrEmpty(model.LastName) ? model.LastName : string.Empty;
                        cdm.Postcode = !string.IsNullOrEmpty(model.Postcode) ? model.Postcode : string.Empty;
                        cdm.State = !string.IsNullOrEmpty(model.State) ? model.State : string.Empty;
                        cdm.Suburb = !string.IsNullOrEmpty(model.City) ? model.City : string.Empty;
                        cdm.Title = !string.IsNullOrEmpty(model.Title) ? model.Title : null;
                    }
                    else
                    {
                        cdm.Addressline1 = string.Empty;
                        cdm.Addressline2 = string.Empty;
                        cdm.FirstName = string.Empty;
                        cdm.LastName = string.Empty;
                        cdm.Postcode = string.Empty;
                        cdm.State = string.Empty;
                        cdm.Suburb = string.Empty;
                        cdm.Title = null;
                        cdm.Company = string.Empty; //"N/A" //TODO: This is not in the UI
                    }

                    if (model.PhoneServiceProvider == "Other")
                    {
                        crm.ServiceProviderOther = model.PhoneServiceProviderOther;
                    }

                    if (!string.IsNullOrEmpty(model.DateConsentWithdrawn))
                    {
                        crm.DateConsentWithdrawn = DateTime.ParseExact(model.DateConsentWithdrawn, "d/M/yyyy", null);
                    }

                    if (!string.IsNullOrEmpty(model.CallType))
                    {
                        crm.CallType = ConvertCallTypeToCallTypeEnum(model.CallType);

                        if (crm.CallType == Enums.CallTypeEnum.Other)
                        {
                            crm.CallTypeOther = model.CallOtherDescription;
                        }
                    }

                    var callIncidentList = new List<CallIncidentModel>();

                    if (model.CallIncidents != null)
                    {
                        foreach (var item in model.CallIncidents)
                        {
                            var incident = new CallIncidentModel
                            {
                                IsFax = false,
                                AMPM = item.CallAmPm,
                                CallDate = DateTime.ParseExact(item.DateOfCall, "d/M/yyyy", null),
                                CallDescription = item.CallDescription,
                                //CallDetails = item.CallDetails,
                                CallReceiveNumber = item.CallFromNumber.FixPhoneNumber(),
                                CallWasTerminated = item.WasCallEnded,
                                RequestedCallTermination = item.AskedCallEnded,
                                Time = item.TimeOfCall,
                                VoiceCall = ConvertCallDescriptionToVoiceCallEnum(item.CallDescription),
                                WasCallerIDContactable =
                                    ConvertCallerIdContactableToWasCallerContactableEnum(
                                        item.CallerIdContactable),
                                WasCallerIDDisplayed =
                                    ConvertHasCallerIdToWasCallerIDDisplayedEnum(item.HasCallerId),
                                CallPurposeAdvertise = item.CallPurposeAdvertise,
                                CallPurposeCaller = item.CallPurposeCaller,
                                CallPurposeDonation = item.CallPurposeDonation,
                                CallPurposeDonationForPolitics = item.CallPurposeDonationForPolitics,
                                CallPurposePoll = item.CallPurposePoll,
                                CallPurposeScam = item.CallPurposeScam,
                                CallPurposeVoting = item.CallPurposeElection,
                                CallPurposeOther = item.CallPurposeOther,
                                AnswerForCallTermination = (item.AskedCallEnded == true) ? item.AnswerForCallTermination : string.Empty
                                // AdditionalDetails = item.AdditionalDetails

                            };

                            if (incident.WasCallerIDDisplayed == Enums.WasCallerIDDisplayedEnum.Yes)
                            {
                                incident.NumberOrTextDisplayed = item.CallerIdNumber;
                            }

                            if (incident.WasCallerIDContactable == Enums.WasCallerContactableEnum.Other)
                                incident.WasCallerIDContactableOther = item.CallerIdContactableOther;

                            if (incident.VoiceCall == Enums.VoiceCallEnum.Other)
                                incident.VoiceCallOther = item.OtherCallDescription;

                            if (incident.CallPurposeOther)
                            {
                                incident.CallPurposeOtherDetails = item.OtherPurpose;
                            }

                            callIncidentList.Add(incident);
                        }
                    }
                    else if (model.FaxIncidents != null)
                    {
                        foreach (var item in model.FaxIncidents)
                        {
                            var incident = new CallIncidentModel
                            {
                                IsFax = true,
                                AMPM = item.FaxAmPm,
                                CallDate = DateTime.ParseExact(item.DateOfFax, "d/M/yyyy", null),
                                Time = item.TimeOfFax,
                            };

                            if (item.FaxFileUpload != null)
                            {
                                var faxByteArray = new byte[item.FaxFileUpload.ContentLength];
                                item.FaxFileUpload.InputStream.Read(faxByteArray, 0, item.FaxFileUpload.ContentLength);
                                incident.Attachment = faxByteArray;
                                incident.AttachmentFileName = item.FaxFileUpload.FileName;
                            }

                            callIncidentList.Add(incident);
                        }
                    }

                    lcm.ContactDetails = cdm;
                    lcm.ComplaintRequest = crm;
                    lcm.CallIncidents = callIncidentList;

                    var result = _consumerDataInterchange.LodgeComplaint(lcm);
                    ViewBag.Timing = result.Timing;

                    if (result.Errors != null)
                    {
                        ViewBag.ErrorMessage = _errorMessageHelper.GenerateErrorMessage(result.Errors);
                    }
                    else
                    {
                        model.IsSubmitted = true;
                        model.RefCode = result.ComplaintNumber;
                    }
                }
            }
            else
            {
                model.IsSubmitted = false;
                ModelState.Remove("ComplaintType");
                ModelState.Remove("CallType");
                ModelState.Remove("CallOtherDescription");
                ModelState.Remove("OtherComplaintType");
                if (model.IsAnonymous)
                {
                    ModelState.Remove("FirstName");
                    ModelState.Remove("LastName");
                    ModelState.Remove("Postcode");
                    ModelState.Remove("Country");
                }
                ModelState.Remove("ComplainantNumber");
                ModelState.Remove("AgreeTelephoneService");
                ModelState.Remove("AgreePassedToBusiness");
                ModelState.Remove("AgreeReferred");
                ModelState.Remove("IsBusinessNumber");
                ModelState.Remove("OnlineSurvey");
                ModelState.Remove("Channel");
                RemoveContains(ModelState, "CallIncidents");
                RemoveContains(ModelState, "FaxIncidents");
            }
            model.CountryList = CountryDropDownHelper.BuildCountryDropDownList(_industryDataInterchange.GetCountries().Countries);
            TempData.Add("ConsumerComplaintViewModel", model);
            return CurrentUmbracoPage();
        }

        private Enums.WasCallerIDDisplayedEnum? ConvertHasCallerIdToWasCallerIDDisplayedEnum(string stringValue)
        {
            if (stringValue == "Phone does not display Caller ID")
                return Enums.WasCallerIDDisplayedEnum.PhoneDoesNotDisplayCallerID;
            if (stringValue == "Yes")
                return Enums.WasCallerIDDisplayedEnum.Yes;
            if (stringValue == "Yes, but did not write it down")
                return Enums.WasCallerIDDisplayedEnum.YesButDidNotCapture;
            if (stringValue == "No")
                return Enums.WasCallerIDDisplayedEnum.No;
            if (stringValue == "Did not notice")
                return Enums.WasCallerIDDisplayedEnum.DidNotNotice;

            return null;
        }

        private Enums.WasCallerContactableEnum? ConvertCallerIdContactableToWasCallerContactableEnum(string stringValue)
        {
            if (stringValue == "Didn't try")
                return Enums.WasCallerContactableEnum.DidNotAttempt;
            if (stringValue == "Made contact")
                return Enums.WasCallerContactableEnum.MadeContact;
            if (stringValue == "Could not contact")
                return Enums.WasCallerContactableEnum.CouldNotContact;
            if (stringValue == "Left a message")
                return Enums.WasCallerContactableEnum.LeftAMessage;
            if (stringValue == "Got some information about the caller")
                return Enums.WasCallerContactableEnum.GotSomeInformation;
            if (stringValue == "Other")
                return Enums.WasCallerContactableEnum.Other;
            
            return null;
        }

        private Enums.VoiceCallEnum ConvertCallDescriptionToVoiceCallEnum(string description)
        {
            if (description == "You had a conversation with a real person")
                return Enums.VoiceCallEnum.ConversationWithARealPerson;
            if (description == "The call was a recorded message or synthetic voice")
                return Enums.VoiceCallEnum.RecordedMessageOrSyntheticVoice;
            if (description == "The call went to the answering machine, and a message was left")
                return Enums.VoiceCallEnum.MessageLeftOnAnsweringMachine;
            if (description == "Silence – when you took the call there was no response")
                return Enums.VoiceCallEnum.Silence;
            if (description == "You did not answer the call")
                return Enums.VoiceCallEnum.DidNotAnswerTheCall;
            if (description == "Other")
                return Enums.VoiceCallEnum.Other;

            return Enums.VoiceCallEnum.Other;
        }

        private void GenerateCallPurposeForWS(List<string> callPurposes, ref CallIncidentModel callIncident)
        {
            callIncident.CallPurposeAdvertise = false;
            callIncident.CallPurposeCaller = false;
            callIncident.CallPurposeDonation = false;
            callIncident.CallPurposeDonationForPolitics = false;
            callIncident.CallPurposePoll = false;
            callIncident.CallPurposeScam = false;
            callIncident.CallPurposeOther = false;

            foreach (var item in callPurposes)
            {
                if (item == "To offer, advertise or promote goods or services or financial opportunities")
                {
                    callIncident.CallPurposeAdvertise = true;
                }
                else if (item == "Were the goods or services those of the caller?")
                {
                    callIncident.CallPurposeCaller = true;
                }
                else if (item == "To ask for donations")
                {
                    callIncident.CallPurposeDonation = true;
                }
                else if (item == "Were the donations fundraising for an election or political party?")
                {
                    callIncident.CallPurposeDonationForPolitics = true;
                }
                else if (item == "To carry out a poll or survey")
                {
                    callIncident.CallPurposePoll = true;
                }
                else if (item == "Believe the call was a scam")
                {
                    callIncident.CallPurposeScam = true;
                }
                else if (item == "Other")
                {
                    callIncident.CallPurposeOther = true;
                }

            }
        }

        private Enums.ConsentToBeContactedEnum? ConvertConsentToConsentToBeContactedEnum(string consentString)
        {
            if (consentString == "Yes")
                return Enums.ConsentToBeContactedEnum.Yes;
            if (consentString == "No")
                return Enums.ConsentToBeContactedEnum.No;
            if (consentString == "Unsure")
                return Enums.ConsentToBeContactedEnum.Unsure;

            return null;
        }

        private Enums.DesignatedPartyEnum? ConvertCallPartiesStringToDesignatedPartyEnum(string callParty)
        {
            if (callParty == "No")
                return Enums.DesignatedPartyEnum.None;
            if (callParty == "Registered Charity")
                return Enums.DesignatedPartyEnum.RegisteredCharity;
            if (callParty == "Government Body")
                return Enums.DesignatedPartyEnum.GovernmentBody;
            if (callParty == "Political party/independent member/candidate")
                return Enums.DesignatedPartyEnum.PoliticalPartyMemberOrCandidate;
            if (callParty == "Educational institution")
                return Enums.DesignatedPartyEnum.EducationalInstitution;

            return null;
        }

        private Enums.CallTypeEnum ConvertCallTypeToCallTypeEnum(string callType)
        {
            if (callType == "Promoting goods, services or financial opportunities")
                return Enums.CallTypeEnum.PromotingGoodsServicesOrFinancialOpportunities;
            if (callType == "Asking for donations")
                return Enums.CallTypeEnum.AskingForDonations;
            if (callType == "Market research poll or survey")
                return Enums.CallTypeEnum.MarketResearchPollOrSurvey;
            if (callType == "Product recall")
                return Enums.CallTypeEnum.ProductRecall;
            if (callType == "Fault rectification")
                return Enums.CallTypeEnum.FaultRectification;
            if (callType == "Appointment rescheduling")
                return Enums.CallTypeEnum.AppointmentRescheduling;
            if (callType == "Appointment reminder")
                return Enums.CallTypeEnum.AppointmentReminder;
            if (callType == "Call relating to a payment or overdue account")
                return Enums.CallTypeEnum.CallRelatingToAPaymentOrOverdueAccount;
            if (callType == "Other")
                return Enums.CallTypeEnum.Other;
            if (callType == "Voting in an upcoming election")
                return Enums.CallTypeEnum.VotingInUpcomingElection;
            if (callType == "Scam")
                return Enums.CallTypeEnum.Scam;
            
            return Enums.CallTypeEnum.Other;
        }

        public static void RemoveContains(ModelStateDictionary modelState, string expression)
        {
            foreach (var ms in modelState.ToArray())
            {
                if (ms.Key.Contains(expression))
                {
                    modelState.Remove(ms);
                }
            }
        }

        #endregion

        #region Industry

        [ChildActionOnly]
        public ActionResult Industry()
        {
            GetViewBagProperties();

            var model = new IndustryComplaintViewModel { Country = "AU", IsSubmitted = false, Channel = "Webform"};
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
                    model.ContactNumber = userResult.PhoneNumber;
                }
            }

            return PartialView("Industry", model);
        }

        [NotChildAction]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Industry(IndustryComplaintViewModel model)
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

            bool isValid;
            if (SessionHelper.AccountId > 0 && SessionHelper.AccountUserId > 0)
            {
                isValid = ModelState.IsValid;
            }
            else
            {
                isValid = ModelState.IsValid && RecaptchaHelper.ValidateRecaptcha(ConfigurationManager.AppSettings["RecaptchaSecretKey"]);
            }
            if (isValid)
            {
                var lcm = new LodgeComplaintModel();

                var cdm = new ContactDetailsModel
                {
                    Email = model.Email,
                    PhoneNumber = model.ContactNumber.FixPhoneNumber(),
                    Country = model.Country,
                };

                if (!model.IsAnonymous)
                {
                    cdm.Addressline1 = !string.IsNullOrEmpty(model.AddressLine1) ? model.AddressLine1 : string.Empty;
                    cdm.Addressline2 = !string.IsNullOrEmpty(model.AddressLine2) ? model.AddressLine2 : string.Empty;
                    cdm.FirstName = !string.IsNullOrEmpty(model.FirstName) ? model.FirstName : string.Empty;
                    cdm.LastName = !string.IsNullOrEmpty(model.LastName) ? model.LastName : string.Empty;
                    cdm.Postcode = !string.IsNullOrEmpty(model.Postcode) ? model.Postcode : string.Empty;
                    cdm.State = !string.IsNullOrEmpty(model.State) ? model.State : string.Empty;
                    cdm.Suburb = !string.IsNullOrEmpty(model.City) ? model.City : string.Empty; 
                    cdm.Title = !string.IsNullOrEmpty(model.Title) ? model.Title : null; 
                    cdm.Company = !string.IsNullOrEmpty(model.OrganisationName) ? model.OrganisationName : string.Empty; //"N/A" //TODO: This is not in the UI
                }
                else
                {
                    cdm.Addressline1 = string.Empty;
                    cdm.Addressline2 = string.Empty;
                    cdm.FirstName = string.Empty;
                    cdm.LastName = string.Empty;
                    cdm.Postcode = string.Empty;
                    cdm.State = string.Empty;
                    cdm.Suburb = string.Empty;
                    cdm.Title = null;
                    cdm.Company = string.Empty; //"N/A" //TODO: This is not in the UI
                }

                var crm = new ComplaintRequestModel { 
                    ComplaintDetails = model.Details,
                    Anonymous = model.IsAnonymous,
                    IsConsumerComplaint = false
                };

                lcm.ContactDetails = cdm;
                lcm.ComplaintRequest = crm;

                var result = _consumerDataInterchange.LodgeComplaint(lcm);

                if (result.Errors != null)
                {
                    ViewBag.ErrorMessage = _errorMessageHelper.GenerateErrorMessage(result.Errors);
                }
                else
                {
                    model.IsSubmitted = true;
                    model.RefCode = result.ComplaintNumber;
                }
            }
            model.CountryList = CountryDropDownHelper.BuildCountryDropDownList(_industryDataInterchange.GetCountries().Countries);
            TempData.Add("IndustryComplaintViewModel", model);
            return CurrentUmbracoPage();
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
    }
}