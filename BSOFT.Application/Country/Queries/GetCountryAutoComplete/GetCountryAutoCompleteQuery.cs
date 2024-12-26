using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BSOFT.Application.Country.Queries.GetCountries;
using MediatR;


namespace BSOFT.Application.Country.Queries.GetCountryAutoComplete
{
    public class GetcountryAutoCompleteQuery : IRequest<List<CountryDto>>
    {
        public string SearchPattern { get; set; }
        // public string SearchText { get; set; }
    }
}