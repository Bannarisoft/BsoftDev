using MediatR;
using AutoMapper;
using Core.Application.Common.Interfaces.ICity;
using Core.Application.Common;
using Core.Domain.Events;

namespace Core.Application.City.Queries.GetCities
{
    public class GetCityQueryHandler : IRequestHandler<GetCityQuery, Result<List<CityDto>>>
    {
        private readonly ICityQueryRepository _cityRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator; 

        public GetCityQueryHandler(ICityQueryRepository cityRepository , IMapper mapper, IMediator mediator)
        {
            _cityRepository = cityRepository;
            _mapper = mapper;
            _mediator = mediator;
        }
        public async Task<Result<List<CityDto>>> Handle(GetCityQuery request, CancellationToken cancellationToken)
        {
            try
            { 
                var cities = await _cityRepository.GetAllCityAsync();
                var citiesList = _mapper.Map<List<CityDto>>(cities);
                //Domain Event
                var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "GetAll",
                    actionCode: "",        
                    actionName: "",
                    details: $"City details was fetched.",
                    module:"City"
                );
                await _mediator.Publish(domainEvent, cancellationToken);
                return Result<List<CityDto>>.Success(citiesList);
            }
            catch (Exception ex)
            {
                return Result<List<CityDto>>.Failure($"An error occurred while fetching the City: {ex.Message}");
            }
        }
    }
}