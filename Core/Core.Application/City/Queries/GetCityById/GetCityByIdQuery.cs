using Core.Application.City.Queries.GetCities;
using Core.Application.Common;
using MediatR;

namespace Core.Application.City.Queries.GetCityById
{
    public class GetCityByIdQuery : IRequest<CityDto>
    {
        public int Id { get; set; }
    }
}