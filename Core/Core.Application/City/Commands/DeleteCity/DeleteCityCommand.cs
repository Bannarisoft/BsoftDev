using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.City.Queries.GetCities;
using Core.Application.Common;
using MediatR;

namespace Core.Application.City.Commands.DeleteCity
{
       public class DeleteCityCommand :  IRequest<Result<CityDto>>  
       {
                public int Id { get; set; }                
       }
    
}