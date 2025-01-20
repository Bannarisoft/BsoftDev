using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain.Common;
using Core.Application.Common.Mappings;
using Core.Domain.Entities;

namespace Core.Application.Country.Queries.GetCountries
{
    public class CountryDto  : IMapFrom<Countries>
    {
        public int Id { get; set; }
        public string? CountryCode { get; set; }
        public string? CountryName { get; set; } 
        public byte IsActive { get; set; }     
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? CreatedByName { get; set; }
        public string? CreatedIP { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public string? ModifiedByName { get; set; }
        public string? ModifiedIP { get; set; }         
    }
}