using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IWorkOrderMaster.IWorkOrder;
using Core.Application.WorkOrder.Queries.GetWorkOrder;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.WorkOrder.Command.UpdateWorkOrder
{
    public class UpdateWorkOrderCommandHandler : IRequestHandler<UpdateWorkOrderCommand, ApiResponseDTO<WorkOrderCombineDto>>
    { 
        private readonly IWorkOrderCommandRepository _workOrderRepository;
        private readonly IWorkOrderQueryRepository _workOrderQueryRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator; 

        public UpdateWorkOrderCommandHandler(IWorkOrderCommandRepository workOrderRepository, IMapper mapper,IWorkOrderQueryRepository workOrderQueryRepository, IMediator mediator)
        {
            _workOrderRepository = workOrderRepository;
            _mapper = mapper;
            _workOrderQueryRepository = workOrderQueryRepository;
            _mediator = mediator;
        }

        public async Task<ApiResponseDTO<WorkOrderCombineDto>> Handle(UpdateWorkOrderCommand request, CancellationToken cancellationToken)
        {
            var assetMaster = await _workOrderQueryRepository.GetByIdAsync(request.WorkOrder.Id);
            if (assetMaster is null)
            return new ApiResponseDTO<WorkOrderCombineDto>
            {
                IsSuccess = false,
                Message = "Invalid AssetId. The specified AssetName does not exist or is inactive."
            };
            var oldAssetName = assetMaster.RequestId;
            assetMaster.RequestId = request.WorkOrder.RequestId;

         
            var updatedAssetMasterEntity = _mapper.Map<Core.Domain.Entities.WorkOrderMaster.WorkOrder>(request);                   
            var updateResult = await _workOrderRepository.UpdateAsync( updatedAssetMasterEntity);            

            var updatedAssetMaster =  await _workOrderQueryRepository.GetByIdAsync(request.WorkOrder.Id);    
            if (updatedAssetMaster != null)
            {
                var assetMasterDto = _mapper.Map<WorkOrderCombineDto>(updatedAssetMaster);
                //Domain Event
                var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "Update",
                    actionCode: assetMasterDto.RequestId ?? string.Empty,
                    actionName: "",                            
                    details: $"WorkOrder '{oldAssetName}' was updated to '{assetMasterDto.RequestId}'",
                    module:"WorkOrder"
                );            
                await _mediator.Publish(domainEvent, cancellationToken);
                if(updateResult>0)
                {
                    return new ApiResponseDTO<WorkOrderCombineDto>
                    {
                        IsSuccess = true,
                        Message = "AssetMaster updated successfully.",
                        Data = assetMasterDto
                    };
                }
                return new ApiResponseDTO<WorkOrderCombineDto>
                {
                    IsSuccess = false,
                    Message = "AssetMaster not updated."
                };                
            }
            else
            {
                return new ApiResponseDTO<WorkOrderCombineDto>{
                    IsSuccess = false,
                    Message = "AssetMaster not found."
                };
            }        
        }
    }
}