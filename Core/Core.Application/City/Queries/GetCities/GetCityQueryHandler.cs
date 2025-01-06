using MediatR;
using AutoMapper;
using Core.Application.Common.Interfaces.ICity;

namespace Core.Application.City.Queries.GetCities
{
    public class GetCityQueryHandler : IRequestHandler<GetCityQuery, List<CityDto>>
    {
        private readonly ICityQueryRepository _cityRepository;
        private readonly IMapper _mapper;

        public GetCityQueryHandler(ICityQueryRepository cityRepository , IMapper mapper)
        {
            _cityRepository = cityRepository;
            _mapper = mapper;
        }
        public async Task<List<CityDto>> Handle(GetCityQuery request, CancellationToken cancellationToken)
        {
            var cities = await _cityRepository.GetAllCityAsync();
            var citiesList = _mapper.Map<List<CityDto>>(cities);
            return citiesList;
        }
    }
}