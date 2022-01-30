using SD.ACMA.DNCR.Infrastructure.Configuration;
using SD.ACMA.InterfaceTier;
using SD.ACMA.DNCR.Infrastructure;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Net;
using System.Security.Cryptography;

namespace SD.ACMA.BusinessLogic.PaymentGateway
{
    public class PaymentGatewayService : IPaymentGatewayService
    {
        #region Payment Gateway Settings

        private const string _VPCVersion = "1";
        private const string _Locale = "en";

        private string _VirtualPaymentClientURL_Pay = ConfigurationHelper.Instance.VirtualPaymentClientURL_Pay;
        private string _VirtualPaymentClientURL_Query = ConfigurationHelper.Instance.VirtualPaymentClientURL_Query;
        private string _SecureSecret = ConfigurationHelper.Instance.SecureSecret;
        private string _MerchantAccessCode = ConfigurationHelper.Instance.MerchantAccessCode;
        private string _MerchantID = ConfigurationHelper.Instance.MerchantID;

        #endregion

        #region Payment Gateway Methods

        private string getResponseDescription(string vResponseCode)
        {
            /* 
             <summary>Maps the vpc_TxnResponseCode to a relevant description</summary>
             <param name="vResponseCode">The vpc_TxnResponseCode returned by the transaction.</param>
             <returns>The corresponding description for the vpc_TxnResponseCode.</returns>
             */
            string result = "Unknown";

            if (vResponseCode.Length > 0)
            {
                switch (vResponseCode)
                {
                    case "0": result = "Transaction Successful"; break;
                    case "1": result = "Transaction Declined"; break;
                    case "2": result = "Bank Declined Transaction"; break;
                    case "3": result = "No Reply from Bank"; break;
                    case "4": result = "Expired Card"; break;
                    case "5": result = "Insufficient Funds"; break;
                    case "6": result = "Error Communicating with Bank"; break;
                    case "7": result = "Payment Server detected an error"; break;
                    case "8": result = "Transaction Type Not Supported"; break;
                    case "9": result = "Bank declined transaction (Do not contact Bank)"; break;
                    case "A": result = "Transaction Aborted"; break;
                    case "B": result = "Transaction Declined - Contact the Bank"; break;
                    case "C": result = "Transaction Cancelled"; break;
                    case "D": result = "Deferred transaction has been received and is awaiting processing"; break;
                    case "F": result = "3-D Secure Authentication failed"; break;
                    case "I": result = "Card Security Code verification failed"; break;
                    case "L": result = "Shopping Transaction Locked (Please try the transaction again later)"; break;
                    case "N": result = "Cardholder is not enrolled in Authentication scheme"; break;
                    case "P": result = "Transaction has been received by the Payment Adaptor and is being processed"; break;
                    case "R": result = "Transaction was not processed - Reached limit of retry attempts allowed"; break;
                    case "S": result = "Duplicate SessionID"; break;
                    case "T": result = "Address Verification Failed"; break;
                    case "U": result = "Card Security Code Failed"; break;
                    case "V": result = "Address Verification and Card Security Code Failed"; break;
                    default: result = "Unable to be determined"; break;
                }
            }
            return result;
        }

        private string getAVSDescription(string vAVSResultCode)
        {
            /*
             <summary>Maps the vpc_AVSResultCode to a relevant description</summary>
             <param name="vAVSResultCode">The vpc_AVSResultCode returned by the transaction.</param>
             <returns>The corresponding description for the vpc_AVSResultCode.</returns>
             */
            string result = "Unknown";

            if (vAVSResultCode.Length > 0)
            {
                if (vAVSResultCode.Equals("Unsupported"))
                {
                    result = "AVS not supported or there was no AVS data provided";
                }
                else
                {
                    switch (vAVSResultCode)
                    {
                        case "X": result = "Exact match - address and 9 digit ZIP/postal code"; break;
                        case "Y": result = "Exact match - address and 5 digit ZIP/postal code"; break;
                        case "S": result = "Service not supported or address not verified (international transaction)"; break;
                        case "G": result = "Issuer does not participate in AVS (international transaction)"; break;
                        case "A": result = "Address match only"; break;
                        case "W": result = "9 digit ZIP/postal code matched, Address not Matched"; break;
                        case "Z": result = "5 digit ZIP/postal code matched, Address not Matched"; break;
                        case "R": result = "Issuer system is unavailable"; break;
                        case "U": result = "Address unavailable or not verified"; break;
                        case "E": result = "Address and ZIP/postal code not provided"; break;
                        case "N": result = "Address and ZIP/postal code not matched"; break;
                        case "0": result = "AVS not requested"; break;
                        default: result = "Unable to be determined"; break;
                    }
                }
            }
            return result;
        }

        private string getCSCDescription(string vCSCResultCode)
        {
            /*
             <summary>Maps the vpc_CSCResultCode to a relevant description</summary>
             <param name="vCSCResultCode">The vpc_CSCResultCode returned by the transaction.</param>
             <returns>The corresponding description for the vpc_CSCResultCode.</returns>
             */
            string result = "Unknown";
            if (vCSCResultCode.Length > 0)
            {
                if (vCSCResultCode.Equals("Unsupported"))
                {
                    result = "CSC not supported or there was no CSC data provided";
                }
                else
                {

                    switch (vCSCResultCode)
                    {
                        case "M": result = "Exact code match"; break;
                        case "S": result = "Merchant has indicated that CSC is not present on the card (MOTO situation)"; break;
                        case "P": result = "Code not processed"; break;
                        case "U": result = "Card issuer is not registered and/or certified"; break;
                        case "N": result = "Code invalid or not matched"; break;
                        default: result = "Unable to be determined"; break;
                    }
                }
            }
            return result;
        }

        private System.Collections.Hashtable splitResponse(string rawData)
        {
            /*
             <summary>Parses a HTTP POST to extract the parmaters from the POST data</summary>
             <param name="rawData">The raw data from the body of a HTTP POST.</param>
             <returns>A Hashtable containing the parameters returned in the body of a HTTP POST.</returns>

             <remarks>
             <para>
             This function parses the content of the VPC response to extract the 
             individual parameter names and values. These names and values are then 
             returned as a Hashtable.
             </para>

             <para>
             The content returned by the VPC is a HTTP POST, so the content will 
             be in the format <c>parameter1=value&parameter2=value&parameter3=value</c>, 
             i.e. key/value pairs separated by ampersands "&".
             </para>
             </remarks>
             */

            System.Collections.Hashtable responseData = new System.Collections.Hashtable();
            try
            {
                // Check if there was a response containing parameters
                if (rawData.IndexOf("=") > 0)
                {
                    // Extract the key/value pairs for each parameter
                    foreach (string pair in rawData.Split('&'))
                    {
                        int equalsIndex = pair.IndexOf("=");
                        if (equalsIndex > 1 && pair.Length > equalsIndex)
                        {
                            string paramKey = System.Web.HttpUtility.UrlDecode(pair.Substring(0, equalsIndex));
                            string paramValue = System.Web.HttpUtility.UrlDecode(pair.Substring(equalsIndex + 1));
                            responseData.Add(paramKey, paramValue);
                        }
                    }
                }
                else
                {
                    // There were no parameters so create an error
                    responseData.Add("vpc_Message", "The data contained in the response was not a valid receipt.<br/>\nThe data was: <pre>" + rawData + "</pre><br/>\n");
                }
                return responseData;
            }
            catch (Exception ex)
            {
                // There was an exception so create an error
                responseData.Add("vpc_Message", "\nThe was an exception parsing the response data.<br/>\nThe data was: <pre>" + rawData + "</pre><br/>\n<br/>\nException: " + ex.ToString() + "<br/>\n");
                return responseData;
            }
        }

        private string CreateMD5Signature(string RawData)
        {
            /*
             <summary>Creates a MD5 Signature</summary>
             <param name="RawData">The string used to create the MD5 signautre.</param>
             <returns>A string containing the MD5 signature.</returns>
             */

            //System.Security.Cryptography.MD5 hasher = System.Security.Cryptography.MD5CryptoServiceProvider.Create();

            byte[] keyByte = HexDecode(_SecureSecret);
            HMACSHA256 hmacsha256 = new HMACSHA256(keyByte);

            byte[] HashValue = hmacsha256.ComputeHash(Encoding.ASCII.GetBytes(RawData));

            return HashEncode(HashValue);
        }

        private static byte[] HexDecode(string hex)
        {
            var bytes = new byte[hex.Length / 2];
            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] = byte.Parse(hex.Substring(i * 2, 2), NumberStyles.HexNumber);
            }
            return bytes;
        }

        private static string HashEncode(byte[] hash)
        {
            return BitConverter.ToString(hash).Replace("-", "").ToUpper();
        }

        private class VPCStringComparer : IComparer
        {
            /*
             <summary>Customised Compare Class</summary>
             <remarks>
             <para>
             The Virtual Payment Client need to use an Ordinal comparison to Sort on 
             the field names to create the MD5 Signature for validation of the message. 
             This class provides a Compare method that is used to allow the sorted list 
             to be ordered using an Ordinal comparison.
             </para>
             </remarks>
             */

            public int Compare(Object a, Object b)
            {
                /*
                 <summary>Compare method using Ordinal comparison</summary>
                 <param name="a">The first string in the comparison.</param>
                 <param name="b">The second string in the comparison.</param>
                 <returns>An int containing the result of the comparison.</returns>
                 */

                // Return if we are comparing the same object or one of the 
                // objects is null, since we don't need to go any further.
                if (a == b) return 0;
                if (a == null) return -1;
                if (b == null) return 1;

                // Ensure we have string to compare
                string sa = a as string;
                string sb = b as string;

                // Get the CompareInfo object to use for comparing
                System.Globalization.CompareInfo myComparer = System.Globalization.CompareInfo.GetCompareInfo("en-US");
                if (sa != null && sb != null)
                {
                    // Compare using an Ordinal Comparison.
                    return myComparer.Compare(sa, sb, System.Globalization.CompareOptions.Ordinal);
                }
                throw new ArgumentException("a and b should be strings.");
            }
        }

        #endregion

        #region Service Methods

        public string GetPaymentGatewayUrl(string transactionId, string orderInfo, int amountInCents, string returnUrl)
        {
            //Below line will upgrade current TLS 1.0 to TLS 1.1 or higher since ANZ will be removing the use of older gateway communication protocols and encryption ciphers
            ServicePointManager.SecurityProtocol =
               SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3;

            string vpc_Command = "pay";
            string url = _VirtualPaymentClientURL_Pay;

            SortedList transactionData = new SortedList(new VPCStringComparer());
            transactionData.Add("vpc_AccessCode", _MerchantAccessCode);
            transactionData.Add("vpc_Amount", amountInCents);
            transactionData.Add("vpc_Command", vpc_Command);
            transactionData.Add("vpc_Locale", _Locale);
            transactionData.Add("vpc_MerchTxnRef", transactionId);
            transactionData.Add("vpc_Merchant", _MerchantID);
            transactionData.Add("vpc_OrderInfo", orderInfo);
            transactionData.Add("vpc_ReturnURL", returnUrl);
            transactionData.Add("vpc_Version", _VPCVersion);

            string rawHashData = String.Empty;//_SecureSecret;
            string seperator = "?";

            if (_SecureSecret.Length > 0)
            {
                foreach (DictionaryEntry item in transactionData)
                {
                    url += seperator + HttpUtility.UrlEncode(item.Key.ToString()) + "=" + HttpUtility.UrlEncode(item.Value.ToString());
                    seperator = "&";

                    rawHashData += item.Key + "=" + item.Value + "&";
                }
            }

            string signature = CreateMD5Signature(rawHashData.Substring(0, rawHashData.Length - 1));
            url += seperator + "vpc_SecureHash=" + signature;
            url += seperator + "vpc_SecureHashType=SHA256";

            return url;
        }

        public void VerifyReceipt(string receipt, out string transactionId, out Enums.PaymentStatusEnum paymentStatus, out string response, out string message, out string creditCardReference, out string receiptNumber, out DateTime? settlementDate, out long? transactionNo, out int? transactionAmountInCents, out string authorizeId, out string transactionType, out string cardType)
        {
            //Below line will upgrade current TLS 1.0 to TLS 1.1 or higher since ANZ will be removing the use of older gateway communication protocols and encryption ciphers
            ServicePointManager.SecurityProtocol =
               SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3;

            transactionId = "UNKNOWN";
            paymentStatus = Enums.PaymentStatusEnum.UNKNOWN;
            response = "UNKNOWN";
            message = "UNKNOWN";
            creditCardReference = "UNKNOWN";
            receiptNumber = "UNKNOWN";            
            settlementDate = null;
            transactionNo = null;
            transactionAmountInCents = null;
            authorizeId = "UNKNOWN";
            transactionType = "UNKNOWN";
            cardType = "UNKNOWN";

            try
            {
                if (receipt.IndexOf('&') > 0 && receipt.IndexOf('=') > 0)
                {
                    var receiptData = receipt
                                        .Split('&')
                                        .Select(x => x.Split('='))
                                        .ToDictionary(x => x[0], x => x[1]);

                    if (receiptData.ContainsKey("vpc_MerchTxnRef"))
                    {
                        bool proceedToExtractValues = true;

                        if (receiptData.ContainsKey("vpc_SecureHash"))
                        {
                            string rawHashData = String.Empty;//_SecureSecret;

                            foreach (var item in receiptData)
                            {
                                if (_SecureSecret.Length > 0 && !item.Key.Equals("vpc_SecureHash") && !item.Key.Equals("vpc_SecureHashType"))
                                {
                                    rawHashData += item.Key + "=" + HttpUtility.UrlDecode(item.Value.ToString()) + "&";
                                }
                            }

                            string signature = CreateMD5Signature(rawHashData.Substring(0, rawHashData.Length - 1));

                            if (!receiptData["vpc_SecureHash"].Equals(signature))
                            {
                                proceedToExtractValues = false;
                                message = "Invalid Hash";
                            }
                        }

                        if (proceedToExtractValues)
                        {
                            paymentStatus = receiptData.ContainsKey("vpc_TxnResponseCode")
                                                        ? (receiptData["vpc_TxnResponseCode"] == "0" ? Enums.PaymentStatusEnum.SUCCESS : Enums.PaymentStatusEnum.DECLINED)
                                                        : Enums.PaymentStatusEnum.UNKNOWN;

                            response = receiptData.ContainsKey("vpc_TxnResponseCode") ? HttpUtility.UrlDecode(receiptData["vpc_TxnResponseCode"]) : "UNKNOWN";
                            message = receiptData.ContainsKey("vpc_Message") ? HttpUtility.UrlDecode(receiptData["vpc_Message"]) : "UNKNOWN";
                            receiptNumber = receiptData.ContainsKey("vpc_ReceiptNo") ? HttpUtility.UrlDecode(receiptData["vpc_ReceiptNo"]) : "UNKNOWN";
                            transactionId = receiptData.ContainsKey("vpc_MerchTxnRef") ? HttpUtility.UrlDecode(receiptData["vpc_MerchTxnRef"]) : "UNKNOWN";
                            creditCardReference = receiptData.ContainsKey("vpc_TransactionNo") ? HttpUtility.UrlDecode(receiptData["vpc_TransactionNo"]) : "UNKNOWN";
                            transactionNo = receiptData.ContainsKey("vpc_TransactionNo") ? (long?)(Convert.ToInt64(HttpUtility.UrlDecode(receiptData["vpc_TransactionNo"]))) : null;
                            transactionAmountInCents = receiptData.ContainsKey("vpc_Amount") ? (int?)(Convert.ToInt32(HttpUtility.UrlDecode(receiptData["vpc_Amount"]))) : null;
                            authorizeId = receiptData.ContainsKey("vpc_AuthorizeId") ? HttpUtility.UrlDecode(receiptData["vpc_AuthorizeId"]) : "UNKNOWN";
                            transactionType = receiptData.ContainsKey("vpc_Command") ? HttpUtility.UrlDecode(receiptData["vpc_Command"]) : "UNKNOWN";
                            cardType = receiptData.ContainsKey("vpc_Card") ? HttpUtility.UrlDecode(receiptData["vpc_Card"]) : "UNKNOWN";

                            DateTime batchDate;

                            if (receiptData.ContainsKey("vpc_BatchNo") && DateTime.TryParseExact(receiptData["vpc_BatchNo"], "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out batchDate))
                            {
                                //batchDate = DateTime.ParseExact(receiptData["vpc_BatchNo"], "yyyyMMdd", CultureInfo.InvariantCulture);
                                settlementDate = batchDate.AddDays(1);
                            }
                        }
                    }
                    else
                    {
                        message = "Transaction Id not found";
                    }
                }
                else
                {
                    message = "Invalid receipt";
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
        }

        public void QueryPayment(string transactionId, out string receipt, out Enums.PaymentStatusEnum paymentStatus, out string response, out string message, out string creditCardReference, out string receiptNumber, out DateTime? settlementDate, out long? transactionNo, out int? transactionAmountInCents, out string authorizeId, out string transactionType, out string cardType)
        {
            //Below line will upgrade current TLS 1.0 to TLS 1.1 or higher since ANZ will be removing the use of older gateway communication protocols and encryption ciphers
            ServicePointManager.SecurityProtocol =
               SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3;

            string vpc_Command = "queryDR";
            string url = _VirtualPaymentClientURL_Query;

            paymentStatus = Enums.PaymentStatusEnum.UNKNOWN;

            receipt = "UNKNOWN";
            response = "UNKNOWN";
            message = "UNKNOWN";
            creditCardReference = "UNKNOWN";
            receiptNumber = "UNKNOWN";
            settlementDate = null;
            transactionNo = null;
            transactionAmountInCents = null;
            authorizeId = "UNKNOWN";
            transactionType = "UNKNOWN";
            cardType = "UNKNOWN";

            try
            {
                StringBuilder sbPostData = new StringBuilder()
                    .Append("vpc_Version=").Append(HttpUtility.UrlEncode(_VPCVersion))
                    .Append("&vpc_Command=").Append(HttpUtility.UrlEncode(vpc_Command))
                    .Append("&vpc_AccessCode=").Append(HttpUtility.UrlEncode(ConfigurationHelper.Instance.MerchantAccessCode))
                    .Append("&vpc_MerchTxnRef=").Append(HttpUtility.UrlEncode(transactionId))
                    .Append("&vpc_Merchant=").Append(HttpUtility.UrlEncode(ConfigurationHelper.Instance.MerchantID))
                    .Append("&vpc_User=").Append(HttpUtility.UrlEncode(ConfigurationHelper.Instance.MerchantAdminUser))
                    .Append("&vpc_Password=").Append(HttpUtility.UrlEncode(ConfigurationHelper.Instance.MerchantAdminPassword));

                string responseData = null;

                using (WebClient webClient = new WebClient())
                {
                    webClient.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                    byte[] queryResponse = webClient.UploadData(url, "POST", Encoding.ASCII.GetBytes(sbPostData.ToString()));

                    responseData = System.Text.Encoding.ASCII.GetString(queryResponse, 0, queryResponse.Length);
                }

                if (!string.IsNullOrEmpty(responseData))
                {
                    string transId = null;

                    receipt = responseData;                    

                    VerifyReceipt(responseData, out transId, out paymentStatus, out response, out message, out creditCardReference, out receiptNumber, out settlementDate, out transactionNo, out transactionAmountInCents, out authorizeId, out transactionType, out cardType);
                }
                else
                {
                    message = "No response from payment gateway";
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
        }

        #endregion
    }
}
