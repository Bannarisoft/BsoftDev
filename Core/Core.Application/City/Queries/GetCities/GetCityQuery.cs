using MediatR;

namespace Core.Application.City.Queries.GetCities
{   
   public class GetCityQuery : IRequest<List<CityDto>>;
          
}