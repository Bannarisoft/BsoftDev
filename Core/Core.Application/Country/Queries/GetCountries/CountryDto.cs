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
        public string CountryCode { get; set; } = string.Empty;
        public string CountryName { get; set; } = string.Empty;    
        public new byte IsActive { get; set; }        
    }
}