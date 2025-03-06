using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain.Entities.AssetMaster;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using static Core.Domain.Common.BaseEntity;

namespace FAM.Infrastructure.Data.Configurations.AssetMaster
{
    public class AssetDisposalConfiguration : IEntityTypeConfiguration<AssetDisposal>
    {
        public void Configure(EntityTypeBuilder<AssetDisposal> builder)
        {
             var statusConverter = new ValueConverter<Status, bool>(
                    v => v == Status.Active,                    
                    v => v ? Status.Active : Status.Inactive    
                );
            // ValueConverter for IsDelete (enum to bit)
                var isDeleteConverter = new ValueConverter<IsDelete, bool>(
                    v => v == IsDelete.Deleted,                 
                    v => v ? IsDelete.Deleted : IsDelete.NotDeleted
                );
            builder.ToTable("AssetDisposal", "FixedAsset");

            // Primary Key
            builder.HasKey(b => b.Id);
            builder.Property(b => b.Id)
                .HasColumnName("Id")
                .HasColumnType("int")
                .IsRequired();

            builder.Property(dg => dg.AssetId)
                .HasColumnType("int")                
                .IsRequired();

              // One-to-One: AssetMasterGenerals
            builder.HasOne(ad => ad.AssetMasterDisposal)
                   .WithOne()
                   .HasForeignKey<AssetDisposal>(ad => ad.AssetId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Property(dg => dg.AssetPurchaseId)
                .HasColumnType("int")                
                .IsRequired();

              // One-to-One: AssetPurchaseDetails
            builder.HasOne(ad => ad.AssetPurchaseDetails)
                   .WithOne()
                   .HasForeignKey<AssetDisposal>(ad => ad.AssetPurchaseId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Property(dg => dg.DisposalDate)
                .HasColumnType("datetimeoffset")                               
                .IsRequired();
            
            builder.Property(dg => dg.DisposalType)
                .HasColumnType("int")                
                .IsRequired();

            // One-to-One: MiscMaster (DisposalType)
            builder.HasOne(ad => ad.AssetMiscDisposalType)
                   .WithOne()
                   .HasForeignKey<AssetDisposal>(ad => ad.DisposalType)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Property(dg => dg.DisposalReason)
                .HasColumnType("nvarchar(250)");

            builder.Property(dg => dg.DisposalAmount)                
                .HasColumnType("decimal(18,3)")                
                .IsRequired();

            builder.Property(b => b.IsActive)                
                .HasColumnType("bit")
                .HasConversion(statusConverter)
                .IsRequired();

            builder.Property(b => b.IsDeleted)                
                .HasColumnType("bit")
                .HasConversion(isDeleteConverter)
                .IsRequired();

            builder.Property(b => b.CreatedByName)
                .IsRequired()
                .HasColumnType("varchar(50)");
    
            builder.Property(b => b.CreatedIP)
                .IsRequired()
                .HasColumnType("varchar(50)");

            builder.Property(b => b.ModifiedByName)
                .HasColumnType("varchar(50)");

            builder.Property(b => b.ModifiedIP)
                .HasColumnType("varchar(50)"); 
        }
    }
}