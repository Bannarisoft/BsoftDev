using Core.Domain.Entities.Item.ItemDetail;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using static Core.Domain.Common.BaseEntity;

namespace InventoryManagement.Infrastructure.Data.Configurations.Item.ItemDetail
{
    public sealed class ItemMasterConfiguration : IEntityTypeConfiguration<ItemMaster>
    {
        public void Configure(EntityTypeBuilder<ItemMaster> b)
        {
            // --- enum -> bit converters (unchanged) ---
            var statusConverter = new ValueConverter<Status, bool>(
                v => v == Status.Active,
                v => v ? Status.Active : Status.Inactive
            );
            var isDeleteConverter = new ValueConverter<IsDelete, bool>(
                v => v == IsDelete.Deleted,
                v => v ? IsDelete.Deleted : IsDelete.NotDeleted
            );

            b.ToTable("ItemMaster", "Inventory");

            // PK
            b.HasKey(x => x.Id);
            b.Property(x => x.Id).HasColumnName("Id").HasColumnType("int").IsRequired();

            // --- Base fields (unchanged) ---
            b.Property(x => x.UnitId).HasColumnName("UnitId").HasColumnType("int").IsRequired();
            b.Property(x => x.ItemCode).HasColumnName("ItemCode").HasColumnType("varchar(50)").IsRequired();
            b.Property(x => x.ItemName).HasColumnName("ItemName").HasColumnType("varchar(200)").IsRequired();

            b.Property(x => x.HSNId).HasColumnName("HSNId").HasColumnType("int").IsRequired(false);
            b.HasOne(x => x.HSNMaster)
             .WithMany(g => g.ItemMasterHSN)
             .HasForeignKey(x => x.HSNId)
             .OnDelete(DeleteBehavior.Restrict);

            b.Property(x => x.ItemGroupId).HasColumnName("ItemGroupId").HasColumnType("int").IsRequired(false);
            b.HasOne(x => x.ItemGroup)
             .WithMany(g => g.ItemMasterGroup)
             .HasForeignKey(x => x.ItemGroupId)
             .OnDelete(DeleteBehavior.Restrict);

            b.Property(x => x.ItemCategoryId).HasColumnName("ItemCategoryId").HasColumnType("int").IsRequired(false);
            b.HasOne(x => x.ItemCategory)
             .WithMany(c => c.ItemMasterCategory)
             .HasForeignKey(x => x.ItemCategoryId)
             .OnDelete(DeleteBehavior.Restrict);

            b.Property(x => x.StockUomId).HasColumnName("StockUomId").HasColumnType("int").IsRequired(false);
            b.HasOne(x => x.UOM)
             .WithMany(c => c.ItemMasterUOM)
             .HasForeignKey(x => x.StockUomId)
             .OnDelete(DeleteBehavior.Restrict);

            b.Property(x => x.ItemClassificationId).HasColumnName("ItemClassificationId").HasColumnType("int").IsRequired(false);
            b.HasOne(x => x.MiscClassification)
             .WithMany(c => c.ItemMasterClassification)
             .HasForeignKey(x => x.ItemClassificationId)
             .OnDelete(DeleteBehavior.NoAction);

            b.Property(x => x.Description).HasColumnName("Description").HasColumnType("varchar(500)").IsRequired(false);
            b.Property(x => x.ValidFrom).HasColumnName("ValidFrom").HasColumnType("date").IsRequired(false);

            b.Property(x => x.XPlantMaterialStatusId).HasColumnName("XPlantMaterialStatusId").HasColumnType("int").IsRequired(false);
            b.HasOne(x => x.MiscStatus)
             .WithMany(c => c.ItemMasterStatus)
             .HasForeignKey(x => x.XPlantMaterialStatusId)
             .OnDelete(DeleteBehavior.NoAction);

            b.Property(x => x.IsStockItem).HasColumnName("IsStockItem").HasColumnType("bit");
            b.Property(x => x.MaintainStock).HasColumnName("MaintainStock").HasColumnType("bit");
            b.Property(x => x.HasVariants).HasColumnName("HasVariants").HasColumnType("bit");

            // Parent template self-reference: block delete if children exist
            b.Property(x => x.ParentItemId).HasColumnName("ParentItemId").HasColumnType("int").IsRequired(false);
            b.HasOne(x => x.ParentItem)
             .WithMany(x => x.ChildItems)
             .HasForeignKey(x => x.ParentItemId)
             .OnDelete(DeleteBehavior.Restrict);

            b.Property(x => x.ItemImage).HasColumnName("ItemImage").HasColumnType("nvarchar(255)").IsRequired(false);

            // ---------- IMPORTANT PART: set dependents to Restrict ----------
            // 1:1 tabs – DO NOT cascade; prevent delete when a tab exists
            b.HasOne(x => x.Purchase)
             .WithOne(p => p.Item)
             .HasForeignKey<ItemPurchase>(p => p.ItemId)
             .OnDelete(DeleteBehavior.Restrict);

            b.HasOne(x => x.Inventory)
             .WithOne(i => i.Item)
             .HasForeignKey<ItemInventory>(i => i.ItemId)
             .OnDelete(DeleteBehavior.Restrict);

            b.HasOne(x => x.Quality)
             .WithOne(q => q.Item)
             .HasForeignKey<ItemQuality>(q => q.ItemId)
             .OnDelete(DeleteBehavior.Restrict);

            // Collections – Restrict so parent delete fails if any rows exist
            b.HasMany(x => x.Suppliers)
             .WithOne(s => s.Item)
             .HasForeignKey(s => s.ItemId)
             .OnDelete(DeleteBehavior.Restrict);

            b.HasMany(x => x.Manufacture)
             .WithOne(m => m.Item)
             .HasForeignKey(m => m.ItemId)
             .OnDelete(DeleteBehavior.Restrict);

            b.HasMany(x => x.ItemUOMs)
             .WithOne(u => u.Item)
             .HasForeignKey(u => u.ItemId)
             .OnDelete(DeleteBehavior.Restrict);

            // Variants – Restrict (prevents deleting item while variant rows exist)
            b.HasMany(x => x.VariantValues)
             .WithOne(v => v.ItemMaster)
             .HasForeignKey(v => v.ItemId)
             .OnDelete(DeleteBehavior.Restrict);

            // If you also have the reverse link (NewItemId -> ItemMaster)
            b.HasMany(x => x.VariantNewItem)
             .WithOne(v => v.NewItem)
             .HasForeignKey(v => v.NewItemId)
             .OnDelete(DeleteBehavior.Restrict);

            // --- BaseEntity columns (unchanged) ---
            b.Property(x => x.IsActive).HasColumnName("IsActive").HasColumnType("bit").HasConversion(statusConverter).IsRequired();
            b.Property(x => x.IsDeleted).HasColumnName("IsDeleted").HasColumnType("bit").HasConversion(isDeleteConverter).IsRequired();
            b.Property(x => x.CreatedBy).HasColumnName("CreatedBy").HasColumnType("int").IsRequired();
            b.Property(x => x.CreatedDate).HasColumnName("CreatedDate").HasColumnType("datetimeoffset").IsRequired(false);
            b.Property(x => x.CreatedByName).HasColumnName("CreatedByName").HasColumnType("varchar(50)").IsRequired(false);
            b.Property(x => x.CreatedIP).HasColumnName("CreatedIP").HasColumnType("varchar(50)").IsRequired(false);
            b.Property(x => x.ModifiedBy).HasColumnName("ModifiedBy").HasColumnType("int").IsRequired(false);
            b.Property(x => x.ModifiedDate).HasColumnName("ModifiedDate").HasColumnType("datetimeoffset").IsRequired(false);
            b.Property(x => x.ModifiedByName).HasColumnName("ModifiedByName").HasColumnType("varchar(50)").IsRequired(false);
            b.Property(x => x.ModifiedIP).HasColumnName("ModifiedIP").HasColumnType("varchar(50)").IsRequired(false);
        }
    }
}
