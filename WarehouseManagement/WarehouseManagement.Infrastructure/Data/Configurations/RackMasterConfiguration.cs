using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using static Core.Domain.Common.BaseEntity;

namespace WarehouseManagement.Infrastructure.Data.Configurations
{
    public class RackMasterConfiguration : IEntityTypeConfiguration<RackMaster>
    {
        public void Configure(EntityTypeBuilder<RackMaster> builder)
        {
                 var statusConverter = new ValueConverter<Status, bool>(
                    v => v == Status.Active,                    // Convert to DB (1 for Active)
                    v => v ? Status.Active : Status.Inactive    // Convert to Entity
                );

            // ValueConverter for IsDelete (enum to bit)
            var isDeleteConverter = new ValueConverter<IsDelete, bool>(
                v => v == IsDelete.Deleted,                 // Convert to DB (1 for Deleted)
                v => v ? IsDelete.Deleted : IsDelete.NotDeleted // Convert to Entity
               );
            

            builder.ToTable("RackMaster", "Warehouse");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.WarehouseId).IsRequired();
            builder.Property(x => x.RackCode).IsRequired().HasMaxLength(50);
           
         


            // Rack -> Warehouse (many-to-one)
            builder.HasOne(x => x.Warehouse)
            .WithMany(w => w.Racks)
            .HasForeignKey(x => x.WarehouseId)
            .OnDelete(DeleteBehavior.Restrict); // avoid cascading deletes across core masters

            // Optional unique per warehouse
            builder.HasIndex(x => new { x.WarehouseId, x.RackCode })
            .IsUnique()
            .HasDatabaseName("UX_Rack_Warehouse_RackCode");

        }
        
    }
}