using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FAM.Infrastructure.Data.Configurations.AssetPurchase
{
    public class AssetPurchaseConfiguration :   IEntityTypeConfiguration<Core.Domain.Entities.AssetPurchase.AssetPurchase>
    {
        public void Configure(EntityTypeBuilder<Core.Domain.Entities.AssetPurchase.AssetPurchase> builder)
        {
            builder.ToTable("AssetPurchase", "FixedAsset");
              
              // Primary Key
                builder.HasKey(b => b.Id);
                builder.Property(b => b.Id)
                .HasColumnName("Id")
                .HasColumnType("int")
                .IsRequired();

                builder.Property(b => b.BudgetType)   
                 .HasColumnName("BudgetType")             
                 .HasColumnType("varchar(50)")
                .IsRequired();  

                builder.Property(b => b.OldUnitId)   
                 .HasColumnName("OldUnitId")             
                 .HasColumnType("nvarchar(10)")
                 .IsRequired();  

                builder.Property(b => b.VendorCode)   
                 .HasColumnName("VendorCode")             
                 .HasColumnType("nvarchar(20)")
                .IsRequired(); 

                 builder.Property(b => b.VendorName)   
                 .HasColumnName("VendorName")             
                 .HasColumnType("nvarchar(500)")
                .IsRequired(); 
                  builder.Property(b => b.PoDate)
                .HasColumnName("PoDate")
                .HasColumnType("datetimeoffset")
                .IsRequired();

            builder.Property(b => b.PoNo)
                .HasColumnName("PoNo")
                .HasColumnType("nvarchar(50)");

            builder.Property(b => b.PoSno)
                .HasColumnName("PoSno")
                .HasColumnType("nvarchar(50)");

            builder.Property(b => b.ItemCode)
                .HasColumnName("ItemCode")
                .HasColumnType("nvarchar(50)");

            builder.Property(b => b.ItemName)
                .HasColumnName("ItemName")
                .HasColumnType("nvarchar(500)");

            builder.Property(b => b.GrnNo)
                .HasColumnName("GrnNo")
                .HasColumnType("nvarchar(50)");

            builder.Property(b => b.GrnSno)
                .HasColumnName("GrnSno")
                .HasColumnType("nvarchar(50)");

            builder.Property(b => b.GrnDate)
                .HasColumnName("GrnDate")
                .HasColumnType("datetimeoffset");

            builder.Property(b => b.QcCompleted)
                .HasColumnName("QcCompleted")
                .HasColumnType("char(1)");

            builder.Property(b => b.AcceptedQty)
                .HasColumnName("AcceptedQty")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(b => b.PurchaseValue)
                .HasColumnName("PurchaseValue")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(b => b.GrnValue)
                .HasColumnName("GrnValue")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(b => b.BillNo)
                .HasColumnName("BillNo")
                .HasColumnType("nvarchar(50)");

            builder.Property(b => b.BillDate)
                .HasColumnName("BillDate")
                .HasColumnType("datetimeoffset");

            builder.Property(b => b.Uom)
                .HasColumnName("Uom")
                .HasColumnType("nvarchar(50)");

            builder.Property(b => b.BinLocation)
                .HasColumnName("BinLocation")
                .HasColumnType("nvarchar(500)");

            builder.Property(b => b.PjYear)
                .HasColumnName("PjYear")
                .HasColumnType("nvarchar(10)");

            builder.Property(b => b.PjDocId)
                .HasColumnName("PjDocId")
                .HasColumnType("nvarchar(50)");

            builder.Property(b => b.PjDocSr)
                .HasColumnName("PjDocSr")
                .HasColumnType("nvarchar(50)");

            builder.Property(b => b.PjDocNo)
                .HasColumnName("PjDocNo")
                .HasColumnType("nvarchar(50)");

            // Relationships
            builder.HasOne(b => b.AssetMaster)
                .WithMany()
                .HasForeignKey(b => b.AssetMasterId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(b => b.AssetSource)
                .WithMany()
                .HasForeignKey(b => b.AssetSourceId)
                .OnDelete(DeleteBehavior.Restrict); 
      
        }
    }
}