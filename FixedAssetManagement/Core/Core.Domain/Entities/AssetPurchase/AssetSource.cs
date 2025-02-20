using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain.Common;

namespace Core.Domain.Entities
{
    public class AssetSource : BaseEntity
    { 
        public string? SourceCode { get; set; }
        public string? SourceName { get; set; }
        public ICollection<Core.Domain.Entities.AssetPurchase.AssetPurchase>? AssetPurchase { get; set; }  

    }
}