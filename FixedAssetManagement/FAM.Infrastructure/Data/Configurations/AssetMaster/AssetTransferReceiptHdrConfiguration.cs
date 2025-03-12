using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain.Entities.AssetMaster;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FAM.Infrastructure.Data.Configurations.AssetMaster
{
    public class AssetTransferReceiptHdrConfiguration : IEntityTypeConfiguration<AssetTransferReceiptHdr>
    {
        public void Configure(EntityTypeBuilder<AssetTransferReceiptHdr> builder)
        {
            builder.ToTable("AssetTransferReceiptHdr", "FixedAsset");
                // Primary Key
            builder.HasKey(b => b.Id);
            builder.Property(b => b.Id)
                .HasColumnName("Id")
                .HasColumnType("int")
                .IsRequired();

            builder.Property(dg => dg.AssetTransferId)                
                .HasColumnType("int")
                .IsRequired();

             // One-to-One: AssetTransferId with AssetTransferIssueHdr
            builder.HasOne(x => x.AssetTransferIssueHdr)
                   .WithOne()
                   .HasForeignKey<AssetTransferReceiptHdr>(x => x.AssetTransferId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Property(dg => dg.DocDate)   
                .HasColumnType("datetimeoffset")                             
                .IsRequired();

              builder.Property(dg => dg.TransferType)
                .HasColumnType("int")
                .IsRequired(); 

             // Configure Foreign Key Relationship
            builder.HasOne(dg => dg.TransferTypeReceiptMiscType)
                .WithMany(ag => ag.AssetTransferReceiptHdr)
                .HasForeignKey(dg => dg.TransferType)                
                .OnDelete(DeleteBehavior.Restrict);

                builder.Property(dg => dg.FromUnitId)
                .HasColumnType("int")
                .IsRequired(); 

            builder.Property(dg => dg.ToUnitId)
                .HasColumnType("int")
                .IsRequired(); 
            
            builder.Property(dg => dg.FromDepartmentId)
                .HasColumnType("int")
                .IsRequired(); 

            builder.Property(dg => dg.ToDepartmentId)
                .HasColumnType("int")
                .IsRequired(); 

            builder.Property(dg => dg.FromCustodianId)
                .HasColumnType("int")
                .IsRequired(); 

            builder.Property(b => b.FromCustodianName)
                .IsRequired()
                .HasColumnType("nvarchar(100)");

            builder.Property(dg => dg.ToCustodianId)
                .HasColumnType("int")
                .IsRequired(); 

            builder.Property(b => b.ToCustodianName)
                .IsRequired()
                .HasColumnType("nvarchar(100)");

            builder.Property(b => b.Sdcno)
                .HasColumnType("nvarchar(50)");

            builder.Property(b => b.GatePassNo)
                .HasColumnType("nvarchar(50)");

            builder.Property(b => b.AuthorizedBy)
                .HasColumnType("varchar(50)")
                .IsRequired(); 

            builder.Property(dg => dg.AuthorizedDate)   
                .HasColumnType("datetimeoffset")                             
                .IsRequired(); 
    
            builder.Property(b => b.AuthorizedByName)
                .HasColumnType("varchar(50)")
                 .IsRequired(); 

            builder.Property(b => b.AuthorizedIP)
                .HasColumnType("varchar(50)")
                .IsRequired(); 

            builder.Property(b => b.Remarks)
                .HasColumnType("nvarchar(250)");          

        }
    }
}