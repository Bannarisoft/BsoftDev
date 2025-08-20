using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain.Common;
using Core.Domain.Entities.Item.ItemDetail.Variant;

namespace Core.Domain.Entities
{
    public class MiscTypeMaster : BaseEntity
    {
        public string? MiscTypeCode { get; set; }
        public string? Description { get; set; }
        public ICollection<MiscMaster>? MiscMaster { get; set; }
        public ICollection<ItemVariantValue>? ItemAttributeGroup { get; set; }        
    }
}