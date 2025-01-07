using Core.Application.Common.Interfaces;
using MediatR;
using Core.Domain.Entities;
using Core.Application.Country.Queries.GetCountries;
using AutoMapper;
using Core.Application.Common;
using Core.Application.Common.Interfaces.ICountry;


namespace Core.Application.Country.Commands.UpdateCountry
{    
    public class UpdateCountryCommandHandler : IRequestHandler<UpdateCountryCommand,Result<CountryDto>>    
    {
        private readonly ICountryCommandRepository _countryRepository;
        private readonly IMapper _mapper;
        private readonly ICountryQueryRepository _countryQueryRepository;

        public UpdateCountryCommandHandler(ICountryCommandRepository countryRepository, IMapper mapper, ICountryQueryRepository countryQueryRepository)
        {
            _countryRepository = countryRepository;
             _mapper = mapper;
            _countryQueryRepository = countryQueryRepository;
        }       
        public async Task<Result<CountryDto>> Handle(UpdateCountryCommand request, CancellationToken cancellationToken)
        {
            var country = await _countryQueryRepository.GetByIdAsync(request.Id);
            if (country == null || country.IsActive != 1)
            {
                return Result<CountryDto>.Failure("Invalid CountryID. The specified Country does not exist or is inactive.");
            }           

            var countryExists = await _countryRepository.GetCountryByCodeAsync(request.CountryCode);
            if (countryExists)
            {
                return Result<CountryDto>.Failure("CountryCode already exists");
            }

            // Map the request to the Cities entity
            var updatedCountryEntity = _mapper.Map<Countries>(request);
            // Perform the update
            var updateResult = await _countryRepository.UpdateAsync(request.Id, updatedCountryEntity);            
            var updatedCountry = await _countryQueryRepository.GetByIdAsync(request.Id);

            // If update was successful, map to DTO and return
            if (updatedCountry != null)
            {
                var countryDto = _mapper.Map<CountryDto>(updatedCountry);
                return Result<CountryDto>.Success(countryDto);
            }
            else
            {
                return Result<CountryDto>.Failure("Country update failed.");
            }          
      }
    }
}