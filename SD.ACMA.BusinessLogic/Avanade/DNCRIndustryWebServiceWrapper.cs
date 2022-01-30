using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel;
using SD.ACMA.BusinessLogic.DNCRAccessSeekerServices;
using SD.ACMA.BusinessLogic.DNCROrderServices;
using SD.ACMA.BusinessLogic.DNCRServicesCMS;
using SD.ACMA.BusinessLogic.DNCRWashingService;
using SD.ACMA.DNCR.Infrastructure;
using SD.ACMA.InterfaceTier;
using SD.ACMA.POCO.Base;
using SD.ACMA.POCO.Industry;
using enquiry_refund_decisionsenquiry_refund_decisionsEnum = SD.ACMA.BusinessLogic.DNCROrderServices.enquiry_refund_decisionsenquiry_refund_decisionsEnum;
using OrganisationGroupOrganisationGroupEnum = SD.ACMA.BusinessLogic.DNCRAccessSeekerServices.OrganisationGroupOrganisationGroupEnum;
using OrganisationOrganisationEnum = SD.ACMA.BusinessLogic.DNCRAccessSeekerServices.OrganisationOrganisationEnum;

namespace SD.ACMA.BusinessLogic.Avanade
{
    public class DNCRIndustryWebServiceWrapper : BaseDNCRWebServiceWrapper, IIndustryDataInterchange
    {
        public DNCRIndustryWebServiceWrapper(ISiteLoggingService siteLoggingService)
            : base(siteLoggingService)
        {

        }

        #region Access Seeker

        public CreateAccountResponse CreateAccount(CreateAccountModel newAccount)
        {
            var r = new CreateAccountResponse();

            try
            {
                Account acc = new Account
                {
                    ABN = newAccount.Account_Model.ABN,
                    AddressLine1 = newAccount.Account_Model.AddressLine1,
                    AddressLine2 = !string.IsNullOrEmpty(newAccount.Account_Model.AddressLine2) ? newAccount.Account_Model.AddressLine2 : string.Empty,
                    BusinessName = newAccount.Account_Model.OrganisationName,
                    CityOrLocality = newAccount.Account_Model.City,
                    Country = newAccount.Account_Model.Country,
                    FaxNumber = !string.IsNullOrEmpty(newAccount.Account_Model.FaxNumber) ? newAccount.Account_Model.FaxNumber : string.Empty,
                    HasABN = newAccount.Account_Model.HasABN,
                    HasACN = newAccount.Account_Model.HasACN,
                    Phone1 = newAccount.Account_Model.PhoneNumber,
                    PostCode = newAccount.Account_Model.Postcode,
                    State = newAccount.Account_Model.State,
                    WashTransactionPreferenceIsDailySummary = newAccount.Account_Model.IsDailySummary
                };

                if (!string.IsNullOrEmpty(newAccount.Account_Model.Industry))
                {
                    acc.BusinessSector = MapIndustryToIndustrySector(newAccount.Account_Model.Industry);
                }

                if (!string.IsNullOrEmpty(newAccount.Account_Model.PrincipalActivity))
                {
                    acc.BusinessType = MapPrincipalActivityToBusinessType(newAccount.Account_Model.PrincipalActivity);
                }

                var admin = new AccountUser
                {
                    EmailAddress = newAccount.Account_User_Model.EmailAddress,
                    FirstName = newAccount.Account_User_Model.FirstName,
                    LastName = newAccount.Account_User_Model.LastName,
                    PhoneNumber1 = newAccount.Account_User_Model.PhoneNumber,
                    WashResultFormatFileWithIndicators = newAccount.WashFormat.WashResultFormatFileWithIndicators,
                    WashResultFormatInvalidNumbers = newAccount.WashFormat.WashResultFormatInvalidNumbers,
                    WashResultFormatRegisteredNumbers = newAccount.WashFormat.WashResultFormatRegisteredNumbers,
                    WashResultFormatUnregisteredNumbers = newAccount.WashFormat.WashResultFormatUnregisteredNumbers,
                    Department = newAccount.Account_Model.CompanyDepartment,
                    CanSeeWashQuote = true //25/05/2015 - Lemuel: hardcoded to true as per phone conference agreement                  
                };

                var args = new CreateAccountRequestArgs
                {
                    AccountDetails = acc,
                    AdminDetails = admin,
                };

                using (var onlineAccountServiceClient = new OnlineAccountServiceClient())
                {
                    var result = onlineAccountServiceClient.CreateAccount(args);

                    r.AccountId = result.AccountId;
                    r.AccountUserId = result.AccountUserId;
                    r.IsAccountPendingApproval = result.IsAccountPendingApproval;
                    r.IsDuplicateAccount = result.IsDuplicateAccount;
                    r.IsSuccessful = result.IsSuccessful;

                    return r;
                }
            }
            catch (FaultException<DNCRAccessSeekerServices.DncrServiceFault> e)
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

        /// <summary>
        /// Defaults to AccountBusinessTypeAccountBusinessTypeEnum.Telemarketing if principalActivity is not found
        /// </summary>
        /// <param name="principalActivity"></param>
        /// <returns></returns>
        private AccountBusinessTypeAccountBusinessTypeEnum MapPrincipalActivityToBusinessType(string principalActivity)
        {
            if (principalActivity == "telephoning")
            {
                return AccountBusinessTypeAccountBusinessTypeEnum.Telemarketing;
            }
            else if (principalActivity == "sending faxes")
            {
                return AccountBusinessTypeAccountBusinessTypeEnum.FaxMarketing;
            }
            else if (principalActivity == "telephoning and sending faxes")
            {
                return AccountBusinessTypeAccountBusinessTypeEnum.Both;
            }
            else
            {
                return AccountBusinessTypeAccountBusinessTypeEnum.Telemarketing;
            }
        }

        /// <summary>
        /// Defaults to telephoning if AccountBusinessTypeAccountBusinessTypeEnum <> Telemarketing, FaxMarketing or Both
        /// </summary>
        /// <param name="businessType"></param>
        /// <returns></returns>
        private string MapPrincipalActivityToBusinessType(AccountBusinessTypeAccountBusinessTypeEnum businessType)
        {
            if (businessType == AccountBusinessTypeAccountBusinessTypeEnum.Telemarketing)
            {
                return "telephoning";
            }
            else if (businessType == AccountBusinessTypeAccountBusinessTypeEnum.FaxMarketing)
            {
                return "sending faxes";
            }
            else if (businessType == AccountBusinessTypeAccountBusinessTypeEnum.Both)
            {
                return "telephoning and sending faxes";
            }
            else
            {
                return "telephoning";
            }
        }

        /// <summary>
        /// Defaults to BusinessSectorBusinessSectorEnum.Person if industry is not matched up
        /// </summary>
        /// <param name="industry"></param>
        /// <returns></returns>
        private BusinessSectorBusinessSectorEnum MapIndustryToIndustrySector(string industry)
        {
            if (industry == "Commonwealth Department")
            {
                return BusinessSectorBusinessSectorEnum.CommonwealthDepartment;
            }
            else if (industry == "Other Commonwealth Agency")
            {
                return BusinessSectorBusinessSectorEnum.OtherCommonwealthAgency;
            }
            else if (industry == "State Government")
            {
                return BusinessSectorBusinessSectorEnum.StateGovernment;
            }
            else if (industry == "Local Government")
            {
                return BusinessSectorBusinessSectorEnum.LocalGovernment;
            }
            else if (industry == "Company")
            {
                return BusinessSectorBusinessSectorEnum.Company;
            }
            else if (industry == "Community or Volunteer Group")
            {
                return BusinessSectorBusinessSectorEnum.CommunityorVolunteerGroup;
            }
            else if (industry == "Person")
            {
                return BusinessSectorBusinessSectorEnum.Person;
            }
            else
            {
                return BusinessSectorBusinessSectorEnum.Person;
            }
        }

        /// <summary>
        /// Returns "Person" if BusinessSectorBusinessSectorEnum is not mapped
        /// </summary>
        /// <param name="industrySector"></param>
        /// <returns></returns>
        private string MapIndustrySectorToIndustry(BusinessSectorBusinessSectorEnum industrySector)
        {
            if (industrySector == BusinessSectorBusinessSectorEnum.CommonwealthDepartment)
            {
                return "Commonwealth Department";
            }
            else if (industrySector == BusinessSectorBusinessSectorEnum.OtherCommonwealthAgency)
            {
                return "Other Commonwealth Agency";
            }
            else if (industrySector == BusinessSectorBusinessSectorEnum.StateGovernment)
            {
                return "State Government";
            }
            else if (industrySector == BusinessSectorBusinessSectorEnum.LocalGovernment)
            {
                return "Local Government";
            }
            else if (industrySector == BusinessSectorBusinessSectorEnum.Company)
            {
                return "Company";
            }
            else if (industrySector == BusinessSectorBusinessSectorEnum.CommunityorVolunteerGroup)
            {
                return "Community or Volunteer Group";
            }
            else if (industrySector == BusinessSectorBusinessSectorEnum.Person)
            {
                return "Person";
            }
            else
            {
                return "Person";
            }
        }

        public GenericResponseModel CreateWashOnlyUser(CreateWashOnlyUserModel washOnlyUser, UserContextModel userContext)
        {
            var r = new GenericResponseModel();
            try
            {
                using (var onlineAccountServiceClient = new OnlineAccountServiceClient())
                {
                    CreateWashOnlyUserRequestArgs args = new CreateWashOnlyUserRequestArgs
                    {
                        AccountId = washOnlyUser.AccountUserModel.AccountID,
                        UserContext = new DNCRAccessSeekerServices.OnlineUserContext
                        {
                            AccountUserID = userContext.AccountUserID,
                            AgentID = userContext.AgentID
                        },
                        AccountUser = new AccountUser
                        {
                            CanSeeWashQuote = washOnlyUser.AccountUserModel.CanSeeWashQuote,
                            Department = !string.IsNullOrEmpty(washOnlyUser.AccountUserModel.CompanyDepartment) ? washOnlyUser.AccountUserModel.CompanyDepartment : string.Empty,
                            EmailAddress = washOnlyUser.AccountUserModel.Email,
                            FirstName = washOnlyUser.AccountUserModel.FirstName,
                            LastName = washOnlyUser.AccountUserModel.LastName,
                            PhoneNumber1 = washOnlyUser.AccountUserModel.PhoneNumber,
                            PhoneNumber2 = !string.IsNullOrEmpty(washOnlyUser.AccountUserModel.PhoneNumber2) ? washOnlyUser.AccountUserModel.PhoneNumber2 : string.Empty,
                            WashResultFormatFileWithIndicators = washOnlyUser.WashFormat.WashResultFormatFileWithIndicators,
                            WashResultFormatInvalidNumbers = washOnlyUser.WashFormat.WashResultFormatInvalidNumbers,
                            WashResultFormatRegisteredNumbers = washOnlyUser.WashFormat.WashResultFormatRegisteredNumbers,
                            WashResultFormatUnregisteredNumbers = washOnlyUser.WashFormat.WashResultFormatUnregisteredNumbers,
                        }
                    };

                    var result = onlineAccountServiceClient.CreateWashOnlyUser(args);

                    r.IsSuccessful = result.IsSuccessful;
                    r.Message = result.ResponseMessage;
                    return r;
                }
            }
            catch (FaultException<DNCRAccessSeekerServices.DncrServiceFault> e)
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

        private AccountBusinessTypeAccountBusinessTypeEnum ConvertInternalBusinessTypeEnumToWSEnum(Enums.BusinessTypeEnum selectedEnumValue)
        {

            switch ((int)selectedEnumValue)
            {
                case 1:
                    return AccountBusinessTypeAccountBusinessTypeEnum.Telemarketing;
                case 2:
                    return AccountBusinessTypeAccountBusinessTypeEnum.FaxMarketing;
                case 3:
                    return AccountBusinessTypeAccountBusinessTypeEnum.Both;
                default:
                    return AccountBusinessTypeAccountBusinessTypeEnum.Telemarketing;
            }
        }

        private Enums.BusinessTypeEnum ConvertWSBusinessTypeEnumToInternalEnum(AccountBusinessTypeAccountBusinessTypeEnum selectedEnumValue)
        {

            switch ((int)selectedEnumValue)
            {
                case 1:
                    return Enums.BusinessTypeEnum.Telemarketing;
                case 2:
                    return Enums.BusinessTypeEnum.FaxMarketing;
                case 3:
                    return Enums.BusinessTypeEnum.Both;
                default:
                    return Enums.BusinessTypeEnum.Telemarketing;
            }
        }

        private Enums.AccountStatusEnum? ConvertAccessSeekerWSAccountStatusEnumToInternalAccountStatusEnum(DNCRAccessSeekerServices.AccountStatusAccountStatusEnum selectedEnumValue)
        {
            if (selectedEnumValue.ToString() == Enums.AccountStatusEnum.Active.ToString())
                return Enums.AccountStatusEnum.Active;

            if (selectedEnumValue.ToString() == Enums.AccountStatusEnum.Closed.ToString())
                return Enums.AccountStatusEnum.Closed;

            if (selectedEnumValue.ToString() == Enums.AccountStatusEnum.ClosedRejected.ToString())
                return Enums.AccountStatusEnum.ClosedRejected;

            if (selectedEnumValue.ToString() == Enums.AccountStatusEnum.Pending.ToString())
                return Enums.AccountStatusEnum.Pending;

            if (selectedEnumValue.ToString() == Enums.AccountStatusEnum.PendingApproval.ToString())
                return Enums.AccountStatusEnum.PendingApproval;

            if (selectedEnumValue.ToString() == Enums.AccountStatusEnum.Suspended.ToString())
                return Enums.AccountStatusEnum.Suspended;

            return null;
        }

        private Enums.AccountStatusEnum? ConvertOrderWSAccountStatusEnumToInternalAccountStatusEnum(DNCROrderServices.AccountStatusAccountStatusEnum selectedEnumValue)
        {
            if (selectedEnumValue.ToString() == Enums.AccountStatusEnum.Active.ToString())
                return Enums.AccountStatusEnum.Active;

            if (selectedEnumValue.ToString() == Enums.AccountStatusEnum.Closed.ToString())
                return Enums.AccountStatusEnum.Closed;

            if (selectedEnumValue.ToString() == Enums.AccountStatusEnum.ClosedRejected.ToString())
                return Enums.AccountStatusEnum.ClosedRejected;

            if (selectedEnumValue.ToString() == Enums.AccountStatusEnum.Pending.ToString())
                return Enums.AccountStatusEnum.Pending;

            if (selectedEnumValue.ToString() == Enums.AccountStatusEnum.PendingApproval.ToString())
                return Enums.AccountStatusEnum.PendingApproval;

            if (selectedEnumValue.ToString() == Enums.AccountStatusEnum.Suspended.ToString())
                return Enums.AccountStatusEnum.Suspended;

            return null;
        }

        private Enums.AccountUserStatusEnum ConvertAccessSeekerWSAccountUserStatusEnumToInternalAccountUserStatusEnum(DNCRAccessSeekerServices.AccountUserStatusAccountUserStatusEnum selectedEnumValue)
        {

            switch ((int)selectedEnumValue)
            {
                case 1:
                    return Enums.AccountUserStatusEnum.Pending;
                case 2:
                    return Enums.AccountUserStatusEnum.Active;
                case 3:
                    return Enums.AccountUserStatusEnum.Inactive;
                case 4:
                    return Enums.AccountUserStatusEnum.Deleted;
                default:
                    return Enums.AccountUserStatusEnum.Pending;
            }
        }

        private Enums.AccountUserStatusEnum ConvertOrderWSAccountUserStatusEnumToInternalAccountUserStatusEnum(DNCROrderServices.AccountUserStatusAccountUserStatusEnum selectedEnumValue)
        {

            switch ((int)selectedEnumValue)
            {
                case 1:
                    return Enums.AccountUserStatusEnum.Pending;
                case 2:
                    return Enums.AccountUserStatusEnum.Active;
                case 3:
                    return Enums.AccountUserStatusEnum.Inactive;
                case 4:
                    return Enums.AccountUserStatusEnum.Deleted;
                default:
                    return Enums.AccountUserStatusEnum.Pending;
            }
        }

        //TODO: This needs to be updated once WS Enum is updated.
        private BusinessSectorBusinessSectorEnum ConvertInternalBusinessSectorEnumToWSEnum(Enums.BusinessSectorEnum selectedEnumValue)
        {
            switch ((int)selectedEnumValue)
            {
                case 1:
                    return BusinessSectorBusinessSectorEnum.CommonwealthDepartment;
                case 2:
                    return BusinessSectorBusinessSectorEnum.OtherCommonwealthAgency;
                case 3:
                    return BusinessSectorBusinessSectorEnum.StateGovernment;
                case 4:
                    return BusinessSectorBusinessSectorEnum.LocalGovernment;
                case 5:
                    return BusinessSectorBusinessSectorEnum.Company;
                case 6:
                    return BusinessSectorBusinessSectorEnum.CommunityorVolunteerGroup;
                case 7:
                    return BusinessSectorBusinessSectorEnum.Person;
                default:
                    return BusinessSectorBusinessSectorEnum.Company;
            }
        }

        private Enums.BusinessSectorEnum ConvertWSBusinessSectorEnumToInternalEnum(BusinessSectorBusinessSectorEnum selectedEnumValue)
        {

            switch ((int)selectedEnumValue)
            {
                case 1:
                    return Enums.BusinessSectorEnum.CommonwealthDepartment;
                case 2:
                    return Enums.BusinessSectorEnum.OtherCommonwealthAgency;
                case 3:
                    return Enums.BusinessSectorEnum.StateGovernment;
                case 4:
                    return Enums.BusinessSectorEnum.LocalGovernment;
                case 5:
                    return Enums.BusinessSectorEnum.Company;
                case 6:
                    return Enums.BusinessSectorEnum.CommunityorVolunteerGroup;
                case 7:
                    return Enums.BusinessSectorEnum.Person;
                default:
                    return Enums.BusinessSectorEnum.Company;
            }
        }

        public GetAccountResponse GetAccount(int accountID, UserContextModel userContext)
        {
            var r = new GetAccountResponse();

            try
            {
                GetAccountRequestArgs args = new GetAccountRequestArgs
                {
                    AccountId = accountID,
                    UserContext = new DNCRAccessSeekerServices.OnlineUserContext { AccountUserID = userContext.AccountUserID, AgentID = userContext.AgentID }
                };

                using (var onlineAccountServiceClient = new OnlineAccountServiceClient())
                {
                    var result = onlineAccountServiceClient.GetAccount(args);

                    if (result != null)
                    {
                        var am = new AccountModel
                        {
                            ABN = result.AccountDetails.ABN,
                            HasABN = result.AccountDetails.HasABN,
                            ACN = result.AccountDetails.ACN,
                            AddressLine1 = result.AccountDetails.AddressLine1,
                            AddressLine2 = result.AccountDetails.AddressLine2,
                            City = result.AccountDetails.CityOrLocality,
                            FaxNumber = result.AccountDetails.FaxNumber,
                            PhoneNumber = result.AccountDetails.Phone1,
                            Postcode = result.AccountDetails.PostCode,
                            State = result.AccountDetails.State,
                            IsDailySummary = result.AccountDetails.WashTransactionPreferenceIsDailySummary,
                            //BusinessType = ConvertWSBusinessTypeEnumToInternalEnum(result.AccountDetails.BusinessType),
                            PrincipalActivity = MapPrincipalActivityToBusinessType(result.AccountDetails.BusinessType),
                            OrganisationName = result.AccountDetails.BusinessName,
                            Country = result.AccountDetails.Country,
                            WashingCredits50Count = result.AccountDetails.WashingCredits50Count,
                            WashingCredits20Count = result.AccountDetails.WashingCredits80Count
                        };                        

                        if (result.AccountDetails.BusinessSector != null)
                        {
                            am.BusinessSector = ConvertWSBusinessSectorEnumToInternalEnum(result.AccountDetails.BusinessSector.Value);
                            am.Industry = MapIndustrySectorToIndustry(result.AccountDetails.BusinessSector.Value);
                        }

                        var aum = new AccountUserModel
                        {
                            EmailAddress = result.AdminAccountUser.EmailAddress,
                            FirstName = result.AdminAccountUser.FirstName,
                            CanSeeWashQuote = result.AdminAccountUser.CanSeeWashQuote,
                            LastName = result.AdminAccountUser.LastName,
                            MobileNumber = result.AdminAccountUser.PhoneNumber2,
                            PhoneNumber = result.AdminAccountUser.PhoneNumber1,
                            AccountUserId = result.AdminAccountUser.AccountUserId,
                            Department = result.AdminAccountUser.Department
                        };

                        var wf = new WashingFormat
                        {
                            WashResultFormatFileWithIndicators = result.AdminAccountUser.WashResultFormatFileWithIndicators,
                            WashResultFormatInvalidNumbers = result.AdminAccountUser.WashResultFormatInvalidNumbers,
                            WashResultFormatRegisteredNumbers = result.AdminAccountUser.WashResultFormatRegisteredNumbers,
                            WashResultFormatUnregisteredNumbers = result.AdminAccountUser.WashResultFormatUnregisteredNumbers
                        };

                        r.Account_Model = am;
                        r.AdminModel = aum;
                        r.WashFormat = wf;
                        return r;
                    }
                    else
                        return null;
                }
            }
            catch (FaultException<DNCRAccessSeekerServices.DncrServiceFault> e)
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

        public GetAccountUserResponse GetAccountUser(int accountUserID, UserContextModel userContext)
        {
            var r = new GetAccountUserResponse();

            try
            {
                using (var onlineAccountServiceClient = new OnlineAccountServiceClient())
                {
                    var args = new GetAccountUserRequestArgs
                    {
                        AccountUserID = accountUserID,
                        UserContext = new DNCRAccessSeekerServices.OnlineUserContext { AccountUserID = userContext.AccountUserID, AgentID = userContext.AgentID }
                    };

                    var result = onlineAccountServiceClient.GetAccountUser(args);

                    if (result != null)
                    {
                        r.CanSeeWashQuote = result.AccountUserDetails.CanSeeWashQuote;
                        r.Department = result.AccountUserDetails.Department;
                        r.EmailAddress = result.AccountUserDetails.EmailAddress;
                        r.FirstName = result.AccountUserDetails.FirstName;
                        r.LastName = result.AccountUserDetails.LastName;
                        r.MobileNumber = result.AccountUserDetails.PhoneNumber2;
                        r.PhoneNumber = result.AccountUserDetails.PhoneNumber1;

                        r.WashFormat = new WashingFormat
                        {
                            WashResultFormatFileWithIndicators = result.AccountUserDetails.WashResultFormatFileWithIndicators,
                            WashResultFormatInvalidNumbers = result.AccountUserDetails.WashResultFormatInvalidNumbers,
                            WashResultFormatRegisteredNumbers = result.AccountUserDetails.WashResultFormatRegisteredNumbers,
                            WashResultFormatUnregisteredNumbers = result.AccountUserDetails.WashResultFormatUnregisteredNumbers
                        };

                        r.ResponseModel = new GenericResponseModel
                        {
                            IsSuccessful = result.IsSuccessful,
                            Message = result.ResponseMessage
                        };

                        return r;
                    }
                    else
                        return null;
                }
            }
            catch (FaultException<DNCRAccessSeekerServices.DncrServiceFault> e)
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

        public LoginResponse Login(string email, string password)
        {
            var r = new LoginResponse();

            try
            {
                using (var onlineAccountServiceClient = new OnlineAccountServiceClient())
                {
                    var args = new LoginRequestArgs
                    {
                        EmailAddress = email,
                        Password = password
                    };

                    var result = onlineAccountServiceClient.Login(args);

                    r.AccountId = result.AccountId;
                    r.AccountUserId = result.AccountUserId;
                    r.IsAdmin = result.IsAdmin;
                    r.IsSuccessful = result.IsSuccessful;
                    r.LastLogedInDateTime = result.LastLogedInDateTime;
                    r.LoginMessage = result.LoginMessage;
                    r.IsLocked = result.IsLocked;
                    r.IsMigrated = result.IsMigrated;

                    return r;
                }
            }
            catch (FaultException<DNCRAccessSeekerServices.DncrServiceFault> e)
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

        public GetAllAccountUsersResponse GetAllAccountUsers(int accountId, UserContextModel userContext)
        {
            var r = new GetAllAccountUsersResponse();

            try
            {
                using (var onlineAccountServiceClient = new OnlineAccountServiceClient())
                {
                    var args = new GetAllAccountUsersRequestArgs
                    {
                        AccountId = accountId,
                        UserContext = new DNCRAccessSeekerServices.OnlineUserContext { AccountUserID = userContext.AccountUserID, AgentID = userContext.AgentID }
                    };

                    var result = onlineAccountServiceClient.GetAllAccountUsers(args);

                    r.ResponseModel = new GenericResponseModel
                    {
                        IsSuccessful = result.IsSuccessful,
                        Message = result.ResponseMessage
                    };

                    if (result.AccountUsers != null && result.AccountUsers.Any())
                    {
                        List<GenericAccountModel> accountList = result.AccountUsers.Select(item => new GenericAccountModel
                        {
                            Account_Model = new AccountModel
                            {
                                AccountID = item.AccountId,
                                CanSeeWashQuote = item.CanSeeWashQuote,
                                Email = item.EmailAddress,
                                FirstName = item.FirstName,
                                LastName = item.LastName,
                                MobileNumber = item.PhoneNumber2,
                                PhoneNumber = item.PhoneNumber1,
                                Status = ConvertAccessSeekerWSAccountUserStatusEnumToInternalAccountUserStatusEnum(item.Status),
                                AccountUserID = item.AccountUserId,
                                CompanyDepartment = item.Department
                            },
                            WashFormat = new WashingFormat
                            {
                                WashResultFormatFileWithIndicators = item.WashResultFormatFileWithIndicators,
                                WashResultFormatInvalidNumbers = item.WashResultFormatInvalidNumbers,
                                WashResultFormatRegisteredNumbers = item.WashResultFormatRegisteredNumbers,
                                WashResultFormatUnregisteredNumbers = item.WashResultFormatUnregisteredNumbers
                            }
                        }).ToList();

                        r.Accounts = accountList;
                    }

                    return r;
                }
            }
            catch (FaultException<DNCRAccessSeekerServices.DncrServiceFault> e)
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

        public GenericResponseModel ResetPassword(string email, string newPassword, string passwordToken)
        {
            var r = new GenericResponseModel();

            try
            {
                using (var onlineAccountServiceClient = new OnlineAccountServiceClient())
                {
                    var args = new ResetPasswordRequestArgs
                    {
                        EmailAddress = WebUtility.UrlEncode(email),
                        NewPassword = newPassword,
                        ResetPasswordToken = WebUtility.UrlEncode(passwordToken)
                    };

                    var result = onlineAccountServiceClient.ResetPassword(args);

                    r.IsSuccessful = result.IsSuccessful;
                    r.Message = result.Message;
                    return r;
                }
            }
            catch (FaultException<DNCRAccessSeekerServices.DncrServiceFault> e)
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

        public GenericResponseModel ForgotPassword(string email)
        {
            var r = new GenericResponseModel();

            try
            {
                using (var onlineAccountServiceClient = new OnlineAccountServiceClient())
                {
                    var args = new ForgotPasswordRequestArgs
                    {
                        EmailAddress = email
                    };

                    var result = onlineAccountServiceClient.ForgotPassword(args);

                    r.IsSuccessful = result.IsSuccessful;
                    r.Message = result.Message;
                    return r;
                }
            }
            catch (FaultException<DNCRAccessSeekerServices.DncrServiceFault> e)
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

        public GenericResponseModel ChangePassword(int accountUserId, string newPassword, string oldPassword)
        {
            var r = new GenericResponseModel();

            try
            {
                using (var onlineAccountServiceClient = new OnlineAccountServiceClient())
                {
                    var args = new ChangePasswordRequestArgs
                    {
                        AccountUserId = accountUserId,
                        OldPassword = oldPassword,
                        NewPassword = newPassword
                    };

                    var result = onlineAccountServiceClient.ChangePassword(args);

                    r.IsSuccessful = result.IsSuccessful;
                    r.Message = result.ChangePasswordMessage;
                    return r;
                }
            }
            catch (FaultException<DNCRAccessSeekerServices.DncrServiceFault> e)
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

        public ActivateAccountResponse ActivateAccount(string activationToken, string adminPassword, string adminEmail)
        {
            var r = new ActivateAccountResponse();

            try
            {
                using (var onlineAccountServiceClient = new OnlineAccountServiceClient())
                {
                    var args = new ActivateAccountRequestArgs
                    {
                        ActivationToken = WebUtility.UrlEncode(activationToken),
                        AdminPassword = adminPassword,
                        AdminEmailAddress = WebUtility.UrlEncode(adminEmail)
                    };

                    var result = onlineAccountServiceClient.ActivateAccount(args);

                    r.CannotActivate = result.CannotActivate;
                    r.IsExpired = result.IsExpired;
                    r.AccountId = result.AccountID;

                    r.GenericResponse = new GenericResponseModel
                    {
                        IsSuccessful = result.IsSuccessful,
                        Message = result.Message
                    };

                    return r;
                }
            }
            catch (FaultException<DNCRAccessSeekerServices.DncrServiceFault> e)
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

        public GenericResponseModel ActivateAccountUser(UserContextModel userContext, int accountUserId)
        {
            var r = new GenericResponseModel();

            try
            {
                using (var onlineAccountServiceClient = new OnlineAccountServiceClient())
                {
                    var args = new ActivateAccountUserRequestArgs
                    {
                        ActivateAccountUserId = accountUserId,
                        UserContext = new DNCRAccessSeekerServices.OnlineUserContext { AccountUserID = userContext.AccountUserID, AgentID = userContext.AgentID }
                    };

                    var result = onlineAccountServiceClient.ActivateAccountUser(args);

                    r.IsSuccessful = result.IsSuccessful;
                    r.Message = result.ResponseMessage;

                    return r;
                }
            }
            catch (FaultException<DNCRAccessSeekerServices.DncrServiceFault> e)
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

        public ActivateWashOnlyUserResponse ActivateWashOnlyUser(string email, string password, string activationToken)
        {
            var r = new ActivateWashOnlyUserResponse();

            try
            {
                using (var onlineAccountServiceClient = new OnlineAccountServiceClient())
                {
                    var args = new ActivateWashOnlyUserArgs
                    {
                        EmailAddress = WebUtility.UrlEncode(email),
                        Password = password,
                        ActivationToken = WebUtility.UrlEncode(activationToken)
                    };

                    var result = onlineAccountServiceClient.ActivateWashOnlyUser(args);

                    r.IsSuccessful = result.IsSuccessful;
                    r.Message = result.Message;
                    r.IsExpired = result.IsExpired;
                    r.AccountUserID = result.AccountUserID;

                    return r;
                }
            }
            catch (FaultException<DNCRAccessSeekerServices.DncrServiceFault> e)
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

        public GenericResponseModel DeActivateAccountUser(UserContextModel userContext, int deactivateAccountUserId)
        {
            var r = new GenericResponseModel();

            try
            {
                using (var onlineAccountServiceClient = new OnlineAccountServiceClient())
                {
                    var args = new DeActivateAccountUserRequestArgs
                    {
                        DeActivateAccountUserId = deactivateAccountUserId,
                        UserContext = new DNCRAccessSeekerServices.OnlineUserContext { AccountUserID = userContext.AccountUserID, AgentID = userContext.AgentID }
                    };

                    var result = onlineAccountServiceClient.DeActivateAccountUser(args);

                    r.IsSuccessful = result.IsSuccessful;
                    r.Message = result.ResponseMessage;
                    return r;
                }
            }
            catch (FaultException<DNCRAccessSeekerServices.DncrServiceFault> e)
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

        public GenericResponseModel UpdateAccount(UserContextModel userContext, UpdateAccountModel updateAccountModel)
        {
            var r = new GenericResponseModel();

            try
            {
                using (var onlineAccountServiceClient = new OnlineAccountServiceClient())
                {
                    var args = new UpdateAccountRequestArgs
                    {
                        UserContext = new DNCRAccessSeekerServices.OnlineUserContext { AccountUserID = userContext.AccountUserID, AgentID = userContext.AgentID },
                        AccountDetails = new Account
                        {
                            ABN = updateAccountModel.AccounDetails.ABN,
                            ACN = updateAccountModel.AccounDetails.ACN,
                            AddressLine1 = updateAccountModel.AccounDetails.AddressLine1,
                            AddressLine2 = updateAccountModel.AccounDetails.AddressLine2,
                            BusinessName = updateAccountModel.AccounDetails.OrganisationName,
                            CityOrLocality = updateAccountModel.AccounDetails.City,
                            Country = updateAccountModel.AccounDetails.Country,
                            FaxNumber = updateAccountModel.AccounDetails.FaxNumber,
                            HasABN = updateAccountModel.AccounDetails.HasABN,
                            HasACN = updateAccountModel.AccounDetails.HasACN,
                            Phone1 = updateAccountModel.AccounDetails.OrganisationPhoneNumber,
                            PostCode = updateAccountModel.AccounDetails.Postcode,
                            State = updateAccountModel.AccounDetails.State,
                            WebAddress = updateAccountModel.AccounDetails.OrganisationWebsite,
                            AccountId = updateAccountModel.AccounDetails.AccountID,
                            WashTransactionPreferenceIsDailySummary = updateAccountModel.AccounDetails.IsDailySummary,
                        },
                    };

                    if (!string.IsNullOrEmpty(updateAccountModel.AccounDetails.Industry))
                    {
                        args.AccountDetails.BusinessSector = MapIndustryToIndustrySector(updateAccountModel.AccounDetails.Industry);
                    }

                    if (!string.IsNullOrEmpty(updateAccountModel.AccounDetails.PrincipalActivity))
                    {
                        args.AccountDetails.BusinessType = MapPrincipalActivityToBusinessType(updateAccountModel.AccounDetails.PrincipalActivity);
                    }

                    var result = onlineAccountServiceClient.UpdateAccount(args);

                    r.IsSuccessful = result.IsSuccessful;
                    r.Message = result.UpdateMessage;

                    return r;
                }
            }
            catch (FaultException<DNCRAccessSeekerServices.DncrServiceFault> e)
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

        public GenericResponseModel UpdateAccountUser(UserContextModel userContext, UpdateAccountUserModel updateAccountUserModel)
        {
            var r = new GenericResponseModel();

            try
            {
                using (var onlineAccountServiceClient = new OnlineAccountServiceClient())
                {
                    var args = new UpdateAccountUserRequestArgs
                    {
                        AccountUserDetails = new AccountUser
                        {
                            CanSeeWashQuote = updateAccountUserModel.AccountUserModel.CanSeeWashQuote,
                            Department = updateAccountUserModel.AccountUserModel.CompanyDepartment,
                            EmailAddress = updateAccountUserModel.AccountUserModel.Email,
                            FirstName = updateAccountUserModel.AccountUserModel.FirstName,
                            LastName = updateAccountUserModel.AccountUserModel.LastName,
                            PhoneNumber1 = updateAccountUserModel.AccountUserModel.PhoneNumber,
                            PhoneNumber2 = !string.IsNullOrEmpty(updateAccountUserModel.AccountUserModel.PhoneNumber2) ? updateAccountUserModel.AccountUserModel.PhoneNumber2 : string.Empty,
                            WashResultFormatFileWithIndicators = updateAccountUserModel.WashFormat.WashResultFormatFileWithIndicators,
                            
                            AccountUserId = updateAccountUserModel.AccountUserModel.AccountUserID,

                        },
                        UserContext = new DNCRAccessSeekerServices.OnlineUserContext { AccountUserID = userContext.AccountUserID, AgentID = userContext.AgentID }
                    };

                    if (updateAccountUserModel.WashFormat.WashResultFormatFileWithIndicators)
                    {
                        args.AccountUserDetails.WashResultFormatInvalidNumbers = false;
                        args.AccountUserDetails.WashResultFormatRegisteredNumbers = false;
                        args.AccountUserDetails.WashResultFormatUnregisteredNumbers = false;
                    }
                    else
                    {
                        args.AccountUserDetails.WashResultFormatInvalidNumbers = updateAccountUserModel.WashFormat.WashResultFormatInvalidNumbers;
                        args.AccountUserDetails.WashResultFormatRegisteredNumbers = updateAccountUserModel.WashFormat.WashResultFormatRegisteredNumbers;
                        args.AccountUserDetails.WashResultFormatUnregisteredNumbers = updateAccountUserModel.WashFormat.WashResultFormatUnregisteredNumbers;
                    }

                    var result = onlineAccountServiceClient.UpdateAccountUser(args);

                    r.IsSuccessful = result.IsSuccessful;
                    r.Message = result.ResponseMessage;

                    return r;
                }
            }
            catch (FaultException<DNCRAccessSeekerServices.DncrServiceFault> e)
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

        public GenericResponseModel DeleteAccountUser(UserContextModel userContext, int deleteAccountUserId)
        {
            var r = new GenericResponseModel();

            try
            {
                using (var onlineAccountServiceClient = new OnlineAccountServiceClient())
                {
                    var args = new DeleteAccountUserRequestArgs
                    {
                        DeleteAccountUserId = deleteAccountUserId,
                        UserContext = new DNCRAccessSeekerServices.OnlineUserContext { AccountUserID = userContext.AccountUserID, AgentID = userContext.AgentID }
                    };

                    var result = onlineAccountServiceClient.DeleteAccountUser(args);

                    r.IsSuccessful = result.IsSuccessful;
                    r.Message = result.ResponseMessage;

                    return r;
                }
            }
            catch (FaultException<DNCRAccessSeekerServices.DncrServiceFault> e)
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

        public ImpersonateResponse Impersonate(int accountUserId, string agentData, string token)
        {
            var r = new ImpersonateResponse();

            try
            {
                using (var onlineAccountServiceClient = new OnlineAccountServiceClient())
                {
                    var args = new ImpersonateArgs
                    {
                        AccountUserId = accountUserId,
                        Token = !string.IsNullOrEmpty(token) ? token : string.Empty,
                        AgentData = !string.IsNullOrEmpty(agentData) ? agentData : string.Empty
                    };

                    var result = onlineAccountServiceClient.Impersonate(args);

                    r.AccountId = result.AccountId;
                    r.AccountUserId = result.AccountUserId;
                    r.AgentId = result.AgentId;
                    r.Message = result.ResponseMessage;
                    r.IsSuccessful = result.Successful;
                    r.IsAdmin = result.IsAdmin;
                    r.IsLocked = result.IsLocked;
                    if (result.Organisation != null)
                        r.Organisation = ConvertOrganisationEnumToInternalEnum(result.Organisation.Value);

                    if (result.Role != null)
                        r.Role = ConvertOrganisationGroupEnumToInternalEnum(result.Role.Value);

                    if(result.AccountUserStatus != null)
                        r.AccountUserStatus = ConvertAccessSeekerWSAccountUserStatusEnumToInternalAccountUserStatusEnum(result.AccountUserStatus.Value);

                    if(result.AccountStatus != null)
                        r.AccountStatus = ConvertAccessSeekerWSAccountStatusEnumToInternalAccountStatusEnum(result.AccountStatus.Value);
                    

                    return r;
                }
            }
            catch (FaultException<DNCRAccessSeekerServices.DncrServiceFault> e)
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

        public GetStatesResponse GetStates(int countryId, int? agentId)
        {
            var r = new GetStatesResponse();

            try
            {
                using (var onlineAccountServiceClient = new OnlineAccountServiceClient())
                {
                    var args = new GetStateRequestArgs
                    {
                        countryID = countryId
                    };

                    if (agentId != null)
                        args.AgentUserId = agentId.Value;

                    var result = onlineAccountServiceClient.GetStates(args);

                    if (result.States != null && result.States.Any())
                    {
                        List<StateModel> listStates = result.States.Select(item => new StateModel
                        {
                            StateCode = item.StateCode, 
                            StateID = item.stateID, 
                            StateName = item.stateName
                        }).ToList();

                        r.States = listStates;
                    }

                    return r;
                }
            }
            catch (FaultException<DNCRAccessSeekerServices.DncrServiceFault> e)
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

        private Enums.OrganisationEnum ConvertOrganisationEnumToInternalEnum(OrganisationOrganisationEnum selectedValue)
        {
            if (selectedValue.ToString() == "IVE")
                return Enums.OrganisationEnum.IVE;
            else if (selectedValue.ToString() == "ACMA")
                return Enums.OrganisationEnum.ACMA;
            else
                return Enums.OrganisationEnum.IVE;
        }

        private Enums.OrganisationGroupEnum ConvertOrganisationGroupEnumToInternalEnum(OrganisationGroupOrganisationGroupEnum selectedValue)
        {
            if (selectedValue.ToString() == "CSR")
                return Enums.OrganisationGroupEnum.CSR;
            else if (selectedValue.ToString() == "SeniorCSR")
                return Enums.OrganisationGroupEnum.SeniorCSR;
            else if (selectedValue.ToString() == "DNCR")
                return Enums.OrganisationGroupEnum.DNCR;
            else if (selectedValue.ToString() == "UCCS")
                return Enums.OrganisationGroupEnum.UCCS;
            else
                return Enums.OrganisationGroupEnum.CSR;
        }

        public GenericResponseModel SendAccountActivationEmail(string adminEmail)
        {
            var r = new GenericResponseModel();

            try
            {
                using (var onlineAccountServiceClient = new OnlineAccountServiceClient())
                {
                    var args = new SendAccountActivationEmailRequestArgs
                    {
                        AdminEmailAddress = adminEmail
                    };

                    var result = onlineAccountServiceClient.SendAccountActivationEmail(args);

                    r.IsSuccessful = result.IsSuccessful;
                    r.Message = result.Message;

                    return r;
                }
            }
            catch (FaultException<DNCRAccessSeekerServices.DncrServiceFault> e)
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



        #endregion

        #region Washing

        public DownloadErrorLogResponse DownloadErrorLog(int accountId, int washingRequestId, UserContextModel userContext)
        {
            var r = new DownloadErrorLogResponse();

            try
            {
                using (var client = new OnlineWashingServiceClient())
                {
                    var args = new DownloadErrorLogRequestArgs
                    {
                        AccountId = accountId,
                        WashingRequestId = washingRequestId,
                        User = new DNCRWashingService.OnlineUserContext { AccountUserID = userContext.AccountUserID, AgentID = userContext.AgentID }
                    };

                    var result = client.DownloadErrorLog(args);

                    r.FileName = result.FileName;
                    r.Content = result.Content;
                    r.ContentType = result.ContentType;

                    return r;
                }
            }
            catch (FaultException<DNCRWashingService.DncrServiceFault> e)
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

        private WashFileType ConvertInternalWashFileTypeEnumToWSEnum(Enums.WashFileTypeEnum selectedEnumValue)
        {
            switch ((int)selectedEnumValue)
            {
                case 0:
                    return WashFileType.Original;
                case 1:
                    return WashFileType.AllInOne;
                case 2:
                    return WashFileType.Registered;
                case 3:
                    return WashFileType.UnRegistered;
                case 4:
                    return WashFileType.Invalid;
                default:
                    return WashFileType.Original;
            }
        }

        private Enums.WashingStatusEnum ConvertWashingStatusToInternalEnum(WashingStatusWashingStatusEnum selectedEnumValue)
        {
            switch ((int)selectedEnumValue)
            {

                case 1:
                    return Enums.WashingStatusEnum.NotStarted;
                case 2:
                    return Enums.WashingStatusEnum.Processing;
                case 3:
                    return Enums.WashingStatusEnum.Complete;
                default:
                    return Enums.WashingStatusEnum.NotStarted;
            }
        }

        private WashingChannelWashingChannelEnum ConvertInternalWashingChannelEnumToWSEnum(Enums.WashingChannelEnum selectedEnumValue)
        {
            switch ((int)selectedEnumValue)
            {
                case 1:
                    return WashingChannelWashingChannelEnum.OnlineQuickCheck;
                case 2:
                    return WashingChannelWashingChannelEnum.OnlineFileUpload;
                case 3:
                    return WashingChannelWashingChannelEnum.AutomatedWashingService;
                case 4:
                    return WashingChannelWashingChannelEnum.RealTimeAccess;
                default:
                    return WashingChannelWashingChannelEnum.OnlineQuickCheck;
            }
        }

        private Enums.WashingChannelEnum ConvertWSEnumToWashingChannelWashingChannelEnum(WashingChannelWashingChannelEnum selectedEnumValue)
        {
            switch ((int)selectedEnumValue)
            {
                case 1:
                    return Enums.WashingChannelEnum.OnlineQuickCheck;
                case 2:
                    return Enums.WashingChannelEnum.OnlineFileUpload;
                case 3:
                    return Enums.WashingChannelEnum.AutomatedWashingService;
                case 4:
                    return Enums.WashingChannelEnum.RealTimeAccess;
                default:
                    return Enums.WashingChannelEnum.OnlineQuickCheck;
            }
        }

        public DownloadWashFileResponse DownloadWashFile(int accountId, int washingRequestId, Enums.WashFileTypeEnum washFileType, UserContextModel userContext)
        {
            var r = new DownloadWashFileResponse();

            try
            {
                using (var client = new OnlineWashingServiceClient())
                {
                    var args = new DownloadWashFileRequestArgs
                    {
                        AccountId = accountId,
                        WashingRequestId = washingRequestId,
                        User = new DNCRWashingService.OnlineUserContext { AccountUserID = userContext.AccountUserID, AgentID = userContext.AgentID },
                        WashFileType = ConvertInternalWashFileTypeEnumToWSEnum(washFileType)
                    };

                    var result = client.DownloadWashFile(args);

                    r.FileName = result.FileName;
                    r.Content = result.Content;
                    r.ContentType = result.ContentType;
                    r.IsExpired = result.IsExpired;

                    return r;
                }
            }
            catch (FaultException<DNCRWashingService.DncrServiceFault> e)
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

        public PreWashResponse PreWash(string clientReference, string relativeInputFilePath, UserContextModel userContext)
        {
            var r = new PreWashResponse();

            try
            {
                using (var client = new OnlineWashingServiceClient())
                {
                    var args = new PreWashFileOnlineArgs
                    {
                        ClientReference = clientReference,
                        RelativeInputFilePath = relativeInputFilePath,
                        UserContext = new DNCRWashingService.OnlineUserContext { AccountUserID = userContext.AccountUserID, AgentID = userContext.AgentID }
                    };

                    var result = client.PreWash(args);

                    r.FileNameAlreadyExists = result.FileNameAlreadyExists;
                    r.HasInValidNumbers = result.HasInValidNumbers;
                    r.HasSufficientWashingCredits = result.HasSufficientWashingCredits;
                    r.HasValidSubscription = result.HasValidSubscription;
                    r.IsFileFormatNotValid = result.IsFileFormatNotValid;
                    r.IsFileNameNotValid = result.IsFileNameNotValid;
                    r.IsFileSizeExceedLimit = result.IsFileSizeExceedLimit;
                    r.IsSuccessful = result.IsSuccessful;
                    r.RelativeInputFilePath = result.RelativeInputFilePath;
                    r.RequiredWashingCredits = result.RequiredWashingCredits;
                    r.ShowWashQuote = result.ShowWashQuote;
                    r.Timestamp = result.Timestamp;
                    r.WashingRequestId = result.WashingRequestId;
                    r.AccountWashingCreditsBalance = result.AccountWashingCreditsBalance;

                    return r;
                }
            }
            catch (FaultException<DNCRWashingService.DncrServiceFault> e)
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

        public QuickWashResponse QuickWash(int accountId, string clientReference, List<string> numbersToWash, UserContextModel userContext)
        {
            var r = new QuickWashResponse();

            try
            {
                using (var client = new OnlineWashingServiceClient())
                {
                    var args = new QuickWashRequestArgs
                    {
                        AccountId = accountId,
                        ClientReference = !string.IsNullOrEmpty(clientReference) ? clientReference : string.Empty,
                        UserContext = new DNCRWashingService.OnlineUserContext { AccountUserID = userContext.AccountUserID, AgentID = userContext.AgentID }
                    };

                    var numbersArray = new string[numbersToWash.Count];
                    var i = 0;
                    foreach (var item in numbersToWash)
                    {
                        numbersArray[i] = item;
                        i++;
                    }
                    args.Numbers = numbersArray;

                    var result = client.QuickWash(args);

                    r.HasInValidNumbers = result.HasInValidNumbers;
                    r.HasSufficientWashingCredits = result.HasSufficientWashingCredits;
                    r.HasValidSubscription = result.HasValidSubscription;
                    r.IsSuccessful = result.IsSuccessful;

                    if (result.Numbers != null)
                    {
                        var listNumbers = result.Numbers.Select(item => new WashNumberModel
                        {
                            Number = item.Number,
                            Flag = item.Flag
                        }).ToList();

                        r.WashNumbers = listNumbers;
                    }

                    return r;
                }
            }
            catch (FaultException<DNCRWashingService.DncrServiceFault> e)
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

        public WashByFileUploadResponse WashByFileUpload(WashByFileUploadModel wasByFileUploadModel, UserContextModel userContext)
        {
            var r = new WashByFileUploadResponse();

            try
            {
                using (var client = new OnlineWashingServiceClient())
                {
                    var args = new WashFileRequestArgs
                    {
                        AccountId = wasByFileUploadModel.AccountId,
                        User = new DNCRWashingService.OnlineUserContext { AccountUserID = userContext.AccountUserID, AgentID = userContext.AgentID },
                        WashingRequestId = wasByFileUploadModel.WashingRequestId,
                        WashResultFormatFileWithIndicators = wasByFileUploadModel.WashResultFormatFileWithIndicators,
                        WashResultFormatInvalidNumbers = wasByFileUploadModel.WashResultFormatInvalidNumbers,
                        WashResultFormatRegisteredNumbers = wasByFileUploadModel.WashResultFormatRegisteredNumbers,
                        WashResultFormatUnregisteredNumbers = wasByFileUploadModel.WashResultFormatUnregisteredNumbers
                    };

                    var result = client.WashByFileUpload(args);

                    r.AllInOneFile = result.AllInOneFile;
                    r.HasSufficientWashingCredits = result.HasSufficientWashingCredits;
                    r.HasValidSubscription = result.HasValidSubscription;
                    r.InvalidNumbersFile = result.InvalidNumbersFile;
                    r.IsSuccessful = result.IsSuccessful;
                    r.OriginalFile = result.OriginalFile;
                    r.RegisteredNumbersFile = result.RegisteredNumbersFile;
                    r.Status = ConvertWashingStatusToInternalEnum(result.Status);
                    r.UnregisteredNumbersFile = result.UnregisteredNumbersFile;
                    r.WashCredits = result.WashCredits;
                    r.WashingRequestId = result.WashingRequestId;

                    return r;
                }
            }
            catch (FaultException<DNCRWashingService.DncrServiceFault> e)
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

        public WashHistoryResponse WashHistory(WashHistoryRequestModel washHistoryModel, UserContextModel userContext)
        {
            var r = new WashHistoryResponse();

            try
            {
                using (var client = new OnlineWashingServiceClient())
                {
                    var args = new WashingSearchRequestArgs
                    {
                        AccountId = washHistoryModel.AccountId,
                        EndDateTime = washHistoryModel.EndDateTime != null ? washHistoryModel.EndDateTime.Value : DateTime.Now,
                        //ExcludeWashWithOneNumber = washHistoryModel.ExcludeWashWithOneNumber,
                        StartDateTime = washHistoryModel.StartDateTime,
                        User = new DNCRWashingService.OnlineUserContext { AccountUserID = userContext.AccountUserID, AgentID = userContext.AgentID != null ? userContext.AgentID : null },
                        WashReferenceId = washHistoryModel.WashReferenceId
                    };

                    //if (washHistoryModel.Channel != null)
                    //    args.Channel = ConvertInternalWashingChannelEnumToWSEnum(washHistoryModel.Channel.Value);

                    var result = client.WashHistory(args);

                    if (result.Results != null && result.Results.Any())
                    {
                        var washHistoryList = result.Results.Select(item => new WashHistoryResponseObject
                        {
                            AccountUserId = item.AccountUserId,
                            AllInOneFile = item.AllInOneFile,
                            AllInOneNumbersCount = item.AllInOneNumbersCount,
                            CanDownload = item.CanDownload,
                            FileName = item.FileName,
                            HasRefunded = item.HasRefunded,
                            InvalidNumbersCount = item.InvalidNumbersCount,
                            InvalidNumbersFile = item.InvalidNumbersFile,
                            NumbersWashed = item.NumbersWashed,
                            OriginalFile = item.OriginalFile,
                            RegisteredNumbersCount = item.RegisteredNumbersCount,
                            RegisteredNumbersFile = item.RegisteredNumbersFile,
                            RequestDate = item.RequestDate,
                            UnregisteredNumbersCount = item.UnregisteredNumbersCount,
                            UnregisteredNumbersFile = item.UnregisteredNumbersFile,
                            WashingChannel = ConvertWSEnumToWashingChannelWashingChannelEnum(item.WashingChannel),
                            WashingRequestId = item.WashingRequestId,
                            AccountUserName = item.AccountUserName,
                            ClientReference = item.ClientReference,
                        }).ToList();

                        r.WashHistoryResponseObjects = washHistoryList;
                    }

                    return r;
                }
            }
            catch (FaultException<DNCRWashingService.DncrServiceFault> e)
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

        #endregion

        #region Subscription

        public GenericResponseModel CancelOrder(int accountUserId, int orderId, int? agentUserId, bool? isCSR)
        {
            var r = new GenericResponseModel();

            try
            {
                using (var client = new OnlineOrderServiceClient())
                {
                    var args = new CancelOrderRequestArgs
                    {
                        AccountUserID = accountUserId,
                        OrderID = orderId,
                        IsCSR = isCSR ?? false
                    };

                    if (agentUserId != null)
                        args.AgentUserID = agentUserId.Value;

                    var result = client.CancelOrder(args);

                    r.IsSuccessful = result.IsSuccess;
                    r.Message = result.ResultMessage;

                    return r;
                }
            }
            catch (FaultException<DNCROrderServices.DncrServiceFault> e)
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

        public GenericResponseModel CloseOrder(int accountUserId, int orderId, int? agentUserId, bool? isCSR)
        {
            var r = new GenericResponseModel();

            try
            {
                using (var client = new OnlineOrderServiceClient())
                {
                    var args = new CloseOrderRequestArgs
                    {
                        AccountUserID = accountUserId,
                        OrderID = orderId,
                        IsCSR = isCSR ?? false
                    };

                    if (agentUserId != null)
                        args.AgentUserID = agentUserId.Value;

                    var result = client.CloseOrder(args);

                    r.IsSuccessful = result.IsSuccess;
                    r.Message = result.ResultMessage;

                    return r;
                }
            }
            catch (FaultException<DNCROrderServices.DncrServiceFault> e)
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

        public CreateOrderResult CreateOrder(int accountId, decimal balanceAmountCanBeUsed, string email, bool includeBalance, bool isCreditCardPayment,
            List<SubscriptionVsQuantityModel> listSubscriptionVsQuantityModel, UserContextModel userContext)
        {
            var r = new CreateOrderResult();

            try
            {
                using (var client = new OnlineOrderServiceClient())
                {
                    var args = new CreateOrderRequestArgs
                    {
                        AccountID = accountId,
                        BalanceAmountCanBeUsed = balanceAmountCanBeUsed,
                        EmailAddress = email,
                        IncludeBalance = includeBalance,
                        IsCreditCardPayment = isCreditCardPayment,
                        UserContext = new DNCROrderServices.OnlineUserContext { AccountUserID = userContext.AccountUserID, AgentID = userContext.AgentID },
                    };

                    if (listSubscriptionVsQuantityModel != null && listSubscriptionVsQuantityModel.Count > 0)
                    {
                        SubscritopnVsQuantity[] subscriptionVsQuantityArray = new SubscritopnVsQuantity[listSubscriptionVsQuantityModel.Count];
                        for (int i = 0; i < listSubscriptionVsQuantityModel.Count; i++)
                        {
                            subscriptionVsQuantityArray[i] = new SubscritopnVsQuantity
                            {
                                Quantity = listSubscriptionVsQuantityModel[i].Quantity,
                                SubscriptionID = listSubscriptionVsQuantityModel[i].SubscriptionID
                            };
                        }
                        args.SubscritopnVsQuantities = subscriptionVsQuantityArray;
                    }

                    var result = client.CreateOrder(args);
                    r.OrderNumber = result.OrderNumber;
                    r.OrderId = result.OrderID;

                    return r;
                }
            }
            catch (FaultException<DNCROrderServices.DncrServiceFault> e)
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

        public GenerateInvoiceResponse GenerateInvoice(int orderId)
        {
            var r = new GenerateInvoiceResponse();

            try
            {
                using (var client = new OnlineOrderServiceClient())
                {
                    var args = new InvoiceRequestArgs
                    {
                        OrderID = orderId
                    };

                    var result = client.GenerateInvoice(args);
                    r.InvoiceStream = result;

                    return r;
                }
            }
            catch (FaultException<DNCROrderServices.DncrServiceFault> e)
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

        public GetAvailableSubscriptionsResponse GetAvailableSubscriptions(int accountId)
        {
            var r = new GetAvailableSubscriptionsResponse();

            try
            {
                using (var client = new OnlineOrderServiceClient())
                {
                    var args = new SubscriptionRequestArgs
                    {
                        AccountID = accountId
                    };

                    var result = client.GetAvailableSubscriptions(args);

                    r.Message = result.ResultMessage;

                    if (result.Subscriptions != null && result.Subscriptions.Any())
                    {
                        List<SubscriptionModel> listSubscriptions = result.Subscriptions.Select(item => new SubscriptionModel
                        {
                            ExpirityInDays = item.ExpirityInDays,
                            IsFreeSubscription = item.IsFreeSubscription,
                            LastUpdatedTimeStamp = item.LastUpdatedTimeStamp,
                            Price = item.Price,
                            SubscriptionName = item.SubscriptionName,
                            SubscriptionRequestID = item.SubscriptionRequestID,
                            WashCredits = item.WashCredits
                        }).ToList();

                        r.Subscriptions = listSubscriptions;
                    }

                    return r;
                }
            }
            catch (FaultException<DNCROrderServices.DncrServiceFault> e)
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

        private List<AccountTransactionModel> GenerateAccountTransactionModelList(AccountTransaction[] accountTransaction)
        {
            return accountTransaction.Select(item => new AccountTransactionModel
            {
                AccountID = item.AccountID,
                AccountTransactionID = item.AccountTransactionID,
                AccountTransactionType = ConvertAccountTransactionTypeToInternalEnum(item.AccountTransactionType),
                Amount = item.Amount,
                CreatedTimeStamp = item.CreatedTimeStamp,
                WashCredits = item.WashCredits,
                Reference = item.Reference
            }).ToList();
        }

        private List<AccountAdjustmentTypesModel> GenerateAccountAdjustmentTypesModelList(AccountAdjustmentTypes[] accountAdjustmentTypes)
        {
            return accountAdjustmentTypes.Select(item => new AccountAdjustmentTypesModel
            {
                AccountAdjustmentTypeID = item.AccountAdjustmentTypeID,
                AdjustmentName = item.AdjustmentName
            }).ToList();
        }

        private List<PaymentModel> GeneratePaymentModelList(Payment[] payments)
        {
            return payments.Select(paymentsItem => new PaymentModel
                {
                    CreatedTimeStamp = paymentsItem.CreatedTimeStamp,
                    CreditCardReference = paymentsItem.CreditCardReference,
                    CreditCardTransactionCode = paymentsItem.CreditCardTransactionCode,
                    OrderID = paymentsItem.OrderID,
                    PaidAmount = paymentsItem.PaidAmount,
                    PaymentFeedEntryID = paymentsItem.PaymentFeedEntryID,
                    PaymentID = paymentsItem.PaymentID,
                    PaymentMethod = ConvertPaymentMethodToInternalEnum(paymentsItem.PaymentMethod),
                    PaymentMethodID = paymentsItem.PaymentMethodID
                }).ToList();
        }

        private List<OrderTransactionModel> GenerateOrderTransactionModelList(OrderTransaction[] orderTransactions)
        {
            return orderTransactions.Select(orderTransactionItem => new OrderTransactionModel
                {
                    Amount = orderTransactionItem.Amount,
                    CreatedTimeStamp = orderTransactionItem.CreatedTimeStamp,
                    OrdeTransactionTypeName = ConvertOrderTransactionTypeToInternalEnum(orderTransactionItem.OrdeTransactionTypeName)
                }).ToList();
        }

        private List<OrderLineModel> GenerateOrderLineModelList(OrderLine[] orderLines)
        {
            return orderLines.Select(orderLinesItem => new OrderLineModel
                {
                    Activated = orderLinesItem.Activated,
                    ExpirityInDays = orderLinesItem.ExpirityInDays,
                    OrderID = orderLinesItem.OrderID,
                    OrderLineID = orderLinesItem.OrderLineID,
                    OrderLineNumber = orderLinesItem.OrderLineNumber,
                    OrderLineStatus = ConvertOrderLineStatusToInternalEnum(orderLinesItem.OrderLineStatus),
                    Price = orderLinesItem.Price,
                    SubscriptionName = orderLinesItem.SubscriptionName,
                    WashCredits = orderLinesItem.WashCredits
                }).ToList();
        }

        private List<OrderNoteModel> GenerateOrderNoteModelList(OrderNote[] orderNotes)
        {
            return orderNotes.Select(noteItem => new OrderNoteModel
                {
                    CreatedTimeStamp = noteItem.CreatedTimeStamp,
                    Note = noteItem.Note
                }).ToList();
        }

        private List<OrderModel> GenerateOrderModelList(Order[] orders)
        {
            var ordersList = new List<OrderModel>();

            foreach (var item in orders)
            {
                var om = new OrderModel
                {
                    AccountID = item.AccountID,
                    AccountUserID = item.AccountUserID,
                    AdditionalEmailAddress = item.AdditionalEmailAddress,
                    Amount = item.Amount,
                    CreatedTimeStamp = item.CreatedTimeStamp,
                    Description = item.Description,
                    GST = item.GST,
                    OrderExpiryDate = item.OrderExpiryDate,
                    OrderID = item.OrderID,
                    OrderNumber = item.OrderNumber,
                    OrderStatus = ConvertOrderStatusEnumToInternalEnum(item.OrderStatus),
                    PaymentActivated = item.PaymentActivated,
                    PaymentOutstanding = item.PaymentOutstanding,
                    PaymentReceived = item.PaymentReceived,
                    PaymentUnallocated = item.PaymentUnallocated
                };

                if (item.Payments != null && item.Payments.Any())
                {
                    om.Payments = GeneratePaymentModelList(item.Payments);
                }

                if (item.OrderTransactions != null && item.OrderTransactions.Any())
                {
                    om.OrderTransactions = GenerateOrderTransactionModelList(item.OrderTransactions);
                }

                if (item.OrderLines != null && item.OrderLines.Any())
                {
                    om.OrderLines = GenerateOrderLineModelList(item.OrderLines);
                }

                if (item.Notes != null && item.Notes.Any())
                {
                    om.Notes = GenerateOrderNoteModelList(item.Notes);
                }

                ordersList.Add(om);
            }

            return ordersList;
        }

        private AccountAdjustmentModel GenerateAccountAdjustment(AccountAdjustment accountAdjustment)
        {
            return new AccountAdjustmentModel
            {
                AccountAdjustmentType = CovertAdjustmentTypeEnumToInternalEnum(accountAdjustment.AccountAdjustmentType),
                AccountAdjustmentTypeID = accountAdjustment.AccountAdjustmentTypeID,
                AccountID = accountAdjustment.AccountID,
                AdjustmentNote = accountAdjustment.AdjustmentNote,
                AgentUserID = accountAdjustment.AgentUserID,
                CreatedTimeStamp = accountAdjustment.CreatedTimeStamp,
                CreditNotes = accountAdjustment.CreditNotes,
                EnquiryID = accountAdjustment.EnquiryID,
                ExpiryDate = accountAdjustment.ExpiryDate,
                RefundOrderID = accountAdjustment.RefundOrderID,
                RefundSubscriptionID = accountAdjustment.RefundSubscriptionID,
                ReservedAccountBalance = accountAdjustment.ReservedAccountBalance,
                ReservedWashingCredits = accountAdjustment.ReservedWashingCredits,
                WashingCredits = accountAdjustment.WashingCredits,
                WashReferenceID = accountAdjustment.WashReferenceID
            };
        }

        public GetFinancialHistoryResponse GetFinancialHistory(int accountId, int? orderId)
        {
            var r = new GetFinancialHistoryResponse();

            try
            {
                using (var client = new OnlineOrderServiceClient())
                {
                    var args = new OrderRequestArgs
                    {
                        AccountID = accountId,
                        OrderID = orderId
                    };

                    var result = client.GetFinantialHistory(args);

                    r.ResultMessage = result.ResultMessage;
                    r.AccountAdjustment = result.AccountAdjustment != null ? GenerateAccountAdjustment(result.AccountAdjustment) : null;

                    if (result.TransactionHistory != null && result.TransactionHistory.Any())
                    {
                        r.TransactionHistory = GenerateAccountTransactionModelList(result.TransactionHistory);
                    }

                    if (result.AccountAdjustmentTypes != null && result.AccountAdjustmentTypes.Any())
                    {
                        r.AccountAdjustmentTypes = GenerateAccountAdjustmentTypesModelList(result.AccountAdjustmentTypes);
                    }

                    if (result.Orders != null && result.Orders.Any())
                    {
                        r.Orders = GenerateOrderModelList(result.Orders);
                    }

                    return r;
                }
            }
            catch (FaultException<DNCROrderServices.DncrServiceFault> e)
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

        private Enums.AccountTransactionTypeEnum ConvertAccountTransactionTypeToInternalEnum(AccountTransactionTypeAccountTransactionTypeEnum selectedEnumValue)
        {
            switch ((int)selectedEnumValue)
            {
                case 1:
                    return Enums.AccountTransactionTypeEnum.OrderActivated;
                case 2:
                    return Enums.AccountTransactionTypeEnum.SubscriptionActivated;
                case 3:
                    return Enums.AccountTransactionTypeEnum.CreditAccountBalance;
                case 4:
                    return Enums.AccountTransactionTypeEnum.DebitAccountBalance;
                case 5:
                    return Enums.AccountTransactionTypeEnum.PaymentReceivedByCreditCard;
                case 6:
                    return Enums.AccountTransactionTypeEnum.PaymentReceivedByEFT;
                case 7:
                    return Enums.AccountTransactionTypeEnum.DeferredCreditCardPaymentReceived;
                case 8:
                    return Enums.AccountTransactionTypeEnum.BalanceRefund;
                case 9:
                    return Enums.AccountTransactionTypeEnum.OrderCancelled;
                case 10:
                    return Enums.AccountTransactionTypeEnum.OrderClosed;
                case 11:
                    return Enums.AccountTransactionTypeEnum.RefundOrder;
                case 12:
                    return Enums.AccountTransactionTypeEnum.RefundSubscription;
                case 13:
                    return Enums.AccountTransactionTypeEnum.WashingCreditsExpired;
                case 14:
                    return Enums.AccountTransactionTypeEnum.ACMABalanceAdjustment;
                case 15:
                    return Enums.AccountTransactionTypeEnum.ACMAWashCreditAdjustment;
                case 16:
                    return Enums.AccountTransactionTypeEnum.ReverseWash;
                case 17:
                    return Enums.AccountTransactionTypeEnum.WashCreditsRollover;
                default:
                    return Enums.AccountTransactionTypeEnum.OrderClosed;
            }
        }

        private Enums.PaymentMethodEnum ConvertPaymentMethodToInternalEnum(PaymentMethodPaymentMethodEnum selectedEnumValue)
        {
            if (selectedEnumValue.ToString() == Enums.PaymentMethodEnum.CreditCard.ToString())
                return Enums.PaymentMethodEnum.CreditCard;

            if (selectedEnumValue.ToString() == Enums.PaymentMethodEnum.BankTransfer.ToString())
                return Enums.PaymentMethodEnum.BankTransfer;

            if (selectedEnumValue.ToString() == Enums.PaymentMethodEnum.Unknown.ToString())
                return Enums.PaymentMethodEnum.Unknown;

            if (selectedEnumValue.ToString() == Enums.PaymentMethodEnum.BalanceFundTransfer.ToString())
                return Enums.PaymentMethodEnum.BalanceFundTransfer;

            if (selectedEnumValue.ToString() == Enums.PaymentMethodEnum.BPay.ToString())
                return Enums.PaymentMethodEnum.BPay;
            
            return Enums.PaymentMethodEnum.Unknown;
        }

        private Enums.OrderTransactionTypeEnum ConvertOrderTransactionTypeToInternalEnum(OrderTransactionTypeOrderTransactionTypeEnum selectedEnumValue)
        {
            switch ((int)selectedEnumValue)
            {
                case 1:
                    return Enums.OrderTransactionTypeEnum.OrderActivated;
                case 2:
                    return Enums.OrderTransactionTypeEnum.SubscriptionActivated;
                case 3:
                    return Enums.OrderTransactionTypeEnum.CreditAccountBalance;
                case 4:
                    return Enums.OrderTransactionTypeEnum.DebitAccountBalance;
                case 5:
                    return Enums.OrderTransactionTypeEnum.PaymentReceivedByCreditCard;
                case 6:
                    return Enums.OrderTransactionTypeEnum.PaymentReceivedByEFT;
                case 7:
                    return Enums.OrderTransactionTypeEnum.DeferredCreditCardPaymentReceived;
                case 8:
                    return Enums.OrderTransactionTypeEnum.CreditOrderBalance;
                case 9:
                    return Enums.OrderTransactionTypeEnum.DebitOrderBalance;
                case 10:
                    return Enums.OrderTransactionTypeEnum.OrderCancelled;
                case 11:
                    return Enums.OrderTransactionTypeEnum.OrderClosed;
                case 12:
                    return Enums.OrderTransactionTypeEnum.RefundOrder;
                case 13:
                    return Enums.OrderTransactionTypeEnum.RefundSubscription;
                default:
                    return Enums.OrderTransactionTypeEnum.OrderClosed;
            }
        }

        private Enums.OrderLineStatusEnum ConvertOrderLineStatusToInternalEnum(OrderLineStatusOrderLineStatusEnum selectedEnumValue)
        {
            switch ((int)selectedEnumValue)
            {
                case 1:
                    return Enums.OrderLineStatusEnum.Paid;
                case 2:
                    return Enums.OrderLineStatusEnum.NotPaid;
                case 3:
                    return Enums.OrderLineStatusEnum.Refunded;
                default:
                    return Enums.OrderLineStatusEnum.NotPaid;
            }
        }

        private Enums.OrderStatusEnum ConvertOrderStatusEnumToInternalEnum(OrderStatusOrderStatusEnum selectedEnumValue)
        {
            switch ((int)selectedEnumValue)
            {
                case 1:
                    return Enums.OrderStatusEnum.NotPaid;
                case 2:
                    return Enums.OrderStatusEnum.FullyPaid;
                case 3:
                    return Enums.OrderStatusEnum.PartPaid;
                case 4:
                    return Enums.OrderStatusEnum.Closed;
                case 5:
                    return Enums.OrderStatusEnum.Expired;
                case 6:
                    return Enums.OrderStatusEnum.Cancelled;
                case 7:
                    return Enums.OrderStatusEnum.Refunded;
                case 8:
                    return Enums.OrderStatusEnum.PartiallyRefunded;
                default:
                    return Enums.OrderStatusEnum.Closed;
            }
        }

        private Enums.AccountAdjustmentTypeEnum CovertAdjustmentTypeEnumToInternalEnum(AccountAdjustmentTypeAccountAdjustmentTypeEnum selectedEnumValue)
        {
            switch ((int)selectedEnumValue)
            {
                case 1:
                    return Enums.AccountAdjustmentTypeEnum.ACMABalanceAdjustment;
                case 2:
                    return Enums.AccountAdjustmentTypeEnum.ACMAWashCreditAdjustment;
                case 3:
                    return Enums.AccountAdjustmentTypeEnum.ReserveAccountBalance;
                case 4:
                    return Enums.AccountAdjustmentTypeEnum.ApprovedAccountBalanceRefund;
                case 5:
                    return Enums.AccountAdjustmentTypeEnum.DeniedAccountBalanceRefund;
                case 6:
                    return Enums.AccountAdjustmentTypeEnum.ReserveWashCredit;
                case 7:
                    return Enums.AccountAdjustmentTypeEnum.ApprovedSubscriptionRefund;
                case 8:
                    return Enums.AccountAdjustmentTypeEnum.DeniedSubscriptionRefund;
                case 9:
                    return Enums.AccountAdjustmentTypeEnum.ReverseWash;
                case 10:
                    return Enums.AccountAdjustmentTypeEnum.WashCreditsRollover;
                default:
                    return Enums.AccountAdjustmentTypeEnum.ACMABalanceAdjustment;
            }
        }

        private Enums.RefundDecisionEnum CovertRefundDecisionEnumToInternalEnum(enquiry_refund_decisionsenquiry_refund_decisionsEnum selectedEnumValue)
        {
            switch ((int)selectedEnumValue)
            {
                case 1:
                    return Enums.RefundDecisionEnum.InProgress;
                case 2:
                    return Enums.RefundDecisionEnum.Approved;
                case 3:
                    return Enums.RefundDecisionEnum.Denied;
                default:
                    return Enums.RefundDecisionEnum.InProgress;
            }
        }        

        public SubscriptionHistoryDetailsResponse GetSubscriptionHistoryDetailsByAccountID(int accountId)
        {
            var r = new SubscriptionHistoryDetailsResponse();

            try
            {
                using (var client = new OnlineOrderServiceClient())
                {
                    var args = new SubscriptionRequestArgs
                    {
                        AccountID = accountId
                    };

                    var result = client.GetSubscriptionHistorydetailsByAccountID(args);

                    r.ResultMessage = result.ResultMessage;

                    if (result.SubscriptionHistory != null && result.SubscriptionHistory.Any())
                    {
                        var subscriptionHistoryList = result.SubscriptionHistory.Select(item => new SubscriptionHistoryModel
                        {
                            AccountTransactionID = item.AccountTransactionID,
                            AccountUser = item.AccountUser,
                            AgentUser = item.AgentUser,
                            Amount = item.Amount,
                            Created = item.Created,
                            Credit = item.Credit,
                            Debit = item.Debit,
                            Description = item.Description,
                            ExpirationDate = item.ExpirationDate,
                            TransactionType = item.TransactionType,
                            WashCreditBalance = item.WashCreditBalance
                        }).ToList();

                        r.SubscriptionHistories = subscriptionHistoryList;
                    }

                    return r;
                }
            }
            catch (FaultException<DNCROrderServices.DncrServiceFault> e)
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

        public SubscriptionSummaryDetailsResponse GetSubscriptionSummaryDetailsByAccountID(int accountId)
        {
            var r = new SubscriptionSummaryDetailsResponse();

            try
            {
                using (var client = new OnlineOrderServiceClient())
                {
                    var args = new SubscriptionRequestArgs
                    {
                        AccountID = accountId
                    };

                    var result = client.GetSubscriptionSummarydetailsByAccountID(args);

                    r.ResultMessage = result.ResultMessage;

                    if (result.SubscriptionSummary != null)
                    {
                        r.SubscriptionSummary = new SubscriptionSummaryModel
                        {
                            AvailableWashCredits = result.SubscriptionSummary.AvailableWashCredits,
                            CreditNotes = result.SubscriptionSummary.CreditNotes,
                            LastPurchaseDate = result.SubscriptionSummary.LastPurchaseDate,
                            ReservedCreditNotes = result.SubscriptionSummary.ReservedCreditNotes,
                            ReservedWashCredits = result.SubscriptionSummary.ReservedWashCredits,
                            SubscriptionExpiryDate = result.SubscriptionSummary.SubscriptionExpiryDate,
                            SubscriptionStatus = result.SubscriptionSummary.SubscriptionStatus

                        };

                        List<EnquiryRefundsModel> erfmList = result.SubscriptionSummary.EnquiryRefunds.Select(item => new EnquiryRefundsModel
                        {
                            EnquiryID = item.EnquiryID,
                            RefundDecision = CovertRefundDecisionEnumToInternalEnum(item.RefundDecision),
                            RefundType = CovertRefundTypeEnumToInternalEnum(item.RefundType)
                        }).ToList();
                        r.SubscriptionSummary.EnquiryRefundsModels = erfmList;
                    }

                    return r;
                }
            }
            catch (FaultException<DNCROrderServices.DncrServiceFault> e)
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

        public PayForOrderResponse PayForOrder(PayForOrderArguments payForOrderArguments)
        {
            var r = new PayForOrderResponse();

            try
            {
                using (var client = new OnlineOrderServiceClient())
                {
                    var args = new PayForOrderRequestArgs
                    {
                        AccountUserID = payForOrderArguments.AccountUserID,
                        AgentUserID = payForOrderArguments.AgentUserID,
                        Amount = payForOrderArguments.Amount,
                        CreditCardReference = payForOrderArguments.CreditCardReference,
                        IsCSR = payForOrderArguments.IsCSR,
                        OrderID = payForOrderArguments.OrderID,
                        AuthorizeID = payForOrderArguments.AuthorizeID,
                        CardType = payForOrderArguments.CardType,
                        ReceiptNo = payForOrderArguments.ReceiptNo,
                        ResponseCode = payForOrderArguments.ResponseCode,
                        TransactionID = payForOrderArguments.TransactionID,
                        TransactionNo = payForOrderArguments.TransactionNo,
                        TransactionType = payForOrderArguments.TransactionType,
                        ExpectedSettelmentDate = payForOrderArguments.ExpectedSettelmentDate,
                        ExternalMessage = payForOrderArguments.ExternalMessage
                    };

                    var result = client.PayForOrder(args);

                    r.IsSuccessful = result.IsSuccess;
                    r.EnquiryId = result.EnquiryId;

                    return r;
                }
            }
            catch (FaultException<DNCROrderServices.DncrServiceFault> e)
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

        public GenericResponseModel RecordPayForOrderFail(PayForOrderArguments payForOrderArguments)
        {
            var r = new GenericResponseModel();

            try
            {
                using (var client = new OnlineOrderServiceClient())
                {
                    var args = new PayForOrderRequestArgs
                    {
                        AccountUserID = payForOrderArguments.AccountUserID,
                        AgentUserID = payForOrderArguments.AgentUserID,
                        Amount = payForOrderArguments.Amount,
                        CreditCardReference = payForOrderArguments.CreditCardReference,
                        IsCSR = payForOrderArguments.IsCSR,
                        OrderID = payForOrderArguments.OrderID,
                        AuthorizeID = payForOrderArguments.AuthorizeID,
                        CardType = payForOrderArguments.CardType,
                        ReceiptNo = payForOrderArguments.ReceiptNo,
                        ResponseCode = payForOrderArguments.ResponseCode,
                        TransactionID = payForOrderArguments.TransactionID,
                        TransactionNo = payForOrderArguments.TransactionNo,
                        TransactionType = payForOrderArguments.TransactionType,
                        ExpectedSettelmentDate = payForOrderArguments.ExpectedSettelmentDate,
                        ExternalMessage = payForOrderArguments.ExternalMessage,
                    };

                    var result = client.RecordPayForOrderFail(args);

                    r.IsSuccessful = result.IsSuccess;

                    return r;
                }
            }
            catch (FaultException<DNCROrderServices.DncrServiceFault> e)
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

        public AccountSummaryDetailResponse GetAccountSummaryDetailByAccountID(int accountId)
        {
            var r = new AccountSummaryDetailResponse();

            try
            {
                using (var client = new OnlineOrderServiceClient())
                {
                    var result = client.GetAccountSummaryDetailByAccountID(new SubscriptionRequestArgs
                    {
                        AccountID = accountId
                    });

                    if (result.Entries != null)
                    {
                        var accountSummaryModelList = result.Entries.Select(item => new AccountSummaryModel
                        {
                            AgentUserDisplayName = item.AgentUserDisplayName, 
                            Amount = item.Amount, 
                            CreatedTimeStampt = item.CreatedTimeStampt, 
                            Description = item.Description, 
                            EnquiryId = item.EnquiryId, 
                            Expires = item.Expires, 
                            OrderNumber = item.OrderNumber, 
                            PaymentReference = item.PaymentReference, 
                            TransactionType = item.TransactionType, 
                            WashCredits = item.WashCredits
                        }).ToList();

                        r.AccountSummaries = accountSummaryModelList;
                    }

                    return r;
                }
            }
            catch (FaultException<DNCROrderServices.DncrServiceFault> e)
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

        public GetAccountUserHeaderInformationResponse GetAccountUserHeaderInformationByAccountUserID(int accountUserId)
        {
            var r = new GetAccountUserHeaderInformationResponse();

            try
            {
                using (var client = new OnlineOrderServiceClient())
                {
                    var args = new AccountUserHeaderInformationArgs
                    {
                        AccountUserID = accountUserId
                    };

                    var result = client.GetAccountUserHeaderInformationByAccountUserID(args);

                    r.AccountID = result.AccountID;
                    r.AccountUserEmailAddress = result.AccountUserEmailAddress;
                    r.AccountUserID = result.AccountUserID;
                    r.CanSeeWashQuote = result.CanSeeWashQuote;
                    r.CompanyName = result.CompanyName;
                    r.IsSuccess = result.IsSuccess;
                    r.WashCredits50Count = result.WashCredits50Count;
                    r.WashCredits80Count = result.WashCredits80Count;
                    r.WashCreditsCount = result.WashCreditsCount;
                    r.ReservedWashCreditsCount = result.BlockedWashingCredits;
                    r.AccountUserStatus = ConvertOrderWSAccountUserStatusEnumToInternalAccountUserStatusEnum(result.AccountUserStatus);
                    r.AccountStatus = ConvertOrderWSAccountStatusEnumToInternalAccountStatusEnum(result.AccountStatus);

                    return r;
                }
            }
            catch (FaultException<DNCROrderServices.DncrServiceFault> e)
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

        #endregion

        #region C&E

        public GetAccountRefundsResponse GetAccountRefunds(int accountId)
        {
            var r = new GetAccountRefundsResponse();

            try
            {
                using (var client = new OnlineCmsServiceClient())
                {
                    var args = new AccountRefundsRequestArgs
                    {
                        AccountID = accountId
                    };

                    var result = client.GetAccountRefunds(args);

                    r.AccountID = result.AccountID;

                    if (result.Refunds != null && result.Refunds.Any())
                    {
                        var refundsList = result.Refunds.Select(item => new AccountRefundsModel
                        {
                            EnquiryID = item.EnquiryID, 
                            EnquiryRefundID = item.EnquiryRefundID, 
                            EnquiryStatus = CovertEnquiryStatusEnumToInternalEnum(item.EnquiryStatus), 
                            LodgedTimeStamp = item.LodgedTimeStamp, 
                            OrderID = item.OrderID, 
                            RefundDecision = CovertRefundDecisionEnumToInternalEnum(item.RefundDecision), 
                            RefundType = CovertRefundTypeEnumToInternalEnum(item.RefundType), 
                            SubscriptionID = item.SubscriptionID, 
                            WashCreditsRolloverTrasactionID = item.WashCreditsRolloverTrasactionID, 
                            WashTransactionID = item.WashTransactionID
                        }).ToList();

                        r.Refunds = refundsList;
                    }

                    return r;
                }
            }
            catch (FaultException<DNCRServicesCMS.DncrServiceFault> e)
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

        public GetManualAdjustmentLimitResponse GetManualAdjustmentWashCreditsLimit(int accountId)
        {
            var r = new GetManualAdjustmentLimitResponse();

            try
            {
                using (var client = new OnlineCmsServiceClient())
                {
                    var args = new GetManualAdjustmentWashCreditsLimitArgs()
                    {
                        AccountID = accountId
                    };

                    var result = client.GetManualAdjustmentWashCreditsLimit(args);

                    r.WashCreditsLimit = result.WashCreditsLimit;

                    return r;
                }
            }
            catch (FaultException<DNCRServicesCMS.DncrServiceFault> e)
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

        #endregion
    }
}
