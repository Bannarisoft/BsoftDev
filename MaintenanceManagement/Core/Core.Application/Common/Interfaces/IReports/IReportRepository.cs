
using Core.Application.Reports.MaintenanceRequestReport;

namespace Core.Application.Common.Interfaces.IReports
{
    public interface IReportRepository
    {
        Task<List<RequestReportDto>> MaintenanceReportAsync( DateTimeOffset? requestFromDate,DateTimeOffset? requestToDate, int? RequestType,int? requestType ,int? departmentId);
    }
}