using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain.Entities.AssetMaster;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FAM.Infrastructure.Data.Configurations.AssetMaster
{
    public class AssetLocationConfiguration : IEntityTypeConfiguration<AssetLocation>
    {
        public void Configure(EntityTypeBuilder<AssetLocation> builder)
        {
               builder.ToTable("AssetLocation", "FixedAsset");


                builder.HasKey(al => al.Id); // Primary Key

                // Foreign Key Relationships
                builder.HasOne(al => al.Location)
                    .WithMany()  // If Location has a collection, use .WithMany(l => l.AssetLocations)
                    .HasForeignKey(al => al.LocationId)
                    .OnDelete(DeleteBehavior.Restrict); // Prevents cascade delete

                builder.HasOne(al => al.SubLocation)
                    .WithMany()
                    .HasForeignKey(al => al.SubLocationId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Other Required Constraints
                builder.Property(al => al.AssetId)
                  .HasColumnName("AssetId")
                   .HasColumnType("int")  // Set as int
                    .IsRequired();

                builder.Property(al => al.UnitId)
                 .HasColumnName("UnitId")
                   .HasColumnType("int")  // Set as int
                    .IsRequired();

                builder.Property(al => al.DepartmentId)
                     .HasColumnName("DepartmentId")
                   .HasColumnType("int")  // Set as int
                    .IsRequired();

                builder.Property(al => al.CustodianId)
                     .HasColumnName("CustodianId")
                   .HasColumnType("NVARCHAR(50)")  
                    .IsRequired();

                builder.Property(al => al.UserID)
                     .HasColumnName("UserId")
                   .HasColumnType("NVARCHAR(50)");            



        }
    }
}