
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IWorkOrder;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.WorkOrder.Command.UpdateWorkOrder.UpdateSchedule
{
    public class UpdateWOScheduleCommandHandler : IRequestHandler<UpdateWOScheduleCommand, ApiResponseDTO<bool>>
    { 
        private readonly IWorkOrderCommandRepository _workOrderRepository;        
        private readonly IMapper _mapper;
        private readonly IMediator _mediator; 

        public UpdateWOScheduleCommandHandler(IWorkOrderCommandRepository workOrderRepository, IMapper mapper,IMediator mediator)
        {
            _workOrderRepository = workOrderRepository;
            _mapper = mapper;            
            _mediator = mediator;
        }

        public async Task<ApiResponseDTO<bool>> Handle(UpdateWOScheduleCommand request, CancellationToken cancellationToken)
        {       
             if (request.WOSchedule == null)
            {
                return new ApiResponseDTO<bool>
                {
                    IsSuccess = false,
                    Message = "Schedule data is missing."
                };
            }   
            var updatedWOEntity = _mapper.Map<Core.Domain.Entities.WorkOrderMaster.WorkOrderSchedule>(request.WOSchedule);                   
            var updateResult = await _workOrderRepository.UpdateScheduleAsync(updatedWOEntity.WorkOrderId, updatedWOEntity);            
        
            //Domain Event
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "Create",
                actionCode: request.WOSchedule.WorkOrderId.ToString(),
                actionName: "",                            
                details: $"WorkOrder Schedule '{request.WOSchedule.WorkOrderId}' was updated",
                module:"WorkOrderSchedule Update"
            );            
            await _mediator.Publish(domainEvent, cancellationToken);
            if(updateResult)
            {
                return new ApiResponseDTO<bool>
                {
                    IsSuccess = true,
                    Message = "WorkOrder Schedule inserted successfully.",                        
                };
            }
            return new ApiResponseDTO<bool>
            {
                IsSuccess = false,
                Message = "WorkOrder Schedule not inserted."
            };                
        }          
    }
 }
