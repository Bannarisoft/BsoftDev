using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BSOFT.Application.Country.DTO;
using MediatR;

namespace BSOFT.Application.Country.Queries
{
         public record GetCountryAutoCompleteQuery(string SearchPattern) : IRequest<List<CountryDto>>;
    
}