using Core.Application.Common.Interfaces.IDashboard;
using Core.Application.Dashboard.Common;
using Core.Application.Dashboard.DashboardQuery;
using MediatR;

public class DashboardQueryHandler : IRequestHandler<DashboardQuery, ChartDto>
{
    private readonly IDashboardQueryRepository _repository;

    public DashboardQueryHandler(IDashboardQueryRepository repository)
    {
        _repository = repository;
    }

    public async Task<ChartDto> Handle(DashboardQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Type))
            throw new ArgumentException("Type is required. Valid values: 'workordersummary', 'itemconsumption', 'maintenancehrs-dept', 'maintenancehrs-machinegroup', 'maintenancehrs-machine'");

        return request.Type.ToLower() switch
        {
            "workordersummary" => await _repository.WorkOrderSummaryAsync(request.FromDate, request.ToDate, request.DepartmentId, request.MachineGroupId),
            "itemconsumption" => await _repository.ItemConsumptionSummaryAsync(request.FromDate, request.ToDate, request.DepartmentId, request.MachineGroupId),
            "itemconsumption-dept" => await _repository.ItemConsumptionDeptSummaryAsync(request.FromDate, request.ToDate,request.Type,request.DepartmentId,request.ItemCode),
            "itemconsumption-machinegroup" => await _repository.ItemConsumptionMachineSummaryAsync(request.FromDate, request.ToDate,request.Type,request.DepartmentId,request.ItemCode),
            "maintenancehrs-dept" => await _repository.MaintenanceHoursDeptAsync(request.FromDate, request.ToDate,request.Type,request.DepartmentId),
            "maintenancehrs-machinegroup" => await _repository.MaintenanceHoursMachineGroupAsync(request.FromDate, request.ToDate,request.Type,request.DepartmentId),
            "maintenancehrs-machine" => await _repository.MaintenanceHoursMachineAsync(request.FromDate, request.ToDate,request.Type, request.DepartmentId, request.MachineGroupId),
            _ => throw new ArgumentException("Invalid type.")
        };
    }
}
