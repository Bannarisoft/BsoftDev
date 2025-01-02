using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.City.Queries.GetCities;
using Core.Application.Common;
using MediatR;

namespace Core.Application.City.Commands.UpdateCity
{
       public class UpdateCityCommand : IRequest<Result<CityDto>> 
       {
                public int Id { get; set; }
                public string CityCode { get; set; }=string.Empty;
                public string CityName { get; set; }=string.Empty;                
                public int StateId { get; set; }
         }
  
}