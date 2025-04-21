
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IWorkOrder;
using Core.Application.WorkOrder.Queries.GetWorkOrderById;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.WorkOrder.Queries.GetWorkOrder
{
    public class GetWorkOrderQueryHandler : IRequestHandler<GetWorkOrderQuery, ApiResponseDTO<List<GetWorkOrderByIdDto>>>
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
        public async Task<ApiResponseDTO<List<GetWorkOrderByIdDto>>> Handle(GetWorkOrderQuery request, CancellationToken cancellationToken)
        {
            var (assetMaster, totalCount) = await _workOrderRepository.GetAllWOAsync(request.PageNumber, request.PageSize, request.SearchTerm);
            var assetMasterList = _mapper.Map<List<GetWorkOrderByIdDto>>(assetMaster);

            //Domain Event
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "GetAll",
                actionCode: "",        
                actionName: "",
                details: $"WorkOrderDetails details was fetched.",
                module:"WorkOrderDetails"
            );
            await _mediator.Publish(domainEvent, cancellationToken);
            return new ApiResponseDTO<List<GetWorkOrderByIdDto>>
            {
                IsSuccess = true,
                Message = "Success",
                Data = assetMasterList,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };            
        }
    }
}