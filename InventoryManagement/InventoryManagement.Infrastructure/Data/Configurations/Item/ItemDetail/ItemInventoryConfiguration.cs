
using Core.Domain.Entities.Item.ItemDetail;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventoryManagement.Infrastructure.Data.Configurations.Item.ItemDetail
{
    public sealed class ItemInventoryConfiguration : IEntityTypeConfiguration<ItemInventory>
    {
        public void Configure(EntityTypeBuilder<ItemInventory> b)
        {
            b.ToTable("ItemInventory");
            b.HasKey(x => x.ItemId);
            b.Property(x => x.Weight).HasColumnType("decimal(18,3)");
            b.Property(x => x.UpperTolerance).HasColumnType("decimal(9,2)");
            b.Property(x => x.LowerTolerance).HasColumnType("decimal(9,2)");
            b.Property(x => x.BatchNumberSeries).HasMaxLength(100);
            b.Property(x => x.SerialNumberSeries).HasMaxLength(100);
        }
    }
}
