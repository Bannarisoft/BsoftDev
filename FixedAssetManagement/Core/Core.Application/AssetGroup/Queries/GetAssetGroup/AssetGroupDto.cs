using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Core.Domain.Common.BaseEntity;

namespace Core.Application.AssetGroup.Queries.GetAssetGroup
{
    public class AssetGroupDto
    {
        public int Id { get; set; }
        public string? Code { get; set; }
        public string? GroupName { get; set; }
        public int SortOrder { get; set; }
        public Status IsActive { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public string? CreatedByName { get; set; }
        public decimal? GroupPercentage { get; set; }


    }
}