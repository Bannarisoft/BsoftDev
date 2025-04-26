
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces;
using Core.Application.Common.Interfaces.IWorkOrder;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.WorkOrder.Command.CreateWorkOrder.CreateSchedule
{
    public class CreateWOScheduleCommandHandler : IRequestHandler<CreateWOScheduleCommand, ApiResponseDTO<bool>>
    { 
        private readonly IWorkOrderCommandRepository _workOrderRepository;        
        private readonly IMapper _mapper;
        private readonly IMediator _mediator; 
        private readonly ITimeZoneService _timeZoneService;

        public CreateWOScheduleCommandHandler(IWorkOrderCommandRepository workOrderRepository, IMapper mapper, IMediator mediator, ITimeZoneService timeZoneService)
        {
            _workOrderRepository = workOrderRepository;
            _mapper = mapper;            
            _mediator = mediator;
            _timeZoneService = timeZoneService; 
        }

        public async Task<ApiResponseDTO<bool>> Handle(CreateWOScheduleCommand request, CancellationToken cancellationToken)
        {   
            var systemTimeZoneId = _timeZoneService.GetSystemTimeZone();
            var systemTimeZone = TimeZoneInfo.FindSystemTimeZoneById(systemTimeZoneId);

            request.WOSchedule.StartTime = TimeZoneInfo.ConvertTime(request.WOSchedule.StartTime, systemTimeZone);
            if (request.WOSchedule.EndTime != null)
            {
                request.WOSchedule.EndTime = TimeZoneInfo.ConvertTime(request.WOSchedule.EndTime.Value, systemTimeZone);
            }            
      
            var createWOEntity = _mapper.Map<Core.Domain.Entities.WorkOrderMaster.WorkOrderSchedule>(request.WOSchedule);                   
            var updateResult = await _workOrderRepository.CreateScheduleAsync(createWOEntity.WorkOrderId, createWOEntity);            
        
            //Domain Event
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "Create",
                actionCode: request.WOSchedule.WorkOrderId.ToString(),
                actionName: "",                            
                details: $"WorkOrder Schedule '{request.WOSchedule.WorkOrderId}' was updated",
                module:"WorkOrderSchedule Create"
            );            
            await _mediator.Publish(domainEvent, cancellationToken);
            if(updateResult!=0)
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
