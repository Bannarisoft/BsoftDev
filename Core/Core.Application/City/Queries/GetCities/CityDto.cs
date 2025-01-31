using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain.Common;
using Core.Application.Common.Mappings;
using Core.Domain.Entities;
using static Core.Domain.Enums.Common.Enums;

namespace Core.Application.City.Queries.GetCities
{
    public class CityDto  : IMapFrom<Cities>
    {
        public int Id { get; set; }
        public string? CityCode { get; set; }
        public string? CityName { get; set; } 
        public int StateId { get; set; }
        public Status IsActive { get; set; }       
    }
}