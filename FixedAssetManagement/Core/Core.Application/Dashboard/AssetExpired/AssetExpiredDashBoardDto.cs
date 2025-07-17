using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.Dashboard.AssetExpired
{
    public class AssetExpiredDashBoardDto
    {
        public string? GroupName { get; set; }
        public int ExpiredAssetCount { get; set; }
        public decimal ResidualValueAmount { get; set; }


    }
}