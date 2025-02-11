using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.ILocation;
using Core.Application.Location.Queries.GetLocations;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.Location.Queries.GetLocationById
{
    public class GetLocationByIdQueryHandler : IRequestHandler<GetLocationByIdQuery, ApiResponseDTO<LocationDto>>
    {
        private readonly ILocationQueryRepository _locationQueryRepository;        
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        public GetLocationByIdQueryHandler(ILocationQueryRepository locationQueryRepository ,IMediator mediator,IMapper mapper)        {
            _locationQueryRepository = locationQueryRepository;
            _mediator = mediator;
            _mapper = mapper;
        }
        public async Task<ApiResponseDTO<LocationDto>> Handle(GetLocationByIdQuery request, CancellationToken cancellationToken)
        {
           var result = await _locationQueryRepository.GetByIdAsync(request.Id);
            if (result is null)
            {
                return new ApiResponseDTO<LocationDto>
                {
                    IsSuccess = false,
                    Message = "LocationId not found"
                };
            }  
           var location = _mapper.Map<LocationDto>(result);

          //Domain Event
                var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "GetById",
                    actionCode: "",        
                    actionName: "",
                    details: $"Location details {location.Id} was fetched.",
                    module:"Location"
                );
                await _mediator.Publish(domainEvent, cancellationToken);
          return new ApiResponseDTO<LocationDto> { IsSuccess = true, Message = "Success", Data = location };
        }
    }
}