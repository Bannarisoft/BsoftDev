

using Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using static Core.Domain.Common.BaseEntity;

namespace FAM.Infrastructure.Data.Configurations
{
    public class DepreciationDetailConfiguration : IEntityTypeConfiguration<DepreciationDetails>
    {
        public void Configure(EntityTypeBuilder<DepreciationDetails> builder)
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

                builder.ToTable("DepreciationDetail", "FixedAsset");
                // Primary Key
                builder.HasKey(b => b.Id);
                builder.Property(b => b.Id)
                .HasColumnName("Id")
                .HasColumnType("int")
                .IsRequired();

                builder.Property(dg => dg.CompanyId)                
                .HasColumnType("int")
                .IsRequired(); 

                builder.Property(dg => dg.UnitId)                
                .HasColumnType("int")
                .IsRequired(); 

                builder.Property(dg => dg.Finyear)                
                .HasColumnType("varchar(10)")
                .IsRequired(); 

                 builder.Property(dg => dg.StartDate)                
                .HasColumnType("datetimeoffset")
                .IsRequired(); 

                builder.Property(dg => dg.EndDate)                
                .HasColumnType("datetimeoffset")
                .IsRequired(); 

                builder.Property(dg => dg.DepreciationType)                
                .HasColumnType("varchar(10)")
                .IsRequired(); 
                
                builder.Property(dg => dg.AssetId)                
                .HasColumnType("int")
                .IsRequired(); 
                // Configure Foreign Key Relationship
                builder.HasOne(dg => dg.AssetMasterId)
                .WithMany(ag => ag.DepreciationDetails)
                .HasForeignKey(dg => dg.AssetId)                
                .OnDelete(DeleteBehavior.Restrict); 

                builder.Property(dg => dg.AssetGroupId)                
                .HasColumnType("int")
                .IsRequired(); 
                  // Configure Foreign Key Relationship
                builder.HasOne(dg => dg.AssetGroup)
                .WithMany(ag => ag.DepreciationDetails)
                .HasForeignKey(dg => dg.AssetGroupId)                
                .OnDelete(DeleteBehavior.Restrict); 

                builder.Property(dg => dg.AssetValue)                
                .HasColumnType("decimal(18,3)")
                .IsRequired(); 
                
                builder.Property(dg => dg.CapitalizationDate)                
                .HasColumnType("datetimeoffset")
                .IsRequired(); 

                builder.Property(dg => dg.ResidualValue)                
                .HasColumnType("decimal(18,3)")
                .IsRequired(); 

                builder.Property(dg => dg.ExpiryDate)                
                .HasColumnType("datetimeoffset")
                .IsRequired(); 

                builder.Property(dg => dg.UsefulLifeDays)                
                .HasColumnType("int")
                .IsRequired(); 

                builder.Property(dg => dg.DaysOpening)                
                .HasColumnType("int")
                .IsRequired(); 
                
                builder.Property(dg => dg.DaysUsed)                
                .HasColumnType("int")
                .IsRequired(); 

                builder.Property(dg => dg.OpeningValue)                
                .HasColumnType("decimal(18,3)")
                .IsRequired(); 

                builder.Property(dg => dg.DepreciationValue)                
                .HasColumnType("decimal(18,3)")
                .IsRequired(); 

                builder.Property(dg => dg.ClosingValue)                
                .HasColumnType("decimal(18,3)")
                .IsRequired(); 

                builder.Property(dg => dg.DepreciationPeriod)                
                .HasColumnType("int")
                .IsRequired(); 
                  // Configure Foreign Key Relationship
                builder.HasOne(dg => dg.DepMiscType)
                .WithMany(ag => ag.DepreciationPeriod)
                .HasForeignKey(dg => dg.DepreciationPeriod)                
                .OnDelete(DeleteBehavior.Restrict); 

                builder.Property(dg => dg.DisposedDate)                
                .HasColumnType("datetimeoffset");
                

                builder.Property(dg => dg.DisposalAmount)                
                .HasColumnType("decimal(18,3)")
                .IsRequired(false); 

                builder.Property(c => c.IsLocked)                
                .HasColumnType("bit")
                .HasConversion(
                    v => v == 1, 
                    v => v ? (byte)1 : (byte)0 
                )
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
