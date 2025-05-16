
using Core.Application.Reports.GetStockLegerReport;
using Core.Application.Reports.MaintenanceRequestReport;
using Core.Application.Reports.WorkOrderItemConsuption;
using Core.Application.Reports.WorkOrderReport;
using Core.Application.StockLedger.Queries.GetCurrentStock;

namespace Core.Application.Common.Interfaces.IReports
{
    public interface IReportRepository
    {
        Task<List<RequestReportDto>> MaintenanceReportAsync(DateTimeOffset? requestFromDate, DateTimeOffset? requestToDate, int? RequestType, int? requestType, int? departmentId);
        Task<List<WorkOrderReportDto>> WorkOrderReportAsync(DateTimeOffset? fromDate, DateTimeOffset? toDate, int? RequestTypeId);
        Task<List<WorkOrderIssueDto>> GetItemConsumptionAsync(DateTimeOffset IssueFromDate, DateTimeOffset IssueToDate, int maintenanceTypeId);
        Task<List<StockLedgerReportDto>> GetSubStoresStockLedger(string OldUnitcode, DateTime FromDate, DateTime ToDate, string? Itemcode);
        Task<List<CurrentStockDto>> GetStockDetails(string OldUnitcode);
    }
}