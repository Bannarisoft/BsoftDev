using Core.Application.City.Queries.GetCities;
using Core.Application.Common;
using MediatR;


namespace Core.Application.City.Queries.GetCityAutoComplete
{
    public class GetCityAutoCompleteQuery : IRequest<Result<List<CityDto>>>
    {
        public string SearchPattern { get; set; }=string.Empty;        
    }
}