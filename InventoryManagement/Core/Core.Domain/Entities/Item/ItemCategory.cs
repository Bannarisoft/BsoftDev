using Core.Domain.Common;

namespace Core.Domain.Entities.Item
{
    public class ItemCategory : BaseEntity
    {
        public int ItemGroupId { get; set; }
        public ItemGroup ItemGroup { get; set; } = null!;
        public string? ItemCategoryName { get; set; }
        public byte? IsGroup { get; set; }
        public int? ParentCategoryId { get; set; }
        public ItemCategory ItemCategoryParent { get; set; } = null!; // Navigation property to ItemGroup
        public ICollection<ItemCategory>? ChildCategories { get; set; } = new List<ItemCategory>(); // For hierarchical categories
        public byte? IsBudgetApplicable { get; set; }
        public int? RootCategoryId { get; set; }
        public ItemCategory? RootCategory { get; set; } 
    }
}