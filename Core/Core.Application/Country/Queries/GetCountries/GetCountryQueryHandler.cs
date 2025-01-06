using MediatR;
using AutoMapper;
using Core.Application.Common.Interfaces;
using Core.Application.Common.Interfaces.ICountry;

namespace Core.Application.Country.Queries.GetCountries
{
    public class GetCountryQueryHandler : IRequestHandler<GetCountryQuery, List<CountryDto>>
    {
        private readonly ICountryQueryRepository _countryRepository;
        private readonly IMapper _mapper;

        public GetCountryQueryHandler(ICountryQueryRepository countryRepository , IMapper mapper)
        {
            _countryRepository = countryRepository;
            _mapper = mapper;
        }
        public async Task<List<CountryDto>> Handle(GetCountryQuery request, CancellationToken cancellationToken)
        {     
            var countries = await _countryRepository.GetAllCountriesAsync();
            var countriesList = _mapper.Map<List<CountryDto>>(countries);
            return countriesList;
        }
    }
}