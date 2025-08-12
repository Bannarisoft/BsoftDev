namespace Core.Domain.Entities.Item.ItemDetail.Variant
{
    public class VariantAttribute
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public int? GroupId { get; set; }
        public virtual ICollection<VariantOption> Options { get; set; } = new List<VariantOption>();
    }
  
}
