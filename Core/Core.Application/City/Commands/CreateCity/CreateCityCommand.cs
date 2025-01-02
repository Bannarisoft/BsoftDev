using Core.Application.City.Queries.GetCities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Core.Application.Common;

namespace Core.Application.City.Commands.CreateCity
{     
      public class CreateCityCommand : IRequest<Result<CityDto>>  // Implements IRequest<Result<CityDto>>
    {
        public int StateId { get; set; }
        public string CityCode { get; set; } = string.Empty;
        public string CityName { get; set; } = string.Empty;        
    }
}