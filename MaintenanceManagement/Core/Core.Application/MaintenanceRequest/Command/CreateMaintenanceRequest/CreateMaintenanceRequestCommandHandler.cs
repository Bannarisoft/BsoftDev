using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Contracts.Events.Notifications.WorkOrder;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces;
using Core.Application.Common.Interfaces.IMaintenanceRequest;
using Core.Application.Common.Interfaces.IWorkOrder;
using Core.Application.Common.RealTimeNotificationHub;
using Core.Application.MaintenanceRequest.Queries.GetMaintenanceRequest;
using Core.Domain.Common;
using Core.Domain.Events;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using static Core.Domain.Common.MiscEnumEntity;

namespace Core.Application.MaintenanceRequest.Command.CreateMaintenanceRequest
{
    public class CreateMaintenanceRequestCommandHandler : IRequestHandler<CreateMaintenanceRequestCommand, ApiResponseDTO<int>>
    {
       

       private readonly IMaintenanceRequestCommandRepository  _maintenanceRequestCommandRepository;
       private readonly IMapper _imapper;
       private readonly IMediator _mediator;
       private readonly IMaintenanceRequestQueryRepository  _maintenanceRequestQueryRepository;
       private readonly IWorkOrderCommandRepository _workOrderCommandRepository;
       private readonly IWorkOrderQueryRepository _workOrderQueryRepository;
       private readonly IIPAddressService _ipAddressService;
       private readonly IHubContext<WorkOrderScheduleHub> _hubContext;
       private readonly IEventPublisher _eventPublisher;
       private readonly ILogger<CreateMaintenanceRequestCommandHandler> _logger;           
        private readonly IPublishEndpoint _publishEndpoint;

        public CreateMaintenanceRequestCommandHandler(IMaintenanceRequestCommandRepository maintenanceRequestCommandRepository, IMapper imapper, IMediator mediator, IMaintenanceRequestQueryRepository maintenanceRequestQueryRepository, IWorkOrderCommandRepository workOrderCommandRepository, IWorkOrderQueryRepository workOrderQueryQueryRepository, IIPAddressService ipAddressService, IHubContext<WorkOrderScheduleHub> hubContext, IEventPublisher eventPublisher, ILogger<CreateMaintenanceRequestCommandHandler> logger,IPublishEndpoint publishEndpoint)
        {
            _maintenanceRequestCommandRepository = maintenanceRequestCommandRepository;
            _imapper = imapper;
            _mediator = mediator;
            _maintenanceRequestQueryRepository = maintenanceRequestQueryRepository;
            _workOrderCommandRepository = workOrderCommandRepository;
            _workOrderQueryRepository = workOrderQueryQueryRepository;
            _ipAddressService = ipAddressService;
            _hubContext = hubContext;
            _eventPublisher = eventPublisher;
            _logger = logger;
            _publishEndpoint = publishEndpoint;
        }

        public async Task<ApiResponseDTO<int>> Handle(CreateMaintenanceRequestCommand request, CancellationToken cancellationToken)
        {
               // Misc status
              var statuses = await _maintenanceRequestQueryRepository.GetMaintenanceOpenstatusAsync();
                    var openStatus = statuses.FirstOrDefault();

            // 🔹 Map request to domain entity
            var maintenanceRequest = _imapper.Map<Core.Domain.Entities.MaintenanceRequest>(request);

            // 🔹 Override status from Misc
                maintenanceRequest.RequestStatusId = openStatus.Id;
                maintenanceRequest.CompanyId=_ipAddressService.GetCompanyId(); 
                maintenanceRequest.UnitId = _ipAddressService.GetUnitId();

            // 🔹 Insert into the database
            var result = await _maintenanceRequestCommandRepository.CreateAsync(maintenanceRequest);            

            if (result <= 0)
            {
                return new ApiResponseDTO<int>
                {
                    IsSuccess = false,
                    Message = "Failed to create Maintenance Request"
                };
            }            
            var requestTypes = await _maintenanceRequestQueryRepository.GetMaintenanceRequestTypeAsync();
            var internalTypeId = requestTypes.FirstOrDefault()?.Id;

            if (internalTypeId.HasValue && maintenanceRequest.RequestTypeId == internalTypeId.Value)
            {
                var workOrder = _imapper.Map<Core.Domain.Entities.WorkOrderMaster.WorkOrder>(maintenanceRequest);
                // workOrder.Id = 0; // important!
                workOrder.RequestId = result;
                workOrder.CompanyId = _ipAddressService.GetCompanyId();
                workOrder.UnitId = _ipAddressService.GetUnitId();

                await _workOrderCommandRepository.CreateAsync(workOrder, request.MaintenanceTypeId, cancellationToken);

                // 👇 Publish the WorkOrderCreatedEvent to trigger Saga
                var correlationId = Guid.NewGuid();
                await _publishEndpoint.Publish(new WorkOrderCreatedEvent
                {
                    CorrelationId =correlationId, // Important for Saga tracking
                    WorkOrderId = workOrder.Id,
                    WorkOrderTitle = "Create",
                    CreatedByName = workOrder.CreatedByName,
                    UnitId = _ipAddressService.GetUnitId(),
                    ModuleName = "WorkOrder",
                    EventTypeId = 14                    
                });

                _logger.LogInformation("✅ Maintenance Request Workorder Created. CorrelationId: {CorrelationId}, WorkOrderId: {WorkOrderId}",
                correlationId, workOrder.Id);                          
            }                                     
            // 🔹 Publish domain event for auditing/logging
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "Create",
                actionCode: request.MachineId.ToString(),
                actionName: "Maintenance Request Created",
                details: $"Maintenance Request was created.",
                module: "MaintenanceRequest"
            );

            await _mediator.Publish(domainEvent, cancellationToken);

            // 🔹 Return success response
            return new ApiResponseDTO<int>
            {
                IsSuccess = true,
                Message = "Maintenance Request created successfully",
                Data = result
            };
        }
    }
}