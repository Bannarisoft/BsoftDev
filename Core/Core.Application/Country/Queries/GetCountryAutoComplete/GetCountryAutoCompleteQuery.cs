using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Country.Queries.GetCountries;
using MediatR;


namespace Core.Application.Country.Queries.GetCountryAutoComplete
{
    public class GetCountryAutoCompleteQuery : IRequest<List<CountryDto>>
    {
        public string SearchPattern { get; set; }=string.Empty;
    }
}