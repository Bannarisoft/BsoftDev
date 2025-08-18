namespace Core.Application.Item.ItemDetail.Queries.GetAllItems
{
    public sealed class ItemListDto
    {
        public int Id { get; set; }
        public string ItemCode { get; set; } = null!;
        public string ItemName { get; set; } = null!;
        public bool HasVariants { get; set; }
        public bool IsStockItem { get; set; }
        public int UnitId { get; set; }
        public string? ParentItemName { get; set; }
        public string? ItemGroupName { get; set; }
        public string? ItemCategoryName { get; set; }
    }
}