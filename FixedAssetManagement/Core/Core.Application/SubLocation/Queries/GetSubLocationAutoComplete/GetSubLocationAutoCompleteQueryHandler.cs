using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.ISubLocation;
using Core.Application.Location.Queries.GetSubLocations;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.SubLocation.Queries.GetSubLocationAutoComplete
{
    public class GetSubLocationAutoCompleteQueryHandler : IRequestHandler<GetSubLocationAutoCompleteQuery, ApiResponseDTO<List<SubLocationAutoCompleteDto>>>
    {
         private readonly ISubLocationQueryRepository _sublocationQueryRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        public GetSubLocationAutoCompleteQueryHandler(ISubLocationQueryRepository sublocationQueryRepository,IMapper mapper,IMediator mediator)
        {
            _sublocationQueryRepository = sublocationQueryRepository;
            _mapper = mapper;
            _mediator = mediator; 
        }
        public async Task<ApiResponseDTO<List<SubLocationAutoCompleteDto>>> Handle(GetSubLocationAutoCompleteQuery request, CancellationToken cancellationToken)
        {
            var result = await _sublocationQueryRepository.GetSubLocation(request.SearchPattern);
            if (result is null || result.Count is 0)
            {
                return new ApiResponseDTO<List<SubLocationAutoCompleteDto>>
                {
                    IsSuccess = false,
                    Message = "No SubLocation found matching the search pattern."
                };
            }
              var sublocations = _mapper.Map<List<SubLocationAutoCompleteDto>>(result);
              //Domain Event
                 var domainEvent = new AuditLogsDomainEvent(
                     actionDetail: "GetSubLocationAutoComplete",
                     actionCode: "",
                     actionName: "",
                     details: $"SubLocation details was fetched.",
                     module:"SubLocation"
                 );
                 await _mediator.Publish(domainEvent, cancellationToken);
            return new ApiResponseDTO<List<SubLocationAutoCompleteDto>> { IsSuccess = true, Message = "Success", Data = sublocations }; 
        }
    }
}