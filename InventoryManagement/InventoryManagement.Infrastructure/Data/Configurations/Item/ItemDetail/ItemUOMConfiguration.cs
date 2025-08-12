// Infrastructure/Data/Configurations/Item/ItemUOMConfiguration.cs
using Core.Domain.Entities.Item.ItemDetail;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace InventoryManagement.Infrastructure.Data.Configurations.Item.ItemDetail
{
    public sealed class ItemUOMConfiguration : IEntityTypeConfiguration<ItemUOM>
    {
        public void Configure(EntityTypeBuilder<ItemUOM> b)
        {
            b.ToTable("ItemUOM");

            // Composite PK (ItemId + ConversionUOMId)
            b.HasKey(x => new { x.ItemId, x.ConversionUOMId });

            b.HasOne(x => x.Item)
             .WithMany(i => i )
             .HasForeignKey(x => x.ItemId)
             .OnDelete(DeleteBehavior.Cascade);

            b.Property(x => x.ConversionRate).HasPrecision(18, 6);
        }
    }
}
