
using Core.Domain.Common;

namespace Core.Domain.Entities
{
    public class AssetMasterGenerals : BaseEntity
    {
        public int CompanyId { get; set; } 
        public int UnitId { get; set; } 
        public string? AssetCode { get; set; }        
        public string? AssetName { get; set; }        
        // Foreign Key
        public int AssetGroupId { get; set; }
        public AssetGroup AssetGroup { get; set; } = null!;        
        public int AssetCategoryId { get; set; }
        public AssetCategories AssetCategories { get; set; } = null!;
        public int AssetSubCategoryId { get; set; }
        public AssetSubCategories AssetSubCategories { get; set; } = null!;
        public int? AssetParentId { get; set; }
        public AssetMasterGenerals AssetParent { get; set; } = null!;
        public ICollection<AssetMasterGenerals>? AssetChildren  { get; set; }        
        public int? AssetType { get; set; }     
        public MiscMaster? AssetMiscType { get; set; } = null!;       
        //End Foreign Key
        public string? MachineCode { get; set; }   
        public int? Quantity { get; set; }
        public int? UOMId { get; set; }
        public UOM UomMaster { get; set; } = null!;
        public string? AssetDescription { get; set; }
        public int? WorkingStatus { get; set; }
        public MiscMaster? AssetWorkType { get; set; } = null!;   
        public string? AssetImage { get; set; }
        public byte ISDepreciated { get; set; }
        public byte IsTangible { get; set; }        
    }
}