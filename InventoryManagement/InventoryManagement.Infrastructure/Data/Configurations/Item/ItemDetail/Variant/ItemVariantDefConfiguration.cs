using Core.Domain.Entities.Item.ItemDetail.Variant;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventoryManagement.Infrastructure.Data.Configurations.Item.ItemDetail.Variant
{
    public sealed class ItemVariantDefConfiguration : IEntityTypeConfiguration<ItemVariantDef>
    {
        public void Configure(EntityTypeBuilder<ItemVariantDef> b)
        {
            b.ToTable("ItemVariantDef");
            b.HasKey(x => x.Id);

            // Template item
            b.HasOne(x => x.Item)
             .WithMany(i => i.VariantDefs)
             .HasForeignKey(x => x.ItemId)
             .OnDelete(DeleteBehavior.Cascade);

            // Attribute used by the template
            b.HasOne(x => x.Attribute)
             .WithMany()
             .HasForeignKey(x => x.AttributeId)
             .OnDelete(DeleteBehavior.Restrict);

            // A template item can only define a given attribute once
            b.HasIndex(x => new { x.ItemId, x.AttributeId }).IsUnique();
        }
    }
}
