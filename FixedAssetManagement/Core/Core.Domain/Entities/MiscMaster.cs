using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain.Common;

namespace Core.Domain.Entities
{
    public class MiscMaster  :BaseEntity
    {
        
         public int MiscTypeMasterId { get; set; }  
        public string? Code { get; set;}
        public string? Description { get; set;}
        public int SortOrder  { get; set;}
            
        public MiscTypeMaster? MiscTypeMaster { get; set; } 
  		public ICollection<DepreciationGroups>? BookType { get; set; }  
        public ICollection<DepreciationGroups>? DepreciationMethod { get; set; }  
        public ICollection<Manufactures>? Manufactures { get; set; }  
        public ICollection<AssetMasterGenerals>? AssetMiscTypeGenerals  { get; set; }  
        public ICollection<AssetMasterGenerals>? AssetWorkTypeGenerals  { get; set; } 
        // Navigation Property for UOMs referencing this MiscMaster
        public ICollection<UOM> UOMs { get; set; } = new List<UOM>();
               
    }
}