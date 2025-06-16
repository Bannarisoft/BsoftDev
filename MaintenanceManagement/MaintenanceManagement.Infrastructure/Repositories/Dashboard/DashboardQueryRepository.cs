// Updated Repository and Supporting Code for Unified ChartDto Across Dashboards

using System.Data;
using Contracts.Interfaces.External.IUser;
using Core.Application.Common.Interfaces;
using Core.Application.Common.Interfaces.IDashboard;
using Core.Application.Dashboard.Common;
using Core.Application.Dashboard.WorkOrderSummary;
using Dapper;
using MaintenanceManagement.Infrastructure.Repositories.Common;

namespace MaintenanceManagement.Infrastructure.Repositories.Dashboard
{
    public class DashboardQueryRepository : BaseQueryRepository, IDashboardQueryRepository
    {
        private readonly IDbConnection _connection;
        private readonly IDepartmentAllGrpcClient _departmentGrpcClient;

        public DashboardQueryRepository(
            IDbConnection connection,
            IDepartmentAllGrpcClient departmentGrpcClient,
            IIPAddressService ipAddressService)
            : base(ipAddressService)
        {
            _connection = connection;
            _departmentGrpcClient = departmentGrpcClient;
        }

        public async Task<List<ChartDto>> WorkOrderDepartmentSummaryAsync(DateTime fromDate, DateTime toDate, string type)
        {
            var data = await _connection.QueryAsync<WorkOrderDashboardDto>(
                "DB_GetMonthlyWorkOrderSummary",
                new { FromDate = fromDate, ToDate = toDate, UnitId = UnitId, Type = type },
                commandType: CommandType.StoredProcedure);

            var departments = await _departmentGrpcClient.GetDepartmentAllAsync();
            var deptLookup = departments.ToDictionary(d => d.DepartmentId, d => d.DepartmentName);

            foreach (var item in data)
                if (deptLookup.TryGetValue(item.Id, out var name))
                    item.Name = name;

            return MapToChartList(data.ToList());
        }

        public async Task<List<ChartDto>> WorkOrderMachineGroupSummaryAsync(DateTime fromDate, DateTime toDate, string type)
        {
            var data = await _connection.QueryAsync<WorkOrderDashboardDto>(
                "DB_GetMonthlyWorkOrderSummary",
                new { FromDate = fromDate, ToDate = toDate, UnitId = UnitId, Type = type },
                commandType: CommandType.StoredProcedure);

            return MapToChartList(data.ToList());
        }

        public async Task<List<ChartDto>> ItemConsumptionDepartmentSummaryAsync(DateTime fromDate, DateTime toDate, string type)
        {
            var data = await _connection.QueryAsync<WorkOrderDashboardDto>(
                "DB_GetMonthlyWorkOrderSummary",
                new { FromDate = fromDate, ToDate = toDate, UnitId = UnitId, Type = type },
                commandType: CommandType.StoredProcedure);

            var departments = await _departmentGrpcClient.GetDepartmentAllAsync();
            var deptLookup = departments.ToDictionary(d => d.DepartmentId, d => d.DepartmentName);

            foreach (var item in data)
                if (deptLookup.TryGetValue(item.Id, out var name))
                    item.Name = name;

            return MapToChartList(data.ToList());
        }

        public async Task<List<ChartDto>> ItemConsumptionMachineGroupSummaryAsync(DateTime fromDate, DateTime toDate, string type)
        {
            var data = await _connection.QueryAsync<WorkOrderDashboardDto>(
                "DB_GetMonthlyWorkOrderSummary",
                new { FromDate = fromDate, ToDate = toDate, UnitId = UnitId, Type = type },
                commandType: CommandType.StoredProcedure);

            return MapToChartList(data.ToList());
        }

        private List<ChartDto> MapToChartList(List<WorkOrderDashboardDto> rawData)
        {
            var result = new List<ChartDto>();

            var groupedByEntity = rawData.GroupBy(x => x.Name ?? "Unknown");

            foreach (var group in groupedByEntity)
            {
                var months = rawData.Select(x => x.Month).Distinct().OrderBy(x => x).ToList();

                var series = group
                .GroupBy(x => x.StatusName)
                .Select(statusGroup => new ChartSeriesDto
                {
                    Name = statusGroup.Key,
                    Data = months.Select(month =>
                        statusGroup.FirstOrDefault(x => x.Month == month)?.Total ?? 0
                    ).ToList()
                }).ToList();

                result.Add(new ChartDto
                {
                    Name = group.Key,
                    Categories = months,
                    Series = series
                });
            }
            return result;
        }
    }
}
