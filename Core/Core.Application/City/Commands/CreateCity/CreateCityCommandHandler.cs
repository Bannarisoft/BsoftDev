using Core.Application.Country.Queries.GetCountries;
using Core.Domain.Entities;
using Core.Application.Common.Interfaces;
using AutoMapper;
using MediatR;
using Core.Application.Common.Interface;
using Core.Application.City.Queries.GetCities;
using Core.Application.Common;

namespace Core.Application.City.Commands.CreateCity
{    
    public class CreateCityCommandHandler : IRequestHandler<CreateCityCommand, Result<CityDto>>
    {
        private readonly IMapper _mapper;
        private readonly ICityRepository _cityRepository;

        // Constructor Injection
        public CreateCityCommandHandler(IMapper mapper, ICityRepository cityRepository)
        {
            _mapper = mapper;
            _cityRepository = cityRepository;
        }

        public async Task<Result<CityDto>> Handle(CreateCityCommand request, CancellationToken cancellationToken)
        {
            var stateExists = await _cityRepository.StateExistsAsync(request.StateId);
            if (!stateExists)
            {
                return Result<CityDto>.Failure("Invalid StateId. The specified state does not exist or is inactive.");
            }
            var cityExists = await _cityRepository.GetCityByCodeAsync(request.CityCode, request.StateId);
            if (cityExists)
            {
            return Result<CityDto>.Failure("CityCode already exists in the specified state.");
            }
            // Map the CreateCityCommand to the City entity
            var cityEntity = _mapper.Map<Cities>(request);
            var result = await _cityRepository.CreateAsync(cityEntity);

            // Map the result to CityDto and return success
            var cityDto = _mapper.Map<CityDto>(result);
            return Result<CityDto>.Success(cityDto);
        }
    }
}
