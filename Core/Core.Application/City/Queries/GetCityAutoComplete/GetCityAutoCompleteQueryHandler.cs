using AutoMapper;
using MediatR;
using Core.Application.City.Queries.GetCities; 
using Core.Application.Common.Interfaces.ICity;
using Core.Domain.Events;
using Core.Application.Common.HttpResponse;

namespace Core.Application.City.Queries.GetCityAutoComplete
{
    public class GetCityAutoCompleteQueryHandler : IRequestHandler<GetCityAutoCompleteQuery, ApiResponseDTO<List<CityDto>>>
    
    {
        private readonly ICityQueryRepository _cityRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator; 

        public GetCityAutoCompleteQueryHandler(ICityQueryRepository cityRepository,  IMapper mapper, IMediator mediator)
        {
            _cityRepository =cityRepository;
            _mapper =mapper;
            _mediator = mediator;
        }

        public async Task<ApiResponseDTO<List<CityDto>>> Handle(GetCityAutoCompleteQuery request, CancellationToken cancellationToken)
        {             
            var result = await _cityRepository.GetByCityNameAsync(request.SearchPattern ?? string.Empty);
            if (result is null || result.Count is 0)
            {
                return new ApiResponseDTO<List<CityDto>>
                {
                    IsSuccess = false,
                    Message = "No Cities found matching the search pattern."
                };
            }
            var cityDto = _mapper.Map<List<CityDto>>(result);
            //Domain Event
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "GetAutoComplete",
                actionCode:"",        
                actionName: request.SearchPattern ?? string.Empty,                
                details: $"City '{request.SearchPattern}' was searched",
                module:"City"
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