
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IWorkOrder;
using Core.Application.WorkOrder.Queries.GetWorkOrderById;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.WorkOrder.Queries.GetWorkOrder
{
    public class GetWorkOrderQueryHandler : IRequestHandler<GetWorkOrderQuery, ApiResponseDTO<List<Dictionary<string, List<GetWorkOrderDto>>>>>
    {
        private readonly IWorkOrderQueryRepository _workOrderRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator; 

        public GetWorkOrderQueryHandler(IWorkOrderQueryRepository workOrderRepository , IMapper mapper, IMediator mediator)
        {
            _workOrderRepository = workOrderRepository;
            _mapper = mapper;
            _mediator = mediator;
        }
  
        public async Task<ApiResponseDTO<List<Dictionary<string, List<GetWorkOrderDto>>>>> Handle(GetWorkOrderQuery request, CancellationToken cancellationToken)
        {
           var (workOrder, totalCount) = await _workOrderRepository.GetAllWOAsync(request.fromDate,request.toDate, request.requestType, request.PageNumber, request.PageSize, request.SearchTerm);            
           var mappedWorkOrders = _mapper.Map<List<GetWorkOrderDto>>(workOrder);
            var groupedWorkOrders = mappedWorkOrders
            .GroupBy(w => w.MaintenanceType ?? "Unknown")
            .Select(g => new Dictionary<string, List<GetWorkOrderDto>>
            {
                [g.Key] = g.ToList()
            })
            .ToList();

            //Domain Event
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "GetAll",
                actionCode: "",        
                actionName: "",
                details: $"WorkOrderDetails details was fetched.",
                module:"WorkOrderDetails"
            );
            await _mediator.Publish(domainEvent, cancellationToken);
           return new ApiResponseDTO<List<Dictionary<string, List<GetWorkOrderDto>>>>
            {
                IsSuccess = true,
                Message = "Success",
                Data = groupedWorkOrders,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };     
        }
    }
}