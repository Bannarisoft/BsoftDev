using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain.Entities.AssetMaster;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FAM.Infrastructure.Data.Configurations.AssetMaster
{
    public class AssetTransferReceiptDtlConfiguration : IEntityTypeConfiguration<AssetTransferReceiptDtl>
    {
        public void Configure(EntityTypeBuilder<AssetTransferReceiptDtl> builder)
        {
           builder.ToTable("AssetTransferReceiptDtl", "FixedAsset");
                // Primary Key
            builder.HasKey(b => b.Id);
            builder.Property(b => b.Id)
                .HasColumnName("Id")
                .HasColumnType("int")
                .IsRequired();

             builder.Property(dg => dg.AssetReceiptId)                
                .HasColumnType("int")
                .IsRequired();

              // One-to-Many: AssetReceiptId with AssetTransferReceiptHdr
            builder.HasOne(x => x.AssetTransferReceiptHdr)
                   .WithMany(x => x.AssetTransferReceiptDtl) // One-to-Many
                   .HasForeignKey(x => x.AssetReceiptId)
                   .OnDelete(DeleteBehavior.Cascade); // Deleting a Receipt Header deletes details

            builder.Property(dg => dg.AssetId)                
                .HasColumnType("int")
                .IsRequired();

             // One-to-Many: AssetId with AssetMasterGenerals
            builder.HasOne(x => x.AssetMasterTransferReceipt)
                   .WithMany(x => x.AssetTransferReceiptMaster) // Assuming no navigation property on AssetMasterGenerals
                   .HasForeignKey(x => x.AssetId)
                   .OnDelete(DeleteBehavior.Restrict); // Prevent accidental deletions

            builder.Property(dg => dg.LocationId)                
                .HasColumnType("int")
                .IsRequired();
             // One-to-Many: LocationId with Location
             builder.HasOne(x => x.Location)
                   .WithMany(x=>x.AssetTransferReceiptLocation)
                   .HasForeignKey(x => x.LocationId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Property(dg => dg.SubLocationId)                
                .HasColumnType("int")
                .IsRequired();

            // One-to-Many: SubLocationId with SubLocation
            builder.HasOne(x => x.SubLocation)
                   .WithMany(x=>x.AssetTransferReceiptSubLocation)
                   .HasForeignKey(x => x.SubLocationId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Property(b => b.UserID)
                .HasColumnType("nvarchar(100)");

            builder.Property(b => b.UserName)
                .HasColumnType("nvarchar(100)");


            


             




            

        }
    }
}