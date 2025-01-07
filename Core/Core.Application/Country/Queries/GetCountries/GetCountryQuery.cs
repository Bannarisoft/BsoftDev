using MediatR;

namespace Core.Application.Country.Queries.GetCountries
{   
   public class GetCountryQuery : IRequest<List<CountryDto>>;
          
}