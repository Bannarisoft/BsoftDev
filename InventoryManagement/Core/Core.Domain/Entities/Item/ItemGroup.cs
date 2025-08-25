using Core.Domain.Common;
using Core.Domain.Entities;
using Core.Domain.Entities.Item.ItemDetail;
using Core.Domain.Entities.Item.PutAway;

namespace Core.Domain.Entities.Item
{
    public class ItemGroup : BaseEntity
    {
        public int UnitId { get; set; }
        public string? ItemGroupCode { get; set; }
        public string? ItemGroupName { get; set; }
        public ICollection<ItemCategory>? ItemCategory { get; set; }
        public ICollection<ItemMaster>? ItemMasterGroup { get; set; } 
        public ICollection<PutAwayRule>? PutAwayRuleGroup { get; set; } = new List<PutAwayRule>();       
    }
}