using Core.Domain.Entities.Item.ItemDetail.Variant;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventoryManagement.Infrastructure.Data.Configurations.Item.ItemDetail.Variant
{
    public sealed class VariantOptionConfiguration : IEntityTypeConfiguration<VariantOption>
    {
        public void Configure(EntityTypeBuilder<VariantOption> b)
        {
            b.ToTable("VariantOption");
            b.HasKey(x => x.Id);

            b.Property(x => x.Value).HasMaxLength(100).IsRequired();

            // Uniqueness: no duplicate option value inside the same attribute
            b.HasIndex(x => new { x.AttributeId, x.Value }).IsUnique();
        }
    }
}
