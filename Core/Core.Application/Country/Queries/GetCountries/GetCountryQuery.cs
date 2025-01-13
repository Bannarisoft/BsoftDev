using Core.Application.Common;
using MediatR;

namespace Core.Application.Country.Queries.GetCountries
{   
   public class GetCountryQuery : IRequest<Result<List<CountryDto>>>;
          
}