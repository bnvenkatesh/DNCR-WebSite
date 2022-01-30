using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SD.ACMA.DNCR.Infrastructure.Configuration;

namespace SD.ACMA.DNCRProject.Website.Helpers
{
    public static class SessionHelper
    {
        public static int AccountId
        {
            get
            {
                return HttpContext.Current.Session[ConfigurationHelper.ACCOUNT_ID] != null ? int.Parse(HttpContext.Current.Session[ConfigurationHelper.ACCOUNT_ID].ToString()) : 0;
            }
            set
            {
                HttpContext.Current.Session[ConfigurationHelper.ACCOUNT_ID] = value;
            }
        }

        public static int AccountUserId
        {
            get
            {
                return HttpContext.Current.Session[ConfigurationHelper.ACCOUNT_USER_ID] != null ? int.Parse(HttpContext.Current.Session[ConfigurationHelper.ACCOUNT_USER_ID].ToString()) : 0;
            }
            set
            {
                HttpContext.Current.Session[ConfigurationHelper.ACCOUNT_USER_ID] = value;
            }
        }

        public static bool IsAdmin
        {
            get
            {
                return HttpContext.Current.Session[ConfigurationHelper.IS_ADMIN] != null && (bool)HttpContext.Current.Session[ConfigurationHelper.IS_ADMIN];
            }
            set
            {
                HttpContext.Current.Session[ConfigurationHelper.IS_ADMIN] = value;
            }
        }

        public static int? AgentId
        {
            get
            {
                return HttpContext.Current.Session[ConfigurationHelper.AGENT_ID] != null ? int.Parse(HttpContext.Current.Session[ConfigurationHelper.AGENT_ID].ToString()) : (int?)null;
            }
            set
            {
                HttpContext.Current.Session[ConfigurationHelper.AGENT_ID] = value;
            }
        }

        public static string CompanyName
        {
            get
            {
                return HttpContext.Current.Session[ConfigurationHelper.COMPANY_NAME] != null ? HttpContext.Current.Session[ConfigurationHelper.COMPANY_NAME].ToString() : null;
            }
            set
            {
                HttpContext.Current.Session[ConfigurationHelper.COMPANY_NAME] = value;
            }
        }

        public static string UserEmail
        {
            get
            {
                return HttpContext.Current.Session[ConfigurationHelper.USER_EMAIL] != null ? HttpContext.Current.Session[ConfigurationHelper.USER_EMAIL].ToString() : null;
            }
            set
            {
                HttpContext.Current.Session[ConfigurationHelper.USER_EMAIL] = value;
            }
        }

        public static bool RequireLogout
        {
            get
            {
                return HttpContext.Current.Session[ConfigurationHelper.REQUIRE_LOGOUT] != null && (bool)HttpContext.Current.Session[ConfigurationHelper.REQUIRE_LOGOUT];
            }
            set
            {
                HttpContext.Current.Session[ConfigurationHelper.REQUIRE_LOGOUT] = value;
            }
        }

        public static bool Below50Credit
        {
            get
            {
                return HttpContext.Current.Session[ConfigurationHelper.BELOW_50_CREDIT] != null && (bool)HttpContext.Current.Session[ConfigurationHelper.BELOW_50_CREDIT];
            }
            set
            {
                HttpContext.Current.Session[ConfigurationHelper.BELOW_50_CREDIT] = value;
            }
        }

        public static bool Below20Credit
        {
            get
            {
                return HttpContext.Current.Session[ConfigurationHelper.BELOW_20_CREDIT] != null && (bool)HttpContext.Current.Session[ConfigurationHelper.BELOW_20_CREDIT];
            }
            set
            {
                HttpContext.Current.Session[ConfigurationHelper.BELOW_20_CREDIT] = value;
            }
        }

        public static bool IsAcma
        {
            get
            {
                return HttpContext.Current.Session[ConfigurationHelper.IS_ACMA] != null && (bool)HttpContext.Current.Session[ConfigurationHelper.IS_ACMA];
            }
            set
            {
                HttpContext.Current.Session[ConfigurationHelper.IS_ACMA] = value;
            }
        }

        public static void SetLoginSessions(int accountId, int accountUserId, bool isAdmin, string userEmail)
        {
            AccountId = accountId;
            AccountUserId = accountUserId;
            IsAdmin = isAdmin;
            UserEmail = userEmail;
            RequireLogout = false;
        }

        public static void ClearSessions()
        {
            HttpContext.Current.Session[ConfigurationHelper.ACCOUNT_ID] = null;
            HttpContext.Current.Session[ConfigurationHelper.ACCOUNT_USER_ID] = null;
            HttpContext.Current.Session[ConfigurationHelper.IS_ADMIN] = null;
            HttpContext.Current.Session[ConfigurationHelper.AGENT_ID] = null;
            HttpContext.Current.Session[ConfigurationHelper.USER_EMAIL] = null;
            HttpContext.Current.Session[ConfigurationHelper.COMPANY_NAME] = null;
            HttpContext.Current.Session[ConfigurationHelper.REQUIRE_LOGOUT] = null;
            HttpContext.Current.Session[ConfigurationHelper.IS_ACMA] = null;
            HttpContext.Current.Session[ConfigurationHelper.BELOW_50_CREDIT] = null;
            HttpContext.Current.Session[ConfigurationHelper.BELOW_20_CREDIT] = null;
        }
    }
}