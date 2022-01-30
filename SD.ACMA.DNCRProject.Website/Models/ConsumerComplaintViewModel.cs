using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Web;
using SD.ACMA.DNCRProject.Website.Helpers;
using System.Web.Mvc;

namespace SD.ACMA.DNCRProject.Website.Models
{
    public class ConsumerComplaintViewModel
    {
        [Display(Name = "What is the complaint about?")]
        [Required(ErrorMessage = "Please identify what your complaint is about to proceed")]
        public SD.ACMA.DNCR.Infrastructure.Enums.ComplaintTypeEnum? ComplaintType { get; set; }

        [Display(Name = "What was the call about?")]
        [Required(ErrorMessage = "Please identify what the call was about to proceed")]
        public string CallType { get; set; }

        [Display(Name = "Specify other")]
        [Required(ErrorMessage = "Please specify what the call was about")]
        [StringLength(400, ErrorMessage = "Other cannot be longer than 400 characters")]
        public string CallOtherDescription { get; set; }

        [Display(Name = "Specify something else")]
        [Required(ErrorMessage = "Please identify what your complaint is about to proceed")]
        public SD.ACMA.DNCR.Infrastructure.Enums.OtherComplaintTypeEnum? OtherComplaintType { get; set; }

        [Display(Name = "Please treat my complaint as anonymous")]
        public bool IsAnonymous { get; set; }

        [Display(Name = "Title")]
        public string Title { get; set; }

        [Display(Name = "First Name")]
        [Required(ErrorMessage = "Please enter your First Name e.g. John")]
        [StringLength(50, ErrorMessage = "First Name cannot be longer than 50 characters")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        [Required(ErrorMessage = "Please enter your Last Name e.g. Smith")]
        [StringLength(50, ErrorMessage = "Last Name cannot be longer than 50 characters")]
        public string LastName { get; set; }

        [Display(Name = "Address Line 1")]
        [StringLength(50, ErrorMessage = "Address Line 1 cannot be longer than 50 characters")]
        public string AddressLine1 { get; set; }

        [Display(Name = "Address Line 2")]
        [StringLength(50, ErrorMessage = "Address Line 2 cannot be longer than 50 characters")]
        public string AddressLine2 { get; set; }

        [Display(Name = "City or Locality")]
        [StringLength(50, ErrorMessage = "City or Locality cannot be longer than 50 characters")]
        public string City { get; set; }

        [Display(Name = "Postcode")]
        //[RegularExpression(@"^[0-9]{4}$", ErrorMessage = "The number you have entered is not an Australian Postcode")]
        [Required(ErrorMessage = "Please enter your Postcode e.g. 2060")]
        public string Postcode { get; set; }

        [Display(Name = "State")]
        [StringLength(50, ErrorMessage = "State cannot be longer than 50 characters")]
        public string State { get; set; }

        [Display(Name = "Country")]
        [Required(ErrorMessage = "Please select Country")]
        public string Country { get; set; }

        [Display(Name = "Email<span class='visuallyhidden'> Please ensure you enter a valid email address e.g. john.smith@domain.com</span>")]
        [StringLength(200, ErrorMessage = "Email cannot be longer than 200 characters")]
        [RegularExpression(@"\b[a-zA-Z0-9'._%+-]+@[a-zA-Z0-9.+-]+\.[a-zA-Z]{2,}\b", ErrorMessage = "This does not appear to be a valid email address e.g. john.smith@domain.com")]
        public string Email { get; set; }

        [Display(Name = "Phone<span class='visuallyhidden'> Numbers must be 10 digits beginning with 0. Numbers starting with 18 must be 7 or 10 digits. Numbers starting with 13 must be 6, 8 or 10 digits. E.g. 0212345678 or 0412345678</span>")]
        [RegularExpression(@"^(([ ().-]*(0)([ ().-]*[0-9][ ().-]*){9})|([ ().-]*(18)([ ().-]*[0-9][ ().-]*){5}(([ ().-]*[0-9][ ().-]*){3})?)|([ ().-]*(13)([ ().-]*[0-9][ ().-]*){4}(([ ()-]*[0-9][ ()-]*){2})?(([ ()-]*[0-9][ ()-]*){2})?))$", ErrorMessage = "The number you have provided is not valid. <br/>Numbers must start with 0, 13 or 18. <br/>Numbers starting with 0 must be 10 digits. <br/>Numbers starting with 18 must be 7 or 10 digits. <br/>Numbers starting with 13 must be 6, 8 or 10 digits. <br/>Optional spaces or - dashes or ( ) brackets are allowed. <br/>Letters (a-z), plus (+) and other characters are not accepted. E.g. 02 1234 5678 or 0412 345 678")]
        public string ContactNumber { get; set; }
      
        [Required(ErrorMessage = "Please provide the phone number you received the call on / received the fax on e.g. 02 1234 5678 or 0412 345 678")]
        [Display(Name = "ctrllabel<span class='visuallyhidden'> This should be the phone number that the caller dialled to contact you. Numbers must be 10 digits beginning with 0. Numbers starting with 18 must be 7 or 10 digits. Numbers starting with 13 must be 6, 8 or 10 digits. E.g. 0212345678 or 0412345678</span>")]
        [RegularExpression(@"^(([ ().-]*(0)([ ().-]*[0-9][ ().-]*){9})|([ ().-]*(18)([ ().-]*[0-9][ ().-]*){5}(([ ().-]*[0-9][ ().-]*){3})?)|([ ().-]*(13)([ ().-]*[0-9][ ().-]*){4}(([ ()-]*[0-9][ ()-]*){2})?(([ ()-]*[0-9][ ()-]*){2})?))$", ErrorMessage = "The number you have provided is not valid. <br/>Numbers must start with 0, 13 or 18. <br/>Numbers starting with 0 must be 10 digits. <br/>Numbers starting with 18 must be 7 or 10 digits. <br/>Numbers starting with 13 must be 6, 8 or 10 digits. <br/>Optional spaces or - dashes or ( ) brackets are allowed. <br/>Letters (a-z), plus (+) and other characters are not accepted. E.g. 02 1234 5678 or 0412 345 678")]
        public string ComplainantNumber { get; set; }

        [Display(Name = "Phone Provider<span class='visuallyhidden'> This is the name of the business that provides the phone service for the number that was called</span>")]
        public string PhoneServiceProvider { get; set; }

        [Display(Name = "Specify other provider")]
        [StringLength(400, ErrorMessage = "Other provider cannot be longer than 400 characters")]
        public string PhoneServiceProviderOther { get; set; }

        [Display(Name = "Is this number on the Do Not Call Register?")]
        public bool? IsNumberOnRegister { get; set; }

        [Display(Name = "Is this number associated with a business and used primarily for business purposes?")]
        [Required(ErrorMessage = "Please specify if the number is associated with a business")]
        public bool? IsBusinessNumber { get; set; }
        
        [Display(Name = "Does the fax have a 'to' number (destination number) in the header?")]
        public bool? HaveDestinationNumber { get; set; }

        [Display(Name = "What is the destination number in the header?")]
        public string DestinationNumber { get; set; }

        [Display(Name = "Name of the business that called you?<span class='visuallyhidden'> If you were given a business name during the call, please provide it as it was given to you</span>")]
        [StringLength(200, ErrorMessage = "Name of the business cannot be longer than 200 characters")]
        public string BusinessNameCalled { get; set; }

        [Display(Name = "Additional details of the business that called you")]
        [StringLength(400, ErrorMessage = "Additional details cannot be longer than 400 characters")]
        public string DetailsBusinessCalled { get; set; }

        [Display(Name = "Name of the business whose product is being sold?<span class='visuallyhidden'> If you were given a business name during the call, please provide it as it was given to you</span>")]
        [StringLength(200, ErrorMessage = "Name of the business cannot be longer than 200 characters")]
        public string ProductBusinessName { get; set; }

        [Display(Name = "Additional details of the business whose product is being sold")]
        [StringLength(400, ErrorMessage = "Additional details cannot be longer than 400 characters")]
        public string DetailsProductBusinessName { get; set; }

        [Display(Name = "If known, name of the product or service being sold?")]
        [StringLength(200, ErrorMessage = "Name of the product cannot be longer than 200 characters")]
        public string ProductName { get; set; }

        [Display(Name = "Was the call made by any of these parties?")]
        public string CallParties { get; set; }

        [Display(Name = "Do you or have you had a business relationship with the company that called you?")]
        public string Consent { get; set; }

        [Display(Name = "Have you previously asked the company to stop calling (i.e. withdrawn consent)?")]
        public string WithdrawnConsent { get; set; }

        [Display(Name = "Have you entered an online competition or completed an online survey?")]
        [Required(ErrorMessage = "Please specify if you have entered an online competition or completed an online survey")]
        public string OnlineSurvey { get; set; }

        [Display(Name = "Additional details")]
        public string OnlineSurveyAddtionalDetails { set;get; }

        [Display(Name = "Date consent withdrawn (DD/MM/YYYY)")]
        [RegularExpression(@"^(((0?[1-9]|[12]\d|3[01])\/(0?[13578]|1[02])\/((1[6-9]|[2-9]\d)\d{2}))|((0?[1-9]|[12]\d|30)\/(0?[13456789]|1[012])\/((1[6-9]|[2-9]\d)\d{2}))|((0?[1-9]|1\d|2[0-8])\/0?2\/((1[6-9]|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0?[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "This does not appear to be a valid date e.g. 31/01/2015")]
        public string DateConsentWithdrawn { get; set; }

        [Display(Name = "Additional details about withdrawing of consent")]
        [StringLength(400, ErrorMessage = "Additional details cannot be longer than 400 characters")]
        public string AdditionalWithdrawnConsent { get; set; }

        [Display(Name = "Do you have a relationship with the caller or product provider (including consenting to receive calls from them)?")]
        public string FaxConsent { get; set; }

        [Display(Name = "Have you ever withdrawn consent?")]
        public string FaxWithdrawnConsent { get; set; }

        [Display(Name = "Date consent withdrawn (DD/MM/YYYY)")]
        [RegularExpression(@"^(((0?[1-9]|[12]\d|3[01])\/(0?[13578]|1[02])\/((1[6-9]|[2-9]\d)\d{2}))|((0?[1-9]|[12]\d|30)\/(0?[13456789]|1[012])\/((1[6-9]|[2-9]\d)\d{2}))|((0?[1-9]|1\d|2[0-8])\/0?2\/((1[6-9]|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0?[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "This does not appear to be a valid date e.g. 31/01/2015")]
        public string FaxDateConsentWithdrawn { get; set; }

        [Display(Name = "Additional details about withdrawing of consent")]
        [StringLength(400, ErrorMessage = "Additional details cannot be longer than 400 characters")]
        public string FaxAdditionalWithdrawnConsent { get; set; }

        public List<CallIncident> CallIncidents { get; set; } 

        public List<FaxIncident> FaxIncidents { get; set; }

        [Display(Name = "I agree, where applicable, that the ACMA may obtain information from my telephone service provider (for example, inbound calling records) to assist in handling my complaint.")]
        [Required(ErrorMessage = "Please ensure you have provided a response to all 3 Declaration questions")]
        public bool? AgreeTelephoneService { get; set; }

        [Display(Name = "I agree, where applicable, that my number may be passed on to the business I am complaining about for the purpose of handling my complaint.")]
        [Required(ErrorMessage = "Please ensure you have provided a response to all 3 Declaration questions")]
        public bool? AgreePassedToBusiness { get; set; }

        [Display(Name = "I agree, where applicable, for details of this complaint to be referred on to other Government agencies.")]
        [Required(ErrorMessage = "Please ensure you have provided a response to all 3 Declaration questions")]
        public bool? AgreeReferred { get; set; }

        //[Display(Name = "Please provide any other relevant information that you feel may assist the ACMA in handling this complaint.")]
        //[StringLength(4000, ErrorMessage = "Final comments cannot be longer than 4000 characters")]
        //public string FinalComments { get; set; }

        [Display(Name = "Channel")]
        [Required(ErrorMessage = "Please select a channel")]
        public string Channel { get; set; }

        public bool IsSubmitted { get; set; }
        public string RefCode { get; set; }

        public List<SelectListItem> CountryList { get; set; }

        public string RedirectUrl { get; set; }
    }

    public class CallIncident
    {
        [Display(Name = "Date of the call (DD/MM/YYYY)")]
        [Required(ErrorMessage = "Please provide the date you received the call e.g. 31/01/2015")]
        [RegularExpression(@"^(((0?[1-9]|[12]\d|3[01])\/(0?[13578]|1[02])\/((1[6-9]|[2-9]\d)\d{2}))|((0?[1-9]|[12]\d|30)\/(0?[13456789]|1[012])\/((1[6-9]|[2-9]\d)\d{2}))|((0?[1-9]|1\d|2[0-8])\/0?2\/((1[6-9]|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0?[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "This does not appear to be a valid date e.g. 31/01/2015")]
        [DateLessThanOrEqualToToday(ErrorMessage = "Date should not be greater than today")]
        public string DateOfCall { get; set; }

        [Display(Name = "Time of the call (best estimate)")]
        [Required(ErrorMessage = "Please provide the time you received the call")]
        public string TimeOfCall { get; set; }

        [Required(ErrorMessage = "Please provide the time you received the call")]
        public string CallAmPm { get; set; }
            
        [Display(Name = "The phone number that called you<span class='visuallyhidden'>This may be the number that displayed on your phone or the number that was provided to you during the call as the calling number</span>")]
        [StringLength(20, ErrorMessage = "The phone number cannot be longer than 20 characters")]
        [ValueNotEqualTo("ComplainantNumber", ErrorMessage = "You already said this was the number you received the call on in Section 2.")]
        public string CallFromNumber { get; set; }

        [Display(Name = "Which of the following best describes the call?")]
        [Required(ErrorMessage = "Please identify the best description of the call")]
        public string CallDescription { get; set; }

        [Display(Name = "Specify other")]
        [StringLength(400, ErrorMessage = "Other cannot be longer than 400 characters")]
        public string OtherCallDescription { get; set; }

        [MinimumElements(1, ".checkboxgroup input[type=checkbox]:checked", ErrorMessage = "Please identify the purpose of the call")]
        public string MinimumCallPurpose { get; set; }

        [Display(Name = "To offer, advertise or promote goods or services or financial opportunities")]
        public bool CallPurposeAdvertise { get; set; }
        [Display(Name = "Were the goods or services those of the caller?")]
        public bool CallPurposeCaller { get; set; }
        [Display(Name = "To ask for donations")]
        public bool CallPurposeDonation { get; set; }
        [Display(Name = "Were the donations fundraising for an election or political party?")]
        public bool CallPurposeDonationForPolitics { get; set; }
        [Display(Name = "To carry out a poll or survey")]
        public bool CallPurposePoll { get; set; }
        [Display(Name = "Believe the call was a scam")]
        public bool CallPurposeScam { get; set; }
        [Display(Name = "Voting in an upcoming election")]
        public bool CallPurposeElection { get; set; }
        [Display(Name = "Other")]        
        public bool CallPurposeOther { get; set; }

        [Display(Name = "Specify other")]
        [Required(ErrorMessage = "Please describe the purpose of the call")]
        [StringLength(400, ErrorMessage = "Other cannot be longer than 400 characters")]
        public string OtherPurpose { get; set; }

        //[Display(Name = "Feel free to describe your concerns or simply list the information not provided using A - K.")]
        //[StringLength(4000, ErrorMessage = "Call details cannot be longer than 4000 characters")]
        //public string CallDetails { get; set; }

        [Display(Name = "At any stage did you ask for the call to be ended?")]
        public bool? AskedCallEnded { get; set; }

        [Display(Name = "What did you say to the caller to indicate you wanted the call to be ended?")]
        [StringLength(4000, ErrorMessage = "Answer For Call Termination cannot be longer than 4000 characters")]
        public string AnswerForCallTermination { get; set; }

        [Display(Name = "Was the call ended immediately?")]
        public bool? WasCallEnded { get; set; }

        [Display(Name = "Was a number or word displayed on your phone during the call?")]
        public string HasCallerId { get; set; }

        [Display(Name = "What was the number or word displayed during the call?")]
        [StringLength(50, ErrorMessage = "The number or word displayed cannot be longer than 50 characters")]
        [ValueNotEqualTo("ComplainantNumber", ErrorMessage = "You already said this was the number you received the call on in Section 2.")]
        public string CallerIdNumber { get; set; }

        [Display(Name = "Did you try to contact the number displayed during the call (not another contact number provided by the caller)?")]
        public string CallerIdContactable { get; set; }

        [Display(Name = "Specify other")]
        [StringLength(400, ErrorMessage = "Other cannot be longer than 400 characters")]
        public string CallerIdContactableOther { get; set; }

        //[Display(Name = "Additional details of the call")]
        //[StringLength(4000, ErrorMessage = "Additional details cannot be longer than 4000 characters")]
        //public string AdditionalDetails { get; set; }

        [Display(Name = "Please provide a summary of the call(s) which are the basis for this complaint. Please be as detailed as possible.")]
        [StringLength(4000, ErrorMessage = "Call Summary cannot be longer than 4000 characters")]
        public string CallSummary { get; set; }

        public string ComplainantNumber { get; set; }
    }

    public class FaxIncident
    {
        [Display(Name = "Date fax received (DD/MM/YYYY)")]
        [Required(ErrorMessage = "Please provide the date you received the fax e.g. 31/01/2015")]
        [RegularExpression(@"^(((0?[1-9]|[12]\d|3[01])\/(0?[13578]|1[02])\/((1[6-9]|[2-9]\d)\d{2}))|((0?[1-9]|[12]\d|30)\/(0?[13456789]|1[012])\/((1[6-9]|[2-9]\d)\d{2}))|((0?[1-9]|1\d|2[0-8])\/0?2\/((1[6-9]|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0?[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "This does not appear to be a valid date e.g. 31/01/2015")]
        [DateLessThanOrEqualToToday(ErrorMessage = "Date should not be greater than today")]
        public string DateOfFax { get; set; }

        [Display(Name = "Time fax received")]
        [Required(ErrorMessage = "Please provide the time you received the fax")]
        public string TimeOfFax { get; set; }

        [Required(ErrorMessage = "Please provide the time you received the fax")]
        public string FaxAmPm { get; set; }

        [Display(Name = "Attach a copy of the fax, if you can")]
        public HttpPostedFileBase FaxFileUpload { get; set; }

        [Display(Name = "Please provide a summary of the fax(s) which are the basis for this complaint. Please be as detailed as possible.")]
        [StringLength(4000, ErrorMessage = "Fax Summary cannot be longer than 4000 characters")]
        public string FaxSummary { get; set; }
    }
}