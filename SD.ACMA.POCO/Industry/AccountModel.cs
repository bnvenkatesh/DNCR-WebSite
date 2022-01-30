using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SD.ACMA.POCO.Industry
{
    public class AccountModel
    {
        public int AccountID { get; set; }
        public string Type { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string PhoneNumber2 { get; set; }
        public string OrganisationPhoneNumber { get; set; }
        public string OrganisationName { get; set; }
        public SD.ACMA.DNCR.Infrastructure.Enums.BusinessSectorEnum? BusinessSector { get; set; }
        //public SD.ACMA.DNCR.Infrastructure.Enums.BusinessTypeEnum? BusinessType { get; set; }
        public string PrincipalActivity { get; set; }
        public string ABN { get; set; }
        public string ACN { get; set; }
        public string State { get; set; }
        public string Industry { get; set; }
        public string CompanyDepartment { get; set; }
        public string Username { get; set; }
        public bool CanSeeWashQuote { get; set; }
        public SD.ACMA.DNCR.Infrastructure.Enums.AccountUserStatusEnum? Status { get; set; }
        public bool Locked { get; set; }

        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string FaxNumber { get; set; }
        public string Postcode { get; set; }
        public string OrganisationWebsite { get; set; }
        public bool HasABN { get; set; }
        public bool HasACN { get; set; }
        public string MobileNumber { get; set; }
        public bool IsDailySummary { get; set; }
        public int AccountUserID { get; set; }

        public int WashingCredits50Count { get; set; }
        public int WashingCredits20Count { get; set; }
    }
}
