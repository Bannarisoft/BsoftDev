using Core.Domain.Common;
using Core.Domain.Entities.AssetMaster;

namespace Core.Domain.Entities
{
    public class SpecificationMasters : BaseEntity
    {        
        public string? SpecificationName { get; set; }     
        public int AssetGroupId { get; set; }
        public AssetGroup AssetGroupMaster { get; set; } = null!;  
        public byte ISDefault { get; set; }

        public ICollection<AssetSpecifications>? AssetSpecification { get; set; }  
    }
}