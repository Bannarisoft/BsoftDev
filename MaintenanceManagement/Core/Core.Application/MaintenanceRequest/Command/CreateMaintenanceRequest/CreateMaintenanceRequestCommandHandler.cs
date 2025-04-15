using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IMaintenanceRequest;
using Core.Application.MaintenanceRequest.Queries.GetMaintenanceRequest;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.MaintenanceRequest.Command.CreateMaintenanceRequest
{
    public class CreateMaintenanceRequestCommandHandler : IRequestHandler<CreateMaintenanceRequestCommand, ApiResponseDTO<GetMaintenanceRequestDto>>
    {
       

       private readonly IMaintenanceRequestCommandRepository  _maintenanceRequestCommandRepository;
       private readonly IMapper _imapper;
       private readonly IMediator _mediator;
       private readonly IMaintenanceRequestQueryRepository  _maintenanceRequestQueryRepository;
       

       public CreateMaintenanceRequestCommandHandler( IMaintenanceRequestCommandRepository maintenanceRequestCommandRepository, IMapper imapper, IMediator mediator, IMaintenanceRequestQueryRepository maintenanceRequestQueryRepository)
       {
           _maintenanceRequestCommandRepository = maintenanceRequestCommandRepository;
           _imapper = imapper;
           _mediator = mediator;
           _maintenanceRequestQueryRepository = maintenanceRequestQueryRepository;
       }

        public async Task<ApiResponseDTO<GetMaintenanceRequestDto>> Handle(CreateMaintenanceRequestCommand request, CancellationToken cancellationToken)
        {
            // 🔹 Map request to domain entity
            var maintenanceRequest = _imapper.Map<Core.Domain.Entities.MaintenanceRequest>(request);

            // 🔹 Insert into the database
            var result = await _maintenanceRequestCommandRepository.CreateAsync(maintenanceRequest);

            if (result.Id <= 0)
            {
                return new ApiResponseDTO<GetMaintenanceRequestDto>
                {
                    IsSuccess = false,
                    Message = "Failed to create Maintenance Request",
                    Data = null
                };
            }

            // 🔹 Fetch newly created record
            var createdMaintenanceRequest = await _maintenanceRequestQueryRepository.GetByIdAsync(result.Id);
            var mappedResult = _imapper.Map<GetMaintenanceRequestDto>(createdMaintenanceRequest);

            // 🔹 Publish domain event for auditing/logging
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "Create",
                actionCode: createdMaintenanceRequest.Machine?.MachineName ?? "Unknown Machine",
                actionName: createdMaintenanceRequest.Remarks ?? "No remarks",
                details: $"Maintenance Request '{createdMaintenanceRequest.Machine?.MachineName}' was created.",
                module: "MaintenanceRequest"
            );

            await _mediator.Publish(domainEvent, cancellationToken);

            // 🔹 Return success response
            return new ApiResponseDTO<GetMaintenanceRequestDto>
            {
                IsSuccess = true,
                Message = "Maintenance Request created successfully",
                Data = mappedResult
            };
        }
    }
}