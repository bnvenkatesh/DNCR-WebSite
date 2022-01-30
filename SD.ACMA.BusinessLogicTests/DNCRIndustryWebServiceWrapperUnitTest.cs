using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SD.ACMA.BusinessLogic.Avanade;
using SD.ACMA.BusinessLogic.Helpers;
using SD.ACMA.DatabaseIntermediary;
using Microsoft.Practices.Unity;
using SD.Core.Data.Repository.PetaPoco.Business.Interface;
using SD.Core.Data.Repository.PetaPoco.Business;
using SD.Core.Data.Repository.PetaPoco.UnityOfWorkContainer;
using SD.ACMA.InterfaceTier;
using SD.ACMA.POCO.Industry;
using SD.ACMA.POCO.Consumer;
using System.Linq;
using System.IO;

namespace SD.ACMA.BusinessLogicTests
{
    [TestClass]
    public class DNCRIndustryWebServiceWrapperUnitTest
    {
        private IIndustryDataInterchange _industryDataInterchange;
        private IConsumerDataInterchange _consumerDataInterchange;

        [TestInitialize]
        public void Init()
        {
            IUnityContainer container = new UnityContainer()
                .RegisterType<IRepository, PetaPocoRepository>()
                .RegisterType<IUnitOfWorkProvider, PetaPocoUnitOfWorkProvider>()
                .RegisterType<ICreditCardPaymentDataRepository, CreditCardPaymentDataRepository>()
                .RegisterType<ISiteLoggingService, SiteLoggingService>()
                .RegisterType<IFileHelper , FileHelper>()
                .RegisterType<IIndustryDataInterchange, DNCRIndustryWebServiceWrapper>()
                .RegisterType<IConsumerDataInterchange, DNCRConsumerWebServiceWrapper>();


            this._industryDataInterchange = container.Resolve<IIndustryDataInterchange>();
            this._consumerDataInterchange = container.Resolve<IConsumerDataInterchange>();
                
        }

        [TestMethod]
        public void TestLogin()
        {
            var response = _industryDataInterchange.Login("sun.sun@gmail.com", "Sun12345678910");

            Assert.IsTrue(response.Errors == null);
        }

        [TestMethod]
        public void TestCreateOrder()
        {
            var response = _industryDataInterchange.CreateOrder(35598, 0, "test@gmail.com", false, false,
                new System.Collections.Generic.List<SubscriptionVsQuantityModel>()
                {
                    new SubscriptionVsQuantityModel{Quantity = 1,SubscriptionID = 2}
                },
                  new UserContextModel
                 {
                     AccountUserID = 35635,
                      AgentID = 1024 }
                 );
            Assert.IsTrue(response.Errors == null);
        }

        [TestMethod]
        public void TestGetAllSubscritions()
        {
            var response = _industryDataInterchange.GetAvailableSubscriptions(35598);
            Assert.IsTrue(response.Errors == null);
        }

        [TestMethod]
        public void CreateAccountTest()
        {
            string business = Guid.NewGuid().ToString("N").Substring(0, 5);
            var response = _industryDataInterchange.CreateAccount(new CreateAccountModel
            {
                Account_Model = new AccountModel
                {
                    FirstName = "webFname",
                    LastName = "webLname",
                    AddressLine1 = "Ad1Test",
                    AddressLine2 = "ad2",
                    BusinessSector = DNCR.Infrastructure.Enums.BusinessSectorEnum.Person,
                    Country = "India",
                    Postcode = "503226",
                    State = "Hyd",
                    PhoneNumber = "0987654321",
                    OrganisationName = business,
                    Email = business+ "@gmail.com",
                    HasABN = false,
                    PrincipalActivity = "telephoning"


                },
                Account_User_Model = new AccountUserModel
                {
                    EmailAddress = business+"@gmail.com",
                    FirstName = "webFname",
                    LastName = "webLname",
                    PhoneNumber = "0987654321"
                },
                WashFormat = new WashingFormat
                {
                    WashResultFormatFileWithIndicators = true
                }
            });
            Assert.IsTrue(response.Errors == null);
            Assert.AreEqual(response.IsSuccessful, true);
       
        }

        [TestMethod]
         public void GetAccountUserTest()
        {
            var response = _industryDataInterchange.GetAccountUser(35635, new UserContextModel { AccountUserID = 35635, AgentID = 1024 });
            Assert.IsTrue(response.Errors == null);
           
        }
        [TestMethod]
        public void GetAllAccountUsersTest()
        {
            var response = _industryDataInterchange.GetAllAccountUsers(35591, new UserContextModel { AccountUserID = 35628, AgentID = 1024 });
            Assert.IsTrue(response.Errors == null);
            Assert.IsNotNull(response.Accounts);

        }

        [TestMethod]
        public void GetAccounttest()
        {
            var response = _industryDataInterchange.GetAccount(35591, new UserContextModel { AccountUserID = 35628, AgentID = 1024 });
            Assert.IsTrue(response.Errors == null);
            Assert.IsNotNull(response.Account_Model.AccountID);
        }
        [TestMethod]
        public void LodgeComplaintTest()
        {
            var response = _industryDataInterchange.LodgeComplaint(new POCO.Base.LodgeComplaintModel
            {
                ContactDetails = new POCO.Base.ContactDetailsModel
                {
                    Title = "MS.",
                    FirstName = "TestFname",
                    LastName = "TestLname",
                    Addressline1 = "Address1",
                    Addressline2 = "Address2",
                    Email = "test@gmail.com",
                    PhoneNumber = "134567",
                    Country = "AU",
                    State = "NewSouthWales" ,
                    Postcode = "5034",
                    Suburb = "LocalityTest",
                    Company = "TestCompany"
                },
                ComplaintRequest = new POCO.Base.ComplaintRequestModel
                {
                    ComplaintType  = DNCR.Infrastructure.Enums.ComplaintTypeEnum.Call,
                    Channel = DNCR.Infrastructure.Enums.ChannelsEnum.Webform,
                    Anonymous = false,
                    ConsentWithdrawn = false,
                    IsNumberOnRegister = true,
                    ReceivingCallNumber = "0987654321",
                    CallType =  DNCR.Infrastructure.Enums.CallTypeEnum.PromotingGoodsServicesOrFinancialOpportunities,
                    OnlineSurveyCompletion = DNCR.Infrastructure.Enums.ConsentToBeContactedEnum.No,
                    IsBusinessNumber = false,
                    ContactBusiness = false
                },
                CallIncidents = new System.Collections.Generic.List<POCO.Base.CallIncidentModel>
                {
                    new POCO.Base.CallIncidentModel
                    {
                         CallDate = DateTime.Now,
                        AMPM = "AM",
                        Time = "1:00 to 1:29",
                        VoiceCall =  DNCR.Infrastructure.Enums.VoiceCallEnum.ConversationWithARealPerson,
                        CallPurposeAdvertise = true,
                        IsFax = false,
                        CallReceiveNumber = "0987654343",
                        
                    }
                }

                
                
            });
            Assert.IsNull(response.Errors);
            Assert.AreEqual(response.IsSuccessful, true);
            Assert.IsNotNull(response.ComplaintNumber);
        }
        [TestMethod]
        public void LodgeEnquiryTest_withrefund()
        {
            var response = _industryDataInterchange.LodgeEnquiry(new POCO.Base.LodgeEnquiryModel
            {
                Addressline1 = "ad1",
                Addressline2 = "ad2",
                Country = "In",
                State = "hyd",
                Postcode = "503022",
                CompanyName = "TestEnqu",
                Details = "Enquiry for test",
                ChannelID = DNCR.Infrastructure.Enums.ChannelsEnum.Webform,
                IsAnonymous = false,
                IsConsumer = true,
                FirstName = "Fname",
                LastName = "Lname",
                Email = "test@gm.com",
                PhoneNumber = "0987657651",
                Suburb = "sub",
                Title = "Miss",
                IsRefund = true,
                RefundRequestModel = new OnlineRefundRequestModel
                {
                    AccountID = 35591,
                    Description = "refund test",
                    EnquiryRefundType = DNCR.Infrastructure.Enums.RefundTypeEnum.SubscriptionRefund,

                }
                
          
            }, null);
            Assert.AreEqual(response.IsSuccessful, true);
            Assert.AreNotEqual(0, response.EnquiryID);
            Assert.IsNull(response.Errors);
        }
        [TestMethod]
        public void GetAccountSummaryDetails_byAccountID()
        {
            var response = _industryDataInterchange.GetAccountSummaryDetailByAccountID(35591);
            Assert.IsNotNull(response.AccountSummaries);
            Assert.IsNull(response.Errors);
        }
        [TestMethod]
        public void GetSubscriptionSummaryDetails_byAccountID()
        {
            var response = _industryDataInterchange.GetSubscriptionSummaryDetailsByAccountID(35588);
            Assert.IsNull(response.Errors);
            Assert.IsNotNull(response.SubscriptionSummary);
        }
        [TestMethod]
        public void GetSubscriptionHistory_byAccountID()
        {

            var response = _industryDataInterchange.GetSubscriptionHistoryDetailsByAccountID(35588);
            Assert.IsNull(response.Errors);
            Assert.IsNotNull(response.SubscriptionHistories);
        }
        [TestMethod]
        public void GetFinancialHistoryTest()
        {
            var response = _industryDataInterchange.GetFinancialHistory(35599, 7322);
            Assert.IsNull(response.Errors);
            Assert.IsNotNull(response.Orders);
        }
        [TestMethod]
        public void WashHistoryTest()
        {

            var response = _industryDataInterchange.WashHistory(new WashHistoryRequestModel
            {
                AccountId = 35588,
                
                EndDateTime = DateTime.Now,
                ExcludeWashWithOneNumber = true,
                StartDateTime = DateTime.Now.AddYears(-1),
            }, new UserContextModel
            {
                AccountUserID = 35625,
                AgentID = 1024
            });
            Assert.IsNull(response.Errors);
            Assert.IsNotNull(response.WashHistoryResponseObjects);

        }

        [TestMethod]
        public void QuickWashTest()
        {
            var response = _industryDataInterchange.QuickWash(35588, "",
                new System.Collections.Generic.List<string> { "0987654322", "0987654321" },
                new UserContextModel { AccountUserID = 35625, AgentID = 1024 });
            Assert.IsNull(response.Errors);
            Assert.AreEqual(response.IsSuccessful, true);
            Assert.AreEqual(response.HasValidSubscription, true);
        }

        [TestMethod]
        public void AccountActivationMailTest()
        {
            var response = _industryDataInterchange.SendAccountActivationEmail("cur2gv4oda@gmail.com");
            Assert.IsNull(response.Errors);
            Assert.AreEqual(response.IsSuccessful, true);
        }

        //[TestMethod]
        //public void DownloadWashFileTest()
        //{
        //    var response = _industryDataInterchange.DownloadWashFile(35588, 3916, DNCR.Infrastructure.Enums.WashFileTypeEnum.Original, new UserContextModel
        //    {
        //        AccountUserID = 35625,
        //        AgentID = 1024
        //    });
        //    Assert.IsNull(response.Errors);
        //    Assert.IsNotNull(response.FileName);
        //}
       
        [TestMethod]
        public void StatesTest()
        {
            var response = _industryDataInterchange.GetStates(13, 1024);
            Assert.IsNull(response.Errors);
            
            Assert.AreNotEqual(0, response.States);
        }

        [TestMethod]
        public void UpdateAccountUserTest()
        {
            var response = _industryDataInterchange.UpdateAccountUser(new UserContextModel { AccountUserID = 35648, AgentID = 1024 },
                new UpdateAccountUserModel
                {
                    WashFormat = new WashingFormat { WashResultFormatFileWithIndicators = true },

                    AccountUserModel = new AccountModel
                    {
                        Email = "changeUser12@gmail.com",
                        Status = DNCR.Infrastructure.Enums.AccountUserStatusEnum.Active,
                        AccountUserID = 35648,
                        FirstName = "changeFNameUser1",
                        LastName = "changeLnameUser2",
                        PhoneNumber = "0988765477"
                    },

                });
            Assert.AreEqual(response.IsSuccessful, true);
            Assert.IsNull(response.Errors);
        }

        [TestMethod]
        public void UpdateAccountTest()
        {
            var response = _industryDataInterchange.UpdateAccount(new UserContextModel { AccountUserID = 35624, AgentID = 1024 },
                new UpdateAccountModel
                {
                    AccounDetails = new AccountModel
                    {
                        BusinessSector = DNCR.Infrastructure.Enums.BusinessSectorEnum.Person,
                        OrganisationName = "Changed Bname",
                        Country = "India",
                        Postcode = "502032",
                        HasABN = false,
                        AddressLine1 = "Addressline changed",
                        State = "Hyderabad",
                        PhoneNumber = "0987654321",
                        OrganisationWebsite = "WebAddressTest",
                        AccountID = 35587,
                        PrincipalActivity = "telephoning",
                    }
                });
            Assert.IsNull(response.Errors);
            Assert.AreEqual(response.IsSuccessful, true);
        }

        [TestMethod]
        public void SubmitBulkRegistrationTest()
        {
            string path = @"C:\_Salmat\DNCR\Salmat.Dncr-defectfix\Salmat.Dncr.Business.Tests\test10.txt";
            byte[] numbers = System.IO.File.ReadAllBytes(path);

            var response = this._consumerDataInterchange.BulkRegistration(new BulkRegistration
            {
                FirstName = "TestFname",
                LastName = "TestLname",
                OrganisationName = "Test",
                PreferredContactMethod = DNCR.Infrastructure.Enums.PreferredContactMethodEnum.Email,
                 PhoneFaxNumber = "0987654322",
                Email= "test@gmail.com",
                AddressLine1 = "address1",
                Country = "India",
                Postcode = "503032",
                City = "locality",
                OperationType  = DNCR.Infrastructure.Enums.OperationTypeEnum.Register,
                PhoneNumbersFile = numbers,
                FaxNumbersFile = null,
                EvidenceFile = new byte[] { 20, 30, 48, 30, 54 }

            });
            Assert.AreEqual(response.IsSuccessful, true);
            Assert.IsNull(response.Errors);
            Assert.AreNotEqual(0, response.RegistrationRequestID);
        }

        [TestMethod]
        public void TestRegister()
        {
            var response = _consumerDataInterchange.Register(new Registration
            {
                FirstName ="TesterF",
                LastName = "TesterL",
                ContactNumber = "134567",
                Email = "newtest@gmail.com",
                OrganisationName = "TEST",
                Numbers = new System.Collections.Generic.List<string>
                {
                    "0987654300",
                    "0987654999"
                }
            });
            Assert.AreNotEqual(0, response.RegistrationRequestID);
            Assert.IsNull(response.Errors);
        }

        [TestMethod]
        public void TestCheck()
        {
            var response = _consumerDataInterchange.CheckRegistration("newtest@gmail.com", new System.Collections.Generic.List<string> { "0987654300", "0987654999" });
            Assert.AreEqual(response.HasEmailSent, true);
            Assert.IsNull(response.Errors);
        }

        //[TestMethod]
        //public void Prewah_Test()
        //{
        //    string text = new string("qwertyuiolgfdsazxcvbnm1234".OrderBy(o => Guid.NewGuid()).Take(6).ToArray());
        //    string name = text + ".txt";
        //    File.Create(@"C:\_Salmat\DNCR\Salmat.Dncr-defectfix\Salmat.Dncr.Business.Tests\" + name);
            
        //    File.Copy(@"C:\_Salmat\DNCR\Salmat.Dncr-defectfix\Salmat.Dncr.Business.Tests\testWash.txt", name, true);

        //    string inputfilepath = @"C:\_Salmat\DNCR\Salmat.Dncr-defectfix\Salmat.Dncr.Business.Tests\";
        //    string filepath = inputfilepath + name ;

        //    var response = _industryDataInterchange.PreWash("", filepath, new UserContextModel { AccountUserID = 35625, AgentID = 1024 });
        //    Assert.IsNull(response.Errors);
        //    Assert.AreEqual(response.HasSufficientWashingCredits, true);
        //    Assert.AreNotEqual(0, response.WashingRequestId);
        //}

        //[TestMethod]
        //public void Washby_file()
        //{
        //    string text = new string("qwertyuiolgfdsazxcvbnm1234".OrderBy(o => Guid.NewGuid()).Take(6).ToArray());
        //    string name = text + ".txt";
        //    File.Create(@"C:\_Salmat\DNCR\Salmat.Dncr-defectfix\Salmat.Dncr.Business.Tests\" + name);

        //    File.Copy(@"C:\_Salmat\DNCR\Salmat.Dncr-defectfix\Salmat.Dncr.Business.Tests\testWash.txt", name, true);

        //    string inputfilepath = @"C:\_Salmat\DNCR\Salmat.Dncr-defectfix\Salmat.Dncr.Business.Tests\";
        //    string filepath = inputfilepath + name;

        //    var response = _industryDataInterchange.PreWash("", filepath, new UserContextModel { AccountUserID = 35625, AgentID = 1024 });
        //    int id = response.WashingRequestId;


        //    var response_wash = _industryDataInterchange.WashByFileUpload(new WashByFileUploadModel
        //    {
        //        AccountId = 35588,
        //        WashingRequestId = id,
        //        WashResultFormatFileWithIndicators = true
        //    }, new UserContextModel
        //    {
        //        AccountUserID = 35625,
                
        //    });
        //    Assert.IsNull(response_wash.Errors);
        //    Assert.AreEqual(response_wash.IsSuccessful, true);
        //    Assert.AreNotEqual(0, response_wash.WashingRequestId);
            
        //}
        [TestMethod]
        public void PayforOrdertest()
        {
            var response = _industryDataInterchange.CreateOrder(35598, 0, "test@gmail.com", false, true,
               new System.Collections.Generic.List<SubscriptionVsQuantityModel>()
               {
                    new SubscriptionVsQuantityModel{Quantity = 1,SubscriptionID = 2}
               },
                 new UserContextModel
                 {
                     AccountUserID = 35635,
                     AgentID = 1024
                 }
                );
            var payment = _industryDataInterchange.PayForOrder(new PayForOrderArguments
            {
                AccountUserID = 35635,
                OrderID = response.OrderId,
                CreditCardReference = "cc - Reference",
                AgentUserID = 1024,
                Amount = 79,
                IsCSR = false,

            });
            Assert.IsNull(payment.Errors);

        }

        [TestMethod]
        public void CancelOrderTest()
        {
            var response = _industryDataInterchange.CreateOrder(35598, 0, "test@gmail.com", false, false,
               new System.Collections.Generic.List<SubscriptionVsQuantityModel>()
               {
                    new SubscriptionVsQuantityModel{Quantity = 1,SubscriptionID = 2}
               },
                 new UserContextModel
                 {
                     AccountUserID = 35635,
                     AgentID = 1024
                 }
                );
            var cancel = _industryDataInterchange.CancelOrder(35635, response.OrderId, 1024, false);
            Assert.IsNull(cancel.Errors);
            Assert.AreEqual(cancel.IsSuccessful, true);
        }
        [TestMethod]
        public void CloseOrderTest()
        {
            var response = _industryDataInterchange.CreateOrder(35598, 0, "test@gmail.com", false, false,
               new System.Collections.Generic.List<SubscriptionVsQuantityModel>()
               {
                    new SubscriptionVsQuantityModel{Quantity = 1,SubscriptionID = 2}
               },
                 new UserContextModel
                 {
                     AccountUserID = 35635,
                     AgentID = 1024
                 });
            var payment = _industryDataInterchange.PayForOrder(new PayForOrderArguments
            {
                AccountUserID = 35635,
                OrderID = response.OrderId,
                CreditCardReference = "cc - Reference",
                AgentUserID = 1024,
                Amount = 50,
                IsCSR = false,

            });

            var close = _industryDataInterchange.CloseOrder(35635, response.OrderId, 1024, false);
            Assert.IsNull(close.Errors);
            Assert.AreEqual(close.IsSuccessful, true);
        }
    }
}
