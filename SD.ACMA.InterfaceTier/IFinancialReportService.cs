using SD.ACMA.DNCR.Infrastructure;
using SD.ACMA.POCO.PetaPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SD.ACMA.InterfaceTier
{
    public interface IFinancialReportService
    {
        List<POCO.PetaPoco.FinancialReport> GetAllFinancialReports();
        int InsertNewFinancialReport(DateTime? fromDate, DateTime toDate, double totalOutstandingExceptionsAmount, double totalCreditNotesAmount, double creditCardTimingDifference, string status, string markedFinalBy);
        void UpdateFinancialReport(int Id, double totalOutstandingExceptionsAmount, double totalCreditNotesAmount, double creditCardTimingDifference, string status);
        POCO.PetaPoco.FinancialReport GetLastFinancialReport();

        FinancialReport_PaymentsReconReport RunPaymentsReconReport(DateTime? fromDate, DateTime toDate);
        FinancialReport_ExceptionsReport RunExceptionsReport(DateTime? fromDate, DateTime toDate);
        FinancialReport_CreditNotesReport RunCreditNotesReport(DateTime? fromDate, DateTime toDate);

        void UpdateAllFinancialReports();
    }
}
