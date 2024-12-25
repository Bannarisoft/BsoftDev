using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BSOFT.Domain.Common;

namespace BSOFT.Application.Country.DTO
{
    public class CountryDto : BaseEntity
    {
        public int Id { get; set; }
        public string CountryCode { get; set; } = string.Empty;
        public string CountryName { get; set; } = string.Empty;    
        public new byte IsActive { get; set; }        
    }
}