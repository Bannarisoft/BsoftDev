using Core.Domain.Entities.Item.ItemDetail.Variant;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventoryManagement.Infrastructure.Data.Configurations.Item.ItemDetail.Variant
{
    public sealed class VariantAttributeConfiguration : IEntityTypeConfiguration<VariantAttribute>
    {
        public void Configure(EntityTypeBuilder<VariantAttribute> b)
        {
            b.ToTable("VariantAttribute");
            b.HasKey(x => x.Id);

            b.Property(x => x.Name).HasMaxLength(100).IsRequired();

            // One attribute -> many options
            b.HasMany(x => x.Options)
             .WithOne(x => x.Attribute)
             .HasForeignKey(x => x.AttributeId)
             .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
