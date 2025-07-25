
using Core.Application.Reports.GetStockLegerReport;
using Core.Application.Reports.MaintenanceRequestReport;
using Core.Application.Reports.WorkOrderItemConsuption;
using Core.Application.Reports.WorkOrderReport;
using Core.Application.StockLedger.Queries.GetCurrentStock;
using Core.Application.Reports.WorkOderCheckListReport;
using Core.Application.Reports.MRS;
using Core.Application.Reports.PowerConsumption;
using Core.Domain.Entities.Power;
using Core.Application.Reports.GeneratorConsumption;

namespace Core.Application.Common.Interfaces.IReports
{
    public interface IReportRepository
    {

        Task<List<RequestReportDto>> MaintenanceReportAsync(DateTimeOffset? requestFromDate, DateTimeOffset? requestToDate, int? RequestType, int? requestType, int? departmentId);
        Task<List<WorkOrderReportDto>> WorkOrderReportAsync(DateTimeOffset? fromDate, DateTimeOffset? toDate, int? RequestTypeId);
        Task<List<WorkOrderIssueDto>> GetItemConsumptionAsync(DateTimeOffset IssueFromDate, DateTimeOffset IssueToDate);
        Task<List<StockLedgerReportDto>> GetSubStoresStockLedger(string OldUnitcode, DateTime FromDate, DateTime ToDate, string? Itemcode, int departmentId);
        Task<List<CurrentStockDto>> GetStockDetails(string OldUnitcode, int departmentId);
        Task<List<WorkOderCheckListReportDto>> GetWorkOrderChecklistReportAsync(DateTimeOffset? requestFromDate, DateTimeOffset? requestToDate, int? machineGroupId, int? machineId, int? activityId);
        Task<List<MRSReportDto>> GetMRSReports(DateTimeOffset IssueFromDate, DateTimeOffset IssueToDate, string OldUnitCode);
        Task<IEnumerable<dynamic>> ScheduleReportAsync(DateTime? FromDueDate, DateTime? ToDueDate);
        Task<IEnumerable<dynamic>> MaterialPlanningReportAsync(DateTime? FromDueDate, DateTime? ToDueDate);
        Task<List<PowerReportDto>> GetPowerReports(DateTimeOffset? FromDate, DateTimeOffset? ToDate);
        Task<List<GeneratorReportDto>> GetGeneratorReports(DateTimeOffset? FromDate, DateTimeOffset? ToDate);
    }
}