using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SD.ACMA.DatabaseIntermediary
{
    partial class FinancialReportDataRepository
    {
        #region Financial Report

        internal string GetAllActivatedSubscriptionsByPeriodQuery = @"select o.OrderNumber,  ol.SubscriptionName, a.BusinessName, ol.Price
        from subscription.OrderTransaction ot
        inner join subscription.OrderLine ol on ot.OrderLineID = ol.OrderLineID
        inner join subscription.[Order] o on ot.OrderID = o.OrderId
        inner join accessseeker.account a on o.AccountID = a.AccountID
        WHERE OT.CreatedTimeStampt BETWEEN @0 AND @1
        and ot.OrderTransactionTypeID = 2 -- SubscriptionActivated";

        #endregion

        #region Credit Notes Report

        internal readonly string Query_GetNewCreditNoteTransactions = @"
           SELECT * FROM (
            SELECT OT.CreatedTimeStampt AS DateReceived, PM.PaymentMethodName AS PaymentType, O.AccountID AS AccountID, (SELECT CN.CreditNoteID from subscription.CreditNote CN WHERE CN.OrderTransactionID = OT.OrderTransactionID) AS CreditNoteNumber, 
            CASE WHEN OTT.TransactionType = 'DebitAccountBalance' THEN OT.Amount * -1 ELSE OT.Amount END AS CreditNoteAmount, 
            (SELECT CN.AccountBalance from subscription.CreditNote CN WHERE CN.OrderTransactionID = OT.OrderTransactionID) AS AccountBalance, 
            CASE 
            	WHEN OTT.TransactionType = 'CreditAccountBalance' 
            		THEN CASE 
            			WHEN EXISTS(SELECT 1 FROM subscription.OrderTransaction OT1 INNER JOIN subscription.OrderTransactionType OTT1 ON OTT1.OrderTransactionTypeID = OT1.OrderTransactionTypeID AND OTT1.TransactionType = 'OrderClosed' WHERE OT1.OrderID = OT.OrderID AND OT.Amount < O.Amount) 
            				THEN 'Closed Order Payment'
            			WHEN EXISTS(SELECT 1 FROM subscription.OrderTransaction OT1 INNER JOIN subscription.OrderTransactionType OTT1 ON OTT1.OrderTransactionTypeID = OT1.OrderTransactionTypeID AND OTT1.TransactionType = 'OrderCancelled' WHERE OT1.OrderID = OT.OrderID AND OT.Amount < O.Amount) 
            				THEN 'Cancelled Order Payment'
            			WHEN EXISTS(SELECT 1 FROM subscription.[Order] O1 INNER JOIN subscription.OrderStatus OS ON O1.OrderStatusID = OS.OrderStatusID AND OS.OrderStatusName = 'Expired' WHERE O1.OrderID = OT.OrderID AND OT.Amount != O.Amount) 
            				THEN 'Expired Order Payment'
            			WHEN EXISTS(SELECT 1 FROM subscription.[Order] O1 INNER JOIN subscription.OrderStatus OS ON O1.OrderStatusID = OS.OrderStatusID AND OS.OrderStatusName = 'FullyPaid' WHERE O1.OrderID = OT.OrderID AND OT.Amount != O.Amount) 
            				THEN 'Over Payment'
            			WHEN EXISTS(SELECT 1 FROM subscription.[Order] O1 INNER JOIN subscription.OrderStatus OS ON O1.OrderStatusID = OS.OrderStatusID AND OS.OrderStatusName = 'FullyPaid' WHERE O1.OrderID = OT.OrderID AND OT.Amount = O.Amount) 
            				THEN 'Duplicate Payment'
            			ELSE OTT.TransactionType
            		END
            	WHEN OTT.TransactionType = 'DebitAccountBalance' 
            		THEN 'Applied To Sales'
            	ELSE OTT.TransactionType
            END AS PaymentNote, 
            O.OrderNumber AS OrderNumber
            FROM subscription.OrderTransaction OT
            INNER JOIN subscription.OrderTransactionType OTT
            ON OTT.OrderTransactionTypeID = OT.OrderTransactionTypeID
            AND OTT.TransactionType IN ('CreditAccountBalance', 'DebitAccountBalance')
            INNER JOIN subscription.[Order] O
            ON O.OrderID = OT.OrderID
            LEFT OUTER JOIN subscription.Payment P
            ON OT.PaymentID = P.PaymentID
            LEFT OUTER JOIN subscription.PaymentMethod PM
            ON PM.PaymentMethodID = P.PaymentMethodID
            WHERE OT.CreatedTimeStampt between @0 and @1
            
			UNION
            
			SELECT AT.CreatedTimeStampt AS DateReceived, 'Balance Refund' AS PaymentType, AT.AccountID AS AccountID, (SELECT CN.CreditNoteID from subscription.CreditNote CN WHERE CN.AccountTransactionID = AT.AccountTransactionID) AS CreditNoteNumber, AT.Amount * -1 AS CreditNoteAmount, (SELECT CN.AccountBalance from subscription.CreditNote CN WHERE CN.AccountTransactionID = AT.AccountTransactionID) AS AccountBalance, ATT.TransactionType AS PaymentNote, NULL AS OrderNumber
            FROM subscription.AccountTransaction AT
            INNER JOIN subscription.AccountTransactionType ATT
            ON ATT.AccountTransactionTypeID = AT.AccountTransactionTypeID
            AND ATT.TransactionType IN ('BalanceRefund')
            LEFT OUTER JOIN subscription.OrderLine OL
            ON OL.OrderLineID = AT.OrderLineID
            LEFT OUTER JOIN subscription.[Order] O
            ON O.OrderID = OL.OrderID
            LEFT OUTER JOIN subscription.Payment P
            ON P.OrderID = O.OrderID
            LEFT OUTER JOIN subscription.PaymentMethod PM
            ON PM.PaymentMethodID = P.PaymentMethodID
            LEFT OUTER JOIN subscription.OrderTransaction OT
            ON OT.OrderID = O.OrderID
            LEFT OUTER JOIN subscription.OrderTransactionType OTT
            ON OTT.OrderTransactionTypeID = OT.OrderTransactionTypeID
            WHERE AT.CreatedTimeStampt  between @0 and @1
			UNION
			SELECT AT.CreatedTimeStampt AS DateReceived, NULL AS PaymentType, AT.AccountID AS AccountID, (SELECT CN.CreditNoteID from subscription.CreditNote CN WHERE CN.AccountTransactionID = AT.AccountTransactionID) AS CreditNoteNumber, AT.Amount AS CreditNoteAmount, (SELECT CN.AccountBalance from subscription.CreditNote CN WHERE CN.AccountTransactionID = AT.AccountTransactionID) AS AccountBalance, 'Matched Payment' AS PaymentNote, NULL AS OrderNumber
            FROM subscription.AccountTransaction AT
            INNER JOIN subscription.AccountTransactionType ATT
            ON ATT.AccountTransactionTypeID = AT.AccountTransactionTypeID
            AND ATT.TransactionType IN ('CreditAccountBalance')
            INNER JOIN subscription.PaymentException PE
            ON AT.PaymentFeedEntryID = PE.PaymentFeedEntryID
            WHERE AT.CreatedTimeStampt  between @0 and @1 AND PE.[NewTelemarkerId] > 0
            UNION
			SELECT AT.CreatedTimeStampt AS DateReceived, NULL AS PaymentType, AT.AccountID AS AccountID, CN.CreditNoteID AS CreditNoteNumber, AT.Amount AS CreditNoteAmount,  CN.AccountBalance AS AccountBalance, 'Manual balance Adjustment' AS PaymentNote, NULL AS OrderNumber
            FROM subscription.AccountTransaction AT
			inner join subscription.CreditNote cn on cn.AccountTransactionID = at.AccountTransactionID
            WHERE AT.CreatedTimeStampt between @0 and @1
			and AT.AccountTransactionTypeId = 14 -- ACMA Balance Adjustment
			) RESULT ORDER BY DateReceived DESC";

        internal readonly string Query_CreditNotesCreatedTodayExcludingBalanceRefunds = @"SELECT isnull(SUM(creditNoteAmount),0) FROM (
            SELECT OT.CreatedTimeStampt AS DateReceived, PM.PaymentMethodName AS PaymentType, O.AccountID AS AccountID, (SELECT CN.CreditNoteID from subscription.CreditNote CN WHERE CN.OrderTransactionID = OT.OrderTransactionID) AS CreditNoteNumber, OT.Amount AS CreditNoteAmount, (SELECT A.CreditNotes FROM accessseeker.Account AS A WHERE A.AccountID = O.AccountID) AS AccountBalance, 
            CASE 
            	WHEN EXISTS(SELECT 1 FROM subscription.OrderTransaction OT1 INNER JOIN subscription.OrderTransactionType OTT1 ON OTT1.OrderTransactionTypeID = OT1.OrderTransactionTypeID AND OTT1.TransactionType = 'OrderClosed' WHERE OT1.OrderID = OT.OrderID AND OT.Amount < O.Amount) 
            		THEN 'Closed Order Payment'
            	WHEN EXISTS(SELECT 1 FROM subscription.OrderTransaction OT1 INNER JOIN subscription.OrderTransactionType OTT1 ON OTT1.OrderTransactionTypeID = OT1.OrderTransactionTypeID AND OTT1.TransactionType = 'OrderCancelled' WHERE OT1.OrderID = OT.OrderID AND OT.Amount < O.Amount) 
            		THEN 'Cancelled Order Payment'
            	WHEN EXISTS(SELECT 1 FROM subscription.[Order] O1 INNER JOIN subscription.OrderStatus OS ON O1.OrderStatusID = OS.OrderStatusID AND OS.OrderStatusName = 'Expired' WHERE O1.OrderID = OT.OrderID AND OT.Amount != O.Amount) 
            		THEN 'Expired Order Payment'
            	WHEN EXISTS(SELECT 1 FROM subscription.[Order] O1 INNER JOIN subscription.OrderStatus OS ON O1.OrderStatusID = OS.OrderStatusID AND OS.OrderStatusName = 'FullyPaid' WHERE O1.OrderID = OT.OrderID AND OT.Amount != O.Amount) 
            		THEN 'Over Payment'
            	WHEN EXISTS(SELECT 1 FROM subscription.[Order] O1 INNER JOIN subscription.OrderStatus OS ON O1.OrderStatusID = OS.OrderStatusID AND OS.OrderStatusName = 'FullyPaid' WHERE O1.OrderID = OT.OrderID AND OT.Amount = O.Amount) 
            		THEN 'Duplicate Payment'
            	ELSE OTT.TransactionType
            END AS PaymentNote, 
            O.OrderNumber AS OrderNumber
            FROM subscription.OrderTransaction OT
            INNER JOIN subscription.OrderTransactionType OTT
            ON OTT.OrderTransactionTypeID = OT.OrderTransactionTypeID
            AND OTT.TransactionType IN ('CreditAccountBalance')
            INNER JOIN subscription.[Order] O
            ON O.OrderID = OT.OrderID
            LEFT OUTER JOIN subscription.Payment P
            ON OT.PaymentID = P.PaymentID
            LEFT OUTER JOIN subscription.PaymentMethod PM
            ON PM.PaymentMethodID = P.PaymentMethodID
            WHERE OT.CreatedTimeStampt BETWEEN @0 AND @1
            UNION
            SELECT AT.CreatedTimeStampt AS DateReceived, NULL AS PaymentType, AT.AccountID AS AccountID, (SELECT CN.CreditNoteID from subscription.CreditNote CN WHERE CN.AccountTransactionID = AT.AccountTransactionID) AS CreditNoteNumber, AT.Amount AS Amount, (SELECT A.CreditNotes FROM accessseeker.Account AS A WHERE A.AccountID = AT.AccountID) AS AccountBalance, 'Matched Payment' AS PaymentNote, NULL AS OrderNumber
            FROM subscription.AccountTransaction AT
            INNER JOIN subscription.AccountTransactionType ATT
            ON ATT.AccountTransactionTypeID = AT.AccountTransactionTypeID
            AND ATT.TransactionType IN ('CreditAccountBalance')
            INNER JOIN subscription.PaymentException PE
            ON AT.PaymentFeedEntryID = PE.PaymentFeedEntryID
            WHERE AT.CreatedTimeStampt BETWEEN @0 AND @1 AND PE.[NewTelemarkerId] > 0
            ---Fix for SDADS-2199
			UNION
            SELECT AT.CreatedTimeStampt AS DateReceived, NULL AS PaymentType, AT.AccountID AS AccountID, (SELECT CN.CreditNoteID from subscription.CreditNote CN WHERE CN.AccountTransactionID = AT.AccountTransactionID) AS CreditNoteNumber, AT.Amount AS Amount, (SELECT A.CreditNotes FROM accessseeker.Account AS A WHERE A.AccountID = AT.AccountID) AS AccountBalance, 'Matched Payment' AS PaymentNote, NULL AS OrderNumber
            FROM subscription.AccountTransaction AT
            INNER JOIN subscription.AccountTransactionType ATT
            ON ATT.AccountTransactionTypeID = AT.AccountTransactionTypeID
            AND ATT.TransactionType IN ('ACMABalanceAdjustment')
            WHERE AT.CreatedTimeStampt BETWEEN @0 AND @1 
            ) RESULT";

        internal readonly string Query_GetCreditNoteRegister = @"SELECT * FROM (
            SELECT OT.CreatedTimeStampt AS DateReceived, PM.PaymentMethodName AS PaymentType, O.AccountID AS AccountID, (SELECT CN.CreditNoteID from subscription.CreditNote CN WHERE CN.OrderTransactionID = OT.OrderTransactionID) AS CreditNoteNumber, 
            CASE WHEN OTT.TransactionType = 'DebitAccountBalance' THEN OT.Amount * -1 ELSE OT.Amount END AS CreditNoteAmount, 
            (SELECT CN.AccountBalance from subscription.CreditNote CN WHERE CN.OrderTransactionID = OT.OrderTransactionID) AS AccountBalance, 
            CASE 
            	WHEN OTT.TransactionType = 'CreditAccountBalance' 
            		THEN CASE 
            			WHEN EXISTS(SELECT 1 FROM subscription.OrderTransaction OT1 INNER JOIN subscription.OrderTransactionType OTT1 ON OTT1.OrderTransactionTypeID = OT1.OrderTransactionTypeID AND OTT1.TransactionType = 'OrderClosed' WHERE OT1.OrderID = OT.OrderID AND OT.Amount < O.Amount) 
            				THEN 'Closed Order Payment'
            			WHEN EXISTS(SELECT 1 FROM subscription.OrderTransaction OT1 INNER JOIN subscription.OrderTransactionType OTT1 ON OTT1.OrderTransactionTypeID = OT1.OrderTransactionTypeID AND OTT1.TransactionType = 'OrderCancelled' WHERE OT1.OrderID = OT.OrderID AND OT.Amount < O.Amount) 
            				THEN 'Cancelled Order Payment'
            			WHEN EXISTS(SELECT 1 FROM subscription.[Order] O1 INNER JOIN subscription.OrderStatus OS ON O1.OrderStatusID = OS.OrderStatusID AND OS.OrderStatusName = 'Expired' WHERE O1.OrderID = OT.OrderID AND OT.Amount != O.Amount) 
            				THEN 'Expired Order Payment'
            			WHEN EXISTS(SELECT 1 FROM subscription.[Order] O1 INNER JOIN subscription.OrderStatus OS ON O1.OrderStatusID = OS.OrderStatusID AND OS.OrderStatusName = 'FullyPaid' WHERE O1.OrderID = OT.OrderID AND OT.Amount != O.Amount) 
            				THEN 'Over Payment'
            			WHEN EXISTS(SELECT 1 FROM subscription.[Order] O1 INNER JOIN subscription.OrderStatus OS ON O1.OrderStatusID = OS.OrderStatusID AND OS.OrderStatusName = 'FullyPaid' WHERE O1.OrderID = OT.OrderID AND OT.Amount = O.Amount) 
            				THEN 'Duplicate Payment'
            			ELSE OTT.TransactionType
            		END
            	WHEN OTT.TransactionType = 'DebitAccountBalance' 
            		THEN 'Applied To Sales'
            	ELSE OTT.TransactionType
            END AS PaymentNote, 
            O.OrderNumber AS OrderNumber
            FROM subscription.OrderTransaction OT
            INNER JOIN subscription.OrderTransactionType OTT
            ON OTT.OrderTransactionTypeID = OT.OrderTransactionTypeID
            AND OTT.TransactionType IN ('CreditAccountBalance', 'DebitAccountBalance')
            INNER JOIN subscription.[Order] O
            ON O.OrderID = OT.OrderID
            LEFT OUTER JOIN subscription.Payment P
            ON OT.PaymentID = P.PaymentID
            LEFT OUTER JOIN subscription.PaymentMethod PM
            ON PM.PaymentMethodID = P.PaymentMethodID
            WHERE OT.CreatedTimeStampt <= @0
            
			UNION
            
			SELECT AT.CreatedTimeStampt AS DateReceived, 'Balance Refund' AS PaymentType, AT.AccountID AS AccountID, (SELECT CN.CreditNoteID from subscription.CreditNote CN WHERE CN.AccountTransactionID = AT.AccountTransactionID) AS CreditNoteNumber, AT.Amount * -1 AS CreditNoteAmount, (SELECT CN.AccountBalance from subscription.CreditNote CN WHERE CN.AccountTransactionID = AT.AccountTransactionID) AS AccountBalance, ATT.TransactionType AS PaymentNote, NULL AS OrderNumber
            FROM subscription.AccountTransaction AT
            INNER JOIN subscription.AccountTransactionType ATT
            ON ATT.AccountTransactionTypeID = AT.AccountTransactionTypeID
            AND ATT.TransactionType IN ('BalanceRefund')
            LEFT OUTER JOIN subscription.OrderLine OL
            ON OL.OrderLineID = AT.OrderLineID
            LEFT OUTER JOIN subscription.[Order] O
            ON O.OrderID = OL.OrderID
            LEFT OUTER JOIN subscription.Payment P
            ON P.OrderID = O.OrderID
            LEFT OUTER JOIN subscription.PaymentMethod PM
            ON PM.PaymentMethodID = P.PaymentMethodID
            LEFT OUTER JOIN subscription.OrderTransaction OT
            ON OT.OrderID = O.OrderID
            LEFT OUTER JOIN subscription.OrderTransactionType OTT
            ON OTT.OrderTransactionTypeID = OT.OrderTransactionTypeID
            WHERE AT.CreatedTimeStampt <= @0
			UNION
			SELECT AT.CreatedTimeStampt AS DateReceived, NULL AS PaymentType, AT.AccountID AS AccountID, (SELECT CN.CreditNoteID from subscription.CreditNote CN WHERE CN.AccountTransactionID = AT.AccountTransactionID) AS CreditNoteNumber, AT.Amount AS CreditNoteAmount,  (SELECT CN.AccountBalance from subscription.CreditNote CN WHERE CN.AccountTransactionID = AT.AccountTransactionID) AS AccountBalance, 'Matched Payment' AS PaymentNote, NULL AS OrderNumber
            FROM subscription.AccountTransaction AT
            INNER JOIN subscription.AccountTransactionType ATT
            ON ATT.AccountTransactionTypeID = AT.AccountTransactionTypeID
            AND ATT.TransactionType IN ('CreditAccountBalance')
            INNER JOIN subscription.PaymentException PE
            ON AT.PaymentFeedEntryID = PE.PaymentFeedEntryID
            WHERE AT.CreatedTimeStampt <= @0 AND PE.[NewTelemarkerId] > 0
            UNION
			SELECT AT.CreatedTimeStampt AS DateReceived, NULL AS PaymentType, AT.AccountID AS AccountID, CN.CreditNoteID AS CreditNoteNumber, AT.Amount AS CreditNoteAmount,  CN.AccountBalance AS AccountBalance, 'Manual Balance Adjustment' AS PaymentNote, NULL AS OrderNumber
            FROM subscription.AccountTransaction AT
			inner join subscription.CreditNote cn on cn.AccountTransactionID = at.AccountTransactionID
            WHERE AT.CreatedTimeStampt <= @0
			and AT.AccountTransactionTypeId = 14 -- ACMA Balance Adjustment
            ) RESULT ORDER BY DateReceived DESC";
        #endregion

        #region Exceptions Report
        internal readonly string Query_UnmatchedPaymentsExceptionReport = @"SELECT ID, DateReceived, PaymentType, Reference, OrderNumber, ExceptionAmount, MAX(Cleared) AS Cleared, MIN(PaymentStatus) AS PaymentStatus FROM (
            SELECT DISTINCT PFE.PaymentFeedEntryID AS ID, PFR.CreatedTimeStamp AS DateReceived, PM.PaymentMethodName AS PaymentType, PFE.Reference AS Reference, O.OrderNumber, PFE.Amount AS ExceptionAmount, NULL AS Cleared, PET.TypeName AS PaymentStatus
            FROM subscription.PaymentExceptionAmendmentSnapshot PEAS
            INNER JOIN subscription.PaymentException PE
            ON PE.PaymentExceptionID = PEAS.PaymentExceptionID
            INNER JOIN subscription.PaymentExceptionType PET
            ON PET.PaymentExceptionTypeID = PEAS.PaymentExceptionTypeID
            AND PET.TypeName = 'UnmatchedPayment'
            INNER JOIN subscription.PaymentFeedEntry PFE
            ON PFE.PaymentFeedEntryID = PE.PaymentFeedEntryID
            INNER JOIN subscription.PaymentFeedRun PFR
            ON PFR.PaymentFeedRunID = PFE.PaymentFeedRunID
            LEFT OUTER JOIN subscription.Payment P
            ON P.PaymentFeedEntryID = PFE.PaymentFeedEntryID
            LEFT OUTER JOIN subscription.PaymentMethod PM
            ON PM.PaymentMethodID = P.PaymentMethodID
            LEFT OUTER JOIN subscription.[Order] O
            ON O.OrderID = P.OrderID
            WHERE PFR.CreatedTimeStamp <= @1 and (pe.ClearedTimeStamp is null or pe.ClearedTimeStamp > @1)
            UNION
            SELECT DISTINCT PFE.PaymentFeedEntryID AS ID, PFR.CreatedTimeStamp AS DateReceived, PM.PaymentMethodName AS PaymentType, PFE.Reference AS Reference, O.OrderNumber, PFE.Amount AS ExceptionAmount, PFE.Amount AS Cleared, 
            	CASE WHEN PE.ClearedByAgentUserID IS NOT NULL THEN 'Cleared'
            		 WHEN EXISTS (SELECT 1 FROM subscription.PaymentException PE1 INNER JOIN subscription.PaymentExceptionType PET1 ON PET1.PaymentExceptionTypeID = PE1.PaymentExceptionTypeID AND PET1.TypeName = 'InvalidPayment' WHERE PE1.PaymentExceptionID = PE.PaymentExceptionID) THEN 'InvalidPayment' 
            		 ELSE 'Matched' 
            	END AS PaymentStatus
            FROM subscription.PaymentExceptionAmendmentSnapshot PEAS
            INNER JOIN subscription.PaymentException PE
            ON PE.PaymentExceptionID = PEAS.PaymentExceptionID
            INNER JOIN subscription.PaymentExceptionType PET
            ON PET.PaymentExceptionTypeID = PEAS.PaymentExceptionTypeID
            AND PET.TypeName = 'UnmatchedPayment'
            INNER JOIN subscription.PaymentFeedEntry PFE
            ON PFE.PaymentFeedEntryID = PE.PaymentFeedEntryID
            INNER JOIN subscription.PaymentFeedRun PFR
            ON PFR.PaymentFeedRunID = PFE.PaymentFeedRunID
            LEFT OUTER JOIN subscription.Payment P
            ON P.PaymentFeedEntryID = PFE.PaymentFeedEntryID
            LEFT OUTER JOIN subscription.PaymentMethod PM
            ON PM.PaymentMethodID = P.PaymentMethodID
            LEFT OUTER JOIN subscription.[Order] O
            ON O.OrderID = P.OrderID
            WHERE PE.Cleared = 1
            AND PE.ClearedTimeStamp BETWEEN @0 AND @1 -- Uncleared Exceptions + Cleared Exceptions during the Report Period
            UNION
            SELECT DISTINCT PFE.PaymentFeedEntryID AS ID, PFR.CreatedTimeStamp AS DateReceived, PM.PaymentMethodName AS PaymentType, PFE.Reference AS Reference, O.OrderNumber, PFE.Amount AS ExceptionAmount, NULL AS Cleared, PET.TypeName AS PaymentStatus
            FROM subscription.PaymentException PE
            INNER JOIN subscription.PaymentExceptionType PET
            ON PET.PaymentExceptionTypeID = PE.PaymentExceptionTypeID
            AND PET.TypeName IN ('UnmatchedPayment', 'InvalidPayment','ReverseDishonourPayment')
            INNER JOIN subscription.PaymentFeedEntry PFE
            ON PFE.PaymentFeedEntryID = PE.PaymentFeedEntryID
            INNER JOIN subscription.PaymentFeedRun PFR
            ON PFR.PaymentFeedRunID = PFE.PaymentFeedRunID
            LEFT OUTER JOIN subscription.Payment P
            ON P.PaymentFeedEntryID = PFE.PaymentFeedEntryID
            LEFT OUTER JOIN subscription.PaymentMethod PM
            ON PM.PaymentMethodID = P.PaymentMethodID
            LEFT OUTER JOIN subscription.[Order] O
            ON O.OrderID = P.OrderID
            WHERE PFR.CreatedTimeStamp <= @1 and (pe.ClearedTimeStamp is null or pe.ClearedTimeStamp > @1)
            UNION
            SELECT DISTINCT PFE.PaymentFeedEntryID AS ID, PFR.CreatedTimeStamp AS DateReceived, PM.PaymentMethodName AS PaymentType, PFE.Reference AS Reference, O.OrderNumber, PFE.Amount AS ExceptionAmount, PFE.Amount AS Cleared, 
            	CASE WHEN PE.ClearedByAgentUserID IS NOT NULL THEN 'Cleared'
            		 WHEN EXISTS (SELECT 1 FROM subscription.PaymentException PE1 INNER JOIN subscription.PaymentExceptionType PET1 ON PET1.PaymentExceptionTypeID = PE1.PaymentExceptionTypeID AND PET1.TypeName = 'InvalidPayment' WHERE PE1.PaymentExceptionID = PE.PaymentExceptionID) THEN 'InvalidPayment' 
            		 ELSE 'Matched' 
            	END AS PaymentStatus
            FROM subscription.PaymentException PE
            INNER JOIN subscription.PaymentExceptionType PET
            ON PET.PaymentExceptionTypeID = PE.PaymentExceptionTypeID
            AND PET.TypeName IN ('UnmatchedPayment', 'InvalidPayment','ReverseDishonourPayment')
            INNER JOIN subscription.PaymentFeedEntry PFE
            ON PFE.PaymentFeedEntryID = PE.PaymentFeedEntryID
            INNER JOIN subscription.PaymentFeedRun PFR
            ON PFR.PaymentFeedRunID = PFE.PaymentFeedRunID
            LEFT OUTER JOIN subscription.Payment P
            ON P.PaymentFeedEntryID = PFE.PaymentFeedEntryID
            LEFT OUTER JOIN subscription.PaymentMethod PM
            ON PM.PaymentMethodID = P.PaymentMethodID
            LEFT OUTER JOIN subscription.[Order] O
            ON O.OrderID = P.OrderID
            WHERE PE.Cleared = 1
            AND PE.ClearedTimeStamp BETWEEN @0 AND @1 -- Uncleared Exceptions + Cleared Exceptions during the Report Period
            UNION
            SELECT OT.OrderTransactionID AS ID, OT.CreatedTimeStampt AS DateReceived, NULL AS PaymentType,
            	(SELECT TOP 1 P.PaymentReference FROM subscription.Payment AS P WHERE P.OrderID = OT.OrderID) AS Reference, 
            	(SELECT O.OrderNumber FROM subscription.[Order] AS O WHERE O.OrderID = OT.OrderID) AS OrderNumber,
            	OT.Amount AS ExceptionAmount,
            	NULL AS Cleared,
            	'Underpayment' AS PaymentStatus
            FROM subscription.OrderTransaction OT
            INNER JOIN subscription.OrderTransactionType OTT
            ON OTT.OrderTransactionTypeID = OT.OrderTransactionTypeID
            AND OTT.TransactionType = 'CreditOrderBalance'
            WHERE OT.CreatedTimeStampt <= @1
            ) RESULT
            GROUP BY ID, DateReceived, PaymentType, Reference, OrderNumber, ExceptionAmount
            ORDER BY DateReceived DESC";

        internal readonly string Query_UnmatchedPaymentsExceptionReport_OrderTransactions_ = @"
SELECT OT.OrderTransactionID, OT.CreatedTimeStampt AS PaymentDate, 
            	(SELECT TOP 1 P.PaymentReference FROM subscription.Payment AS P WHERE P.OrderID = OT.OrderID) AS PaymentReference, 
            	(SELECT O.AccountID FROM subscription.[Order] AS O WHERE O.OrderID = OT.OrderID) AS AccountID, 
            	OT.Amount AS Amount, 
            	(SELECT O.OrderNumber FROM subscription.[Order] AS O WHERE O.OrderID = OT.OrderID) AS OrderNumber,
            	OTT.TransactionType
            FROM subscription.OrderTransaction OT
            INNER JOIN subscription.OrderTransactionType OTT
            ON OTT.OrderTransactionTypeID = OT.OrderTransactionTypeID
            AND OTT.TransactionType IN ('CreditOrderBalance', 'OrderActivated', 'SubscriptionActivated', 'DebitOrderBalance', 'OrderClosed')
            WHERE OT.CreatedTimeStampt <= @1";
        #endregion
    }
}

//Credit Notes register, suspect even this is wrong
//declare @0 datetime, @1 datetime

//set @0 = '2015-09-09 17:44:13' 
//set @1 = '2015-09-10 15:41:07'

//SELECT * FROM (
//            SELECT OT.OrderTransactionID as unionpart, OT.CreatedTimeStampt AS DateReceived, PM.PaymentMethodName AS PaymentType, O.AccountID AS AccountID, (SELECT CN.CreditNoteID from subscription.CreditNote CN WHERE CN.OrderTransactionID = OT.OrderTransactionID) AS CreditNoteNumber, 
//            CASE WHEN OTT.TransactionType = 'DebitAccountBalance' THEN OT.Amount * -1 ELSE OT.Amount END AS CreditNoteAmount, 
//            (SELECT A.CreditNotes FROM accessseeker.Account AS A WHERE A.AccountID = O.AccountID) AS AccountBalance, 
//            CASE 
//                WHEN OTT.TransactionType = 'CreditAccountBalance' 
//                    THEN CASE 
//                        WHEN EXISTS(SELECT 1 FROM subscription.OrderTransaction OT1 INNER JOIN subscription.OrderTransactionType OTT1 ON OTT1.OrderTransactionTypeID = OT1.OrderTransactionTypeID AND OTT1.TransactionType = 'OrderClosed' WHERE OT1.OrderID = OT.OrderID AND OT.Amount < O.Amount) 
//                            THEN 'Closed Order Payment'
//                        WHEN EXISTS(SELECT 1 FROM subscription.OrderTransaction OT1 INNER JOIN subscription.OrderTransactionType OTT1 ON OTT1.OrderTransactionTypeID = OT1.OrderTransactionTypeID AND OTT1.TransactionType = 'OrderCancelled' WHERE OT1.OrderID = OT.OrderID AND OT.Amount < O.Amount) 
//                            THEN 'Cancelled Order Payment'
//                        WHEN EXISTS(SELECT 1 FROM subscription.[Order] O1 INNER JOIN subscription.OrderStatus OS ON O1.OrderStatusID = OS.OrderStatusID AND OS.OrderStatusName = 'Expired' WHERE O1.OrderID = OT.OrderID AND OT.Amount != O.Amount) 
//                            THEN 'Expired Order Payment'
//                        WHEN EXISTS(SELECT 1 FROM subscription.[Order] O1 INNER JOIN subscription.OrderStatus OS ON O1.OrderStatusID = OS.OrderStatusID AND OS.OrderStatusName = 'FullyPaid' WHERE O1.OrderID = OT.OrderID AND OT.Amount != O.Amount) 
//                            THEN 'Over Payment'
//                        WHEN EXISTS(SELECT 1 FROM subscription.[Order] O1 INNER JOIN subscription.OrderStatus OS ON O1.OrderStatusID = OS.OrderStatusID AND OS.OrderStatusName = 'FullyPaid' WHERE O1.OrderID = OT.OrderID AND OT.Amount = O.Amount) 
//                            THEN 'Duplicate Payment'
//                        ELSE OTT.TransactionType
//                    END
//                WHEN OTT.TransactionType = 'DebitAccountBalance' 
//                    THEN 'Applied To Sales'
//                ELSE OTT.TransactionType
//            END AS PaymentNote, 
//            O.OrderNumber AS OrderNumber
//            FROM subscription.OrderTransaction OT
//            INNER JOIN subscription.OrderTransactionType OTT
//            ON OTT.OrderTransactionTypeID = OT.OrderTransactionTypeID
//            AND OTT.TransactionType IN ('CreditAccountBalance', 'DebitAccountBalance')
//            INNER JOIN subscription.[Order] O
//            ON O.OrderID = OT.OrderID
//            LEFT OUTER JOIN subscription.Payment P
//            ON OT.PaymentID = P.PaymentID
//            LEFT OUTER JOIN subscription.PaymentMethod PM
//            ON PM.PaymentMethodID = P.PaymentMethodID
//            WHERE OT.CreatedTimeStampt between @0 and @1
            
//            UNION
            
//            SELECT 2, AT.CreatedTimeStampt AS DateReceived, 'Balance Refund' AS PaymentType, AT.AccountID AS AccountID, (SELECT CN.CreditNoteID from subscription.CreditNote CN WHERE CN.AccountTransactionID = AT.AccountTransactionID) AS CreditNoteNumber, AT.Amount * -1 AS CreditNoteAmount, (SELECT A.CreditNotes FROM accessseeker.Account AS A WHERE A.AccountID = AT.AccountID) AS AccountBalance, ATT.TransactionType AS PaymentNote, NULL AS OrderNumber
//            FROM subscription.AccountTransaction AT
//            INNER JOIN subscription.AccountTransactionType ATT
//            ON ATT.AccountTransactionTypeID = AT.AccountTransactionTypeID
//            AND ATT.TransactionType IN ('BalanceRefund')
//            LEFT OUTER JOIN subscription.OrderLine OL
//            ON OL.OrderLineID = AT.OrderLineID
//            LEFT OUTER JOIN subscription.[Order] O
//            ON O.OrderID = OL.OrderID
//            LEFT OUTER JOIN subscription.Payment P
//            ON P.OrderID = O.OrderID
//            LEFT OUTER JOIN subscription.PaymentMethod PM
//            ON PM.PaymentMethodID = P.PaymentMethodID
//            LEFT OUTER JOIN subscription.OrderTransaction OT
//            ON OT.OrderID = O.OrderID
//            LEFT OUTER JOIN subscription.OrderTransactionType OTT
//            ON OTT.OrderTransactionTypeID = OT.OrderTransactionTypeID
//            WHERE AT.CreatedTimeStampt  between @0 and @1
//            UNION
//            SELECT 3, AT.CreatedTimeStampt AS DateReceived, 
//                NULL AS PaymentType, 
//                AT.AccountID AS AccountID, 
//                CN.CreditNoteID AS CreditNoteNumber, 
//                AT.Amount AS CreditNoteAmount, 
//                (SELECT A.CreditNotes FROM accessseeker.Account AS A WHERE A.AccountID = AT.AccountID) AS AccountBalance, 
//                'Matched Payment' AS PaymentNote, 
//                NULL AS OrderNumber
//            FROM subscription.CreditNote CN
//            inner join subscription.AccountTransaction AT on CN.AccountTransactionID = AT.AccountTransactionID
//            INNER JOIN subscription.AccountTransactionType ATT ON ATT.AccountTransactionTypeID = AT.AccountTransactionTypeID AND ATT.TransactionType IN ('CreditAccountBalance')
//            INNER JOIN subscription.PaymentException PE
//            ON AT.PaymentFeedEntryID = PE.PaymentFeedEntryID
//            WHERE AT.CreatedTimeStampt  between @0 and @1 AND PE.[NewTelemarkerId] > 0
//            union
//            SELECT 4, AT.CreatedTimeStampt AS DateReceived, 
//                NULL AS PaymentType, 
//                AT.AccountID AS AccountID, 
//                CN.CreditNoteID AS CreditNoteNumber, 
//                AT.Amount AS CreditNoteAmount, 
//                (SELECT A.CreditNotes FROM accessseeker.Account AS A WHERE A.AccountID = AT.AccountID) AS AccountBalance, 
//                'Matched Payment' AS PaymentNote, 
//                NULL AS OrderNumber
//            FROM subscription.CreditNote CN
//            inner join subscription.AccountTransaction AT on CN.AccountTransactionID = AT.AccountTransactionID
//            inner join subscription.AccountTransactionType ATT on ATT.AccountTransactionTypeID = AT.AccountTransactionTypeID
//            WHERE AT.CreatedTimeStampt  between @0 and @1
//            AND AT.AccountTransactionTypeID IN (4)
//            ) RESULT ORDER BY DateReceived DESC