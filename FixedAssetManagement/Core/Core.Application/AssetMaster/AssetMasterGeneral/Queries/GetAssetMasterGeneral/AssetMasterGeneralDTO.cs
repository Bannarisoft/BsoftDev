using Core.Application.Common.Mappings;
using Core.Domain.Entities;
using Core.Domain.Entities.AssetPurchase;
using static Core.Domain.Common.BaseEntity;

namespace Core.Application.AssetMaster.AssetMasterGeneral.Queries.GetAssetMasterGeneral
{
    public class AssetMasterGeneralDTO : IMapFrom<AssetMasterGenerals>
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }       
        public int UnitId { get; set; }    
        public string? CompanyName { get; set; }     
        public string? UnitName { get; set; }     
        public string? AssetCode { get; set; }        
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
        public string? AssetImageBase64 { get; set; } 
        public bool? NonDepreciated { get; set; }
        public bool? Tangible { get; set; }
        public Status IsActive { get; set; }
        public int CreatedBy { get; set; }
        public DateTimeOffset?  CreatedAt { get; set; }
        public string? CreatedByName { get; set; }
        public string? CreatedIP { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTimeOffset?  ModifiedAt { get; set; }
        public string? ModifiedByName { get; set; }
        public string? ModifiedIP { get; set; }
        public string? AssetGroupName { get; set; }
        public string? AssetCategoryDesc { get; set; }
        public string? AssetSubCategoryDesc { get; set; }
        public string? UOMName { get; set; }
        public string? WorkingStatusDesc { get; set; }
        public string? AssetTypeDesc { get; set; } 
        public string? ParentAssetDesc { get; set; } 
        public IsDelete IsDeleted { get; set; }    
         public List<AssetPurchaseDetails> AssetPurchase { get; set; }= new List<AssetPurchaseDetails>(); 
         public List<Core.Domain.Entities.AssetMaster.AssetLocation> AssetLocations { get; set; }= new List<Core.Domain.Entities.AssetMaster.AssetLocation>();
    }
}
