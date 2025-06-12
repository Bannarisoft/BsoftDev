using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.Power.IFeeder;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.Power.Feeder.Queries.GetFeederById
{
    public class GetFeederByIdQueryHandler : IRequestHandler<GetFeederByIdQuery, ApiResponseDTO<GetFeederByIdDto>>
    {
        private readonly IFeederQueryRepository _feederGroupRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;


        public GetFeederByIdQueryHandler(IFeederQueryRepository feederGroupRepository, IMapper mapper, IMediator mediator)
        {
            _feederGroupRepository = feederGroupRepository;
            _mapper = mapper;
            _mediator = mediator;
        }
         public async Task<ApiResponseDTO<GetFeederByIdDto>> Handle(GetFeederByIdQuery request, CancellationToken cancellationToken)
        {
            var result = await _feederGroupRepository.GetFeederByIdAsync(request.Id);

            if (result == null)
            {
                return new ApiResponseDTO<GetFeederByIdDto>
                {
                    IsSuccess = false,
                    Message = "Feeder not found.",
                    Data = null
                };
            }

            var feederDto = _mapper.Map<GetFeederByIdDto>(result);

            // Domain Event: Audit Logging
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "GetById",
                actionCode: "FEEDER_VIEW",
                actionName: "View Feeder",
                details: $"Feeder details fetched for Id: {request.Id}",
                module: "Feeder"
            );
            await _mediator.Publish(domainEvent, cancellationToken);

            return new ApiResponseDTO<GetFeederByIdDto>
            {
                IsSuccess = true,
                Message = "Success",
                Data = feederDto
            };
        }
            
        

       }
       
    
}