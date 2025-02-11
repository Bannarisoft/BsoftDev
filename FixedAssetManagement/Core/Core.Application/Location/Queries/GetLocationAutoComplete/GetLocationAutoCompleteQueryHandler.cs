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

namespace Core.Application.Location.Queries.GetLocationAutoComplete
{
    public class GetLocationAutoCompleteQueryHandler : IRequestHandler<GetLocationAutoCompleteQuery, ApiResponseDTO<List<LocationAutoCompleteDto>>>
    {
        
        private readonly ILocationQueryRepository _locationQueryRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        public GetLocationAutoCompleteQueryHandler(ILocationQueryRepository locationQueryRepository,IMediator mediator,IMapper mapper)
        {
            _locationQueryRepository = locationQueryRepository;
            _mediator = mediator;
            _mapper = mapper;    
        }
        public async Task<ApiResponseDTO<List<LocationAutoCompleteDto>>> Handle(GetLocationAutoCompleteQuery request, CancellationToken cancellationToken)
        {
            var result = await _locationQueryRepository.GetLocation(request.SearchPattern);
            if (result is null || result.Count is 0)
            {
                return new ApiResponseDTO<List<LocationAutoCompleteDto>>
                {
                    IsSuccess = false,
                    Message = "No Location found matching the search pattern."
                };
            }
              var locations = _mapper.Map<List<LocationAutoCompleteDto>>(result);
              //Domain Event
                 var domainEvent = new AuditLogsDomainEvent(
                     actionDetail: "GetLocationAutoComplete",
                     actionCode: "",
                     actionName: "",
                     details: $"Location details was fetched.",
                     module:"Location"
                 );
                 await _mediator.Publish(domainEvent, cancellationToken);
            return new ApiResponseDTO<List<LocationAutoCompleteDto>> { IsSuccess = true, Message = "Success", Data = locations };  
        }
    }
}