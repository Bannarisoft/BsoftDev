
using Core.Application.Reports.MaintenanceRequestReport;
using Core.Application.Reports.WorkOrderReport;
using Core.Application.Reports.WorkOderCheckListReport;

namespace Core.Application.Common.Interfaces.IReports
{
    public interface IReportRepository
    {
        Task<List<RequestReportDto>> MaintenanceReportAsync(DateTimeOffset? requestFromDate, DateTimeOffset? requestToDate, int? RequestType, int? requestType, int? departmentId);
        Task<List<WorkOrderReportDto>> WorkOrderReportAsync( DateTimeOffset? fromDate,DateTimeOffset? toDate, int? RequestTypeId);
		Task<List<WorkOderCheckListReportDto>> GetWorkOrderChecklistReportAsync( DateTimeOffset? requestFromDate,DateTimeOffset? requestToDate, int? machineGroupId, int? machineId,int? activityId);
    }
}