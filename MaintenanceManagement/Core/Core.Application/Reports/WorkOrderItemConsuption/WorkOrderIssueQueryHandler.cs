using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Contracts.Interfaces.External.IUser;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.External.IUnit;
using Core.Application.Common.Interfaces.IReports;
using Core.Application.Common.Interfaces.IWorkOrder;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.Reports.WorkOrderItemConsuption
{
    public class WorkOrderIssueQueryHandler : IRequestHandler<WorkOrderIssueQuery, ApiResponseDTO<List<WorkOrderIssueDto>>>
    {
        private readonly IReportRepository _repository;
        private readonly IMapper _mapper;
        private readonly IUnitGrpcClient _unitGrpcClient;
         private readonly IMediator _mediator;

        public WorkOrderIssueQueryHandler(IReportRepository repository, IMapper mapper, IUnitGrpcClient unitGrpcClient, IMediator mediator)
        {
            _repository = repository;
            _mapper = mapper;
            _unitGrpcClient = unitGrpcClient;
            _mediator = mediator;
        }

        public async Task<ApiResponseDTO<List<WorkOrderIssueDto>>> Handle(WorkOrderIssueQuery request, CancellationToken cancellationToken)
        {
            var fromDate = request.IssueFrom ?? throw new ArgumentNullException(nameof(request.IssueFrom));
            var toDate = request.IssueTo ?? throw new ArgumentNullException(nameof(request.IssueTo));
                   // Get data from repository
            var workOrders = await _repository.GetItemConsumptionAsync(
                        fromDate,
                        toDate,
                        request.MaintenanceTypeId
                    );

                    var workOrderDtos = _mapper.Map<List<WorkOrderIssueDto>>(workOrders);
                     var units = await _unitGrpcClient.GetUnitAutoCompleteAsync();
                    var unitLookup = units.ToDictionary(u => u.UnitId, u => u.UnitName);
                    foreach (var dto in workOrderDtos)
                    {
                        if (unitLookup.TryGetValue(dto.UnitId, out var unitName))
                        {
                            dto.UnitName = unitName;
                        }
                    }

                    // Domain event log
            var domainEvent = new AuditLogsDomainEvent(
                        actionDetail: "GetWorkOrderIssue",
                        actionCode: "Get",
                        actionName: workOrders.Count.ToString(),
                        details: "Work order issue list fetched.",
                        module: "WorkOrder"
                    );
                    await _mediator.Publish(domainEvent, cancellationToken);

                    return new ApiResponseDTO<List<WorkOrderIssueDto>>
                    {
                        IsSuccess = true,
                        Message = "Success",
                        Data = workOrderDtos,
                        TotalCount = workOrderDtos.Count
                    };
        }
    }
}