using Core.Application.Common.Mappings;

namespace Core.Application.Item.ItemCategory.Queries.GetItemCategory
    {
    public class ItemCategoryDto : IMapFrom<Domain.Entities.Item.ItemCategory>
    {
        public int Id { get; set; }
        public string? ItemCategoryName { get; set; }
        public int ItemGroupId { get; set; }
        public string? ItemGroupName { get; set; }
        public bool  IsGroup { get; set; }
        public int? ParentCategoryId { get; set; }
        public string? ParentCategoryName { get; set; }
        public bool  IsBudgetApplicable { get; set; }
        public int IsActive { get; set; }
        public int IsDeleted { get; set; }
        public int CreatedBy { get; set; }
        public DateTimeOffset? CreatedDate { get; set; }
        public string? CreatedByName { get; set; }
        public string? CreatedIP { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTimeOffset? ModifiedDate { get; set; }
        public string? ModifiedByName { get; set; }
        public string? ModifiedIP { get; set; }
          // 👇 Subgroups
        public List<ItemCategoryDto> SubGroups { get; set; } = new();
    }      
}