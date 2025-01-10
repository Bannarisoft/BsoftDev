using AutoMapper;
using MediatR;
using Core.Application.City.Queries.GetCities; 
using Core.Application.Common;
using Core.Application.Common.Interfaces.ICity;
using Core.Domain.Events;

namespace Core.Application.City.Queries.GetCityAutoComplete
{
    public class GetCityAutoCompleteQueryHandler : IRequestHandler<GetCityAutoCompleteQuery, Result<List<CityDto>>>
    
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

        public async Task<Result<List<CityDto>>> Handle(GetCityAutoCompleteQuery request, CancellationToken cancellationToken)
        {
            try
            {   
                var result = await _cityRepository.GetByCityNameAsync(request.SearchPattern);
                if (!result.IsSuccess || result.Data == null || !result.Data.Any())
                {
                    return Result<List<CityDto>>.Failure("No Cities found matching the search pattern.");
                }
                var cityDto = _mapper.Map<List<CityDto>>(result.Data);
                 //Domain Event
                var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "GetAutoComplete",
                    actionCode:"",        
                    actionName: request.SearchPattern,                
                    details: $"City '{request.SearchPattern}' was searched",
                    module:"City"
                );
                await _mediator.Publish(domainEvent, cancellationToken);
                return Result<List<CityDto>>.Success(cityDto);
            }
            catch (Exception ex)
            {
                return Result<List<CityDto>>.Failure($"An error occurred while fetching the City: {ex.Message}");
            }
        }
    }
  
}