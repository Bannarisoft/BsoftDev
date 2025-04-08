using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MaintenanceManagement.Infrastructure.Data.Configurations
{
    public class PreventiveSchedulerDtlConfiguration : IEntityTypeConfiguration<PreventiveSchedulerDtl>
    {
        public void Configure(EntityTypeBuilder<PreventiveSchedulerDtl> builder)
        {
            builder.ToTable("PreventiveSchedulerDtl", "Maintenance");
            builder.HasKey(t => t.Id);
            builder.Property(t => t.PreventiveSchedulerId)
                .HasColumnName("PreventiveSchedulerId")
                .HasColumnType("int")
                .IsRequired();

            builder.Property(t => t.MachineId)
                .HasColumnName("MachineId")
                .HasColumnType("int")
                .IsRequired();

            builder.Property(t => t.StartDate)
                .HasColumnName("StartDate")
                .HasColumnType("date")
                .IsRequired();

            builder.Property(t => t.NextDueDate)
                .HasColumnName("NextDueDate")
                .HasColumnType("date")
                .IsRequired();

                builder.HasOne(t => t.PreventiveScheduler)
                .WithMany(t => t.PreventiveSchedulerDtls)
                .HasForeignKey(t => t.PreventiveSchedulerId)
                .OnDelete(DeleteBehavior.Restrict);

                 builder.HasOne(t => t.Machine)
                .WithMany(t => t.PreventiveSchedulerDtls)
                .HasForeignKey(t => t.MachineId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}