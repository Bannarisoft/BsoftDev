using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using static Core.Domain.Common.BaseEntity;

namespace MaintenanceManagement.Infrastructure.Data.Configurations
{
    public class ActivityMasterConfiguration : IEntityTypeConfiguration<ActivityMaster>
    {
        public void Configure(EntityTypeBuilder<ActivityMaster> builder)
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
            builder.ToTable("ActivityMaster", "Maintenance");

            builder.HasKey(t => t.Id);
            builder.Property(t => t.Id)
                .HasColumnName("Id")
                .HasColumnType("int")
                .IsRequired();

            builder.Property(t => t.ActivityCode)
                .HasColumnName("ActivityCode")
                .HasColumnType("varchar(20)")
                .IsRequired();
            builder.Property(t => t.ActivityName)
                .HasColumnName("ActivityName")
                .HasColumnType("varchar(100)")
                .IsRequired();
            builder.Property(t => t.Description)
                .HasColumnName("Description")
                .HasColumnType("varchar(500)")
                .IsRequired();
                builder.Property(t => t.DepartmentId)
                .HasColumnName("DepartmentId")
                .HasColumnType("int")
                .IsRequired();

            builder.Property(t => t.MachineGroup)
                .HasColumnName("MachineGroup")
                .HasColumnType("varchar(100)")
                .IsRequired();

            builder.Property(t => t.EstimatedDuration)
                .HasColumnName("EstimatedDuration")
                .HasColumnType("int")
                .IsRequired();

            builder.Property(t => t.ActivityType)
                .HasColumnName("ActivityType")
                .HasColumnType("int")
                .IsRequired();




               
        }
    }
}