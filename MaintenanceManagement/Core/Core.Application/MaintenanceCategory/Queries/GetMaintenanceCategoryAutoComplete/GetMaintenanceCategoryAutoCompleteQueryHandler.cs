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

namespace Core.Application.MaintenanceCategory.Queries.GetMaintenanceCategoryAutoComplete
{
    public class GetMaintenanceCategoryAutoCompleteQueryHandler : IRequestHandler<GetMaintenanceCategoryAutoCompleteQuery,ApiResponseDTO<List<MaintenanceCategoryAutoCompleteDto>>>
    {
         private readonly IMaintenanceCategoryQueryRepository _iMaintenanceCategoryQueryRepository;        
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public GetMaintenanceCategoryAutoCompleteQueryHandler(IMaintenanceCategoryQueryRepository iMaintenanceCategoryQueryRepository, IMapper mapper, IMediator mediator)
        {
            _iMaintenanceCategoryQueryRepository = iMaintenanceCategoryQueryRepository;            
            _mapper = mapper;
            _mediator = mediator;
        }

        public async Task<ApiResponseDTO<List<MaintenanceCategoryAutoCompleteDto>>> Handle(GetMaintenanceCategoryAutoCompleteQuery request, CancellationToken cancellationToken)
        {
             var result = await _iMaintenanceCategoryQueryRepository.GetMaintenanceCategoryAsync(request.SearchPattern);
            var maintenanceCategories = _mapper.Map<List<MaintenanceCategoryAutoCompleteDto>>(result);
             //Domain Event
                var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "GetAll",
                    actionCode: "GetMaintenanceCategoryAutoCompleteQuery",        
                    actionName: maintenanceCategories.Count.ToString(),
                    details: $"MaintenanceCategory details was fetched.",
                    module:"MaintenanceCategory"
                );
                await _mediator.Publish(domainEvent, cancellationToken);
            return new ApiResponseDTO<List<MaintenanceCategoryAutoCompleteDto>> { IsSuccess = true, Message = "Success", Data = maintenanceCategories };
        }
    }
}