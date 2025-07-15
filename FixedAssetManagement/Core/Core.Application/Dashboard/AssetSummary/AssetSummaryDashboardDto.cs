using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.Dashboard.AssetSummary
{
    public class AssetSummaryDashboardDto
    {

        public string GroupName { get; set; }

        public int Assetcount { get; set; }
        
        public decimal PurchaseValue { get; set; }

        
    }
}