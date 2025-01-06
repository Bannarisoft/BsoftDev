using Core.Application.Common;
using Core.Application.Country.Queries.GetCountries;
using MediatR;

namespace Core.Application.Country.Queries.GetCountryAutoComplete
{
    public class GetCountryAutoCompleteQuery : IRequest<Result<List<CountryDto>>>
    {
        public string SearchPattern { get; set; }=string.Empty;
    }
}