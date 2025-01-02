using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.City.Queries.GetCities;
using Core.Application.Common;
using Core.Application.Common.Interfaces;
using Core.Domain.Entities;
using MediatR;

namespace Core.Application.City.Commands.DeleteCity
{
    public class DeleteCityCommandHandler : IRequestHandler<DeleteCityCommand, Result<CityDto>>
    {
        private readonly ICityRepository _cityRepository;
        private readonly IMapper _mapper;
        public DeleteCityCommandHandler(ICityRepository cityRepository, IMapper mapper)
        {
            _cityRepository = cityRepository;
             _mapper = mapper;
        }

        public async Task<Result<CityDto>> Handle(DeleteCityCommand request, CancellationToken cancellationToken)
        {
            // Fetch the city to be deleted
            var city = await _cityRepository.GetByIdAsync(request.Id);
            if (city == null || city.IsActive != 1)
            {
                return Result<CityDto>.Failure("Invalid CityID. The specified City does not exist or is inactive.");
            }

            // Mark the city as inactive (soft delete)
            var cityUpdate = new Cities
            {
                Id = request.Id,
                CityCode = city.CityCode, // Preserve original CityCode
                CityName = city.CityName, // Preserve original CityName
                StateId = city.StateId,
                IsActive = 0
            };

            var updateResult = await _cityRepository.DeleteAsync(request.Id, cityUpdate);
            if (updateResult > 0)
            {
               var cityDto = _mapper.Map<CityDto>(cityUpdate);               
                return Result<CityDto>.Success(cityDto);
            }

            return Result<CityDto>.Failure("City deletion failed.");
        }
    }
}
