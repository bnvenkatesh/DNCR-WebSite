using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SD.ACMA.DNCR.Infrastructure
{
    public class Enums
    {
        public enum Media
        {
            Twitter = 1,
            Facebook = 2,
            Instagram = 3,
            Vine = 4,
            YahooNews = 5,
            YouTube = 6
        }

        public const string TYPE_STATUS = "status";
        public const string TYPE_PHOTO = "photo";
        public const string TYPE_VIDEO = "video";

        public enum BusinessSectorEnum
        {
            CommonwealthDepartment = 1,
            OtherCommonwealthAgency = 2,
            StateGovernment = 3,
            LocalGovernment = 4,
            Company = 5,
            CommunityorVolunteerGroup = 6,
            Person = 7
        }

        public enum BusinessTypeEnum
        {
            Telemarketing = 1,
            FaxMarketing = 2,
            Both = 3           
        }

        public enum AccountUserStatusEnum
        {
            Pending = 1,
            Active = 2,
            Inactive = 3,
            Deleted = 4            
        }

        public enum OperationTypeEnum
        {
            Register = 1,
            Deregister = 2,
            Check = 3
        }

        public enum PreferredContactMethodEnum
        {
            Email = 1,
            Phone = 2,
            Post = 3
        }

        public enum StateEnum
        {
            NewSouthWales = 1,            
            NorthernTerritory = 2,
            AustralianCapitalTerritory = 3,
            Tasmania = 4,
            SouthAustralia = 5,
            WesternAustralia = 6,
            Queensland = 7,
            Victoria = 8,
        }

        public enum TitleEnum
        {
            Mr = 1,
            Mrs = 2,
            Miss = 3,
            Dr = 4,
            Ms = 5,
            None = 6,
        }

        public enum WashFileTypeEnum
        {
            Original = 0,
            AllInOne = 1,            
            Registered = 2,            
            UnRegistered = 3,            
            Invalid = 4
        }

        public enum WashingStatusEnum
        {
            NotStarted = 1,           
            Processing = 2,
            Complete = 3
        }

        public enum WashingChannelEnum
        {
            OnlineQuickCheck = 1,           
            OnlineFileUpload = 2,
            AutomatedWashingService = 3,
            RealTimeAccess = 4,
        }

        public enum PaymentMethodEnum
        {
            CreditCard = 1,            
            BankTransfer = 2,
            Unknown = 3,
            BalanceFundTransfer = 4,
            BPay = 5,
        }

        public enum OrderLineStatusEnum
        {
            Paid = 1,
            NotPaid = 2,
            Refunded = 3,
        }

        public enum OrderTransactionTypeEnum
        {
            OrderActivated = 1,
            SubscriptionActivated = 2,
            CreditAccountBalance = 3,
            DebitAccountBalance = 4,
            PaymentReceivedByCreditCard = 5,
            PaymentReceivedByEFT = 6,
            DeferredCreditCardPaymentReceived = 7,
            CreditOrderBalance = 8,
            DebitOrderBalance = 9,
            OrderCancelled = 10,
            OrderClosed = 11,
            RefundOrder = 12,
            RefundSubscription = 13,
            PaymentAttemptFailedByCreditCard = 14,
        }

        public enum OrderStatusEnum 
        {
            NotPaid = 1,
            FullyPaid = 2,
            PartPaid = 3,
            Closed = 4,
            Expired = 5,
            Cancelled = 6,
            Refunded = 7,
            PartiallyRefunded = 8
        }

        public enum AccountAdjustmentTypeEnum
        {
            ACMABalanceAdjustment = 1,
            ACMAWashCreditAdjustment = 2,
            ReserveAccountBalance = 3,
            ApprovedAccountBalanceRefund = 4,
            DeniedAccountBalanceRefund = 5,
            ReserveWashCredit = 6,
            ApprovedSubscriptionRefund = 7, 
            DeniedSubscriptionRefund = 8,
            ReverseWash = 9,
            WashCreditsRollover = 10
        }

        public enum AccountTransactionTypeEnum
        {
            OrderActivated = 1,
            SubscriptionActivated = 2,
            CreditAccountBalance = 3,
            DebitAccountBalance = 4,
            PaymentReceivedByCreditCard = 5,
            PaymentReceivedByEFT = 6,
            DeferredCreditCardPaymentReceived = 7,
            BalanceRefund = 8,
            OrderCancelled = 9,
            OrderClosed = 10,
            RefundOrder = 11,
            RefundSubscription = 12,
            WashingCreditsExpired = 13,
            ACMABalanceAdjustment = 14,
            ACMAWashCreditAdjustment = 15,
            ReverseWash = 16,
            WashCreditsRollover = 17,
            PaymentAttemptFailedByCreditCard = 18,
        }

        public enum ChannelsEnum
        {
            OasisIVR = 1,
            Symposium = 2,
            Agent = 3,
            Webform = 4,
            Other = 5,
            Phone = 6,
            Email = 7,
            Letter = 8,
            Fax = 9,
            ACMA = 10,
            Split = 11
        }

        public enum ComplaintTypeEnum
        {
            Call = 1,
            Registration = 2,
            Access = 3,
            Other = 4,
            Industry = 5,
            Fax = 6
        }

        public enum OtherComplaintTypeEnum
        {
            Enquiry,
            Registration,
            Spam,
            Other
        }

        public enum VoiceCallEnum 
        {
            ConversationWithARealPerson = 1,
            RecordedMessageOrSyntheticVoice = 2,
            MessageLeftOnAnsweringMachine = 3,
            Silence = 4,
            DidNotAnswerTheCall = 5,
            Other = 6,
        }

        public enum WasCallerContactableEnum 
        {
            MadeContact = 1,
            CouldNotContact = 2,
            LeftAMessage = 3,
            GotSomeInformation = 4,
            DidNotAttempt = 5,
            Other = 6
        }

        public enum WasCallerIDDisplayedEnum
        {
            PhoneDoesNotDisplayCallerID = 1,
            Yes = 2,
            No = 3,
            DidNotNotice = 4,
            YesButDidNotCapture = 5
        }

        public enum CallTypeEnum
        {
            PromotingGoodsServicesOrFinancialOpportunities = 1,
            AskingForDonations = 2,
            MarketResearchPollOrSurvey = 3,
            ProductRecall = 4,
            FaultRectification = 5,
            AppointmentRescheduling = 6,
            AppointmentReminder = 7,
            CallRelatingToAPaymentOrOverdueAccount = 8,
            Other = 9,
            VotingInUpcomingElection = 10,
            Scam = 11
        }

        public enum ConsentToBeContactedEnum
        {
            Yes = 1,
            No = 2,
            Unsure = 3
        }

        public enum DesignatedPartyEnum
        {
            GovernmentBody = 1,
            RegisteredCharity = 2,
            PoliticalPartyMemberOrCandidate = 3,
            EducationalInstitution = 4,
            None = 5
        }

        public enum PaymentStatusEnum
        {
            SUCCESS,
            DECLINED,
            UNKNOWN,
            TIMEOUT
        }

        public enum RefundDecisionEnum
        {
            InProgress = 1,
            Approved = 2,
            Denied = 3
        }
               
        public enum EnquiryTypeEnum
        {
            Business = 1,
            Consumer = 2,
            Other = 3,
        }

        public enum EnquiryTelemarketerTypeEnum
        {
            None = 1,
            BalanceRefund = 2,
            BalanceTransfer = 3,
            WashCreditsRollover = 4,
            WashCreditsRefund = 5,
            WashCreditsToBalanceRefund = 6,
        }

        public enum RefundTypeEnum
        {
            SubscriptionRefund = 1,
            WashCreditsRollOver = 2,
            ReverseWash = 3,
            BalanceRefund = 4,
            ManualAdjustment = 6,
        }

        public enum OrganisationEnum
        {
            IVE = 8,
            ACMA = 9
        }

        public enum OrganisationGroupEnum
        {
            CSR = 7,
            SeniorCSR = 8,
            DNCR = 9,
            UCCS = 10,
        }

        public enum FinancialReportType
        {
            PaymentsReconciliation,
            Exceptions,
            CreditNotes
        }

        public enum AccountStatusEnum
        {
            Active = 2,
            Suspended = 3,
            Pending = 4,
            ClosedRejected = 5,
            Closed = 7,
            PendingApproval = 8,
        }

        public enum EnquryStatusEnum
        {
            Open = 1,
            Closed = 2,
            Deregistered = 3,
            OrganisationOwnerChanged = 4,
            UserOwnerChanged = 5,
            AwaitingFurtherInformation = 6,
        }
    }
}
