using Core.Domain.Common;

namespace Core.Domain.Entities.Item.ItemDetail
{    
    public class ItemSupplier : BaseEntity
    {
        public int ItemId { get; set; }
        public ItemMaster Item { get; set; } = null!;
        public int SupplierId { get; set; }  
        public int UnitId { get; set; }      
        public string? SupplierPartNo { get; set; }
    }
}
