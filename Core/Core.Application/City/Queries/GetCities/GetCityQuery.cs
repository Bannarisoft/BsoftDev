using Core.Application.Common;
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.City.Queries.GetCities
{   
   public class GetCityQuery : IRequest<ApiResponseDTO<List<CityDto>>>;
          
}