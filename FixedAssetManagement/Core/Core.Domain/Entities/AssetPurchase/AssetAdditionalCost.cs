using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Domain.Entities.AssetPurchase
{
    public class AssetAdditionalCost
    {
        public int Id { get; set; }
        public int AssetId { get; set; }   
        public int AssetSourceId { get; set; }   
        public decimal Amount { get; set; }
        public string? JournalNo { get; set; }
        public int? CostType { get; set; }        
        public MiscMaster CostMiscType { get; set; } = null!;      
        public AssetMasterGenerals Asset { get; set; } = null!;       
        public AssetSource AssetSource { get; set; } = null!;
    }
}