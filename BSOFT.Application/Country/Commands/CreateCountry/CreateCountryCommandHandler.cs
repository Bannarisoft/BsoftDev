using BSOFT.Application.Country.Queries.GetCountries;
using BSOFT.Domain.Entities;
using BSOFT.Application.Common.Interfaces;
using AutoMapper;
using MediatR;
using BSOFT.Application.Common.Interface;

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
            var result=await _countryRepository.CreateAsync(countryEntity);
            return _mapper.Map<CountryDto>(result);
        }
    }
}