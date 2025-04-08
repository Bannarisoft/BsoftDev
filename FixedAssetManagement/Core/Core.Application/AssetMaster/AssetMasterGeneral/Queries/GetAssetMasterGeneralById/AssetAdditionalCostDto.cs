using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.AssetMaster.AssetMasterGeneral.Queries.GetAssetMasterGeneralById
{
    public class AssetAdditionalCostDto
    {
        public int Id { get; set; }
        public int AssetSourceId { get; set; }
        public decimal Amount { get; set; }
        public string? JournalNo { get; set; }
        public int CostType { get; set; }  
        public string? CostTypeDesc { get; set; }    
    }
}
