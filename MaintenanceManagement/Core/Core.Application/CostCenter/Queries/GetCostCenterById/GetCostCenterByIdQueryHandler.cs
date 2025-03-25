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

namespace Core.Application.CostCenter.Queries.GetCostCenterById
{
    public class GetCostCenterByIdQueryHandler : IRequestHandler<GetCostCenterByIdQuery,ApiResponseDTO<CostCenterDto>>
    {
        
        private readonly ICostCenterQueryRepository _iCostCenterQueryRepository;        
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public GetCostCenterByIdQueryHandler(ICostCenterQueryRepository iCostCenterQueryRepository, IMapper mapper, IMediator mediator)
        {
            _iCostCenterQueryRepository = iCostCenterQueryRepository;            
            _mapper = mapper;
            _mediator = mediator;
        }

        public async Task<ApiResponseDTO<CostCenterDto>> Handle(GetCostCenterByIdQuery request, CancellationToken cancellationToken)
        {
            var result = await _iCostCenterQueryRepository.GetByIdAsync(request.Id);
            // Check if the entity exists
            if (result is null)
            {
                return new ApiResponseDTO<CostCenterDto> { IsSuccess = false, Message =$"CostCenter ID {request.Id} not found." };
            }
            // Map a single entity
            var costCenter = _mapper.Map<CostCenterDto>(result);

          //Domain Event
                var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "GetById",
                    actionCode: "GetCostCenterByIdQuery",        
                    actionName: costCenter.Id.ToString(),
                    details: $"CostCenter details {costCenter.Id} was fetched.",
                    module:"CostCenter"
                );
                await _mediator.Publish(domainEvent, cancellationToken);
          return new ApiResponseDTO<CostCenterDto> { IsSuccess = true, Message = "Success", Data = costCenter };
        }

    }
}