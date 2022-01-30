using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SD.ACMA.DNCR.Infrastructure.Configuration
{
    public class ConfigurationHelper
    {
        private const string CONNECTION_STRING_NAME = "ConnectionStringName";
        private const string CONNECTION_STRING_NAME_PORTAL = "ConnectionStringName_Portal";
        private const string TWITTER_CONSUMER_KEY = "TwitterConsumerKey";
        private const string TWITTER_CONSUMER_SECRET = "TwitterConsumerSecret";
        private const string TWITTER_ACCESS_TOKEN = "TwitterAccessToken";
        private const string TWITTER_ACCESS_TOKEN_SECRET = "TwitterAccessTokenSecret";
        private const string BULK_FILE_UPLOAD_LOCATION = "BulkFileUploadLocation";
        private const string BULK_FILE_DOWNLOAD_LOCATION = "BulkFileDownloadLocation";
        private const string SOCIAL_MEDIA_CREDENTIALS_URL = "SocialMediaCredentialsURL";
        private const string UNHANDLED_ERROR_MESSAGE_SUBSTITUTE = "UnhandledErrorMessageSubstitute";

        private const string ACCOUNTSUMMARY_SUBSCRIPTIONDATE_PARSEEXACTFORMAT = "AccountSummary.SubscriptionDate.ParseExactFormat";

        private const string LOOP_INTERVAL = "LoopInterval";

        private const string SUPPRESS_UPDATABLE_CHECK_AFTER_CREATION_PERIOD = "SuppressUpdatableCheckAfterCreationPeriod";

        private const string YOUTUBE_MAX_REQUEST = "YouTubeMaxRequest";

        private const string FINANCIAL_REPORT_INTERNAL_HOST = "FinancialReportInternalHost";
        private const string FINANCIAL_REPORT_INTERNAL_USERNAME = "FinancialReportInternalUsername";
        private const string FINANCIAL_REPORT_INTERNAL_PASSWORD = "FinancialReportInternalPassword"; 

        #region Session Names

        public const string ACCOUNT_ID = "AccountID";
        public const string ACCOUNT_USER_ID = "AccountUserID";
        public const string IS_ADMIN = "IsAdmin";
        public const string AGENT_ID = "AgentID";
        public const string COMPANY_NAME = "CompanyName";
        public const string USER_EMAIL = "UserEmail";
        public const string REQUIRE_LOGOUT = "RequireLogout";
        public const string BELOW_50_CREDIT = "Below50Credit";
        public const string BELOW_20_CREDIT = "Below20Credit";
        public const string IS_ACMA = "IsAcma";

        #endregion

        #region Constants

        public const string WASH_FORMAT_FILE_WITH_INDICATORS = "File with indicators";
        public const string WASH_FORMAT_INVALID_NUMBERS = "Invalid";
        public const string WASH_FORMAT_REGISTERED_NUMBERS = "Registered";
        public const string WASH_FORMAT_UNREGISTERED_NUMBERS = "Unregistered";

        public const string NUMBER_REGISTERED = "Y";
        public const string NUMBER_NOT_REGISTERED = "N";
        public const string NUMBER_INVALID = "I";

        #endregion

        #region Payment Gateway Configurations

        private const string VIRTUAL_PAYMENT_CLIENT_URL_PAY = "VirtualPaymentClientURL_Pay";
        private const string VIRTUAL_PAYMENT_CLIENT_URL_QUERY = "VirtualPaymentClientURL_Query";
        private const string SECURE_SECRET = "SecureSecret";
        private const string MERCHANT_ACCESS_CODE = "MerchantAccessCode";
        private const string MERCHANT_ID = "MerchantID";
        private const string MERCHANT_ADMIN_USER = "MerchantAdminUser";
        private const string MERCHANT_ADMIN_PASSWORD = "MerchantAdminPassword";
        private const string PENDING_PAYMENT_TIMEOUT_HOURS = "PendingPaymentTimeOutHours";

        #endregion

        private static ConfigurationHelper _instance;
        public static ConfigurationHelper Instance
        {
            get
            {
                lock (typeof(ConfigurationHelper))
                {
                    if (_instance == null)
                        _instance = new ConfigurationHelper();

                    return _instance;
                }
            }
        }

        private string GetConfiguration(string name)
        {
            return System.Configuration.ConfigurationManager.AppSettings[name];
        }
        public string GetConnectionString()
        {
            return System.Configuration.ConfigurationManager.ConnectionStrings[ConnectionStringName].ConnectionString;
        }

        private string _unhandledErrorMessageSubstitute;
        public string UnhandledErrorMessageSubstitute
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_unhandledErrorMessageSubstitute))
                {
                    _unhandledErrorMessageSubstitute = GetConfiguration(UNHANDLED_ERROR_MESSAGE_SUBSTITUTE);
                }

                return _unhandledErrorMessageSubstitute;
            }
        }

        private string _socialMediaCredentialsURL;
        public string SocialMediaCredentialsURL
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_socialMediaCredentialsURL))
                {
                    _socialMediaCredentialsURL = GetConfiguration(SOCIAL_MEDIA_CREDENTIALS_URL);
                }

                return _socialMediaCredentialsURL;
            }
        }

        private string _connectionStringName;
        public string ConnectionStringName
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_connectionStringName))
                {
                    _connectionStringName = GetConfiguration(CONNECTION_STRING_NAME);
                }

                return _connectionStringName;
            }
        }

        private string _connectionStringName_Portal;
        public string ConnectionStringName_Portal
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_connectionStringName_Portal))
                {
                    _connectionStringName_Portal = GetConfiguration(CONNECTION_STRING_NAME_PORTAL);
                }

                return _connectionStringName_Portal;
            }
        }

        private string _twitterConsumerKey;
        public string TwitterConsumerKey
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_twitterConsumerKey))
                {
                    _twitterConsumerKey = GetConfiguration(TWITTER_CONSUMER_KEY);
                }

                return _twitterConsumerKey;
            }
        }

        private string _twitterConsumerSecret;
        public string TwitterConsumerSecret
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_twitterConsumerSecret))
                {
                    _twitterConsumerSecret = GetConfiguration(TWITTER_CONSUMER_SECRET);
                }

                return _twitterConsumerSecret;
            }
        }

        private string _twitterAccessToken;
        public string TwitterAccessToken
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_twitterAccessToken))
                {
                    _twitterAccessToken = GetConfiguration(TWITTER_ACCESS_TOKEN);
                }

                return _twitterAccessToken;
            }
        }

        private string _twitterAccessTokenSecret;
        public string TwitterAccessTokenSecret
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_twitterAccessTokenSecret))
                {
                    _twitterAccessTokenSecret = GetConfiguration(TWITTER_ACCESS_TOKEN_SECRET);
                }

                return _twitterAccessTokenSecret;
            }
        }

        private string _bulkFileUploadLocation;
        public string BulkFileUploadLocation
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_bulkFileUploadLocation))
                {
                    _bulkFileUploadLocation = GetConfiguration(BULK_FILE_UPLOAD_LOCATION);
                }

                return _bulkFileUploadLocation;
            }
        }

        private string _bulkFileDownloadLocation;
        public string BulkFileDownloadLocation
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_bulkFileDownloadLocation))
                {
                    _bulkFileDownloadLocation = GetConfiguration(BULK_FILE_DOWNLOAD_LOCATION);
                }

                return _bulkFileDownloadLocation;
            }
        }

        private int _youTubeMaxRequest;
        public int YouTubeMaxRequest
        {
            get
            {
                if (_youTubeMaxRequest == 0)
                {
                    _youTubeMaxRequest = int.Parse(GetConfiguration(YOUTUBE_MAX_REQUEST));
                }
                return _youTubeMaxRequest;
            }
        }

        private int _loopInterval;
        public int LoopInterval
        {
            get
            {
                if (_loopInterval == 0)
                {
                    _loopInterval = int.Parse(GetConfiguration(LOOP_INTERVAL));
                }
                return _loopInterval;
            }
        }

        private int _suppressUpdatableCheckAfterCreationPeriod;
        public int SuppressUpdatableCheckAfterCreationPeriod
        {
            get
            {
                if (_suppressUpdatableCheckAfterCreationPeriod == 0)
                {
                    _suppressUpdatableCheckAfterCreationPeriod = int.Parse(GetConfiguration(SUPPRESS_UPDATABLE_CHECK_AFTER_CREATION_PERIOD));
                }
                return _suppressUpdatableCheckAfterCreationPeriod;
            }
        }

        private string _VirtualPaymentClientURL_Pay;
        public string VirtualPaymentClientURL_Pay
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_VirtualPaymentClientURL_Pay))
                {
                    _VirtualPaymentClientURL_Pay = GetConfiguration(VIRTUAL_PAYMENT_CLIENT_URL_PAY);
                }

                return _VirtualPaymentClientURL_Pay;
            }
        }

        private string _VirtualPaymentClientURL_Query;
        public string VirtualPaymentClientURL_Query
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_VirtualPaymentClientURL_Query))
                {
                    _VirtualPaymentClientURL_Query = GetConfiguration(VIRTUAL_PAYMENT_CLIENT_URL_QUERY);
                }

                return _VirtualPaymentClientURL_Query;
            }
        }

        private string _SecureSecret;
        public string SecureSecret
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_SecureSecret))
                {
                    _SecureSecret = GetConfiguration(SECURE_SECRET);
                }

                return _SecureSecret;
            }
        }

        private string _MerchantAccessCode;
        public string MerchantAccessCode
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_MerchantAccessCode))
                {
                    _MerchantAccessCode = GetConfiguration(MERCHANT_ACCESS_CODE);
                }

                return _MerchantAccessCode;
            }
        }

        private string _MerchantID;
        public string MerchantID
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_MerchantID))
                {
                    _MerchantID = GetConfiguration(MERCHANT_ID);
                }

                return _MerchantID;
            }
        }

        private string _MerchantAdminUser;
        public string MerchantAdminUser
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_MerchantAdminUser))
                {
                    _MerchantAdminUser = GetConfiguration(MERCHANT_ADMIN_USER);
                }

                return _MerchantAdminUser;
            }
        }

        private string _MerchantAdminPassword;
        public string MerchantAdminPassword
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_MerchantAdminPassword))
                {
                    _MerchantAdminPassword = GetConfiguration(MERCHANT_ADMIN_PASSWORD);
                }

                return _MerchantAdminPassword;
            }
        }

        private int _pendingPaymentTimeOutHours;
        public int PendingPaymentTimeOutHours
        {
            get
            {
                if (_pendingPaymentTimeOutHours == 0)
                {
                    _pendingPaymentTimeOutHours = int.Parse(GetConfiguration(PENDING_PAYMENT_TIMEOUT_HOURS));
                }
                return _pendingPaymentTimeOutHours;
            }
        }

        private string _financialReportInternalHost;
        public string FinancialReportInternalHost
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_financialReportInternalHost))
                {
                    _financialReportInternalHost = GetConfiguration(FINANCIAL_REPORT_INTERNAL_HOST);
                }

                return _financialReportInternalHost;
            }
        }

        private string _financialReportInternalUsername;
        public string FinancialReportInternalUsername
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_financialReportInternalUsername))
                {
                    _financialReportInternalUsername = GetConfiguration(FINANCIAL_REPORT_INTERNAL_USERNAME);
                }

                return _financialReportInternalUsername;
            }
        }

        private string _financialReportInternalPassword;
        public string FinancialReportInternalPassword
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_financialReportInternalPassword))
                {
                    _financialReportInternalPassword = GetConfiguration(FINANCIAL_REPORT_INTERNAL_PASSWORD);
                }

                return _financialReportInternalPassword;
            }
        }

        private string _accountSummary_SubscriptionDate_ParseExactFormat;
        public string AccountSummary_SubscriptionDate_ParseExactFormat
        {
            get
            {
                if (string.IsNullOrEmpty(_accountSummary_SubscriptionDate_ParseExactFormat))
                {
                    _accountSummary_SubscriptionDate_ParseExactFormat = GetConfiguration(ACCOUNTSUMMARY_SUBSCRIPTIONDATE_PARSEEXACTFORMAT);
                }
                return _accountSummary_SubscriptionDate_ParseExactFormat;
            }
        }
    }
}
