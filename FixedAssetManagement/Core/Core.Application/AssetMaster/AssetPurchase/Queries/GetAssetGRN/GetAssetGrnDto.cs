using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.AssetMaster.AssetPurchase.Queries
{
    public class GetAssetGrnDto
    {
        public string? OldUnitId { get; set; } 
        public string? GrnNo { get; set; }
    }
}