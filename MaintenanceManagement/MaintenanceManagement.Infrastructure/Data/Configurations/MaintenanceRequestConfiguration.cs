using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using static Core.Domain.Common.BaseEntity;

namespace MaintenanceManagement.Infrastructure.Data.Configurations
{
    public class MaintenanceRequestConfiguration : IEntityTypeConfiguration<MaintenanceRequest>
    {
        public void Configure(EntityTypeBuilder<MaintenanceRequest> builder)
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

               builder.ToTable("MaintenanceRequest", "Maintenance");

                // Primary Key
                   builder.HasKey(m => m.Id);
                builder.Property(m => m.Id)                
                .HasColumnType("int")
                .IsRequired();
                
                 builder.Property(m => m.RequestTypeId)                
                 .HasColumnType("int")
                 .IsRequired();

                 builder.Property(m => m.MaintenanceTypeId)                
                 .HasColumnType("int")                 
                 .IsRequired();

                 builder.Property(m => m.MachineId)               
                 .HasColumnType("int")
                 .IsRequired();

                 builder.Property(m => m.CompanyId)                
                 .HasColumnType("int")
                 .IsRequired();

                 builder.Property(m => m.UnitId)                
                 .HasColumnType("int")
                 .IsRequired();

                 builder.Property(m => m.DepartmentId)                 
                 .HasColumnType("int")
                 .IsRequired();


                builder.Property(m => m.Remarks)                 
                 .HasColumnType("nvarchar(max)")
                 .IsRequired(false);

                 builder.Property(m => m.RequestId)                 
                 .HasColumnType("nvarchar(max)")
                 .IsRequired();

                 builder.Property(m => m.RequestStatusId)                 
                 .HasColumnType("int")
                 .IsRequired(false);


                builder.HasOne(b => b.MiscRequestType)
                .WithMany(b => b.RequestType)
                .HasForeignKey(b => b.RequestTypeId)
                .OnDelete(DeleteBehavior.Restrict);

                 builder.HasOne(b => b.MiscMaintenanceType)
                .WithMany(b => b.MaintenanceType)
                .HasForeignKey(b => b.MaintenanceTypeId)
                .OnDelete(DeleteBehavior.Restrict);

                builder.HasOne(b => b.Machine)
                .WithMany(b => b.MaintenanceRequest)
                .HasForeignKey(b => b.MachineId)
                .OnDelete(DeleteBehavior.Restrict);

        

                builder.HasOne(b => b.RequestStatus)
                .WithMany(b => b.RequestStatus)
                .HasForeignKey(b => b.RequestStatusId)
                .OnDelete(DeleteBehavior.Restrict);

             

            
        }

    }
}