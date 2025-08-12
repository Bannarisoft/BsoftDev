using Core.Domain.Entities.Item.ItemDetail.Variant;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace InventoryManagement.Infrastructure.Data.Configurations.Item.ItemDetail.Variant
{
    public sealed class ItemVariantValueConfiguration : IEntityTypeConfiguration<ItemVariantValue>
    {
        public void Configure(EntityTypeBuilder<ItemVariantValue> b)
        {
            b.ToTable("ItemVariantValue");
            b.HasKey(x => x.Id);

            b.HasOne(x => x.ItemMaster)                
             .WithMany(i => i.VariantValues)
             .HasForeignKey(x => x.ItemId)
             .OnDelete(DeleteBehavior.Cascade);

            b.HasOne(x => x.Attribute)
             .WithMany()
             .HasForeignKey(x => x.AttributeId)
             .OnDelete(DeleteBehavior.Restrict);

            b.HasOne(x => x.Option)
             .WithMany()
             .HasForeignKey(x => x.OptionId)
             .OnDelete(DeleteBehavior.Restrict);

            b.HasIndex(x => new { x.ItemId, x.AttributeId }).IsUnique();
        }
    }
}

