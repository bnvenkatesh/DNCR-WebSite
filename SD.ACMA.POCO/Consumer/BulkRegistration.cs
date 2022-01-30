using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SD.ACMA.POCO.Consumer
{
    public class BulkRegistration
    {        
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public byte[] PhoneNumbersFile { get; set; }
        public byte[] FaxNumbersFile { get; set; }
        public string PhoneFaxNumber { get; set; }
        public string OrganisationName { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string Postcode { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public byte[] EvidenceFile { get; set; }
        public SD.ACMA.DNCR.Infrastructure.Enums.OperationTypeEnum OperationType { get; set; }
        public SD.ACMA.DNCR.Infrastructure.Enums.PreferredContactMethodEnum PreferredContactMethod { get; set; }
    }
}
