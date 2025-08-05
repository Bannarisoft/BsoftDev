using Core.Domain.Common;
using Core.Domain.Entities;

namespace Core.Domain.Entities.Item
{
    public class ItemGroup : BaseEntity
    {
        public int UnitId { get; set; }
        public string? ItemGroupCode { get; set; }
        public string? ItemGroupName { get; set; }
        public ICollection<ItemCategory>? ItemCategory { get; set; } 

       
    }
}