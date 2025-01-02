using Core.Application.Country.Queries.GetCountries;
using Core.Domain.Entities;
using Core.Application.Common.Interfaces;
using AutoMapper;
using MediatR;
using Core.Application.Common.Interface;
using Core.Application.State.Queries.GetStates;
using Core.Application.Common;

namespace Core.Application.Country.Commands.CreateCountry
{        
    public class CreateCountryCommandHandler : IRequestHandler<CreateCountryCommand, Result<CountryDto>>
    {
        private readonly IMapper _mapper;
        private readonly ICountryRepository _countryRepository;

        // Constructor Injection
        public CreateCountryCommandHandler(IMapper mapper, ICountryRepository countryRepository)
        {
            _mapper = mapper;
            _countryRepository = countryRepository;
        }

        public async Task<Result<CountryDto>> Handle(CreateCountryCommand request, CancellationToken cancellationToken)
        {
            var countryExists = await _countryRepository.GetCountryByCodeAsync(request.CountryCode);
            if (countryExists)
            {
            return Result<CountryDto>.Failure("CountryCode already exists");
            }
            // Map the CreateCityCommand to the City entity
            var countryEntity = _mapper.Map<Countries>(request);
            var result = await _countryRepository.CreateAsync(countryEntity);

            // Map the result to CityDto and return success
            var countryDto = _mapper.Map<CountryDto>(result);
            return Result<CountryDto>.Success(countryDto);
        }
    }
}