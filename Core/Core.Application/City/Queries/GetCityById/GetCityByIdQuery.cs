using Core.Application.City.Queries.GetCities;
using Core.Application.Country.Queries.GetCountries;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Application.City.Queries.GetCityById
{
    public class GetCityByIdQuery : IRequest<CityDto>
    {
        public int Id { get; set; }
    }
}