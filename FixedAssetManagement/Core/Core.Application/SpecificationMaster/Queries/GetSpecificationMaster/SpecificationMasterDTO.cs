using Core.Application.Common.Mappings;
using Core.Domain.Entities;
using static Core.Domain.Common.BaseEntity;

namespace Core.Application.SpecificationMaster.Queries.GetSpecificationMaster
{
    public class SpecificationMasterDTO : IMapFrom<SpecificationMasters>
    {
        public int Id { get; set; }
        public string? SpecificationName { get; set; }      
        public int? AssetGroupId { get; set; } 
        public bool? ISDefault { get; set; }        
        public Status IsActive { get; set; }
        public int CreatedBy { get; set; }
        public DateTimeOffset?  CreatedAt { get; set; }
        public string? CreatedByName { get; set; }
        public string? CreatedIP { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTimeOffset?  ModifiedAt { get; set; }
        public string? ModifiedByName { get; set; }
        public string? ModifiedIP { get; set; }      
    }
}