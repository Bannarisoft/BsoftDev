using Core.Application.Common;
using MediatR;

namespace Core.Application.City.Queries.GetCities
{   
   public class GetCityQuery : IRequest<Result<List<CityDto>>>;
          
}