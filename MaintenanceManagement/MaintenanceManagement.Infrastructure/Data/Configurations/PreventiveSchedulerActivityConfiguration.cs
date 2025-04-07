using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MaintenanceManagement.Infrastructure.Data.Configurations
{
    public class PreventiveSchedulerActivityConfiguration : IEntityTypeConfiguration<PreventiveSchedulerActivity>
    {
        public void Configure(EntityTypeBuilder<PreventiveSchedulerActivity> builder)
        {
            builder.ToTable("PreventiveSchedulerActivity", "Maintenance");
            builder.HasKey(t => t.Id);
            builder.Property(t => t.PreventiveSchedulerHdrId)
                .HasColumnName("PreventiveSchedulerHdrId")
                .HasColumnType("int")
                .IsRequired();

            builder.Property(t => t.ActivityId)
                .HasColumnName("ActivityId")
                .HasColumnType("int")
                .IsRequired();

            builder.Property(t => t.EstimatedTimeHrs)
                .HasColumnName("EstimatedTimeHrs")
                .HasColumnType("decimal(18,2)")
                .IsRequired();
            builder.Property(t => t.Description)
                .HasColumnName("Description")
                .HasColumnType("varchar(250)");

                builder.HasOne(t => t.PreventiveScheduler)
                .WithMany(t => t.PreventiveSchedulerActivities)
                .HasForeignKey(t => t.PreventiveSchedulerHdrId)
                .OnDelete(DeleteBehavior.Cascade);

                builder.HasOne(t => t.Activity)
                .WithMany(t => t.PreventiveSchedulerActivities)
                .HasForeignKey(t => t.ActivityId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}