using Core.Domain.Common;

namespace Core.Domain.Entities
{
    public class DepreciationGroups : BaseEntity
    {
        public string? Code { get; set; }
        public string? BookType { get; set; }        
        public string? DepreciationGroupName { get; set; }        
        // Foreign Key
        public int AssetGroupId { get; set; }
        public AssetGroup AssetGroup { get; set; } = null!;
        public int? UsefulLife { get; set; }
        public int? DepreciationMethod { get; set; }
        public int? ResidualValue { get; set; }
        public int SortOrder { get; set; }
    }
}