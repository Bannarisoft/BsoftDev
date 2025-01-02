using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.Interface;
using MediatR;
using Core.Domain.Entities;
using Core.Application.Country.Queries.GetCountries;
using AutoMapper;
using Core.Application.Common;


namespace Core.Application.Country.Commands.UpdateCountry
{    
    public class UpdateCountryCommandHandler : IRequestHandler<UpdateCountryCommand,Result<CountryDto>>    
    {
        private readonly ICountryRepository _countryRepository;
        private readonly IMapper _mapper;

        public UpdateCountryCommandHandler(ICountryRepository countryRepository, IMapper mapper)
        {
            _countryRepository = countryRepository;
             _mapper = mapper;
        }
       
        public async Task<Result<CountryDto>> Handle(UpdateCountryCommand request, CancellationToken cancellationToken)
        {
            var country = await _countryRepository.GetByIdAsync(request.Id);
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
            var updatedCountry = await _countryRepository.GetByIdAsync(request.Id);

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