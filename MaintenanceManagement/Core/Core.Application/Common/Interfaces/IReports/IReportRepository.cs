
using Core.Application.Reports.MaintenanceRequestReport;
using Core.Application.Reports.WorkOrderReport;

namespace Core.Application.Common.Interfaces.IReports
{
    public interface IReportRepository
    {
        Task<List<RequestReportDto>> MaintenanceReportAsync( DateTimeOffset? requestFromDate,DateTimeOffset? requestToDate, int? RequestType,int? requestType ,int? departmentId);
        Task<List<WorkOrderReportDto>> WorkOrderReportAsync( DateTimeOffset? fromDate,DateTimeOffset? toDate, int? RequestTypeId);
    }
}