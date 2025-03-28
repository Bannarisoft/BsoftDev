using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IMaintenanceType;
using Core.Application.MaintenanceType.Queries.GetMaintenanceType;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.MaintenanceType.Command.CreateMaintenanceType
{
    public class CreateMaintenanceTypeCommandHandler : IRequestHandler<CreateMaintenanceTypeCommand, ApiResponseDTO<int>>
    {
        private readonly IMaintenanceTypeCommandRepository _iMaintenanceTypeCommandRepository;
        private readonly IMediator _imediator;
        private readonly IMapper _imapper;
        public CreateMaintenanceTypeCommandHandler(IMaintenanceTypeCommandRepository iMaintenanceTypeCommandRepository, IMediator imediator, IMapper imapper)
        {
            _iMaintenanceTypeCommandRepository = iMaintenanceTypeCommandRepository;
            _imediator = imediator;
            _imapper = imapper;
        }

        public async Task<ApiResponseDTO<int>> Handle(CreateMaintenanceTypeCommand request, CancellationToken cancellationToken)
        {
             var maintenanceType = _imapper.Map<Core.Domain.Entities.MaintenanceType>(request);
            
            var result = await _iMaintenanceTypeCommandRepository.CreateAsync(maintenanceType);

            //Domain Event
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "Create",
                actionCode: maintenanceType.Id.ToString(),
                actionName: maintenanceType.TypeName,
                details: $"MaintenanceType details was created",
                module: "MaintenanceType");
            await _imediator.Publish(domainEvent, cancellationToken);
          
            var costcenterGroupDtoDto = _imapper.Map<MaintenanceTypeDto>(maintenanceType);
            if (result > 0)
                  {
                    
                        return new ApiResponseDTO<int>
                       {
                           IsSuccess = true,
                           Message = "MaintenanceType created successfully",
                           Data = result
                      };
                 }
            return new ApiResponseDTO<int>
            {
                IsSuccess = true,
                Message = "MaintenanceType Creation Failed",
                Data = result
            };
        }
    }
}