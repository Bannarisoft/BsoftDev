using static Core.Domain.Common.BaseEntity;

namespace Core.Application.AssetMaster.AssetMasterGeneral.Queries.GetAssetMasterGeneral
{
    public class AssetMasterUpdateDto
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public string? CompanyName { get; set; }   
        public string? AssetCode { get; set; }   
        public int UnitId { get; set; }
        public string? UnitName { get; set; } 
        public string? AssetName { get; set; }                
        public int AssetGroupId { get; set; }        
        public int AssetCategoryId { get; set; }        
        public int AssetSubCategoryId { get; set; }        
        public int? AssetParentId { get; set; }        
        public int? AssetType { get; set; }                
        public string? MachineCode { get; set; }   
        public int? Quantity { get; set; }
        public int? UOMId { get; set; }
        public string? AssetDescription { get; set; }
        public int? WorkingStatus { get; set; }
        public string? AssetImage { get; set; }
        public bool? NonDepreciated { get; set; }
        public bool? Tangible { get; set; }      
        public Status IsActive { get; set; }  
        public AssetLocationCombineDto?  AssetLocation  { get; set; }
        public ICollection<AssetPurchaseCombineDto>? AssetPurchaseDetails{ get; set; }    
    }
}