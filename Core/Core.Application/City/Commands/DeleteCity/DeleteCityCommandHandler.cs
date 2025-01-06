using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.City.Queries.GetCities;
using Core.Application.Common;
using Core.Application.Common.Interfaces;
using Core.Application.Common.Interfaces.ICity;
using Core.Domain.Entities;
using MediatR;

namespace Core.Application.City.Commands.DeleteCity
{
    public class DeleteCityCommandHandler : IRequestHandler<DeleteCityCommand, Result<CityDto>>
    {
        private readonly ICityCommandRepository _cityRepository;
        private readonly IMapper _mapper;
        private readonly ICityQueryRepository _cityQueryRepository;
        public DeleteCityCommandHandler(ICityCommandRepository cityRepository, IMapper mapper, ICityQueryRepository cityQueryRepository)
        {
            _cityRepository = cityRepository;
             _mapper = mapper;
            _cityQueryRepository = cityQueryRepository;
        }

        public async Task<Result<CityDto>> Handle(DeleteCityCommand request, CancellationToken cancellationToken)
        {
            // Fetch the city to be deleted
            var city = await _cityQueryRepository.GetByIdAsync(request.Id);
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
