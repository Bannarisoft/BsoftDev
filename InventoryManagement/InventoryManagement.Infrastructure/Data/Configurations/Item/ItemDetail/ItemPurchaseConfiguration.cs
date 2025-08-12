
using Core.Domain.Entities.Item.ItemDetail;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventoryManagement.Infrastructure.Data.Configurations.Item.ItemDetail
{
    public sealed class ItemPurchaseConfiguration : IEntityTypeConfiguration<ItemPurchase>
    {
        public void Configure(EntityTypeBuilder<ItemPurchase> b)
        {
            b.ToTable("ItemPurchase");
            b.HasKey(x => x.ItemId);
            b.Property(x => x.PurchaseRate).HasColumnType("decimal(18,2)");
            b.Property(x => x.TariffNumber).HasMaxLength(50);
        }
    }
}
