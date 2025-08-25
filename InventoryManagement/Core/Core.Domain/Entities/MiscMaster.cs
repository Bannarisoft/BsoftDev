using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain.Common;
using Core.Domain.Entities.Budget;
using Core.Domain.Entities.Item.ItemDetail;
using Core.Domain.Entities.Item.ItemDetail.Variant;
using Core.Domain.Entities.Item.PutAway;

namespace Core.Domain.Entities
{

    public class MiscMaster : BaseEntity
    {
        public int MiscTypeId { get; set; }
        public string? Code { get; set; }
        public string? Description { get; set; }

        public int SortOrder { get; set; }
        public MiscTypeMaster? MiscTypeMaster { get; set; }
        public ICollection<BudgetLog>? BudgetAction { get; set; }
        public ICollection<HSNMaster>? HSNMasters { get; set; }
        public ICollection<HSNMaster>? TypeHSNs { get; set; }
        public ICollection<UOM> UOMs { get; set; } = new List<UOM>();

        public ICollection<ItemMaster>? ItemMasterStatus { get; set; }
        public ICollection<ItemMaster>? ItemMasterClassification { get; set; }
        public ICollection<ItemInventory>? ItemInventoryRequestType { get; set; }
        public ICollection<ItemInventory>? ItemInventoryValuationMethod { get; set; }
        public ICollection<ItemInventory>? ItemInventoryDefaultMaterialRequestType { get; set; }
        public ICollection<ItemManufacture>? ItemManufactureType { get; set; }
        public ICollection<ItemQuality>? ItemQualityCertificateType { get; set; }
        public ICollection<ItemVariantValue>? ItemAttribute { get; set; }
        public ICollection<ItemVariantValue>? ItemAttributeBasedOn { get; set; }
        public ICollection<PutAwayStrategy>? PutAwayStrategyStorageType { get; set; }       
        public ICollection<PutAwayStrategy>? PutAwayStrategyPriority { get; set; } 
    }
}