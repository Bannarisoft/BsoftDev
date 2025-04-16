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
    public class PreventiveSchedulerHdrConfiguration : IEntityTypeConfiguration<PreventiveSchedulerHeader>
    {
        public void Configure(EntityTypeBuilder<PreventiveSchedulerHeader> builder)
        {
            var isActiveConverter = new ValueConverter<Status, bool>
               (
                    v => v == Status.Active,  
                    v => v ? Status.Active : Status.Inactive 
                );

                var isDeletedConverter = new ValueConverter<IsDelete, bool>
                (
                 v => v == IsDelete.Deleted,  
                 v => v ? IsDelete.Deleted : IsDelete.NotDeleted 
                );
                
            builder.ToTable("PreventiveSchedulerHeader", "Maintenance");

            builder.HasKey(b => b.Id);

            builder.Property(b => b.MachineGroupId)
                .HasColumnName("MachineGroupId")
                .HasColumnType("int")
                .IsRequired();

            builder.Property(b => b.DepartmentId)
                .HasColumnName("DepartmentId")
                .HasColumnType("int")
                .IsRequired();
            builder.Property(b => b.MaintenanceCategoryId)
                .HasColumnName("MaintenanceCategoryId")
                .HasColumnType("int")
                .IsRequired();
            builder.Property(b => b.ScheduleId)
                .HasColumnName("ScheduleId")
                .HasColumnType("int")
                .IsRequired();
            builder.Property(b => b.DueTypeId)
                .HasColumnName("DueTypeId")
                .HasColumnType("int")
                .IsRequired();
            builder.Property(b => b.DuePeriod)
                .HasColumnName("DuePeriod")
                .HasColumnType("int")
                .IsRequired();
            builder.Property(b => b.FrequencyId)
                .HasColumnName("FrequencyId")
                .HasColumnType("int")
                .IsRequired();
            builder.Property(b => b.EffectiveDate)
                .HasColumnName("EffectiveDate")
                .HasColumnType("date")
                .IsRequired();
            builder.Property(b => b.GraceDays)
                .HasColumnName("GraceDays")
                .HasColumnType("int")
                .IsRequired();
            builder.Property(b => b.ReminderWorkOrderDays)
                .HasColumnName("ReminderWorkOrderDays")
                .HasColumnType("int")
                .IsRequired();
            builder.Property(b => b.ReminderMaterialReqDays)
                .HasColumnName("ReminderMaterialReqDays")
                .HasColumnType("int")
                .IsRequired();
            builder.Property(b => b.IsDownTimeRequired)
                .HasColumnName("IsDownTimeRequired")
                .HasColumnType("bit")
                  .HasConversion(
                    v => v == 1, 
                    v => v ? (byte)1 : (byte)0 
                )
                .IsRequired();

                 builder.Property(cf => cf.IsActive)
                .HasColumnName("IsActive")
                .HasColumnType("bit")
                .HasConversion(isActiveConverter)
                .IsRequired();

            builder.Property(cf => cf.IsDeleted)
                 .HasColumnName("IsDeleted")
                 .HasColumnType("bit")
                 .HasConversion(isDeletedConverter)
                 .IsRequired();

            builder.Property(cf => cf.CreatedByName)
                .IsRequired()
                .HasColumnType("varchar(50)");

            builder.Property(cf => cf.CreatedIP)
                .IsRequired()
                .HasColumnType("varchar(255)");

            builder.Property(cf => cf.ModifiedByName)
                 .HasColumnType("varchar(50)");

            builder.Property(cf => cf.ModifiedIP)
                .HasColumnType("varchar(255)");
                
                builder.HasOne(b => b.MachineGroup)
                .WithMany(b => b.PreventiveSchedulerHeaders)
                .HasForeignKey(b => b.MachineGroupId)
                .OnDelete(DeleteBehavior.Restrict);

                builder.HasOne(b => b.MaintenanceCategory)
                .WithMany(b => b.PreventiveSchedulerHeaders)
                .HasForeignKey(b => b.MaintenanceCategoryId)
                .OnDelete(DeleteBehavior.Restrict);

                builder.HasOne(b => b.MiscSchedule)
                .WithMany(b => b.Schedule)
                .HasForeignKey(b => b.ScheduleId)
                .OnDelete(DeleteBehavior.Restrict);

                builder.HasOne(b => b.MiscDueType)
                .WithMany(b => b.DueType)
                .HasForeignKey(b => b.DueTypeId)
                .OnDelete(DeleteBehavior.Restrict);

                builder.HasOne(b => b.MiscFrequency)
                .WithMany(b => b.Frequency)
                .HasForeignKey(b => b.FrequencyId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}