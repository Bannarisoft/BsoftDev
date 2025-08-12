using Core.Domain.Entities.Item.ItemDetail.Variant;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventoryManagement.Infrastructure.Data.Configurations.Item.ItemDetail.Variant
{
    public sealed class ItemVariantDefOptionConfiguration : IEntityTypeConfiguration<ItemVariantDefOption>
    {
        public void Configure(EntityTypeBuilder<ItemVariantDefOption> b)
        {
            b.ToTable("ItemVariantDefOption");
            b.HasKey(x => x.Id);

            b.HasOne(x => x.ItemVariantDef)
             .WithMany(d => d.Options)
             .HasForeignKey(x => x.ItemVariantDefId)
             .OnDelete(DeleteBehavior.Cascade);

            b.HasOne(x => x.Option)
             .WithMany()
             .HasForeignKey(x => x.OptionId)
             .OnDelete(DeleteBehavior.Restrict);

            // Prevent duplicate option assignment within a single def
            b.HasIndex(x => new { x.ItemVariantDefId, x.OptionId }).IsUnique();
        }
    }
}
