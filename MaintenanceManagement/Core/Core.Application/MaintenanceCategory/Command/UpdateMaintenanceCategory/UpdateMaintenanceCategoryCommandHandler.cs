using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IMaintenanceCategory;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.MaintenanceCategory.Command.UpdateMaintenanceCategory
{
    public class UpdateMaintenanceCategoryCommandHandler : IRequestHandler<UpdateMaintenanceCategoryCommand, ApiResponseDTO<int>>
    {
        private readonly IMaintenanceCategoryCommandRepository _imaintenanceCategoryCommandRepository;
        private readonly IMediator _imediator;
        private readonly IMapper _imapper;
        public UpdateMaintenanceCategoryCommandHandler(IMaintenanceCategoryCommandRepository imaintenanceCategoryCommandRepository, IMediator imediator, IMapper imapper)
        {
            _imaintenanceCategoryCommandRepository = imaintenanceCategoryCommandRepository;
            _imediator = imediator;
            _imapper = imapper;
        }

        public async Task<ApiResponseDTO<int>> Handle(UpdateMaintenanceCategoryCommand request, CancellationToken cancellationToken)
        {
            var maintenanceCategory = _imapper.Map<Core.Domain.Entities.MaintenanceCategory>(request);
            var result = await _imaintenanceCategoryCommandRepository.UpdateAsync(request.Id, maintenanceCategory);
            if (result <= 0) // CostCenter not found
            {
               
                return new ApiResponseDTO<int> { IsSuccess = false, Message = "MaintenanceCategory  not found." };
            }
            //Domain Event
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "Update",
                actionCode: maintenanceCategory.Id.ToString(),
                actionName: maintenanceCategory.CategoryName,
                details: $"MaintenanceCategory details was updated",
                module: "MaintenanceCategory");
            await _imediator.Publish(domainEvent, cancellationToken);
           
            return new ApiResponseDTO<int> { IsSuccess = true, Message = "MaintenanceCategory Updated Successfully.", Data = result };  
        }
    }
}