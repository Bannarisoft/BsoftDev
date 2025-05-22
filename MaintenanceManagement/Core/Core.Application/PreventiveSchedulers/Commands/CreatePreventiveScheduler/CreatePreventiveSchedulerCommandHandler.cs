using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IPreventiveScheduler;
using Core.Domain.Entities;
using Core.Domain.Events;
using MediatR;
using Core.Application.Common.Interfaces.IMachineMaster;
using Core.Application.Common.Interfaces.IMiscMaster;
using Core.Application.WorkOrder.Command.CreateWorkOrder;
using static Core.Domain.Common.MiscEnumEntity;
using Hangfire;
using Core.Application.Common.Interfaces.IWorkOrder;
// using Core.Application.Common.Interfaces.IBackgroundService;
using Core.Application.Common.Interfaces;
using Contracts.Events.Maintenance.PreventiveScheduler;

namespace Core.Application.PreventiveSchedulers.Commands.CreatePreventiveScheduler
{
    public class CreatePreventiveSchedulerCommandHandler : IRequestHandler<CreatePreventiveSchedulerCommand, ApiResponseDTO<int>>
    {
        private readonly IPreventiveSchedulerCommand _preventiveSchedulerCommand;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        private readonly IEventPublisher _eventPublisher;
        private readonly IIPAddressService _ipAddressService;

        public CreatePreventiveSchedulerCommandHandler(IPreventiveSchedulerCommand preventiveSchedulerCommand, IMapper mapper, IMediator mediator,
         IEventPublisher eventPublisher, IIPAddressService iPAddressService)
        {
            _preventiveSchedulerCommand = preventiveSchedulerCommand;
            _mapper = mapper;
            _mediator = mediator;
            _eventPublisher = eventPublisher;
            _ipAddressService = iPAddressService;

        }
        public async Task<ApiResponseDTO<int>> Handle(CreatePreventiveSchedulerCommand request, CancellationToken cancellationToken)
        {
            
            var preventiveScheduler  = _mapper.Map<PreventiveSchedulerHeader>(request);

                var response = await _preventiveSchedulerCommand.CreateAsync(preventiveScheduler);
            var UnitId = _ipAddressService.GetUnitId();
                 if (response > 0 || response != null)
            {
                var correlationId = Guid.NewGuid();
                var @event = new PreventiveSchedulerHeaderCreationEvent
                {
                    CorrelationId = correlationId,
                    PreventiveSchedulerHeaderId = response,
                    MachineGroupId = preventiveScheduler.MachineGroupId,
                    FrequencyUnitId = preventiveScheduler.FrequencyUnitId,
                    EffectiveDate = preventiveScheduler.EffectiveDate,
                    FrequencyInterval = preventiveScheduler.FrequencyInterval,
                    ReminderWorkOrderDays = preventiveScheduler.ReminderWorkOrderDays,
                    UnitId =UnitId
                };
                // Save and publish event (RabbitMQ/Saga)
                await _eventPublisher.SaveEventAsync(@event);
                await _eventPublisher.PublishPendingEventsAsync();
            }
                
            //    var machineMaster = await _machineMasterQueryRepository.GetMachineByGroupAsync(request.MachineGroupId);
                
            //     var details = _mapper.Map<List<PreventiveSchedulerDetail>>(machineMaster);
            //     var frequencyUnit = await _miscMasterQueryRepository.GetByIdAsync(request.FrequencyUnitId);
                
                // var miscdetail = await _miscMasterQueryRepository.GetMiscMasterByName(WOStatus.MiscCode,StatusOpen.Code);
                //  foreach (var detail in details)
                //  {
                        // var lastMaintenanceDate = await _preventiveSchedulerQuery.GetLastMaintenanceDateAsync(detail.MachineId);

                    //  DateTime baseDate = (!lastMaintenanceDate.HasValue || lastMaintenanceDate.Value < request.EffectiveDate.ToDateTime(TimeOnly.MinValue))
                    //   request.EffectiveDate.ToDateTime(TimeOnly.MinValue)
                    //  : lastMaintenanceDate.Value.DateTime;

                    //     var (nextDate, reminderDate) = await _preventiveSchedulerQuery.CalculateNextScheduleDate(request.EffectiveDate.ToDateTime(TimeOnly.MinValue), request.FrequencyInterval, frequencyUnit.Code ?? "", request.ReminderWorkOrderDays);
                    //     var (ItemNextDate, ItemReminderDate) = await _preventiveSchedulerQuery.CalculateNextScheduleDate(request.EffectiveDate.ToDateTime(TimeOnly.MinValue), request.FrequencyInterval, frequencyUnit.Code ?? "", request.ReminderMaterialReqDays);

                    //  detail.PreventiveSchedulerHeaderId = response;
                    //  detail.WorkOrderCreationStartDate = DateOnly.FromDateTime(reminderDate); 
                    //  detail.ActualWorkOrderDate = DateOnly.FromDateTime(nextDate);
                    //  detail.MaterialReqStartDays = DateOnly.FromDateTime(ItemReminderDate);

                    //    var detailsResponse = await _preventiveSchedulerCommand.CreateDetailAsync(detail);
                    //    var delay = reminderDate - DateTime.Now;
                      
                     //   var workorderDocno =await _workOrderQueryRepository.GetLatestWorkOrderDocNo(preventiveScheduler.MaintenanceCategoryId);
                        // var workOrderRequest =  _mapper.Map<Core.Domain.Entities.WorkOrderMaster.WorkOrder>(preventiveScheduler, opt =>
                        // {
                        //     opt.Items["StatusId"] = miscdetail.Id;
                        //     // opt.Items["WorkOrderDocNo"] = workorderDocno;
                        //     opt.Items["PreventiveSchedulerDetailId"] = detailsResponse.Id;
                        // });
               
                     
                 
                      

                        //  string newJobId;
                        //  var delayInMinutes = (int)delay.TotalMinutes;
                        // if (delay.TotalSeconds > 0)
                        // {
                        //     //  newJobId =  BackgroundJob.Schedule(() => 
                        //     // _workOrderRepository.CreateAsync(workOrderRequest,preventiveScheduler.MaintenanceCategoryId, cancellationToken),
                        //     //  delay);

                        //     newJobId =  await _backgroundServiceClient.ScheduleWorkOrder(detailsResponse.Id,delayInMinutes);
                        // }
                        // else
                        // {
                        //     //    newJobId =  BackgroundJob.Schedule(() => 
                        //     // _workOrderRepository.CreateAsync(workOrderRequest,preventiveScheduler.MaintenanceCategoryId, cancellationToken),
                        //     //  TimeSpan.FromMinutes(15));
                        //    newJobId =  await _backgroundServiceClient.ScheduleWorkOrder(detailsResponse.Id,5);
                        // }
                  
                        //  await _preventiveSchedulerCommand.UpdateDetailAsync(detail.Id,newJobId);
                           //  {
                           //      PreventiveScheduleId = response,
                           //      StatusId = miscdetail.Id,
                           //      WorkOrderActivity = _mapper.Map<List<WorkOrderActivityDto>>(preventiveScheduler.PreventiveSchedulerActivities),
                           //      WorkOrderItem = _mapper.Map<List<WorkOrderItemDto>>(preventiveScheduler.PreventiveSchedulerItems)
                           //  };
                //  }
                


                 
                //  if(!detailsResponse)
                //  {
                //       await _preventiveSchedulerCommand.DeleteAsync(response,preventiveScheduler);
                //      return new ApiResponseDTO<int>
                //      {
                //          IsSuccess = false, 
                //          Message = "Preventive scheduler not created"
                //      };
                //  }
              
               
               
                    var domainEvent = new AuditLogsDomainEvent(
                     actionDetail: "Create",
                     actionCode: "Create preventive scheduler",
                     actionName: "Create",
                     details: $"Preventive scheduler created",
                     module:"Preventive scheduler"
                 );
                 await _mediator.Publish(domainEvent, cancellationToken);
                 
                    return new ApiResponseDTO<int>
                    {
                        IsSuccess = true, 
                        Message = "Preventive scheduler created successfully",
                         Data = response
                    };
            
                
        }
       
    }
}