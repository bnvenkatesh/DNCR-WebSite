using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SD.ACMA.POCO.Base;
using SD.ACMA.POCO.Consumer;

namespace SD.ACMA.InterfaceTier
{
    public interface IConsumerDataInterchange
    {
        RegistrationResult Register(Registration reg);

        RegistrationResult RegisterGovtOrBusinessFax(Registration reg);

        RegistrationRequestResult CheckRegistration(string email, List<string> numbers);

        BulkRegistrationResponse BulkRegistration(BulkRegistration bulkRegistration);        

        RegistrationConfirmation ConfirmRegistration(string activationToken);

        GenericResponseModel SendAttachMailResponse(AttachMailResponseRequest consumerEmail);

        LodgeEnquiryResponse LodgeEnquiry(LodgeEnquiryModel lodgeEnquiryModel, int? agentId);

        LodgeComplaintResponse LodgeComplaint(LodgeComplaintModel lodgeComplaintModel);

        ImpersonateCSRResponse ImpersonateCSR(string secureData);

        GetServiceProvidersResponse GetServiceProviders();

        GetCountriesResponse GetCountries();
    }
}
