

using Core.Domain.Entities.Item.ItemDetail;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventoryManagement.Infrastructure.Data.Configurations.Item.ItemDetail
{  
    public sealed class ItemMasterConfiguration : IEntityTypeConfiguration<ItemMaster>
    {
        public void Configure(EntityTypeBuilder<ItemMaster> b)
        {
            b.ToTable("ItemMaster");
            b.HasKey(x => x.Id);

            b.Property(x => x.ItemCode).HasMaxLength(50).IsRequired();
            b.Property(x => x.ItemName).HasMaxLength(200).IsRequired();

            b.HasIndex(x => x.ItemCode).IsUnique();         
            b.HasIndex(x => x.ParentItemId);

            b.HasOne(x => x.ParentItem)
             .WithMany()
             .HasForeignKey(x => x.ParentItemId)
             .OnDelete(DeleteBehavior.Restrict);

            // 1-1 shared key tabs (optional)
            b.HasOne(x => x.General).WithOne(x => x.Item)
             .HasForeignKey<ItemGeneral>(x => x.ItemId).OnDelete(DeleteBehavior.Cascade);

            b.HasOne(x => x.Purchase).WithOne(x => x.Item)
             .HasForeignKey<ItemPurchase>(x => x.ItemId).OnDelete(DeleteBehavior.Cascade);

            b.HasOne(x => x.Inventory).WithOne(x => x.Item)
             .HasForeignKey<ItemInventory>(x => x.ItemId).OnDelete(DeleteBehavior.Cascade);

            b.HasOne(x => x.Quality).WithOne(x => x.Item)
             .HasForeignKey<ItemQuality>(x => x.ItemId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}