using Core.Domain.Common;

namespace Core.Domain.Entities.Item
{
    public class ItemGroup : BaseEntity
    {
        public string? ItemGroupCode { get; set; }
        public string? ItemGroupName { get; set; }
        public int UnitId { get; set; }
        public ICollection<ItemCategory>? ItemCategory { get; set; } 
    }
}