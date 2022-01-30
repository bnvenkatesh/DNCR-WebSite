using SD.ACMA.DNCR.Infrastructure.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
//using Winnovative.WnvHtmlConvert;
using EvoPdf;

namespace SD.ACMA.BusinessLogic.Helpers
{
    public class PDFConverter : IDisposable
    {
        private PdfConverter _converter;

        public PDFConverter(string username, string password)
        {            
            _converter = new PdfConverter();
            _converter.LicenseKey = "SsTXxdbWxdTS0tLF08vVxdbUy9TXy9zc3NzF1Q==";

            if (!String.IsNullOrEmpty(username) && !String.IsNullOrEmpty(password))
            {
                _converter.AuthenticationOptions.Username = username;
                _converter.AuthenticationOptions.Password = password;
            }
        }        

        public byte[] ConvertFromURL(string url)
        {
            _converter.JavaScriptEnabled = true; // so we can run the jquery                         

            _converter.PdfDocumentOptions.InternalLinksEnabled = false;
            _converter.PdfDocumentOptions.PdfPageSize = PdfPageSize.A4;
            _converter.PdfDocumentOptions.PdfPageOrientation = PdfPageOrientation.Portrait;
            _converter.PdfDocumentOptions.FitWidth = true;
            _converter.PdfDocumentOptions.FitHeight = false;
            
            _converter.PdfDocumentOptions.SinglePage = false;
            _converter.PdfDocumentOptions.BottomMargin = 50;
            _converter.PdfDocumentOptions.TopMargin = 50;
            _converter.PdfDocumentOptions.BottomMargin = 50;
            _converter.PdfDocumentOptions.TopMargin = 50;

            byte[] pdfBuff = _converter.GetPdfBytesFromUrl(url);

            return pdfBuff;
        }

        public byte[] ConvertFromStream(Stream sHTML)
        {
            _converter.JavaScriptEnabled = true; // so we can run the jquery             

            _converter.PdfDocumentOptions.InternalLinksEnabled = false;
            _converter.PdfDocumentOptions.PdfPageSize = PdfPageSize.A4;
            _converter.PdfDocumentOptions.PdfPageOrientation = PdfPageOrientation.Portrait;
            _converter.PdfDocumentOptions.FitWidth = true;
            _converter.PdfDocumentOptions.FitHeight = false;

            _converter.PdfDocumentOptions.SinglePage = false;
            _converter.PdfDocumentOptions.BottomMargin = 50;
            _converter.PdfDocumentOptions.TopMargin = 50;
            _converter.PdfDocumentOptions.BottomMargin = 50;
            _converter.PdfDocumentOptions.TopMargin = 50;

            byte[] pdfBuff = _converter.GetPdfBytesFromHtmlStream(sHTML, Encoding.UTF8);

            return pdfBuff;
        }

        public byte[] ConvertFromHTMLString(string html)
        {
            _converter.JavaScriptEnabled = true; // so we can run the jquery             

            _converter.PdfDocumentOptions.InternalLinksEnabled = false;
            _converter.PdfDocumentOptions.PdfPageSize = PdfPageSize.A4;
            _converter.PdfDocumentOptions.PdfPageOrientation = PdfPageOrientation.Portrait;
            _converter.PdfDocumentOptions.FitWidth = true;
            _converter.PdfDocumentOptions.FitHeight = false;

            _converter.PdfDocumentOptions.SinglePage = false;
            _converter.PdfDocumentOptions.BottomMargin = 50;
            _converter.PdfDocumentOptions.TopMargin = 50;
            _converter.PdfDocumentOptions.BottomMargin = 50;
            _converter.PdfDocumentOptions.TopMargin = 50;

            byte[] pdfBuff = _converter.GetPdfBytesFromHtmlString(html);

            return pdfBuff;
        }

        public void Dispose()
        {
            _converter = null;
        }
    }
}
