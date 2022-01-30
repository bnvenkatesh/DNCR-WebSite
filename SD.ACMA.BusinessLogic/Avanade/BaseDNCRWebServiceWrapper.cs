using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using SD.ACMA.BusinessLogic.DNCRServicesCMS;
using SD.ACMA.POCO.Base;
using SD.ACMA.POCO.Consumer;
using SD.ACMA.InterfaceTier;
using SD.ACMA.POCO.Industry;
using SD.ACMA.DNCR.Infrastructure;
using SD.ACMA.DNCR.Infrastructure.Configuration;

namespace SD.ACMA.BusinessLogic.Avanade
{
    public class BaseDNCRWebServiceWrapper
    {
        private ISiteLoggingService _siteLoggingService;

        public BaseDNCRWebServiceWrapper(ISiteLoggingService siteLoggingService)
        {
            _siteLoggingService = siteLoggingService;
        }

        #region C&E

        public GenericResponseModel SendAttachMailResponse(AttachMailResponseRequest consumerEmail)
        {
            var response = new GenericResponseModel();

            try
            {
                var args = new AttachMailResponseRequestArgs
                {
                    Body = consumerEmail.Body,
                    EmailAsAttachment = consumerEmail.EmailData,
                    From = consumerEmail.From,
                    MatchingToken = consumerEmail.Token,
                    Subject = consumerEmail.Subject,
                    IsEnquiry = consumerEmail.IsEnquiry
                };

                using (var client = new OnlineCmsServiceClient())
                {
                    var result = client.AttachMailResponse(args);
                    response.IsSuccessful = result.IsSuccess;
                    response.Message = result.ResultMessage;

                    return response;
                }
            }
            catch (FaultException<DncrServiceFault> e)
            {
                response.Errors = ExtractErrorsFromException(e);
                response.IsSuccessful = false;
                return response;
            }
            catch (Exception ex)
            {
                response.Errors = ExtractErrorsFromException(ex);
                response.IsSuccessful = false;
                return response;
            }
        }

        public LodgeEnquiryResponse LodgeEnquiry(LodgeEnquiryModel lodgeEnquiryModel, int? agentId)
        {
            var response = new LodgeEnquiryResponse();

            try
            {
                using (var client = new OnlineCmsServiceClient())
                {
                    var countryValue = lodgeEnquiryModel.Country;//GetCountries().Countries.FirstOrDefault(x => x.countryName.ToLower() == lodgeEnquiryModel.Country.ToLower());

                    var args = new LodgeEnquiryRequestArgs
                    {
                        ContactDetails = new OnlineContactRequest
                        {
                            Addressline1 = lodgeEnquiryModel.Addressline1,
                            Addressline2 = lodgeEnquiryModel.Addressline2,
                            Company = !string.IsNullOrEmpty(lodgeEnquiryModel.CompanyName) ? lodgeEnquiryModel.CompanyName : string.Empty,
                            CountryISO = lodgeEnquiryModel.Country, //GetCountries().Countries.FirstOrDefault(x => x.countryName.ToLower() == lodgeEnquiryModel.Country.ToLower()).CountryISO,
                            Email = lodgeEnquiryModel.Email,
                            FirstName = lodgeEnquiryModel.FirstName,
                            LastName = lodgeEnquiryModel.LastName,
                            PhoneNumber = lodgeEnquiryModel.PhoneNumber,
                            Postcode = lodgeEnquiryModel.Postcode,
                            Title = ConvertTitleToWSEnum(lodgeEnquiryModel.Title),
                            Suburb = lodgeEnquiryModel.Suburb
                        },
                        EnquiryDetails = new OnlineEnquiryRequest
                        {
                            Channel = ConvertChannelToWSEnum(lodgeEnquiryModel.ChannelID),
                            CompanyName = lodgeEnquiryModel.CompanyName,
                            Details = lodgeEnquiryModel.Details,
                            AgentUserId = agentId ?? null,
                            IsRefund = lodgeEnquiryModel.IsRefund,
                            Anonymous = lodgeEnquiryModel.IsAnonymous
                        }
                    };

                    if (!string.IsNullOrEmpty(lodgeEnquiryModel.State))
                    {
                        if (countryValue != "AU")
                        {
                            args.ContactDetails.StateOther = lodgeEnquiryModel.State;
                        }
                        else
                        {
                            args.ContactDetails.State = ConvertStateToWSEnum(lodgeEnquiryModel.State);
                        }
                    }

                    if (args.EnquiryDetails.IsRefund != null && args.EnquiryDetails.IsRefund.Value)
                    {
                        args.RefundDetails = CreateRefundObject(lodgeEnquiryModel.RefundRequestModel);
                    }

                    //Hardcoded to None: SDACMA-817, SDACMA-818
                    args.EnquiryDetails.EnquiryTelemarketerType = enquiry_telemarketer_typeenquiry_telemarketer_typeEnum.None;

                    args.EnquiryDetails.EnquiryCategory = lodgeEnquiryModel.IsConsumer ? enquiry_categoryenquiry_categoryEnum.Consumer : enquiry_categoryenquiry_categoryEnum.Industry;

                    if (!string.IsNullOrEmpty(lodgeEnquiryModel.Subject))
                    {
                        var enquiryType = ConvertConsumerEnquirySubjectToWSEnquiryTypeEnum(lodgeEnquiryModel.Subject);
                        args.EnquiryDetails.EnquiryType = enquiryType;
                        args.EnquiryDetails.Subject = enquiryType == enquiry_typesenquiry_typesEnum.Other ? lodgeEnquiryModel.Subject : null;
                    }

                    var result = client.LodgeEnquiry(args);

                    response.EnquiryID = result.ReferenceID;
                    response.IsSuccessful = result.IsSuccessful;

                    return response;
                }
            }
            catch (FaultException<DncrServiceFault> e)
            {
                response.Errors = ExtractErrorsFromException(e);
                return response;
            }
            catch (Exception ex)
            {
                response.Errors = ExtractErrorsFromException(ex);
                return response;
            }
        }

        private OnlineRefundRequest CreateRefundObject(OnlineRefundRequestModel refundObject)
        {
            var onlineRefundRequest = new OnlineRefundRequest
            {
                AccountID = refundObject.AccountID,
                BankDetailsAccountName = refundObject.BankDetailsAccountName,
                BankDetailsAccountNumber = refundObject.BankDetailsAccountNumber,
                BankDetailsBSB = refundObject.BankDetailsBSB,
                BankDetailsOther = refundObject.BankDetailsOther,
                Description = refundObject.Description,
                DishonouredPaymentRefundID = refundObject.DishonouredPaymentRefundID,
                EnquiryRefundPaymentType = refundObject.EnquiryRefundPaymentType,
                EnquiryRefundType = ConvertRefundTypeEnumToWSEnum(refundObject.EnquiryRefundType),
                RefundAcmaIncrementAccountBalance = refundObject.RefundAcmaIncrementAccountBalance,
                RefundAcmaIncrementWashCredits = refundObject.RefundAcmaIncrementWashCredits,
                RefundAmountFromSubscription = refundObject.RefundAmountFromSubscription,
                RefundOrderID = refundObject.RefundOrderID,
                RefundReservedAccountBalance = refundObject.RefundReservedAccountBalance,
                RefundReservedWashCredits = refundObject.RefundReservedWashCredits,
                RefundSubscriptionID = refundObject.RefundSubscriptionID,
                RefundWashCreditsRemainingFromSubscription = refundObject.RefundWashCreditsRemainingFromSubscription,
                RefundWashTransactionID = refundObject.RefundWashTransactionID,
                WashCreditsRolloverAmount = refundObject.WashCreditsRolloverAmount,
                WashCreditsRolloverTransactionID = refundObject.WashCreditsRolloverTransactionID,
            };

            return onlineRefundRequest;
        }

        public LodgeComplaintResponse LodgeComplaint(LodgeComplaintModel lodgeComplaintModel)
        {
            var response = new LodgeComplaintResponse();

            try
            {
                using (var client = new OnlineCmsServiceClient())
                {
                    var countryValue = lodgeComplaintModel.ContactDetails.Country;//GetCountries().Countries.FirstOrDefault(x => x.countryName.ToLower() == lodgeComplaintModel.ContactDetails.Country.ToLower());

                    if (countryValue != null)
                    {
                        var contactDetails = CreateOnlineContactRequest(lodgeComplaintModel, countryValue);

                        if (!string.IsNullOrEmpty(lodgeComplaintModel.ContactDetails.State))
                        {
                            if (countryValue != "AU")
                            {
                                contactDetails.StateOther = lodgeComplaintModel.ContactDetails.State;
                            }
                            else
                            {
                                contactDetails.State = ConvertStateToWSEnum(lodgeComplaintModel.ContactDetails.State);
                            }
                        }

                        if (!string.IsNullOrEmpty(lodgeComplaintModel.ContactDetails.Title))
                            contactDetails.Title = ConvertTitleToWSEnum(lodgeComplaintModel.ContactDetails.Title);

                        var complaintDetails = CreateOnlineComplaintRequest(lodgeComplaintModel);

                        if (lodgeComplaintModel.ComplaintRequest.AdditionalAssist != null)
                            complaintDetails.AdditionalAssist = lodgeComplaintModel.ComplaintRequest.AdditionalAssist;

                        if (lodgeComplaintModel.ComplaintRequest.AgentUserId != null)
                            complaintDetails.AgentUserId = lodgeComplaintModel.ComplaintRequest.AgentUserId;

                        if (lodgeComplaintModel.ComplaintRequest.DateConsentWithdrawn != null)
                            complaintDetails.DateConsentWithdrawn = lodgeComplaintModel.ComplaintRequest.DateConsentWithdrawn;

                        var incidentCount = lodgeComplaintModel.CallIncidents != null
                            ? lodgeComplaintModel.CallIncidents.Count
                            : 0;
                        var counter = 0;


                        CallOrFaxDetail[] callArray = new CallOrFaxDetail[incidentCount];

                        if (incidentCount > 0)
                        {
                            foreach (var item in lodgeComplaintModel.CallIncidents)
                            {
                                var incident = CreateCallOrFaxDetail(item);

                                if (item.Attachment != null)
                                {
                                    incident.Attachment = item.Attachment;
                                    incident.AttachmentFileName = item.AttachmentFileName;
                                }

                                callArray[counter] = incident;
                                counter++;
                            }
                        }
                        var args = new LodgeComplaintRequestArgs
                        {
                            ContactDetails = contactDetails,
                            ComplaintDetails = complaintDetails,
                            Calls = callArray
                        };

                        var result = client.LodgeComplaint(args);

                        response.ComplaintNumber = result.ReferenceID;
                        response.IsSuccessful = result.IsSuccessful;
                        response.Timing = result.Timming;
                    }

                    return response;
                }
            }
            catch (FaultException<DncrServiceFault> e)
            {
                response.Errors = ExtractErrorsFromException(e);
                return response;
            }
            catch (Exception ex)
            {
                response.Errors = ExtractErrorsFromException(ex);
                return response;
            }
        }

        private CallOrFaxDetail CreateCallOrFaxDetail(CallIncidentModel item)
        {
            return new CallOrFaxDetail
            {
                IsFax = item.IsFax,
                AMPM = item.AMPM,
                CallDate = item.CallDate,
                CallDetails =
                    !string.IsNullOrEmpty(item.AdditionalDetails)
                        ? item.AdditionalDetails
                        : string.Empty,
                CallReceiveNumber =
                    !string.IsNullOrEmpty(item.CallReceiveNumber)
                        ? item.CallReceiveNumber
                        : string.Empty,
                CallWasTerminated = item.CallWasTerminated,
                NumberOrTextDisplayed =
                    !string.IsNullOrEmpty(item.NumberOrTextDisplayed)
                        ? item.NumberOrTextDisplayed
                        : string.Empty,
                RequestedCallTermination = item.RequestedCallTermination,
                Time = item.Time,
                VoiceCall = ConvertVoiceCallToWSEnum(item.VoiceCall),
                VoiceCallOther =
                    !string.IsNullOrEmpty(item.VoiceCallOther)
                        ? item.VoiceCallOther
                        : string.Empty,
                WasCallerIDContactable = ConvertWasCallerIDContactableToWSEnum(item.WasCallerIDContactable),
                WasCallerIDContactableOther =
                    !string.IsNullOrEmpty(item.WasCallerIDContactableOther)
                        ? item.WasCallerIDContactableOther
                        : string.Empty,
                WasCallerIDDisplayed =
                    ConvertWasCallerIDDisplayedToWSEnum(item.WasCallerIDDisplayed),

                CallPurposeAdvertise = item.CallPurposeAdvertise,
                CallPurposeCaller = item.CallPurposeCaller,
                CallPurposeDonation = item.CallPurposeDonation,
                CallPurposeDonationForPolitics = item.CallPurposeDonationForPolitics,
                CallPurposeOther = item.CallPurposeOther,
                CallPurposePoll = item.CallPurposePoll,
                CallPurposeScam = item.CallPurposeScam,
                CallPurposeVoting = item.CallPurposeVoting,

                CallPurposeOtherDetails =
                    !string.IsNullOrEmpty(item.CallPurposeOtherDetails)
                        ? item.CallPurposeOtherDetails
                        : string.Empty,
                AnswerForCallTermination = !string.IsNullOrEmpty(item.AnswerForCallTermination) ? item.AnswerForCallTermination : string.Empty
                //InformationNotProvided = !string.IsNullOrEmpty(item.CallDetails) ? item.CallDetails : string.Empty
            };
        }

        private OnlineContactRequest CreateOnlineContactRequest(LodgeComplaintModel lodgeComplaintModel, string countryValue)
        {
            return new OnlineContactRequest
            {
                Addressline1 = !string.IsNullOrEmpty(lodgeComplaintModel.ContactDetails.Addressline1) ? lodgeComplaintModel.ContactDetails.Addressline1 : string.Empty,
                Addressline2 = !string.IsNullOrEmpty(lodgeComplaintModel.ContactDetails.Addressline2) ? lodgeComplaintModel.ContactDetails.Addressline2 : string.Empty,
                Company = !string.IsNullOrEmpty(lodgeComplaintModel.ContactDetails.Company) ? lodgeComplaintModel.ContactDetails.Company : string.Empty,
                CountryISO = countryValue,
                Email = !string.IsNullOrEmpty(lodgeComplaintModel.ContactDetails.Email) ? lodgeComplaintModel.ContactDetails.Email : string.Empty,
                FirstName = !string.IsNullOrEmpty(lodgeComplaintModel.ContactDetails.FirstName) ? lodgeComplaintModel.ContactDetails.FirstName : string.Empty,
                LastName = !string.IsNullOrEmpty(lodgeComplaintModel.ContactDetails.LastName) ? lodgeComplaintModel.ContactDetails.LastName : string.Empty,
                PhoneNumber = !string.IsNullOrEmpty(lodgeComplaintModel.ContactDetails.PhoneNumber) ? lodgeComplaintModel.ContactDetails.PhoneNumber : string.Empty,
                Suburb = !string.IsNullOrEmpty(lodgeComplaintModel.ContactDetails.Suburb) ? lodgeComplaintModel.ContactDetails.Suburb : string.Empty,
                Postcode = !string.IsNullOrEmpty(lodgeComplaintModel.ContactDetails.Postcode) ? lodgeComplaintModel.ContactDetails.Postcode : string.Empty
            };
        }

        private OnlineComplaintRequest CreateOnlineComplaintRequest(LodgeComplaintModel lodgeComplaintModel)
        {
            return new OnlineComplaintRequest
            {
                AdditionalDetailsConsentWithdrawn = !string.IsNullOrEmpty(lodgeComplaintModel.ComplaintRequest.AdditionalDetailsConsentWithdrawn) ?
                    lodgeComplaintModel.ComplaintRequest.AdditionalDetailsConsentWithdrawn : string.Empty,
                Anonymous = lodgeComplaintModel.ComplaintRequest.Anonymous,
                CallType = ConvertCallTypeToWSEnum(lodgeComplaintModel.ComplaintRequest.CallType),
                CallTypeOther = !string.IsNullOrEmpty(lodgeComplaintModel.ComplaintRequest.CallTypeOther) ? lodgeComplaintModel.ComplaintRequest.CallTypeOther : string.Empty,
                Channel = ConvertChannelToWSEnum(lodgeComplaintModel.ComplaintRequest.Channel),
                ComplaintDetails = !string.IsNullOrEmpty(lodgeComplaintModel.ComplaintRequest.ComplaintDetails) ? lodgeComplaintModel.ComplaintRequest.ComplaintDetails : string.Empty,
                ComplaintType = ConvertComplaintTypeToWSEnum(lodgeComplaintModel.ComplaintRequest.ComplaintType),
                ComplaintTypeOther = !string.IsNullOrEmpty(lodgeComplaintModel.ComplaintRequest.ComplaintTypeOther) ? lodgeComplaintModel.ComplaintRequest.ComplaintTypeOther : string.Empty,
                ConsentToBeContacted = ConvertConsentToBeContactedToWSEnum(lodgeComplaintModel.ComplaintRequest.ConsentToBeContacted),
                ConsentWithdrawn = lodgeComplaintModel.ComplaintRequest.ConsentWithdrawn,
                ContactingBusinessDetails = !string.IsNullOrEmpty(lodgeComplaintModel.ComplaintRequest.ContactingBusinessDetails) ? lodgeComplaintModel.ComplaintRequest.ContactingBusinessDetails : string.Empty,
                ContactingBusinessName = !string.IsNullOrEmpty(lodgeComplaintModel.ComplaintRequest.ContactingBusinessName) ? lodgeComplaintModel.ComplaintRequest.ContactingBusinessName : string.Empty,
                DesignatedParty = ConvertDesignatedPartyToWSEnum(lodgeComplaintModel.ComplaintRequest.DesignatedParty),
                FurtherInformation = !string.IsNullOrEmpty(lodgeComplaintModel.ComplaintRequest.FurtherInformation) ? lodgeComplaintModel.ComplaintRequest.FurtherInformation : string.Empty,
                ProductOffered = !string.IsNullOrEmpty(lodgeComplaintModel.ComplaintRequest.ProductOffered) ? lodgeComplaintModel.ComplaintRequest.ProductOffered : string.Empty,
                ProductProvider = !string.IsNullOrEmpty(lodgeComplaintModel.ComplaintRequest.ProductProvider) ? lodgeComplaintModel.ComplaintRequest.ProductProvider : string.Empty,
                ProductProviderDetails = !string.IsNullOrEmpty(lodgeComplaintModel.ComplaintRequest.ProductProviderDetails) ? lodgeComplaintModel.ComplaintRequest.ProductProviderDetails : string.Empty,
                ReceivingCallNumber = !string.IsNullOrEmpty(lodgeComplaintModel.ComplaintRequest.ReceivingCallNumber) ? lodgeComplaintModel.ComplaintRequest.ReceivingCallNumber : string.Empty,
                ServiceProviderID = lodgeComplaintModel.ComplaintRequest.ServiceProviderID != null ? lodgeComplaintModel.ComplaintRequest.ServiceProviderID : null,
                ServiceProviderOther = !string.IsNullOrEmpty(lodgeComplaintModel.ComplaintRequest.ServiceProviderOther) ? lodgeComplaintModel.ComplaintRequest.ServiceProviderOther : string.Empty,
                SubjectACMA = !string.IsNullOrEmpty(lodgeComplaintModel.ComplaintRequest.SubjectACMA) ? lodgeComplaintModel.ComplaintRequest.SubjectACMA : string.Empty,

                ContactBusiness = lodgeComplaintModel.ComplaintRequest.ContactBusiness ?? false,
                ContactOtherParty = lodgeComplaintModel.ComplaintRequest.ContactOtherParty != null ? lodgeComplaintModel.ComplaintRequest.ContactOtherParty : null,
                ContactServiceProvider = lodgeComplaintModel.ComplaintRequest.ContactServiceProvider ?? false,
                IsConsumerComplaint = lodgeComplaintModel.ComplaintRequest.IsConsumerComplaint,
                IsNumberOnDNCR = lodgeComplaintModel.ComplaintRequest.IsNumberOnRegister,
                FaxHeaderHasDestinationNumber = lodgeComplaintModel.ComplaintRequest.HaveDestinationNumber,
                DestinationNumber = lodgeComplaintModel.ComplaintRequest.DestinationNumber,
                IsBusinessNumber = lodgeComplaintModel.ComplaintRequest.IsBusinessNumber,
                AdditionalDetailsConsentOnlineSurvey = !string.IsNullOrEmpty(lodgeComplaintModel.ComplaintRequest.OnlineSurveyAdditionalDetails) ?
                    lodgeComplaintModel.ComplaintRequest.OnlineSurveyAdditionalDetails : string.Empty,
                ConsentOnlineSurveryCompletion = ConvertConsentToBeContactedToWSEnum(lodgeComplaintModel.ComplaintRequest.OnlineSurveyCompletion),
            };
        }

        public GetCountriesResponse GetCountries()
        {
            var response = new GetCountriesResponse();

            try
            {
                using (var client = new OnlineCmsServiceClient())
                {
                    var result = client.GetCountries();

                    if (result.Countries != null)
                    {
                        var countriesList = result.Countries.Select(item => new CountryModel
                        {
                            countryID = item.countryID,
                            CountryISO = item.CountryISO,
                            countryName = item.countryName
                        }).ToList();

                        response.Countries = countriesList;
                    }

                    return response;
                }
            }
            catch (FaultException<DncrServiceFault> e)
            {
                response.Errors = ExtractErrorsFromException(e);
                return response;
            }
            catch (Exception ex)
            {
                response.Errors = ExtractErrorsFromException(ex);
                return response;
            }
        }

        public GetServiceProvidersResponse GetServiceProviders()
        {
            var r = new GetServiceProvidersResponse();

            try
            {
                using (var client = new OnlineCmsServiceClient())
                {
                    var result = client.GetServiceProviders();

                    r.IsSuccessful = result.IsSuccessful;

                    if (result.ServiceProviders != null)
                    {
                        var serviceProviderList = result.ServiceProviders.Select(item => new ServiceProviderModel
                        {
                            ServiceProviderID = item.ServiceProviderID,
                            ServiceProviderName = item.ServiceProviderName
                        }).ToList();

                        r.ServiceProviders = serviceProviderList;
                    }

                    return r;
                }
            }
            catch (FaultException<DncrServiceFault> e)
            {
                r.Errors = ExtractErrorsFromException(e);
                return r;
            }
            catch (Exception ex)
            {
                r.Errors = ExtractErrorsFromException(ex);
                return r;
            }
        }

        public GetSuburbForPostCodeResponse GetServiceProviders(string postcode)
        {
            var r = new GetSuburbForPostCodeResponse();

            try
            {
                using (var client = new OnlineCmsServiceClient())
                {
                    var args = new SuburbsInPostCodeRequestArgs
                    {
                        PostCode = postcode
                    };

                    var result = client.GetSuburbForPostCode(args);

                    r.IsSuccessful = result.IsSuccess;
                    r.Message = result.ResultMessage;

                    if (result.Suburbs != null)
                    {
                        var suburbList = result.Suburbs.ToList();

                        r.Suburbs = suburbList;
                    }

                    return r;
                }
            }
            catch (FaultException<DncrServiceFault> e)
            {
                r.Errors = ExtractErrorsFromException(e);
                return r;
            }
            catch (Exception ex)
            {
                r.Errors = ExtractErrorsFromException(ex);
                return r;
            }
        }

        public ImpersonateCSRResponse ImpersonateCSR(string secureData)
        {
            var r = new ImpersonateCSRResponse();

            try
            {
                using (var client = new OnlineCmsServiceClient())
                {
                    var args = new ImpersonateRequestArgs
                    {
                        SecureData = secureData
                    };

                    var result = client.ImpersonateCSR(args);

                    r.IsSuccessful = result.IsSuccessful;
                    r.Message = result.ResultMessage;
                    r.AgentId = result.AgentId;
                    r.DisplayName = result.DisplayName;
                    r.LoginName = result.LogginName;
                    r.IsAcma = result.Organisation == OrganisationOrganisationEnum.ACMA;

                    return r;
                }
            }
            catch (FaultException<DncrServiceFault> e)
            {
                r.Errors = ExtractErrorsFromException(e);
                return r;
            }
            catch (Exception ex)
            {
                r.Errors = ExtractErrorsFromException(ex);
                return r;
            }
        }

        private channelschannelsEnum ConvertChannelToWSEnum(DNCR.Infrastructure.Enums.ChannelsEnum selectedEnumValue)
        {
            switch ((int)selectedEnumValue)
            {
                case 1:
                    return channelschannelsEnum.OasisIVR;
                case 2:
                    return channelschannelsEnum.Symposium;
                case 3:
                    return channelschannelsEnum.Agent;
                case 4:
                    return channelschannelsEnum.Webform;
                case 5:
                    return channelschannelsEnum.Other;
                case 6:
                    return channelschannelsEnum.Phone;
                case 7:
                    return channelschannelsEnum.Email;
                case 8:
                    return channelschannelsEnum.Letter;
                case 9:
                    return channelschannelsEnum.Fax;
                case 10:
                    return channelschannelsEnum.ACMA;
                case 11:
                    return channelschannelsEnum.Split;
                default:
                    return channelschannelsEnum.Webform;
            }
        }

        #endregion

        private complaint_call_typecomplaint_call_typeEnum ConvertCallTypeToWSEnum(DNCR.Infrastructure.Enums.CallTypeEnum selectedEnumValue)
        {
            if (selectedEnumValue.ToString() == complaint_call_typecomplaint_call_typeEnum.PromotingGoodsServicesOrFinancialOpportunities.ToString())
                return complaint_call_typecomplaint_call_typeEnum.PromotingGoodsServicesOrFinancialOpportunities;

            if (selectedEnumValue.ToString() == complaint_call_typecomplaint_call_typeEnum.AskingForDonations.ToString())
                return complaint_call_typecomplaint_call_typeEnum.AskingForDonations;

            if (selectedEnumValue.ToString() == complaint_call_typecomplaint_call_typeEnum.MarketResearchPollOrSurvey.ToString())
                return complaint_call_typecomplaint_call_typeEnum.MarketResearchPollOrSurvey;

            if (selectedEnumValue.ToString() == complaint_call_typecomplaint_call_typeEnum.ProductRecall.ToString())
                return complaint_call_typecomplaint_call_typeEnum.ProductRecall;

            if (selectedEnumValue.ToString() == complaint_call_typecomplaint_call_typeEnum.FaultRectification.ToString())
                return complaint_call_typecomplaint_call_typeEnum.FaultRectification;

            if (selectedEnumValue.ToString() == complaint_call_typecomplaint_call_typeEnum.AppointmentRescheduling.ToString())
                return complaint_call_typecomplaint_call_typeEnum.AppointmentRescheduling;

            if (selectedEnumValue.ToString() == complaint_call_typecomplaint_call_typeEnum.AppointmentReminder.ToString())
                return complaint_call_typecomplaint_call_typeEnum.AppointmentReminder;

            if (selectedEnumValue.ToString() == complaint_call_typecomplaint_call_typeEnum.CallRelatingToAPaymentOrOverdueAccount.ToString())
                return complaint_call_typecomplaint_call_typeEnum.CallRelatingToAPaymentOrOverdueAccount;

            if (selectedEnumValue.ToString() == complaint_call_typecomplaint_call_typeEnum.VotingInAnUpcomingElection.ToString())
                return complaint_call_typecomplaint_call_typeEnum.VotingInAnUpcomingElection;

            if (selectedEnumValue.ToString() == complaint_call_typecomplaint_call_typeEnum.Scam.ToString())
                return complaint_call_typecomplaint_call_typeEnum.Scam;

            return complaint_call_typecomplaint_call_typeEnum.Other;
        }

        private titlestitlesEnum? ConvertTitleToWSEnum(string title)
        {
            if (title == null)
                return null;
            var value = title.ToLower();

            if (value == "mr")
            {
                return titlestitlesEnum.Mr;
            }
            if (value == "mrs")
            {
                return titlestitlesEnum.Mrs;
            }
            if (value == "miss")
            {
                return titlestitlesEnum.Miss;
            }
            if (value == "ms")
            {
                return titlestitlesEnum.Ms;
            }
            if (value == "dr")
            {
                return titlestitlesEnum.Dr;
            }

            return titlestitlesEnum.None;
        }

        private StateStateEnum ConvertStateToWSEnum(string state)
        {
            var value = state.ToUpper();

            if (value == "NSW")
            {
                return StateStateEnum.NewSouthWales;
            }
            if (value == "ACT")
            {
                return StateStateEnum.AustralianCapitalTerritory;
            }
            if (value == "NT")
            {
                return StateStateEnum.NorthernTerritory;
            }
            if (value == "QLD")
            {
                return StateStateEnum.Queensland;
            }
            if (value == "SA")
            {
                return StateStateEnum.SouthAustralia;
            }
            if (value == "TAS")
            {
                return StateStateEnum.Tasmania;
            }
            if (value == "VIC")
            {
                return StateStateEnum.Victoria;
            }
            if (value == "WA")
            {
                return StateStateEnum.WesternAustralia;
            }

            return StateStateEnum.NewSouthWales;
        }

        private complaint_typescomplaint_typesEnum ConvertComplaintTypeToWSEnum(DNCR.Infrastructure.Enums.ComplaintTypeEnum selectedEnumValue)
        {
            switch ((int)selectedEnumValue)
            {
                case 1:
                    return complaint_typescomplaint_typesEnum.Call;
                case 2:
                    return complaint_typescomplaint_typesEnum.Registration;
                case 3:
                    return complaint_typescomplaint_typesEnum.Access;
                case 4:
                    return complaint_typescomplaint_typesEnum.Other;
                case 5:
                    return complaint_typescomplaint_typesEnum.Industry;
                case 6:
                    return complaint_typescomplaint_typesEnum.Fax;
                default:
                    return complaint_typescomplaint_typesEnum.Other;
            }
        }

        private calling_partycalling_partyEnum? ConvertDesignatedPartyToWSEnum(DNCR.Infrastructure.Enums.DesignatedPartyEnum? selectedEnumValue)
        {
            if (selectedEnumValue != null)
            {
                switch ((int) selectedEnumValue)
                {
                    case 1:
                        return calling_partycalling_partyEnum.GovernmentBody;
                    case 2:
                        return calling_partycalling_partyEnum.RegisteredCharity;
                    case 3:
                        return calling_partycalling_partyEnum.PoliticalPartyMemberOrCandidate;
                    case 4:
                        return calling_partycalling_partyEnum.EducationalInstitution;
                    case 5:
                        return calling_partycalling_partyEnum.None;
                    default:
                        return null;
                }
            }
            return null;
        }

        private consentconsentEnum? ConvertConsentToBeContactedToWSEnum(DNCR.Infrastructure.Enums.ConsentToBeContactedEnum? selectedEnumValue)
        {
            if (selectedEnumValue != null)
            {
                switch ((int) selectedEnumValue)
                {
                    case 1:
                        return consentconsentEnum.Yes;
                    case 2:
                        return consentconsentEnum.No;
                    case 3:
                        return consentconsentEnum.Unsure;

                    default:
                        return null;
                }
            }
            return null;
        }

        private cli_contactablecli_contactableEnum? ConvertWasCallerIDContactableToWSEnum(DNCR.Infrastructure.Enums.WasCallerContactableEnum? selectedEnumValue)
        {
            if (selectedEnumValue != null)
            {
                switch ((int) selectedEnumValue)
                {
                    case 1:
                        return cli_contactablecli_contactableEnum.MadeContact;
                    case 2:
                        return cli_contactablecli_contactableEnum.CouldNotContact;
                    case 3:
                        return cli_contactablecli_contactableEnum.LeftAMessage;
                    case 4:
                        return cli_contactablecli_contactableEnum.GotSomeInformation;
                    case 5:
                        return cli_contactablecli_contactableEnum.DidNotAttempt;
                    case 6:
                        return cli_contactablecli_contactableEnum.Other;
                    default:
                        return null;
                }
            }
            return null;
        }

        private cli_displayedcli_displayedEnum? ConvertWasCallerIDDisplayedToWSEnum(DNCR.Infrastructure.Enums.WasCallerIDDisplayedEnum? selectedEnumValue)
        {
            if (selectedEnumValue != null)
            {
                switch ((int) selectedEnumValue)
                {
                    case 1:
                        return cli_displayedcli_displayedEnum.PhoneDoesNotDisplayCallerID;
                    case 2:
                        return cli_displayedcli_displayedEnum.Yes;
                    case 3:
                        return cli_displayedcli_displayedEnum.No;
                    case 4:
                        return cli_displayedcli_displayedEnum.DidNotNotice;
                    case 5:
                        return cli_displayedcli_displayedEnum.YesButDidNotCapture;
                    default:
                        return null;
                }
            }
            return null;
        }

        private voice_callvoice_callEnum ConvertVoiceCallToWSEnum(DNCR.Infrastructure.Enums.VoiceCallEnum selectedEnumValue)
        {
            switch ((int)selectedEnumValue)
            {
                case 1:
                    return voice_callvoice_callEnum.ConversationWithARealPerson;
                case 2:
                    return voice_callvoice_callEnum.RecordedMessageOrSyntheticVoice;
                case 3:
                    return voice_callvoice_callEnum.MessageLeftOnAnsweringMachine;
                case 4:
                    return voice_callvoice_callEnum.Silence;
                case 5:
                    return voice_callvoice_callEnum.DidNotAnswerTheCall;
                case 6:
                    return voice_callvoice_callEnum.Other;
                default:
                    return voice_callvoice_callEnum.Silence;
            }
        }

        private enquiry_typesenquiry_typesEnum ConvertEnquiryTelemarketerTypeToWSEnum(DNCR.Infrastructure.Enums.EnquiryTypeEnum selectedEnumValue)
        {
            if (selectedEnumValue.ToString() == enquiry_typesenquiry_typesEnum.Business.ToString())
                return enquiry_typesenquiry_typesEnum.Business;

            if (selectedEnumValue.ToString() == enquiry_typesenquiry_typesEnum.Consumer.ToString())
                return enquiry_typesenquiry_typesEnum.Consumer;

            return enquiry_typesenquiry_typesEnum.Other;
        }

        private enquiry_telemarketer_typeenquiry_telemarketer_typeEnum ConvertEnquiryTelemarketerTypeEnumToWSEnum(DNCR.Infrastructure.Enums.EnquiryTelemarketerTypeEnum selectedEnumValue)
        {
            switch ((int)selectedEnumValue)
            {
                case 1:
                    return enquiry_telemarketer_typeenquiry_telemarketer_typeEnum.None;
                case 2:
                    return enquiry_telemarketer_typeenquiry_telemarketer_typeEnum.BalanceRefund;
                case 3:
                    return enquiry_telemarketer_typeenquiry_telemarketer_typeEnum.BalanceTransfer;
                case 4:
                    return enquiry_telemarketer_typeenquiry_telemarketer_typeEnum.WashCreditsRollover;
                case 5:
                    return enquiry_telemarketer_typeenquiry_telemarketer_typeEnum.WashCreditsRefund;
                case 6:
                    return enquiry_telemarketer_typeenquiry_telemarketer_typeEnum.WashCreditsToBalanceRefund;
                default:
                    return enquiry_telemarketer_typeenquiry_telemarketer_typeEnum.None;
            }
        }

        private enquiry_typesenquiry_typesEnum ConvertConsumerEnquirySubjectToWSEnquiryTypeEnum(string subject)
        {
            if (subject == "How the Do Not Call Register affects my business or organisation")
            {
                return enquiry_typesenquiry_typesEnum.Business;
            }
            if (subject == "How the Do Not Call Register affects me as a consumer")
            {
                return enquiry_typesenquiry_typesEnum.Consumer;
            }
            if (subject == "Other")
            {
                return enquiry_typesenquiry_typesEnum.Other;
            }

            return enquiry_typesenquiry_typesEnum.Other;
        }

        //TODO: Double-check Balance Transfer Between Accounts & Others - SDACMA-818
        private enquiry_telemarketer_typeenquiry_telemarketer_typeEnum ConvertIndustryEnquirySubjectToWSEnquiryTypeEnum(string subject)
        {
            if (subject == "Balance Refunds")
            {
                return enquiry_telemarketer_typeenquiry_telemarketer_typeEnum.BalanceRefund;
            }
            if (subject == "Wash Credit Refunds")
            {
                return enquiry_telemarketer_typeenquiry_telemarketer_typeEnum.WashCreditsRefund;
            }
            if (subject == "Wash Credit Rollovers")
            {
                return enquiry_telemarketer_typeenquiry_telemarketer_typeEnum.WashCreditsRollover;
            }
            if (subject == "Balance Transfer Between Accounts")
            {
                return enquiry_telemarketer_typeenquiry_telemarketer_typeEnum.WashCreditsToBalanceRefund;
            }
            if (subject == "Others")
            {
                return enquiry_telemarketer_typeenquiry_telemarketer_typeEnum.None;
            }

            return enquiry_telemarketer_typeenquiry_telemarketer_typeEnum.None;
        }

        #region Exception Handler

        public WebServiceFault ExtractErrorsFromException<T>(FaultException<T> e)
        {
            var error = new WebServiceFault();

            dynamic serviceFault = (T)e.Detail;

            if (serviceFault != null)
            {
                error.Message = serviceFault.Message;
                error.CorrelationToken = serviceFault.CorrelationToken;
                error.ExceptionType = serviceFault.ExceptionTypeFullName;

                if (serviceFault.Reasons != null)
                {
                    error.FaultReasons = new List<WebServiceFaultReason>();

                    if (serviceFault.Reasons != null)
                    {
                        foreach (var reason in serviceFault.Reasons)
                        {
                            error.FaultReasons.Add(new WebServiceFaultReason { Message = reason.Message, PropertyName = reason.PropertyName });
                        }
                    }
                }
            }
            else
            {
                error.Message = e.Message;
                error.StackTrace = e.StackTrace;

                try
                {
                    string errorMessage = string.Format("{0} : {1}", error.Message, error.StackTrace);
                    errorMessage = errorMessage.Length > 5000 ? errorMessage.Substring(0, 5000) : errorMessage;

                    _siteLoggingService.Insert(errorMessage, null, null);
                }
                catch //(Exception ex)
                {

                }
            }

            if (error.Message.ToLower().Contains("unhandle"))
                error.Message = ConfigurationHelper.Instance.UnhandledErrorMessageSubstitute;
            else
                error.Message = serviceFault.Message;

            return error;
        }

        public WebServiceFault ExtractErrorsFromException(Exception e)
        {
            var error = new WebServiceFault { Message = e.Message, StackTrace = e.StackTrace };

            try
            {
                string errorMessage = string.Format("{0} : {1}", error.Message, error.StackTrace);
                errorMessage = errorMessage.Length > 5000 ? errorMessage.Substring(0, 5000) : errorMessage;

                _siteLoggingService.Insert(errorMessage, null, null);
            }
            catch //(Exception ex)
            {

            }

            if (error.Message.ToLower().Contains("unhandle"))
                error.Message = ConfigurationHelper.Instance.UnhandledErrorMessageSubstitute;

            return error;
        }

        #endregion

        internal enquiry_refund_typesenquiry_refund_typesEnum ConvertRefundTypeEnumToWSEnum(Enums.RefundTypeEnum selectedEnumValue)
        {
            switch ((int)selectedEnumValue)
            {
                case 1:
                    return enquiry_refund_typesenquiry_refund_typesEnum.SubscriptionRefund;
                case 2:
                    return enquiry_refund_typesenquiry_refund_typesEnum.WashCreditsRollOver;
                case 3:
                    return enquiry_refund_typesenquiry_refund_typesEnum.ReverseWash;
                case 4:
                    return enquiry_refund_typesenquiry_refund_typesEnum.BalanceRefund;
                //case 5:
                //    return Enums.RefundTypeEnum.DishonourPayment;
                case 6:
                    return enquiry_refund_typesenquiry_refund_typesEnum.ManualAdjustment;
                default:
                    return enquiry_refund_typesenquiry_refund_typesEnum.ManualAdjustment;
            }
        }

        internal Enums.RefundTypeEnum CovertRefundTypeEnumToInternalEnum(DNCROrderServices.enquiry_refund_typesenquiry_refund_typesEnum selectedEnumValue)
        {
            switch ((int)selectedEnumValue)
            {
                case 1:
                    return Enums.RefundTypeEnum.SubscriptionRefund;
                case 2:
                    return Enums.RefundTypeEnum.WashCreditsRollOver;
                case 3:
                    return Enums.RefundTypeEnum.ReverseWash;
                case 4:
                    return Enums.RefundTypeEnum.BalanceRefund;
                //case 5:
                //    return Enums.RefundTypeEnum.DishonourPayment;
                case 6:
                    return Enums.RefundTypeEnum.ManualAdjustment;
                default:
                    return Enums.RefundTypeEnum.ManualAdjustment;
            }
        }

        internal Enums.RefundTypeEnum CovertRefundTypeEnumToInternalEnum(DNCRServicesCMS.enquiry_refund_typesenquiry_refund_typesEnum selectedEnumValue)
        {
            switch ((int)selectedEnumValue)
            {
                case 1:
                    return Enums.RefundTypeEnum.SubscriptionRefund;
                case 2:
                    return Enums.RefundTypeEnum.WashCreditsRollOver;
                case 3:
                    return Enums.RefundTypeEnum.ReverseWash;
                case 4:
                    return Enums.RefundTypeEnum.BalanceRefund;
                //case 5:
                //    return Enums.RefundTypeEnum.DishonourPayment;
                case 6:
                    return Enums.RefundTypeEnum.ManualAdjustment;
                default:
                    return Enums.RefundTypeEnum.ManualAdjustment;
            }
        }

        internal Enums.RefundDecisionEnum CovertRefundDecisionEnumToInternalEnum(DNCRServicesCMS.enquiry_refund_decisionsenquiry_refund_decisionsEnum selectedEnumValue)
        {
            if (selectedEnumValue.ToString() == SD.ACMA.DNCR.Infrastructure.Enums.RefundDecisionEnum.Approved.ToString())
                return SD.ACMA.DNCR.Infrastructure.Enums.RefundDecisionEnum.Approved;

            if (selectedEnumValue.ToString() == SD.ACMA.DNCR.Infrastructure.Enums.RefundDecisionEnum.Denied.ToString())
                return SD.ACMA.DNCR.Infrastructure.Enums.RefundDecisionEnum.Denied;

            return SD.ACMA.DNCR.Infrastructure.Enums.RefundDecisionEnum.InProgress;
        }

        internal Enums.EnquryStatusEnum CovertEnquiryStatusEnumToInternalEnum(DNCRServicesCMS.enquiry_statusesenquiry_statusesEnum selectedEnumValue)
        {
            if (selectedEnumValue.ToString() == SD.ACMA.DNCR.Infrastructure.Enums.EnquryStatusEnum.Open.ToString())
                return SD.ACMA.DNCR.Infrastructure.Enums.EnquryStatusEnum.Open;

            if (selectedEnumValue.ToString() == SD.ACMA.DNCR.Infrastructure.Enums.EnquryStatusEnum.Closed.ToString())
                return SD.ACMA.DNCR.Infrastructure.Enums.EnquryStatusEnum.Closed;

            if (selectedEnumValue.ToString() == SD.ACMA.DNCR.Infrastructure.Enums.EnquryStatusEnum.Deregistered.ToString())
                return SD.ACMA.DNCR.Infrastructure.Enums.EnquryStatusEnum.Deregistered;

            if (selectedEnumValue.ToString() == SD.ACMA.DNCR.Infrastructure.Enums.EnquryStatusEnum.OrganisationOwnerChanged.ToString())
                return SD.ACMA.DNCR.Infrastructure.Enums.EnquryStatusEnum.OrganisationOwnerChanged;

            if (selectedEnumValue.ToString() == SD.ACMA.DNCR.Infrastructure.Enums.EnquryStatusEnum.UserOwnerChanged.ToString())
                return SD.ACMA.DNCR.Infrastructure.Enums.EnquryStatusEnum.UserOwnerChanged;

            return SD.ACMA.DNCR.Infrastructure.Enums.EnquryStatusEnum.AwaitingFurtherInformation;
        }

        
    }
}
