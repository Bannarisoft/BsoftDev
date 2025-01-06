using System.Data;
using AutoMapper;
using Core.Application.City.Queries.GetCities;
using Core.Application.City.Queries.GetCityById;
using Core.Application.Common;
using Core.Application.Common.Interfaces;
using Core.Application.Common.Interfaces.ICity;
using Dapper;
using MediatR;

namespace Core.Application.Country.Queries.GetCountryById
{
    public class GetCityByIdQueryHandler : IRequestHandler<GetCityByIdQuery, Result<CityDto>>
    {
    private readonly ICityQueryRepository _cityRepository;
        private readonly IMapper _mapper;

        public GetCityByIdQueryHandler(ICityQueryRepository cityRepository, IMapper mapper)
        {
           _cityRepository = cityRepository;
            _mapper = mapper;
        }

        public async Task<Result<CityDto>> Handle(GetCityByIdQuery request, CancellationToken cancellationToken)
        {         
            var city = await _cityRepository.GetByIdAsync(request.Id);
            if (city == null)
            {
                return Result<CityDto>.Failure($"Country with ID {request.Id} not found.");
            }
            
            var cityDto = _mapper.Map<CityDto>(city);
            return Result<CityDto>.Success(cityDto);
        }
    }
}