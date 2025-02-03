using Core.Application.City.Queries.GetCities;
using Core.Application.Common.HttpResponse;
using Core.Application.State.Queries.GetStates;
using MediatR;

namespace Core.Application.City.Queries.GetCityByStateId
{
    public class GetCityByStateIdQuery : IRequest<ApiResponseDTO<List<CityDto>>>
    {
        public int Id { get; set; }
    }
}