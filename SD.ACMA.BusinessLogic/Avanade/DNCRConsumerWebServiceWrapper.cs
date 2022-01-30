using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using SD.ACMA.BusinessLogic.DNCRAccessSeekerServices;
using SD.ACMA.BusinessLogic.DNCRServicesCMS;
using SD.ACMA.BusinessLogic.DNCRServicesRegistration;
using SD.ACMA.DNCR.Infrastructure.Configuration;
using SD.ACMA.InterfaceTier;
using SD.ACMA.POCO.Consumer;
using SD.ACMA.POCO.Industry;

namespace SD.ACMA.BusinessLogic.Avanade
{
    public class DNCRConsumerWebServiceWrapper : BaseDNCRWebServiceWrapper, IConsumerDataInterchange
    {
        private IFileHelper _fileHelper;

        public DNCRConsumerWebServiceWrapper(ISiteLoggingService siteLoggingService, IFileHelper fileHelper)
            : base(siteLoggingService)
        {
            _fileHelper = fileHelper;
        }

        #region Registration

        public RegistrationResult Register(Registration reg)
        {
            return BaseRegister(reg);
        }

        public RegistrationResult RegisterGovtOrBusinessFax(Registration reg)
        {
            return BaseRegister(reg);
        }

        private RegistrationResult BaseRegister(Registration reg)
        {
            var r = new RegistrationResult();

            try
            {
                using (var onlineRegistrationService = new OnlineRegistrationServiceClient())
                {
                    var args = new OnlineRegisterRequestArgs
                    {
                        ContactPhone = reg.ContactNumber,
                        EmailAddress = reg.Email,
                        FirstName = reg.FirstName,
                        LastName = reg.LastName,
                        OrganisationName = string.IsNullOrEmpty(reg.OrganisationName) ? string.Empty : reg.OrganisationName
                    };

                    if (reg.Numbers != null && reg.Numbers.Count > 0)
                    {
                        string[] phoneNumberArray = new string[reg.Numbers.Count];
                        for (int i = 0; i < reg.Numbers.Count; i++)
                        {
                            phoneNumberArray[i] = reg.Numbers.ElementAt(i);
                        }
                        args.PhoneNumbers = phoneNumberArray;
                    }

                    if (reg.FaxNumbers != null && reg.FaxNumbers.Count > 0)
                    {
                        string[] faxNumberArray = new string[reg.FaxNumbers.Count];
                        for (int i = 0; i < reg.FaxNumbers.Count; i++)
                        {
                            faxNumberArray[i] = reg.FaxNumbers.ElementAt(i);
                        }
                        args.FaxNumbers = faxNumberArray;
                    }

                    var result = onlineRegistrationService.Register(args);
                    r.HasEmailSent = result.HasEmailSent;
                    r.RegistrationRequestID = result.RegistrationRequestID;

                    return r;
                }
            }
            catch (FaultException<SD.ACMA.BusinessLogic.DNCRServicesRegistration.DncrServiceFault> e)
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

        public RegistrationRequestResult CheckRegistration(string email, List<string> numbers)
        {
            var r = new RegistrationRequestResult();

            try
            {
                using (var onlineRegistrationService = new OnlineRegistrationServiceClient())
                {
                    var args = new CheckOnlineRegisterRequestArgs
                    {
                        EmailAddress = email
                    };

                    if (numbers != null)
                    {
                        string[] phoneNumberArray = new string[numbers.Count];
                        for (int i = 0; i < numbers.Count; i++)
                        {
                            phoneNumberArray[i] = numbers.ElementAt(i);
                        }
                        args.Numbers = phoneNumberArray;
                    }

                    var result = onlineRegistrationService.Check(args);
                    r.HasEmailSent = result.HasEmailSent;

                    return r;
                }
            }
            catch (FaultException<SD.ACMA.BusinessLogic.DNCRServicesRegistration.DncrServiceFault> e)
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

        public RegistrationConfirmation ConfirmRegistration(string activationToken)
        {
            var returnObject = new RegistrationConfirmation();

            try
            {
                using (var onlineRegistrationService = new OnlineRegistrationServiceClient())
                {
                    ConfirmOnlineRegistrationRequestArgs args = new ConfirmOnlineRegistrationRequestArgs
                    {
                        ActivationToken = activationToken
                    };

                    var result = onlineRegistrationService.Confirm(args);

                    returnObject.IsSuccessful = result.IsSuccessful;
                    returnObject.IsTokenConsumed = result.IsTokenConsumed;
                    returnObject.IsTokenExpired = result.IsTokenExpired;
                    returnObject.RegisteredNumberCount = result.RegisteredNumberCount;

                    return returnObject;
                }
            }
            catch (FaultException<SD.ACMA.BusinessLogic.DNCRServicesRegistration.DncrServiceFault> e)
            {
                returnObject.Errors = ExtractErrorsFromException(e);
                return returnObject;
            }
            catch (Exception ex)
            {
                returnObject.Errors = ExtractErrorsFromException(ex);
                return returnObject;
            }
        }

        public BulkRegistrationResponse BulkRegistration(BulkRegistration bulkRegistration)
        {
            var response = new BulkRegistrationResponse();

            try
            {
                var args = new BulkRegistrationRequestArgs
                {
                    AddressLine1 = bulkRegistration.AddressLine1,
                    AddressLine2 = string.IsNullOrEmpty(bulkRegistration.AddressLine2) ? null : bulkRegistration.AddressLine2,
                    CompanyName = string.IsNullOrEmpty(bulkRegistration.OrganisationName) ? null : bulkRegistration.OrganisationName,
                    Country = bulkRegistration.Country,
                    EmailAddress = bulkRegistration.Email,
                    FirstName = bulkRegistration.FirstName,
                    LastName = bulkRegistration.LastName,
                    PostCode = bulkRegistration.Postcode,
                    State = bulkRegistration.State,
                    Suburb = bulkRegistration.City,
                    PreferredContactMethod = ConvertInternalPreferredContactMethodEnumToWSEnum(bulkRegistration.PreferredContactMethod),
                    OperationType = ConvertInternalOperationTypeEnumToWSEnum(bulkRegistration.OperationType),
                    EvidenceFile = bulkRegistration.EvidenceFile ?? null,
                    FaxNumbersFile = bulkRegistration.FaxNumbersFile ?? null,
                    NumbersFile = bulkRegistration.PhoneNumbersFile,
                };

                using (var onlineRegistrationService = new OnlineRegistrationServiceClient())
                {
                    var result = onlineRegistrationService.SubmitBulkRegistrationRequest(args);
                    response.IsSuccessful = result.IsSuccessful;
                    response.RegistrationRequestID = result.RequestID;
                    response.HasErrorsInFax = result.HasErrorsInFax;
                    response.HasErrorsInPhone = result.HasErrorsInPhone;
                    response.ErrorsInFaxFile = result.ErrorsInFaxFile != null ? result.ErrorsInFaxFile : null;
                    response.ErrorsInPhoneFile = result.ErrorsInPhoneFile != null ? result.ErrorsInPhoneFile : null;
                }

                if (response.HasErrorsInFax || response.HasErrorsInPhone)
                {
                    //File name format will be: RegistrationRequestID_Phone/Fax_YYYY_MM_DD_Min_Sec
                    var currentTime = DateTime.Now;

                    if (response.HasErrorsInFax)
                    {
                        var fileName = _fileHelper.GenerateFileName(response.RegistrationRequestID, "Fax", currentTime);
                        response.FaxErrorFileLocation = _fileHelper.WriteByteArrayToFile(response.ErrorsInFaxFile,
                            fileName, ConfigurationHelper.Instance.BulkFileDownloadLocation, ".txt");
                        response.FaxErrorFileName = fileName;
                    }

                    if (response.HasErrorsInPhone)
                    {
                        var fileName = _fileHelper.GenerateFileName(response.RegistrationRequestID, "Phone", currentTime);
                        response.PhoneErrorFileLocation = _fileHelper.WriteByteArrayToFile(response.ErrorsInPhoneFile,
                            fileName, ConfigurationHelper.Instance.BulkFileDownloadLocation, ".txt");
                        response.PhoneErrorFileName = fileName;
                    }
                }

                return response;
            }
            catch (FaultException<SD.ACMA.BusinessLogic.DNCRServicesRegistration.DncrServiceFault> e)
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

        private BulkRegistrationRequestTypeBulkRegistrationRequestTypeEnum ConvertInternalOperationTypeEnumToWSEnum(SD.ACMA.DNCR.Infrastructure.Enums.OperationTypeEnum selectedEnumValue)
        {

            switch ((int)selectedEnumValue)
            {
                case 1:
                    return BulkRegistrationRequestTypeBulkRegistrationRequestTypeEnum.Register;
                case 2:
                    return BulkRegistrationRequestTypeBulkRegistrationRequestTypeEnum.Deregister;
                case 3:
                    return BulkRegistrationRequestTypeBulkRegistrationRequestTypeEnum.Check;

                default:
                    return BulkRegistrationRequestTypeBulkRegistrationRequestTypeEnum.Register;
            }
        }

        private RegistrationRequestAccountContactPreferenceRegistrationRequestAccountContactPreferenceEnum ConvertInternalPreferredContactMethodEnumToWSEnum(
            SD.ACMA.DNCR.Infrastructure.Enums.PreferredContactMethodEnum selectedEnumValue)
        {

            switch ((int)selectedEnumValue)
            {
                case 1:
                    return RegistrationRequestAccountContactPreferenceRegistrationRequestAccountContactPreferenceEnum.Email;
                case 2:
                    return RegistrationRequestAccountContactPreferenceRegistrationRequestAccountContactPreferenceEnum.Phone;
                case 3:
                    return RegistrationRequestAccountContactPreferenceRegistrationRequestAccountContactPreferenceEnum.Post;
                default:
                    return RegistrationRequestAccountContactPreferenceRegistrationRequestAccountContactPreferenceEnum.Email;
            }
        }

        #endregion
    }
}
