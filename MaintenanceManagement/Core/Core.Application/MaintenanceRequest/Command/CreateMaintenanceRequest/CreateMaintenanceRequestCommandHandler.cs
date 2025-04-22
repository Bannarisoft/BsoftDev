using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IMaintenanceRequest;
using Core.Application.Common.Interfaces.IWorkOrder;
using Core.Application.MaintenanceRequest.Queries.GetMaintenanceRequest;
using Core.Domain.Events;
using MediatR;

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
       

       public CreateMaintenanceRequestCommandHandler( IMaintenanceRequestCommandRepository maintenanceRequestCommandRepository, IMapper imapper, IMediator mediator, IMaintenanceRequestQueryRepository maintenanceRequestQueryRepository, IWorkOrderCommandRepository workOrderCommandRepository , IWorkOrderQueryRepository workOrderQueryRepository )
       {
           _maintenanceRequestCommandRepository = maintenanceRequestCommandRepository;
           _imapper = imapper;
           _mediator = mediator;
           _maintenanceRequestQueryRepository = maintenanceRequestQueryRepository;
           _workOrderCommandRepository = workOrderCommandRepository;
           _workOrderQueryRepository = workOrderQueryRepository;
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

            // 🔹 Insert into the database
            var result = await _maintenanceRequestCommandRepository.CreateAsync(maintenanceRequest);
        var WorkorderDocNo = await _workOrderQueryRepository.GetLatestWorkOrderDocNo(maintenanceRequest.RequestTypeId);
        var workorderData = _imapper.Map<Core.Domain.Entities.WorkOrderMaster.WorkOrder>(maintenanceRequest);
        workorderData.WorkOrderDocNo = WorkorderDocNo;

            if (result <= 0)
            {
                return new ApiResponseDTO<int>
                {
                    IsSuccess = false,
                    Message = "Failed to create Maintenance Request"
                };
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