
using Core.Application.Dashboard.Common;
using Core.Application.Dashboard.WorkOrderSummary;

namespace Core.Application.Common.Interfaces.IDashboard
{
    public interface IDashboardQueryRepository
    {
        Task<ChartDto> WorkOrderSummaryAsync(DateTime fromDate, DateTime toDate, string? departmentId=null, string? machineGroupId=null);        
        Task<ChartDto> ItemConsumptionSummaryAsync(DateTime fromDate, DateTime toDate,string? departmentId=null, string? machineGroupId = null);           
        Task<ChartDto> MaintenanceHoursAsync(DateTime fromDate, DateTime toDate,string type, string? departmentId = null, string? machineGroupId = null);

    }
}