using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BSOFT.Application.Country.DTO;
using BSOFT.Domain.Common.Interface;
using BSOFT.Domain.Entities;
using MediatR;

namespace BSOFT.Application.Country.Commands.CreateCountry
{    
     public class CreateCountryCommandHandler : IRequestHandler<CreateCountryCommand, CountryDto>
     
    {
        private readonly IMapper _mapper;
        private readonly ICountryRepository _countryRepository;

        // Constructor Injection
        public CreateCountryCommandHandler(IMapper mapper, ICountryRepository countryRepository)
        {
            _mapper = mapper;
            _countryRepository = countryRepository;
        }

        public async Task<CountryDto> Handle(CreateCountryCommand request, CancellationToken cancellationToken)
        {
         // Map the CreateCountryCommand to the Country entity
             var countryEntity = _mapper.Map<Countries>(request);

            // Save the country entity to the repository
          /*      await _countryRepository.CreateAsync(countryEntity);

            // Map the saved country entity to CountryDto
            var countryDto = _mapper.Map<CountryDto>(countryEntity);

            // Return the CountryDto
            return countryDto;
 */

            var result=await _countryRepository.CreateAsync(countryEntity);
            return _mapper.Map<CountryDto>(result);
        }
    }
}