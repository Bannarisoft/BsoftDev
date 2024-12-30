using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace Core.Application.Country.Queries.GetCountries
{   
   public class GetCountryQuery : IRequest<List<CountryDto>>;
          
}