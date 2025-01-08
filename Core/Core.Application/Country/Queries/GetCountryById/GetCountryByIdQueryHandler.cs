using Core.Application.Country.Queries.GetCountries;
using Core.Application.Common.Interfaces;
using MediatR;
using AutoMapper;
using Core.Application.Common;
using Core.Application.Common.Interfaces.ICountry;

namespace Core.Application.Country.Queries.GetCountryById
{
    public class GetCountryByIdQueryHandler : IRequestHandler<GetCountryByIdQuery, Result<CountryDto>>
    {
        private readonly ICountryQueryRepository _countryRepository;
        private readonly IMapper _mapper;

        public GetCountryByIdQueryHandler(ICountryQueryRepository countryRepository, IMapper mapper)
        {
            _countryRepository = countryRepository;
            _mapper = mapper;
        }

        public async Task<Result<CountryDto>> Handle(GetCountryByIdQuery request, CancellationToken cancellationToken)
        {
            var country = await _countryRepository.GetByIdAsync(request.Id);
            if (country == null)
            {
                return Result<CountryDto>.Failure($"Country with ID {request.Id} not found.");
            }
            
            var countryDto = _mapper.Map<CountryDto>(country);
            return Result<CountryDto>.Success(countryDto);
        }
    }

}