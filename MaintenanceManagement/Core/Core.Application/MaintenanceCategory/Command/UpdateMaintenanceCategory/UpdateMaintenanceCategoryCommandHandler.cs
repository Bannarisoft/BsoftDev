using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.Exceptions;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IMaintenanceCategory;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.MaintenanceCategory.Command.UpdateMaintenanceCategory
{
    public class UpdateMaintenanceCategoryCommandHandler : IRequestHandler<UpdateMaintenanceCategoryCommand,int>
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

        public async Task<int> Handle(UpdateMaintenanceCategoryCommand request, CancellationToken cancellationToken)
        {
            var maintenanceCategory = _imapper.Map<Core.Domain.Entities.MaintenanceCategory>(request);
            var result = await _imaintenanceCategoryCommandRepository.UpdateAsync(request.Id, maintenanceCategory);
           
            //Domain Event
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "Update",
                actionCode: maintenanceCategory.Id.ToString(),
                actionName: maintenanceCategory.CategoryName,
                details: $"MaintenanceCategory details was updated",
                module: "MaintenanceCategory");
            await _imediator.Publish(domainEvent, cancellationToken);
           
            return  result  <= 0 ? throw new ExceptionRules("MaintenanceCategory Update Failed.") : result;
        }
    }
}