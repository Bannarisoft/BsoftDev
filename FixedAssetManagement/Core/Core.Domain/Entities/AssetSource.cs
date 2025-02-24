using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain.Common;
using Core.Domain.Entities.AssetPurchase;

namespace Core.Domain.Entities
{
    public class AssetSource : BaseEntity
    { 
        public string? SourceCode { get; set; }
        public string? SourceName { get; set; }
        public ICollection<Core.Domain.Entities.AssetPurchase.AssetPurchaseDetails>? AssetPurchase { get; set; }   
        public ICollection<AssetAdditionalCost>? AssetAdditionalCost { get; set; }

    }
}