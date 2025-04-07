using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MaintenanceManagement.Infrastructure.Data.Configurations
{
    public class PreventiveSchedulerHdrConfiguration : IEntityTypeConfiguration<PreventiveSchedulerHdr>
    {
        public void Configure(EntityTypeBuilder<PreventiveSchedulerHdr> builder)
        {
            builder.ToTable("PreventiveSchedulerHdr", "Maintenance");

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
                
        }
    }
}