using AutoMapper;
using MediatR;
using Core.Application.City.Queries.GetCities; 
using Core.Application.Common;
using Core.Application.Common.Interfaces;
using Core.Application.Common.Interfaces.ICity;

namespace Core.Application.City.Queries.GetCityAutoComplete
{
    public class GetCityAutoCompleteQueryHandler : IRequestHandler<GetCityAutoCompleteQuery, Result<List<CityDto>>>
    
    {
        private readonly ICityQueryRepository _cityRepository;
        private readonly IMapper _mapper;

        public GetCityAutoCompleteQueryHandler(ICityQueryRepository cityRepository,  IMapper mapper)
        {
            _cityRepository =cityRepository;
            _mapper =mapper;
        }

        public async Task<Result<List<CityDto>>> Handle(GetCityAutoCompleteQuery request, CancellationToken cancellationToken)
        {
            var result = await _cityRepository.GetByCityNameAsync(request.SearchPattern);
            if (!result.IsSuccess || result.Data == null || !result.Data.Any())
            {
                return Result<List<CityDto>>.Failure("No Cities found matching the search pattern.");
            }
            var cityDto = _mapper.Map<List<CityDto>>(result.Data);
            return Result<List<CityDto>>.Success(cityDto);
        }
    }
  
}