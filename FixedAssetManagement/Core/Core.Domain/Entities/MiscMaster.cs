using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain.Common;
using Core.Domain.Entities.AssetPurchase;

namespace Core.Domain.Entities
{
    public class MiscMaster  :BaseEntity
    {
        
         public int MiscTypeId { get; set; }  
        public string? Code { get; set;}
        public string? Description { get; set;}
        public int SortOrder  { get; set;}
            
        public MiscTypeMaster? MiscTypeMaster { get; set; } 
		//Depreciation
  		public ICollection<DepreciationGroups>? BookType { get; set; }  
        public ICollection<DepreciationGroups>? DepreciationMethod { get; set; }  
		//Manufacture
        public ICollection<Manufactures>? Manufactures { get; set; }  
		//AssetGeneral
        public ICollection<AssetMasterGenerals>? AssetMiscTypeGenerals  { get; set; }  
        public ICollection<AssetMasterGenerals>? AssetWorkTypeGenerals  { get; set; } 
		//Warranty
        public ICollection<AssetWarranties>? WarrantyClaim  { get; set; } 
        public ICollection<AssetWarranties>? WarrantyType  { get; set; } 
        // Navigation Property for UOMs referencing this MiscMaster
        public ICollection<UOM> UOMs { get; set; } = new List<UOM>();
        public ICollection<AssetAdditionalCost>? AssetAdditionalCost  { get; set; } 
               
    }
}