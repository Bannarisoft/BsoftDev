using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain.Common;
using Core.Application.Common.Mappings;
using Core.Domain.Entities;

namespace Core.Application.City.Queries.GetCities
{
    public class CityDto  : IMapFrom<Cities>
    {
        public int Id { get; set; }
        public string CityCode { get; set; } = string.Empty;
        public string CityName { get; set; } = string.Empty;    
        public int StateId { get; set; }
        public byte IsActive { get; set; }  
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedByName { get; set; }=string.Empty;
        public string CreatedIP { get; set; }=string.Empty;
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public string? ModifiedByName { get; set; }
        public string? ModifiedIP { get; set; }      
    }
}