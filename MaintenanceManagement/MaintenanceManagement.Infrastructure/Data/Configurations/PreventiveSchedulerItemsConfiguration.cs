using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MaintenanceManagement.Infrastructure.Data.Configurations
{
    public class PreventiveSchedulerItemsConfiguration : IEntityTypeConfiguration<PreventiveSchedulerItems>
    {
        public void Configure(EntityTypeBuilder<PreventiveSchedulerItems> builder)
        {
            builder.ToTable("PreventiveSchedulerItems", "Maintenance");
            builder.HasKey(t => t.Id);
             builder.Property(t => t.PreventiveSchedulerHeaderId)
                .HasColumnName("PreventiveSchedulerHeaderId")
                .HasColumnType("int")
                .IsRequired();

            builder.Property(t => t.ItemId)
                .HasColumnName("ItemId")
                .HasColumnType("int");
            builder.Property(t => t.RequiredQty)
                .HasColumnName("RequiredQty")
                .HasColumnType("int")
                .IsRequired();
            builder.Property(t => t.SourceId)
                .HasColumnName("SourceId")
                .HasColumnType("int");
            builder.Property(t => t.OldItemId)
                .HasColumnName("OldItemId")
                .HasColumnType("varchar(50)");
                  builder.Property(t => t.OldCategoryDescription)
                .HasColumnName("OldCategoryDescription")
                .HasColumnType("varchar(50)");
                  builder.Property(t => t.OldGroupName)
                .HasColumnName("OldGroupName")
                .HasColumnType("varchar(50)");

                builder.HasOne(t => t.PreventiveScheduler)
                .WithMany(t => t.PreventiveSchedulerItems)
                .HasForeignKey(t => t.PreventiveSchedulerHeaderId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}