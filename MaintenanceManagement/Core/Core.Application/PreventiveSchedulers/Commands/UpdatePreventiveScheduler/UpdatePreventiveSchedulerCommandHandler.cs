using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Contracts.Dtos.Maintenance.Preventive;
using Contracts.Events.Maintenance.PreventiveScheduler.PreventiveSchedulerUpdate;
using Contracts.Interfaces.External.IMaintenance;
using Core.Application.Common;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces;
// using Core.Application.Common.Interfaces.IBackgroundService;
using Core.Application.Common.Interfaces.IMiscMaster;
using Core.Application.Common.Interfaces.IPreventiveScheduler;
using Core.Application.Common.Interfaces.IPreventiveSchedulerLog;
using Core.Application.Common.Interfaces.IWorkOrder;
using Core.Application.PreventiveSchedulers.Commands.CreatePreventiveScheduler;
using Core.Domain.Entities;
using Core.Domain.Events;
using Hangfire;
using MediatR;
using Newtonsoft.Json;
using static Core.Domain.Common.MiscEnumEntity;

namespace Core.Application.PreventiveSchedulers.Commands.UpdatePreventiveScheduler
{
    public class UpdatePreventiveSchedulerCommandHandler : IRequestHandler<UpdatePreventiveSchedulerCommand, ApiResponseDTO<bool>>
    {
        private readonly IPreventiveSchedulerCommand _preventiveSchedulerCommand;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        
        private readonly IPreventiveSchedulerQuery _preventiveSchedulerQuery;
        private readonly IWorkOrderCommandRepository _workOrderRepository;
        private readonly IIPAddressService _ipAddressService;
        private readonly ITimeZoneService _timeZoneService;
        
        private readonly IEventPublisher _eventPublisher;
        private readonly IPreventiveScheduleLogService _preventiveScheduleLogService;
        public UpdatePreventiveSchedulerCommandHandler(IPreventiveSchedulerCommand preventiveSchedulerCommand, IMapper mapper, IMediator mediator,
         IPreventiveSchedulerQuery preventiveSchedulerQuery, IWorkOrderCommandRepository workOrderRepository,
        IIPAddressService ipAddressService, ITimeZoneService timeZoneService, IEventPublisher eventPublisher, IPreventiveScheduleLogService preventiveScheduleLogService)
        {
            _preventiveSchedulerCommand = preventiveSchedulerCommand;
            _mapper = mapper;
            _mediator = mediator;

            _preventiveSchedulerQuery = preventiveSchedulerQuery;
            _workOrderRepository = workOrderRepository;
            _ipAddressService = ipAddressService;
            _timeZoneService = timeZoneService;

            _eventPublisher = eventPublisher;
            _preventiveScheduleLogService = preventiveScheduleLogService;
        }
        public async Task<ApiResponseDTO<bool>> Handle(UpdatePreventiveSchedulerCommand request, CancellationToken cancellationToken)
        {
           await _preventiveScheduleLogService.CaptureLogs(request.Id,null,"Update",JsonConvert.SerializeObject(request));
            var preventiveScheduler  = _mapper.Map<PreventiveSchedulerHeader>(request);

            var existingPreventiveScheduler = await _preventiveSchedulerQuery.GetByIdAsync(request.Id);

            var rollbackHeader = _mapper.Map<RollbackHeaderDto>(existingPreventiveScheduler);
            
            bool isFrequencyChanged =
            request.FrequencyInterval != existingPreventiveScheduler.FrequencyInterval ||
            request.FrequencyTypeId != existingPreventiveScheduler.FrequencyTypeId ||
            request.FrequencyUnitId != existingPreventiveScheduler.FrequencyUnitId;

            var metaDataResponse = await _preventiveSchedulerCommand.UpdateScheduleMetadata(preventiveScheduler);

            if (metaDataResponse != null && metaDataResponse.Id > 0)
            {
                if (isFrequencyChanged)
                {

                var UnitId = _ipAddressService.GetUnitId();
               
                    var correlationId = Guid.NewGuid();
                    var @event = new HeaderUpdateEvent
                    {
                        CorrelationId = correlationId,
                        PreventiveSchedulerHeaderId = metaDataResponse.Id,
                        UnitId = UnitId,
                        FrequencyUnitId = metaDataResponse.FrequencyUnitId,
                        FrequencyInterval = metaDataResponse.FrequencyInterval,
                        ReminderWorkOrderDays = metaDataResponse.ReminderWorkOrderDays,
                        ReminderMaterialReqDays = metaDataResponse.ReminderMaterialReqDays,
                        rollbackHeaders = rollbackHeader
                    };

                    await _eventPublisher.SaveEventAsync(@event);
                    await _eventPublisher.PublishPendingEventsAsync();
                }
            }

              await AuditLogPublisher.PublishAuditLogAsync(
                     _mediator,
                     actionDetail: $"Schedule Update request",
                     actionCode: "Schedule Update",
                     actionName: "Schedule Update",
                     module: "Preventive",
                     requestData: request,
                     cancellationToken
                    );
                 
              
                if(metaDataResponse.Id > 0)
                {
                    return new ApiResponseDTO<bool>
                    {
                        IsSuccess = true, 
                        Message = "Preventive Scheduler updated successfully."
                    };
                }

                return new ApiResponseDTO<bool>
                {
                    IsSuccess = false, 
                    Message = "Preventive Scheduler not updated."
                };
        }
    }
}