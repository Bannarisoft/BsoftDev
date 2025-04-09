
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IWorkOrderMaster.IWorkOrder;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.WorkOrder.Queries.GetWorkOrderById
{
    public class GetWorkOrderByIdQueryHandler : IRequestHandler<GetWorkOrderByIdQuery, ApiResponseDTO<GetWorkOrderByIdDto>>
    {
        private readonly IWorkOrderQueryRepository _workOrderQueryRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;         

        public GetWorkOrderByIdQueryHandler(IWorkOrderQueryRepository workOrderQueryRepository,  IMapper mapper, IMediator mediator)
        {
            _workOrderQueryRepository =workOrderQueryRepository;
            _mapper =mapper;
            _mediator = mediator;            
        }
        public async Task<ApiResponseDTO<GetWorkOrderByIdDto>> Handle(GetWorkOrderByIdQuery request, CancellationToken cancellationToken)
        {
          //  var assetMaster = await _assetMasterRepository.GetByIdAsync(request.Id);
          var (woResult, woActivity, woItem,woTechnician) = await _workOrderQueryRepository.GetWorkOrderByIdAsync(request.Id);
          var asset = _mapper.Map<GetWorkOrderByIdDto>(woResult);

            if (woActivity != null)
             {
                 asset.workOrderActivity = _mapper.Map<GetWorkOrderActivityByIdDto>(woResult);
             }
             if (woItem != null)
             {
                 asset.workOrderItem = _mapper.Map<GetWorkOrderItemByIdDto>(woItem);
             }
             if (woTechnician != null)
             {
                asset.workOrderTechnician = _mapper.Map<List<GetWorkOrderTechnicianByIdDto>>(woTechnician);
             }           

            if (asset is null)
            {                
                return new ApiResponseDTO<GetWorkOrderByIdDto>
                {
                    IsSuccess = false,
                    Message = "AssetName with ID {request.Id} not found."
                };   
            }       
            //Domain Event
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "GetById",
                actionCode:"",        
                actionName: "",                
                details: $"Asset ",
                module:"AssetMasterGeneral"
            );
            await _mediator.Publish(domainEvent, cancellationToken);
            return new ApiResponseDTO<GetWorkOrderByIdDto>
            {
                IsSuccess = true,
                Message = "Success",
                Data = asset
            };       
        }      
    }
}