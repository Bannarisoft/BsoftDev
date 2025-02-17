using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.AssetSubCategories.Queries.GetAssetSubCategories
{
    public class AssetSubCategoriesDto
    {
        public int Id { get; set; }
        public string? Code { get; set; }
        public string? SubCategoryName { get; set; }
        public string? Description { get; set; }
        public int SortOrder { get; set; }
        public int AssetCategoriesId { get; set; }
    }
}