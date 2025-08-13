// Infrastructure/Data/Configurations/Item/ItemDetail/Variant/ItemVariantValueConfiguration.cs
using Core.Domain.Entities.Item.ItemDetail;
using Core.Domain.Entities.Item.ItemDetail.Variant;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventoryManagement.Infrastructure.Data.Configurations.Item.ItemDetail.Variant
{
    public sealed class ItemVariantValueConfiguration : IEntityTypeConfiguration<ItemVariantValue>
    {
        public void Configure(EntityTypeBuilder<ItemVariantValue> b)
        {
            b.ToTable("ItemVariantValue", "Inventory");

            b.HasKey(x => x.Id);

            b.Property(x => x.Id)
             .HasColumnName("Id")
             .HasColumnType("int");

            b.Property(x => x.ItemId)
             .HasColumnName("ItemId")
             .HasColumnType("int")
             .IsRequired();
            b.HasOne(x => x.ItemMaster)
             .WithMany(i => i.VariantValues)
             .HasForeignKey(x => x.ItemId)
             .OnDelete(DeleteBehavior.Cascade);

            b.Property(x => x.VariantBasedOn)
             .HasColumnName("VariantBasedOn")
             .HasColumnType("int")
             .IsRequired();
            b.HasOne(x => x.MiscVariantBasedOn)
             .WithMany(i => i.ItemAttributeBasedOn)
             .HasForeignKey(x => x.VariantBasedOn)
             .OnDelete(DeleteBehavior.Cascade);


            b.Property(x => x.AttributeId)
             .HasColumnName("AttributeId")
             .HasColumnType("int")
             .IsRequired();
            b.HasOne(x => x.MiscAttribute)
             .WithMany(i => i.ItemAttribute)
             .HasForeignKey(x => x.AttributeId)
             .OnDelete(DeleteBehavior.Restrict);

            b.Property(x => x.OptionValue)
             .HasColumnName("OptionValue")
             .HasColumnType("varchar(100)")
             .IsRequired();
        }
    }
}
