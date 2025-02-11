using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Core.Domain.Common.BaseEntity;

namespace Core.Application.AssetCategories.Queries.GetAssetCategories
{
    public class AssetCategoriesDto
    {
        public int Id { get; set; }
        public string? Code { get; set; }
        public string? CategoryName { get; set; }
        public string? Description { get; set; }
        public int SortOrder { get; set; }
        public int AssetGroupId { get; set; }
        public Status IsActive { get; set; }

    }
}