using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IWorkOrder;
using Core.Application.WorkOrder.Queries.GetWorkOrder;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.WorkOrder.Command.UpdateWorkOrder
{
    public class UpdateWorkOrderCommandHandler : IRequestHandler<UpdateWorkOrderCommand, ApiResponseDTO<bool>>
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

        public async Task<ApiResponseDTO<bool>> Handle(UpdateWorkOrderCommand request, CancellationToken cancellationToken)
        {
            var assetMaster = await _workOrderRepository.GetByIdAsync(request.WorkOrder.Id);
            if (assetMaster is null)
            return new ApiResponseDTO<bool>
            {
                IsSuccess = false,
                Message = "Invalid AssetId. The specified AssetName does not exist or is inactive."
            };  
            var oldAssetName = assetMaster.WorkOrderDocNo;
            assetMaster.WorkOrderDocNo = request.WorkOrder.WorkOrderDocNo;

         
            var updatedAssetMasterEntity = _mapper.Map<Core.Domain.Entities.WorkOrderMaster.WorkOrder>(request);                   
            var updateResult = await _workOrderRepository.UpdateAsync(updatedAssetMasterEntity.Id, updatedAssetMasterEntity);            
         
                //Domain Event
                var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "Update",
                    actionCode: request.WorkOrder.WorkOrderDocNo ?? string.Empty,
                    actionName: "",                            
                    details: $"WorkOrder '{oldAssetName}' was updated to '{request.WorkOrder.RequestId}'",
                    module:"WorkOrder"
                );            
                await _mediator.Publish(domainEvent, cancellationToken);
                if(updateResult)
                {
                    return new ApiResponseDTO<bool>
                    {
                        IsSuccess = true,
                        Message = "WorkOrder updated successfully.",                        
                    };
                }
                return new ApiResponseDTO<bool>
                {
                    IsSuccess = false,
                    Message = "WorkOrder not updated."
                };                
            }          
        }
 }
