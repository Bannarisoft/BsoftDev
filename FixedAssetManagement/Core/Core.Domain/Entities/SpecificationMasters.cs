using Core.Domain.Common;

namespace Core.Domain.Entities
{
    public class SpecificationMasters : BaseEntity
    {        
        public string? SpecificationName { get; set; }     
        public int AssetGroupId { get; set; }
        public AssetGroup AssetGroupMaster { get; set; } = null!;  
        public byte ISDefault { get; set; }
    }
}