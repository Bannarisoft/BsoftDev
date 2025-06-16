using Core.Application.Common.Interfaces.IDashboard;
using Core.Application.Dashboard.Common;
using Core.Application.Dashboard.DashboardQuery;
using MediatR;

public class DashboardQueryHandler : IRequestHandler<DashboardQuery, List<ChartDto>>
{
    private readonly IDashboardQueryRepository _repository;

    public DashboardQueryHandler(IDashboardQueryRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<ChartDto>> Handle(DashboardQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Type))
            throw new ArgumentException("Type is required. Valid values: 'department', 'machinegroup', 'itemconsumption-dept', 'itemconsumption-machine'");

        return request.Type.ToLower() switch
        {
            "department" => await _repository.WorkOrderDepartmentSummaryAsync(request.FromDate, request.ToDate, request.Type),
            "machinegroup" => await _repository.WorkOrderMachineGroupSummaryAsync(request.FromDate, request.ToDate, request.Type),
            "itemconsumption-dept" => await _repository.ItemConsumptionDepartmentSummaryAsync(request.FromDate, request.ToDate, request.Type),
            "itemconsumption-machine" => await _repository.ItemConsumptionMachineGroupSummaryAsync(request.FromDate, request.ToDate, request.Type),
            _ => throw new ArgumentException("Invalid type. Must be one of: 'department', 'machinegroup', 'itemconsumption-dept', 'itemconsumption-machine'")
        };
    }
}
