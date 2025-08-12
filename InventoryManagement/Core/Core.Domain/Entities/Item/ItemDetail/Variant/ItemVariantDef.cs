namespace Core.Domain.Entities.Item.ItemDetail.Variant
{
    public class ItemVariantDef
    {
        public int Id { get; set; }
        public int ItemId { get; set; }               
        public ItemMaster Item { get; set; } = null!;
        public int AttributeId { get; set; }
        public VariantAttribute Attribute { get; set; } = null!;
        public ICollection<ItemVariantDefOption> Options { get; set; } = new List<ItemVariantDefOption>();
    }  
}
