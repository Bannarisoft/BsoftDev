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
using Core.Application.Common;

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
                    ScheduleId = preventiveScheduler.ScheduleId,
                    FrequencyTypeId = preventiveScheduler.FrequencyTypeId,
                    FrequencyUnitId = preventiveScheduler.FrequencyUnitId,
                    GraceDays = preventiveScheduler.GraceDays,
                    ReminderWorkOrderDays = preventiveScheduler.ReminderWorkOrderDays,
                    ReminderMaterialReqDays = preventiveScheduler.ReminderMaterialReqDays,
                    IsDownTimeRequired = preventiveScheduler.IsDownTimeRequired,
                    DownTimeEstimateHrs = preventiveScheduler.DownTimeEstimateHrs,
                    EffectiveDate = preventiveScheduler.EffectiveDate,
                    FrequencyInterval = preventiveScheduler.FrequencyInterval,
                    UnitId =UnitId
                };
                // Save and publish event (RabbitMQ/Saga)
                await _eventPublisher.SaveEventAsync(@event);
                await _eventPublisher.PublishPendingEventsAsync();
            }
              
                     await AuditLogPublisher.PublishAuditLogAsync(
                     _mediator,
                     actionDetail: "Create",
                     actionCode: "NewData",
                     actionName: "Preventive Schedule Creation",
                     module: "Preventive",
                     requestData: request,
                     cancellationToken
                    );
               
              
                 
                    return new ApiResponseDTO<int>
                    {
                        IsSuccess = true, 
                        Message = "Preventive scheduler created successfully",
                         Data = response
                    };
            
                
        }
       
    }
}