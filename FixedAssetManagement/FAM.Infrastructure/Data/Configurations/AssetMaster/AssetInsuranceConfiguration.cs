using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain.Entities.AssetMaster;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FAM.Infrastructure.Data.Configurations.AssetMaster
{
    public class AssetInsuranceConfiguration : IEntityTypeConfiguration<AssetInsurance>
    {
       public void Configure(EntityTypeBuilder<AssetInsurance> builder)
        {
           builder.ToTable("AssetInsurance", "FixedAsset");

            builder.HasKey(al => al.Id); // Primary Key            

            builder.HasOne(ai => ai.AssetMaster)  
               .WithMany(amg => amg.AssetInsurance)  // Change to WithOne() if it's a One-to-One relationship
               .HasForeignKey(ai => ai.AssetId)  
               .OnDelete(DeleteBehavior.Restrict); // Prevent cascading delete

            builder.Property(ai => ai.PolicyNo)               
               .HasColumnType("varchar(50)")
               .IsRequired();
              
            builder.Property(ai => ai.StartDate)
               .HasColumnType("datetime")
               .IsRequired();;
               
            builder.Property(ai => ai.PolicyAmount)
                .HasColumnType("decimal(18,3)") // Defines precision and scale
                .IsRequired(false);

            builder.Property(ai => ai.VendorCode)
                .HasColumnType("nvarchar(50)")
                .HasMaxLength(50)
                .IsRequired(false);

            builder.Property(ai => ai.RenewalStatus)  
                .HasColumnType("varchar(50)")              
                .HasMaxLength(50)
                .IsRequired(false);

            builder.Property(ai => ai.RenewedDate)
                .HasColumnType("datetime")
                .IsRequired();

            builder.Property(ai => ai.InsuranceStatus)
                .HasColumnType("varchar(50)")
                .HasMaxLength(50)
                .IsRequired(false); 

          

        }

        
    }
}