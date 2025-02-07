using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.AssetGroup.Queries.GetAssetGroup
{
    public class AssetGroupDto
    {
        public int Id { get; set; }
        public string? Code { get; set; }
        public string? Name { get; set; }
    }
}