using Core.Application.City.Queries.GetCities;
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.City.Queries.GetCityAutoComplete
{
    public class GetCityAutoCompleteQuery : IRequest<ApiResponseDTO<List<CityAutoCompleteDTO>>>
    {
        public string? SearchPattern { get; set; }
    }
}