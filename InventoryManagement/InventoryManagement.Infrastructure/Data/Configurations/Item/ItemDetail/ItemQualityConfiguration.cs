using Core.Domain.Entities.Item.ItemDetail;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventoryManagement.Infrastructure.Data.Configurations.Item.ItemDetail
{
    public sealed class ItemQualityConfiguration : IEntityTypeConfiguration<ItemQuality>
    {
        public void Configure(EntityTypeBuilder<ItemQuality> b)
        {
            b.ToTable("ItemQuality");
            b.HasKey(x => x.ItemId);
        }
    }
}
