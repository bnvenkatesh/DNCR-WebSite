using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SD.ACMA.POCO.Base
{
    public class LodgeComplaintModel
    {
        public ContactDetailsModel ContactDetails { get; set; }
        public ComplaintRequestModel ComplaintRequest { get; set; }
        public List<CallIncidentModel> CallIncidents { get; set; }
    }

    public class ContactDetailsModel
    {
        public string Addressline1 { get; set; }
        public string Addressline2 { get; set; } 
        public string Company { get; set; } 
        public string Country { get; set; } 
        public string Email { get; set; } 
        public string FirstName { get; set; } 
        public string LastName { get; set; } 
        public string PhoneNumber { get; set; } 
        public string Postcode { get; set; }
        public string State { get; set; } 
        public string Suburb { get; set; }
        public string Title { get; set; } 
    }

    public class CallIncidentModel
    {
        public string AMPM { get; set; }
        public byte[] Attachment { get; set; }   //If Attachment is not empty then it is a fax incident   
        public string AttachmentFileName { get; set; }
        public DateTime CallDate { get; set; }
        //public string CallDetails { get; set; }
        public string AdditionalDetails { get; set; }
        public string CallDescription { get; set; }
        public bool CallPurposeAdvertise { get; set; }
        public bool CallPurposeCaller { get; set; }        
        public bool CallPurposeDonation { get; set; }       
        public bool CallPurposeDonationForPolitics { get; set; }
        public bool CallPurposeOther { get; set; }
        public string CallPurposeOtherDetails { get; set; }
        public bool CallPurposePoll { get; set; }
        public bool CallPurposeScam { get; set; }
        public bool CallPurposeVoting { get; set; }
        public string CallReceiveNumber { get; set; } 
        public bool? CallWasTerminated { get; set; } 
        public string NumberOrTextDisplayed { get; set; } 
        public bool? RequestedCallTermination { get; set; } 
        public string Time { get; set; }
        public SD.ACMA.DNCR.Infrastructure.Enums.VoiceCallEnum VoiceCall { get; set; }
        public string VoiceCallOther { get; set; }
        public SD.ACMA.DNCR.Infrastructure.Enums.WasCallerContactableEnum? WasCallerIDContactable { get; set; } 
        public string WasCallerIDContactableOther { get; set; }
        public SD.ACMA.DNCR.Infrastructure.Enums.WasCallerIDDisplayedEnum? WasCallerIDDisplayed { get; set; }
        public bool IsFax { get; set; }
        public string AnswerForCallTermination { get; set; }        
    }    

    public class ComplaintRequestModel
    { 
        public bool? AdditionalAssist { get; set; }
        public string AdditionalDetailsConsentWithdrawn { get; set; }
        public int? AgentUserId { get; set; }        
        public bool Anonymous { get; set; }
        public SD.ACMA.DNCR.Infrastructure.Enums.CallTypeEnum CallType { get; set; }
        public string CallTypeOther { get; set; }
        public SD.ACMA.DNCR.Infrastructure.Enums.ChannelsEnum Channel { get; set; }
        public string ComplaintDetails { get; set; }
        public SD.ACMA.DNCR.Infrastructure.Enums.ComplaintTypeEnum ComplaintType { get; set; }
        public string ComplaintTypeOther { get; set; }
        public SD.ACMA.DNCR.Infrastructure.Enums.ConsentToBeContactedEnum? ConsentToBeContacted { get; set; }
        public SD.ACMA.DNCR.Infrastructure.Enums.ConsentToBeContactedEnum? OnlineSurveyCompletion { get; set; }
        public string OnlineSurveyAdditionalDetails { get; set; }
        public bool? ConsentWithdrawn { get; set; }
        public bool? ContactBusiness { get; set; }
        public bool? ContactOtherParty { get; set; }
        public bool? ContactServiceProvider { get; set; }
        public string ContactingBusinessDetails { get; set; }        
        public string ContactingBusinessName { get; set; }
        public DateTime? DateConsentWithdrawn { get; set; }
        public SD.ACMA.DNCR.Infrastructure.Enums.DesignatedPartyEnum? DesignatedParty { get; set; }
        public string FurtherInformation { get; set; }
        public string ProductOffered { get; set; }
        public string ProductProvider { get; set; }
        public string ProductProviderDetails { get; set; }
        public string ReceivingCallNumber { get; set; }
        public int? ServiceProviderID { get; set; }
        public string ServiceProviderOther { get; set; }
        public string SubjectACMA { get; set; }
        public bool? IsNumberOnRegister { get; set; }
        public bool IsConsumerComplaint { get; set; }
        public bool? HaveDestinationNumber { get; set; }
        public string DestinationNumber { get; set; }
        public bool? IsBusinessNumber { get; set; }
    }
}
