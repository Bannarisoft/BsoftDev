namespace Core.Application.Item.ItemDetail.Queries.GetAllItems
{
    public sealed class ItemDto
    {
        // ItemMaster (base)
        public int UnitId { get; set; }
        public string ItemCode { get; set; } = null!;
        public string ItemName { get; set; } = null!;
        public string? HSNCode { get; set; }
        public int? ItemGroupId { get; set; }
        public int? ItemCategoryId { get; set; }
        public int? DefaultUomId { get; set; }
        public int? ItemClassificationId { get; set; }
        public string? Description { get; set; }
        public DateOnly? ValidFrom { get; set; }
        public int? XPlantMaterialStatusId { get; set; }
        public int? DepartmentId { get; set; }
        public bool IsStockItem { get; set; }
        public bool MaintainStock { get; set; }
        public bool HasVariants { get; set; }
        public int? ParentItemId { get; set; }

        // Tabs
        public ItemPurchaseDto? Purchase { get; set; }
        public ItemInventoryDto? Inventory { get; set; }
        public ItemQualityDto? Quality { get; set; }

        // Collections
        public List<ItemSupplierDto> Suppliers { get; set; } = new();
        public List<ItemManufactureDto> Manufacture { get; set; } = new();
        public List<ItemUomDto> Uoms { get; set; } = new();

        // Variants (optional – wire if you’re ready)
        public List<VariantDefDto>? VariantDefs { get; set; }
    }

    public sealed class ItemPurchaseDto
    {
        public int? PurchaseUomId { get; set; }
        public int? LeadTimeDays { get; set; }
        public int? SafetyStock { get; set; }
        public int? GrProcessingTimeDays { get; set; }
        public decimal? PurchaseRate { get; set; }
        public bool AutomaticPo { get; set; }
        public int? OriginCountryId { get; set; }
        public string? TariffNumber { get; set; }
    }

    public sealed class ItemInventoryDto
    {
        public decimal? Weight { get; set; }
        public int? WeightUomId { get; set; }
        public int? DefaultMaterialRequestTypeId { get; set; }
        public int? ValuationMethodId { get; set; }
        public int? ShelfLife { get; set; }
        public decimal? UpperTolerance { get; set; }
        public decimal? LowerTolerance { get; set; }
        public string? BatchNumberSeries { get; set; }
        public string? SerialNumberSeries { get; set; }
        public int? ReorderLevel { get; set; }
        public int? ReorderQty { get; set; }
        public int? RequestTypeId { get; set; }
        public bool AllowNegativeStock { get; set; }
        public bool BatchManagement { get; set; }
        public bool ApplyBatchNumber { get; set; }
    }

    public sealed class ItemQualityDto
    {
        public int? InspectionTemplateId { get; set; }
        public int? CertificateTypeId { get; set; }
        public int? InspLotProcessingTime { get; set; }
        public bool InspectionRequired { get; set; }
        public bool QualityInspectionFree { get; set; }
        public bool IsCertificateRequiredFromSupplier { get; set; }
    }

    public sealed class ItemSupplierDto
    {
        public int SupplierId { get; set; }
        public int UnitId { get; set; }
        public string? SupplierPartNo { get; set; }
    }

    public sealed class ItemManufactureDto
    {
        public int UnitId { get; set; }
        public int ManufacturingTypeId { get; set; }
    }

    public sealed class VariantDefDto
    {
        public int AttributeId { get; set; }
        public List<int> OptionIds { get; set; } = new();
    }

    // For GetAll list (lighter)
    public sealed class ItemListDto
    {
        public int Id { get; set; }
        public string ItemCode { get; set; } = null!;
        public string ItemName { get; set; } = null!;
        public bool HasVariants { get; set; }
        public bool IsStockItem { get; set; }
        public int UnitId { get; set; }
    }
    public sealed class ItemUomDto
    {
        public int? BaseUOMId { get; set; }
        public int? ConversionUOMId { get; set; }
        public decimal? ConversionRate { get; set; }
    }
}
