
using Core.Application.Dashboard.Common;
using Core.Application.Dashboard.WorkOrderSummary;

namespace Core.Application.Common.Interfaces.IDashboard
{
    public interface IDashboardQueryRepository
    {
        Task<List<ChartDto>> WorkOrderDepartmentSummaryAsync(DateTime fromDate, DateTime toDate, string type);
        Task<List<ChartDto>> WorkOrderMachineGroupSummaryAsync(DateTime fromDate, DateTime toDate, string type);
        Task<List<ChartDto>> ItemConsumptionMachineGroupSummaryAsync(DateTime fromDate, DateTime toDate, string type);
        Task<List<ChartDto>> ItemConsumptionDepartmentSummaryAsync(DateTime fromDate, DateTime toDate, string type); 
    }
}