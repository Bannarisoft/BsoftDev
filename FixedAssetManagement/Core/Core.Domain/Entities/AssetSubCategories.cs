using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain.Common;

namespace Core.Domain.Entities
{
    public class AssetSubCategories : BaseEntity
    {
        public string? Code { get; set; }
        public string? SubCategoryName { get; set; }
        public string? Description { get; set; }
        public int SortOrder { get; set; }
        public int AssetCategoriesId { get; set; }
        public AssetCategories AssetCategories { get; set; } = null!;
        public ICollection<AssetMasterGenerals>? AssetMasterGeneral { get; set; } 
    }
}