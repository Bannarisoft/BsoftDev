using MediatR;
using AutoMapper;
using Core.Domain.Events;
using Core.Application.Common.HttpResponse;
using Core.Application.City.Queries.GetCities;
using Core.Application.Common.Interfaces.ICity;

namespace Core.Application.City.Queries.GetCityByStateId
{
    public class GetCityByStateIdQueryHandler : IRequestHandler<GetCityByStateIdQuery, ApiResponseDTO<List<CityDto>>>    
    {
        private readonly ICityQueryRepository _cityRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator; 
        public GetCityByStateIdQueryHandler(ICityQueryRepository cityRepository, IMapper mapper, IMediator mediator)
        {
            _cityRepository =cityRepository;
            _mapper = mapper;
            _mediator = mediator;
        }

        public async Task<ApiResponseDTO<List<CityDto>>> Handle(GetCityByStateIdQuery request, CancellationToken cancellationToken)
        {            
            var result = await _cityRepository.GetCityByStateIdAsync(request.Id);               
            if (result is null || !result.Any())
            {                
                 return new ApiResponseDTO<List<CityDto>>
                {
                    IsSuccess = false,
                    Message = "No States found matching the search pattern."
                };
            }                        
             var cityDto = _mapper.Map<List<CityDto>>(result); 
            //Domain Event
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "GetCityByStateId",
                actionCode:"" ,        
                actionName: "",                
                details: $"Get City by StateId: {request.Id}. details was fetched.",
                module:"State"
            );
            await _mediator.Publish(domainEvent, cancellationToken);            
            return new ApiResponseDTO<List<CityDto>>
            {
                IsSuccess = true,
                Message = "Success",
                Data = cityDto
            };   
        }
    }
}
