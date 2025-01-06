using Core.Application.Common.Interfaces;
using Core.Application.Country.Queries.GetCountries;
using AutoMapper;
using MediatR;
using Core.Application.Common;
using Core.Application.Common.Interfaces.ICountry;

namespace Core.Application.Country.Queries.GetCountryAutoComplete
{
    public class GetCountryAutoCompleteQueryHandler : IRequestHandler<GetCountryAutoCompleteQuery, Result<List<CountryDto>>>
    {
        private readonly ICountryQueryRepository _countryRepository;
        private readonly IMapper _mapper;

        public GetCountryAutoCompleteQueryHandler(ICountryQueryRepository countryRepository, IMapper mapper)
        {
            _countryRepository =countryRepository;
            _mapper =mapper;
        }

        public async Task<Result<List<CountryDto>>> Handle(GetCountryAutoCompleteQuery request, CancellationToken cancellationToken)
        {            
            var result = await _countryRepository.GetByCountryNameAsync(request.SearchPattern);
            if (!result.IsSuccess || result.Data == null || !result.Data.Any())
            {
                return Result<List<CountryDto>>.Failure("No countries found matching the search pattern.");
            }
            var countryDto = _mapper.Map<List<CountryDto>>(result.Data);
            return Result<List<CountryDto>>.Success(countryDto);
            
        }
    }
  
}