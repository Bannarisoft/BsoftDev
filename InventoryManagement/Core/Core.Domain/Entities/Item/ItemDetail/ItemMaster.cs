using Core.Domain.Common;
using Core.Domain.Entities.Item.ItemDetail.Variant;

namespace Core.Domain.Entities.Item.ItemDetail
{
    public class ItemMaster : BaseEntity
    {
        public int UnitId { get; set; }
        public string ItemCode { get; set; } = null!;
        public string ItemName { get; set; } = null!;
        public int? HSNId { get; set; }
        public HSNMaster HSNMaster { get; set; } = null!;
        public int? ItemGroupId { get; set; }
        public ItemGroup ItemGroup { get; set; } = null!;
        public int? ItemCategoryId { get; set; }
        public ItemCategory ItemCategory { get; set; } = null!;
        public int? StockUomId { get; set; }
        public UOM UOM { get; set; } = null!;
        public int? ItemClassificationId { get; set; }
        public MiscMaster MiscClassification { get; set; } = null!;
        public string? Description { get; set; }
        public DateOnly? ValidFrom { get; set; }
        public int? XPlantMaterialStatusId { get; set; }
        public MiscMaster MiscStatus { get; set; } = null!;
        //public int? DepartmentId { get; set; }
        public bool IsStockItem { get; set; }
        public bool MaintainStock { get; set; }
        public bool HasVariants { get; set; }
        public int? ParentItemId { get; set; }
        public ItemMaster? ParentItem { get; set; }
        public ICollection<ItemMaster> ChildItems { get; set; } = new List<ItemMaster>();        
        public string? ItemImage { get; set; }        
        public ItemPurchase? Purchase { get; set; }
        public ItemInventory? Inventory { get; set; }
        public ItemQuality? Quality { get; set; }     
        public ICollection<ItemVariantValue> VariantValues { get; set; } = new List<ItemVariantValue>();
        public ICollection<ItemVariantValue> VariantNewItem { get; set; } = new List<ItemVariantValue>();
        public ICollection<ItemSupplier> Suppliers { get; set; } = new List<ItemSupplier>();
        public ICollection<ItemManufacture> Manufacture { get; set; } = new List<ItemManufacture>();
        public ICollection<ItemUOM> ItemUOMs { get; set; } = new List<ItemUOM>();        
    }
}