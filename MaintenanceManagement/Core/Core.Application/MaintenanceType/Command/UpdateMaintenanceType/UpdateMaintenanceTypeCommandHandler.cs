using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IMaintenanceType;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.MaintenanceType.Command.UpdateMaintenanceType
{
    public class UpdateMaintenanceTypeCommandHandler : IRequestHandler<UpdateMaintenanceTypeCommand, ApiResponseDTO<int>>
    {
        private readonly IMaintenanceTypeCommandRepository _iMaintenanceTypeCommandRepository;
        private readonly IMediator _imediator;
        private readonly IMapper _imapper;
        public UpdateMaintenanceTypeCommandHandler(IMaintenanceTypeCommandRepository iMaintenanceTypeCommandRepository, IMediator imediator, IMapper imapper)
        {
            _iMaintenanceTypeCommandRepository = iMaintenanceTypeCommandRepository;
            _imediator = imediator;
            _imapper = imapper;
        }

        public async Task<ApiResponseDTO<int>> Handle(UpdateMaintenanceTypeCommand request, CancellationToken cancellationToken)
        {
            var maintenanceCategory = _imapper.Map<Core.Domain.Entities.MaintenanceType>(request);
            var result = await _iMaintenanceTypeCommandRepository.UpdateAsync(request.Id, maintenanceCategory);
            if (result <= 0) // CostCenter not found
            {
               
                return new ApiResponseDTO<int> { IsSuccess = false, Message = "MaintenanceType  not found." };
            }
            //Domain Event
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "Update",
                actionCode: maintenanceCategory.Id.ToString(),
                actionName: maintenanceCategory.TypeName,
                details: $"MaintenanceType details was updated",
                module: "MaintenanceType");
            await _imediator.Publish(domainEvent, cancellationToken);
           
            return new ApiResponseDTO<int> { IsSuccess = true, Message = "MaintenanceType Updated Successfully.", Data = result };  
        }
    }
}