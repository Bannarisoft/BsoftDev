using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.AssetMaster.AssetPurchase.Queries
{
    public class GetAssetGrnDto
    {
        public int OldUnitId { get; set; } 
        public string? GrnNo { get; set; }
    }
}