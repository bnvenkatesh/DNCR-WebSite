using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SD.ACMA.DatabaseIntermediary;
using SD.Core.Data.Repository.PetaPoco.Business.Interface;
using SD.Core.Data.Repository.PetaPoco.Business;
using SD.Core.Data.Repository.PetaPoco.UnityOfWorkContainer;
using SD.ACMA.InterfaceTier;
using SD.ACMA.BusinessLogic.PaymentGateway;
using SD.ACMA.DNCR.Infrastructure;
using SD.ACMA.POCO.PetaPoco;
using Microsoft.Practices.Unity;
using System.Text;
using System.Xml;
using System.Text.RegularExpressions;

namespace SD.ACMA.BusinessLogicTests
{
    [TestClass]
    public class SiteLoggingServiceUnitTest
    {
        private ISiteLoggingService _siteLoggingService; // = new SiteLoggingService(new PetaPocoRepository(), new PetaPocoUnitOfWorkProvider());

        [TestInitialize]
        public void Init()
        {
            IUnityContainer container = new UnityContainer()
                .RegisterType<IRepository, PetaPocoRepository>()
                .RegisterType<IUnitOfWorkProvider, PetaPocoUnitOfWorkProvider>()                
                .RegisterType<ISiteLoggingService, SiteLoggingService>();

            this._siteLoggingService = container.Resolve<ISiteLoggingService>();
        }

        [TestMethod]
        public void TestCreatePayment()
        {
            string xml = "<s:Envelope xmlns:s=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n  <s:Header>\r\n    <Action s:mustUnderstand=\"1\" xmlns=\"http://schemas.microsoft.com/ws/2005/05/addressing/none\">http://tempuri.org/IOnlineCmsService/ImpersonateCSR</Action>\r\n  </s:Header>\r\n  <s:Body>\r\n    <ImpersonateCSR xmlns=\"http://tempuri.org/\">\r\n      <impersonateRequestArgs xmlns:d4p1=\"http://schemas.datacontract.org/2004/07/Salmat.Dncr.Business.Entities.Cms\" xmlns:i=\"http://www.w3.org/2001/XMLSchema-instance\">\r\n        <d4p1:SecureData>&#x1;&#x0;&#x0;&#x0;ÐŒß&#x1;&#x15;Ñ&#x11;Œz&#x0;ÀOÂ—ë&#x1;&#x0;&#x0;&#x0;„Q…/?F‰B‚nˆh`—GA&#x4;&#x0;&#x0;&#x0;&#x2;&#x0;&#x0;&#x0;&#x0;&#x0;&#x3;f&#x0;&#x0;À&#x0;&#x0;&#x0;&#x10;&#x0;&#x0;&#x0;å¢”‡Ì€\tUÍLìñô“Ùm&#x0;&#x0;&#x0;&#x0;&#x4;€&#x0;&#x0; &#x0;&#x0;&#x0;&#x10;&#x0;&#x0;&#x0;à²EãÍUN&#xF;&#x1B;\rN¾«1ãü&#x8;&#x0;&#x0;&#x0;µI_—¡! &#x14;&#x0;&#x0;&#x0;w&#x5;&#x5;1ë&#x16;9@Â‡tÿË:”&#x10;&#x4;²&#x16;L</d4p1:SecureData>\r\n      </impersonateRequestArgs>\r\n    </ImpersonateCSR>\r\n  </s:Body>\r\n</s:Envelope>";

            //xml = Regex.Replace(xml, @"[^\u0000-\u007F]", "_");
            //xml = Regex.Replace(xml, @"[^\u0009\u000a\u000d\u0020-\ud7ff\ue000-\ufffd]|([\ud800-\udbff](?![\udc00-\udfff]))|((?<![\ud800-\udbff])[\udc00-\udfff])", "_");

            xml = xml.Replace("&", "&amp;");
            
            Assert.IsTrue(_siteLoggingService.InsertXML(xml, null, null) > 0);
        }

        private string CleanXML(string xml)
        {
            if (xml == null) return null;

            StringBuilder newString = new StringBuilder();
            char ch;

            for (int i = 0; i < xml.Length; i++)
            {

                ch = xml[i];
                // remove any characters outside the valid UTF-8 range as well as all control characters
                // except tabs and new lines
                //if ((ch < 0x00FD && ch > 0x001F) || ch == '\t' || ch == '\n' || ch == '\r')
                //if using .NET version prior to 4, use above logic
                if (XmlConvert.IsXmlChar(ch)) //this method is new in .NET 4
                {
                    newString.Append(ch);
                }
            }

            return newString.ToString();
        }

        private String stripNonValidXMLCharacters(string textIn)
        {
            StringBuilder textOut = new StringBuilder(); // Used to hold the output.
            char current; // Used to reference the current character.


            if (textIn == null || textIn == string.Empty) return string.Empty; // vacancy test.
            for (int i = 0; i < textIn.Length; i++)
            {
                current = textIn[i];

                if ((current == 0x9 || current == 0xA || current == 0xD) ||
                    ((current >= 0x20) && (current <= 0xD7FF)) ||
                    ((current >= 0xE000) && (current <= 0xFFFD)) ||
                    ((current >= 0x10000) && (current <= 0x10FFFF)))
                {
                    textOut.Append(current);
                }
            }
            return textOut.ToString();
        }
    }
}
