using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain.Common;

namespace Core.Domain.Entities
{
    public class AssetGroup :BaseEntity
    {
        public string? Code { get; set; }
        public string? GroupName { get; set; }
        public int SortOrder { get; set; }
        public ICollection<DepreciationGroups>? DepreciationGroups { get; set; }
        public ICollection<AssetCategories>? AssetCategories { get; set; } 
        public ICollection<AssetMasterGenerals>? AssetMasterGeneral { get; set; }    
    }
}