namespace Core.Domain.Entities.Item.ItemDetail.Variant
{
    public class ItemVariantValue
    {
        public int Id { get; set; }
        public int ItemId { get; set; }                         // child item
        public virtual ItemMaster ItemMaster { get; set; } = default!;
        public int VariantBasedOn { get; set; }
        public MiscMaster MiscVariantBasedOn { get; set; } = null!;
        public int AttributeId { get; set; }
        public MiscMaster MiscAttribute { get; set; } = null!;
        public string OptionValue { get; set; } = null!;
    }
}
