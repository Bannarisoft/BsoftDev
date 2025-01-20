using Core.Application.Common.Mappings;
using Core.Domain.Entities;

namespace Core.Application.Country.Queries.GetCountries
{
    public class CountryDto  : IMapFrom<Countries>
    {
        public int Id { get; set; }
        public string CountryCode { get; set; } = string.Empty;
        public string CountryName { get; set; } = string.Empty;    
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