using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IMaintenanceCategory;
using Core.Application.MaintenanceCategory.Queries.GetMaintenanceCategory;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.MaintenanceCategory.Queries.GetMaintenanceCategoryById
{
    public class GetMaintenanceCategoryByIdQueryHandler : IRequestHandler<GetMaintenanceCategoryByIdQuery, MaintenanceCategoryDto>
    {
        
         private readonly IMaintenanceCategoryQueryRepository _iMaintenanceCategoryQueryRepository;        
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
          public GetMaintenanceCategoryByIdQueryHandler(IMaintenanceCategoryQueryRepository iMaintenanceCategoryQueryRepository, IMapper mapper, IMediator mediator)
        {
            _iMaintenanceCategoryQueryRepository = iMaintenanceCategoryQueryRepository;            
            _mapper = mapper;
            _mediator = mediator;
        }
        public async Task<MaintenanceCategoryDto> Handle(GetMaintenanceCategoryByIdQuery request, CancellationToken cancellationToken)
        {
           var result = await _iMaintenanceCategoryQueryRepository.GetByIdAsync(request.Id);
           
            // Map a single entity
            var maintenanceCategory = _mapper.Map<MaintenanceCategoryDto>(result);

          //Domain Event
                var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "GetById",
                    actionCode: "GetMaintenanceCategoryByIdQuery",        
                    actionName: maintenanceCategory.Id.ToString(),
                    details: $"MaintenanceCategory details {maintenanceCategory.Id} was fetched.",
                    module:"MaintenanceCategory"
                );
                await _mediator.Publish(domainEvent, cancellationToken);
          return  maintenanceCategory;
        }
    }
}