using Core.Domain.Entities.Item.ItemDetail;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace InventoryManagement.Infrastructure.Data.Configurations.Item.ItemDetail
{
    public sealed class ItemManufacturingConfiguration : IEntityTypeConfiguration<ItemManufacture>
    {
        public void Configure(EntityTypeBuilder<ItemManufacture> b)
        {
            b.ToTable("ItemManufacturings");
            b.HasKey(x => x.Id);

            b.HasOne(x => x.Item)
             .WithMany(i => i.Manufacture)
             .HasForeignKey(x => x.ItemId)
             .OnDelete(DeleteBehavior.Cascade);

            // One active row per (Item, Unit, ManufacturingType)
            b.HasIndex(x => new { x.ItemId, x.UnitId, x.ManufacturingTypeId })
             .IsUnique()
             .HasFilter("[IsDeleted] = 0"); // only unique for not-deleted rows

            // Map enums to ints if needed
            b.Property(x => x.IsActive).HasConversion<int>();
            b.Property(x => x.IsDeleted).HasConversion<int>();
        }
    }
}
