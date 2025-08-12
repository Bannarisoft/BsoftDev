namespace Core.Domain.Entities.Item.ItemDetail.Variant
{
    public class ItemVariantDefOption
    {
        public int Id { get; set; }
        public int ItemVariantDefId { get; set; }
        public virtual ItemVariantDef ItemVariantDef { get; set; } = default!;
        public int OptionId { get; set; }
        public virtual VariantOption Option { get; set; } = default!;
    }
}