using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.Mappings;
using Core.Domain.Entities.AssetMaster;

namespace Core.Application.AssetMaster.AssetWarranty.Queries.GetAssetWarranty
{
    public class AssetWarrantyImage  : IMapFrom<AssetWarranties>
    {
          public string? AssetImage { get; set; }
        public string? AssetImageBase64 { get; set; } 
    }
}