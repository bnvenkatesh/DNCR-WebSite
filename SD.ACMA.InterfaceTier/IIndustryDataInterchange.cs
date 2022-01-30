using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SD.ACMA.POCO.Base;
using SD.ACMA.POCO.Industry;

namespace SD.ACMA.InterfaceTier
{
    public interface IIndustryDataInterchange
    {
        #region Access Seeker

        LoginResponse Login(string username, string password);        

        CreateAccountResponse CreateAccount(CreateAccountModel newAccount);

        GenericResponseModel CreateWashOnlyUser(CreateWashOnlyUserModel washOnlyUser, UserContextModel userContext);

        GetAccountResponse GetAccount(int accountID, UserContextModel userContext);

        GetAccountUserResponse GetAccountUser(int accountUserID, UserContextModel userContext);

        GetAllAccountUsersResponse GetAllAccountUsers(int accountId, UserContextModel userContext);

        GenericResponseModel ResetPassword(string email, string newPassword, string passwordToken);
               
        GenericResponseModel ForgotPassword(string email);

        GenericResponseModel ChangePassword(int accountUserId, string newPassword, string oldPassword);

        ActivateAccountResponse ActivateAccount(string activationTaken, string adminPassword, string adminEmail);

        GenericResponseModel ActivateAccountUser(UserContextModel userContext, int accountUserId);

        ActivateWashOnlyUserResponse ActivateWashOnlyUser(string email, string password, string activationToken);

        GenericResponseModel DeActivateAccountUser(UserContextModel userContext, int deactivateAccountUserId);

        GenericResponseModel UpdateAccount(UserContextModel userContext, UpdateAccountModel updateAccountModel);

        GenericResponseModel UpdateAccountUser(UserContextModel userContext, UpdateAccountUserModel updateAccountUserModel);

        GenericResponseModel DeleteAccountUser(UserContextModel userContext, int deleteAccountUserId);

        ImpersonateResponse Impersonate(int accountUserId, string agentData, string token);

        GenericResponseModel SendAccountActivationEmail(string adminEmail);

        GetStatesResponse GetStates(int countryId, int? agentId);

        GetCountriesResponse GetCountries();

        #endregion

        #region Washing

        DownloadErrorLogResponse DownloadErrorLog(int accountId, int washingRequestId, UserContextModel userContext);

        DownloadWashFileResponse DownloadWashFile(int accountId, int washingRequestId, SD.ACMA.DNCR.Infrastructure.Enums.WashFileTypeEnum washFileType, UserContextModel userContext);

        PreWashResponse PreWash(string clientReference, string relativeInputFilePath, UserContextModel userContext);

        QuickWashResponse QuickWash(int accountId, string clientReference, List<string> numbersToWash, UserContextModel userContext);

        WashByFileUploadResponse WashByFileUpload(WashByFileUploadModel wasByFileUploadModel, UserContextModel userContext);

        WashHistoryResponse WashHistory(WashHistoryRequestModel washHistoryModel, UserContextModel userContext);

        #endregion

        #region C&E

        LodgeEnquiryResponse LodgeEnquiry(LodgeEnquiryModel lodgeEnquiryModel, int? agentId);

        LodgeComplaintResponse LodgeComplaint(LodgeComplaintModel lodgeComplaintModel);

        GetAccountRefundsResponse GetAccountRefunds(int accountId);

        GetManualAdjustmentLimitResponse GetManualAdjustmentWashCreditsLimit(int accountId);

        #endregion

        #region Subscription

        GenericResponseModel CancelOrder(int accountUserId, int orderId, int? agentUserId, bool? isCSR);

        GenericResponseModel CloseOrder(int accountUserId, int orderId, int? agentUserId, bool? isCSR);

        CreateOrderResult CreateOrder(int accountId, decimal balanceAmountCanBeUsed, string email, bool includeBalance, bool isCreditCardPayment,
            List<SubscriptionVsQuantityModel> listSubscriptionVsQuantityModel, UserContextModel userContext);

        GenerateInvoiceResponse GenerateInvoice(int orderId);

        GetAvailableSubscriptionsResponse GetAvailableSubscriptions(int accountId);

        GetFinancialHistoryResponse GetFinancialHistory(int accountId, int? orderId);

        SubscriptionHistoryDetailsResponse GetSubscriptionHistoryDetailsByAccountID(int accountId);

        SubscriptionSummaryDetailsResponse GetSubscriptionSummaryDetailsByAccountID(int accountId);

        PayForOrderResponse PayForOrder(PayForOrderArguments payForOrderArguments);

        GenericResponseModel RecordPayForOrderFail(PayForOrderArguments payForOrderArguments);

        AccountSummaryDetailResponse GetAccountSummaryDetailByAccountID(int accountId);

        GetAccountUserHeaderInformationResponse GetAccountUserHeaderInformationByAccountUserID(int accountUserId);

        #endregion
    }
}
