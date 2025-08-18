using Core.Domain.Common;

namespace Core.Domain.Entities.Item.ItemDetail
{    
    public class ItemSupplier 
    {
        public int Id { get; set; }
        public int ItemId { get; set; }
        public ItemMaster Item { get; set; } = null!;
        public int SupplierId { get; set; }  
        public int UnitId { get; set; }      
        public string? SupplierPartNo { get; set; }
    }
}
