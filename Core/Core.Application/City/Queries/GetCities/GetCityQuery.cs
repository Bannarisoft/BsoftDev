using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.City.Queries.GetCities;
using MediatR;

namespace Core.Application.City.Queries.GetCities
{   
   public class GetCityQuery : IRequest<List<CityDto>>;
          
}