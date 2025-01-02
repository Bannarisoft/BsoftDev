using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common;
using Core.Application.Common.Interface;
using Core.Application.Country.Queries.GetCountries;
using Core.Domain.Entities;
using MediatR;

namespace Core.Application.Country.Commands.DeleteCountry
{  
  public class DeleteCountryCommandHandler : IRequestHandler<DeleteCountryCommand, Result<CountryDto>>
    {
        private readonly ICountryRepository _countryRepository;
        private readonly IMapper _mapper;
        public DeleteCountryCommandHandler(ICountryRepository countryRepository, IMapper mapper)
        {
            _countryRepository = countryRepository;
            _mapper = mapper;
        }
       
        public async Task<Result<CountryDto>> Handle(DeleteCountryCommand request, CancellationToken cancellationToken)
        {
            var country = await _countryRepository.GetByIdAsync(request.Id);
            if (country == null || country.IsActive != 1)
            {
                return Result<CountryDto>.Failure("Invalid CountryID. The specified Country does not exist or is inactive.");
            }
                       
            var countryUpdate = new Countries
            {
                Id = request.Id,
                CountryCode = country.CountryCode, // Preserve original CityCode
                CountryName = country.CountryName, // Preserve original CityName
                IsActive = 0
            };

            var updateResult = await _countryRepository.DeleteAsync(request.Id, countryUpdate);
            if (updateResult > 0)
            {
               var countryDto = _mapper.Map<CountryDto>(countryUpdate);               
                return Result<CountryDto>.Success(countryDto);
            }

            return Result<CountryDto>.Failure("Country deletion failed.");
        }
    }
}