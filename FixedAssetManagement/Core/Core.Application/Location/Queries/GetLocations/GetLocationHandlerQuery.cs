using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.ILocation;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.Location.Queries.GetLocations
{
    public class GetLocationHandlerQuery : IRequestHandler<GetLocationQuery, ApiResponseDTO<List<LocationDto>>>
    {
        private readonly ILocationQueryRepository _locationQueryRepository;        
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        public GetLocationHandlerQuery(ILocationQueryRepository locationQueryRepository,IMediator mediator,IMapper mapper)
        {
            _locationQueryRepository = locationQueryRepository;
            _mediator = mediator;
            _mapper = mapper;
        }
        public async Task<ApiResponseDTO<List<LocationDto>>> Handle(GetLocationQuery request, CancellationToken cancellationToken)
        {
            var (locations, totalCount) = await _locationQueryRepository.GetAllLocationAsync(request.PageNumber, request.PageSize, request.SearchTerm);
            var locationList = _mapper.Map<List<LocationDto>>(locations);

             //Domain Event
                var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "GetLocations",
                    actionCode: "",        
                    actionName: "",
                    details: $"Location details was fetched.",
                    module:"Location"
                );
                await _mediator.Publish(domainEvent, cancellationToken);
            return new ApiResponseDTO<List<LocationDto>> 
            { 
                IsSuccess = true, 
                Message = "Success", 
                Data = locationList ,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
                };
        }
    }
}