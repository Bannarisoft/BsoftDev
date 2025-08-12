namespace Core.Domain.Entities.Item.ItemDetail.Variant
{
    public class ItemVariantValue
    {
        public int Id { get; set; }
        public int ItemId { get; set; }                         // child item
        public virtual ItemMaster ItemMaster { get; set; } = default!;
        public int AttributeId { get; set; }
        public virtual VariantAttribute Attribute { get; set; } = default!;
        public int OptionId { get; set; }
        public virtual VariantOption Option { get; set; } = default!;
    }
}
