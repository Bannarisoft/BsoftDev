using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.Power.IFeederGroup;
using Core.Application.Power.FeederGroup.Queries.GetFeederGroup;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.Power.FeederGroup.Queries.GetFeederGroupById
{
    public class GetFeederGroupByIdQueryHandler  : IRequestHandler<GetFeederGroupByIdQuery, ApiResponseDTO<GetFeederGroupByIdDto>>
    { 

         private readonly IFeederGroupQueryRepository _feederGroupQueryRepository;        
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public GetFeederGroupByIdQueryHandler(IFeederGroupQueryRepository feederGroupQueryRepository, IMapper mapper, IMediator mediator)
        {
            _feederGroupQueryRepository = feederGroupQueryRepository;
            _mapper = mapper;
            _mediator = mediator;
            
        }

       public async Task<ApiResponseDTO<GetFeederGroupByIdDto>> Handle(GetFeederGroupByIdQuery request, CancellationToken cancellationToken)
        {
            var result = await _feederGroupQueryRepository.GetFeederGroupByIdAsync(request.Id);

            if (result == null)
            {
                return new ApiResponseDTO<GetFeederGroupByIdDto>
                {
                    IsSuccess = false,
                    Message = "FeederGroup not found.",
                    Data = null
                };
            }

            var feederGroupDto = _mapper.Map<GetFeederGroupByIdDto>(result);

            // Domain Event: Audit Logging
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "GetById",
                actionCode: "FEEDERGROUP_VIEW",
                actionName: "View FeederGroup",
                details: $"FeederGroup details fetched for Id: {request.Id}",
                module: "FeederGroup"
            );
            await _mediator.Publish(domainEvent, cancellationToken);

            return new ApiResponseDTO<GetFeederGroupByIdDto>
            {
                IsSuccess = true,
                Message = "Success",
                Data = feederGroupDto
            };
        }
    }
}