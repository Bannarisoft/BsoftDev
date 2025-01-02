using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.Interface;
using MediatR;
using Core.Domain.Entities;
using AutoMapper;
using Core.Application.Common.Interfaces;
using Core.Application.City.Commands.UpdateCity;
using Core.Application.City.Queries.GetCities;
using Core.Application.Common;

namespace Core.Application.City.Commands.UpdateCity
{       
    public class UpdateCityCommandHandler : IRequestHandler<UpdateCityCommand, Result<CityDto>>
    {
        private readonly ICityRepository _cityRepository;
        private readonly IMapper _mapper;

        public UpdateCityCommandHandler(ICityRepository cityRepository, IMapper mapper)
        {
            _cityRepository = cityRepository;
             _mapper = mapper;
        }
    public async Task<Result<CityDto>> Handle(UpdateCityCommand request, CancellationToken cancellationToken)
    {
        var city = await _cityRepository.GetByIdAsync(request.Id);
        if (city == null || city.IsActive != 1)
        {
            return Result<CityDto>.Failure("Invalid CityID. The specified City does not exist or is inactive.");
        }

        var stateExists = await _cityRepository.StateExistsAsync(request.StateId);
        if (!stateExists)
        {
            return Result<CityDto>.Failure("Invalid StateId. The specified state does not exist or is inactive.");
        }

        var cityExists = await _cityRepository.GetCityByCodeAsync(request.CityCode, request.StateId);
        if (cityExists)
        {
            return Result<CityDto>.Failure("CityCode already exists in the specified State.");
        }

        // Map the request to the Cities entity
        var updatedCityEntity = _mapper.Map<Cities>(request);

        // Perform the update
        var updateResult = await _cityRepository.UpdateAsync(request.Id, updatedCityEntity);

        // Fetch the updated city to map to the DTO
        var updatedCity = await _cityRepository.GetByIdAsync(request.Id);

        // If update was successful, map to DTO and return
        if (updatedCity != null)
        {
            var cityDto = _mapper.Map<CityDto>(updatedCity);
            return Result<CityDto>.Success(cityDto);
        }
        else
        {
            return Result<CityDto>.Failure("City update failed.");
        }
    }
    }
}