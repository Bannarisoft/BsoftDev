
namespace Core.Domain.Entities.Item.ItemDetail
{
    public class ItemUOM
    {
        public int ItemId { get; set; }
        public ItemMaster Item { get; set; } = null!;                
        public int? ConversionUOMId { get; set; }
        public UOM ConversionUOM { get; set; } = null!;
        public decimal? ConversionRate { get; set; }
    }
}