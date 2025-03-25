using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.ICostCenter;
using Core.Application.CostCenter.Queries.GetCostCenter;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.CostCenter.Queries.GetCostCenterAutoComplete
{
    public class GetCostCenterAutoCompleteQueryHandler : IRequestHandler<GetCostCenterAutoCompleteQuery,ApiResponseDTO<List<CostCenterAutoCompleteDto>>>
    {
        private readonly ICostCenterQueryRepository _iCostCenterQueryRepository;        
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        public GetCostCenterAutoCompleteQueryHandler(ICostCenterQueryRepository iCostCenterQueryRepository, IMapper mapper, IMediator mediator)
        {
            _iCostCenterQueryRepository = iCostCenterQueryRepository;            
            _mapper = mapper;
            _mediator = mediator;
        }

        public async Task<ApiResponseDTO<List<CostCenterAutoCompleteDto>>> Handle(GetCostCenterAutoCompleteQuery request, CancellationToken cancellationToken)
        {
            var result = await _iCostCenterQueryRepository.GetCostCenterGroups(request.SearchPattern);
            var costCenters = _mapper.Map<List<CostCenterAutoCompleteDto>>(result);
             //Domain Event
                var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "GetAll",
                    actionCode: "GetCostCenterAutoCompleteQueryHandler",        
                    actionName: costCenters.Count.ToString(),
                    details: $"CostCenter details was fetched.",
                    module:"CostCenter"
                );
                await _mediator.Publish(domainEvent, cancellationToken);
            return new ApiResponseDTO<List<CostCenterAutoCompleteDto>> { IsSuccess = true, Message = "Success", Data = costCenters };
        }
    }
}