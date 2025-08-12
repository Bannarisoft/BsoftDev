namespace Core.Domain.Entities.Item.ItemDetail.Variant
{
    public class VariantOption
    {
        public int Id { get; set; }
        public int AttributeId { get; set; }
        public virtual VariantAttribute Attribute { get; set; } = default!;
        public string Value { get; set; } = default!;
    }
}