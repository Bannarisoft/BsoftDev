using Core.Domain.Common;
using Core.Domain.Entities.Item.ItemDetail;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace InventoryManagement.Infrastructure.Data.Configurations.Item.ItemDetail
{
    public sealed class ItemSupplierConfiguration : IEntityTypeConfiguration<ItemSupplier>
    {
        public void Configure(EntityTypeBuilder<ItemSupplier> b)
        {
            b.ToTable("ItemSuppliers");
            b.HasKey(x => x.Id);

           b.HasOne(x => x.Item)
            .WithMany(i => i.Suppliers)            // ✅ correct nav
            .HasForeignKey(x => x.ItemId)
            .OnDelete(DeleteBehavior.Cascade);

            b.Property(x => x.SupplierPartNo).HasMaxLength(100);

            // Ensure one active row per (Item, Supplier, Unit)
            b.HasIndex(x => new { x.ItemId, x.SupplierId, x.UnitId, x.IsDeleted })
             .IsUnique();

            // Defaults
            b.Property(x => x.IsActive).HasConversion<int>();
            b.Property(x => x.IsDeleted).HasConversion<int>();
        }
    }
}
