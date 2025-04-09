
using Core.Domain.Entities.WorkOrderMaster;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using static Core.Domain.Common.BaseEntity;

namespace MaintenanceManagement.Infrastructure.Data.Configurations.WorkOrderMaster
{
    public class WorkOrderActivityConfiguration   : IEntityTypeConfiguration<WorkOrderActivity>
    {       
       public void Configure(EntityTypeBuilder<WorkOrderActivity> builder)
        { 
            var statusConverter = new ValueConverter<Status, bool>(
                v => v == Status.Active,                   
                v => v ? Status.Active : Status.Inactive   
            );
            var isDeleteConverter = new ValueConverter<IsDelete, bool>(
                v => v == IsDelete.Deleted,                 
                v => v ? IsDelete.Deleted : IsDelete.NotDeleted 
            );
            
            builder.ToTable("WorkOrderActivity", "Maintenance");

            builder.HasKey(t => t.Id);
            builder.Property(t => t.Id)
                .HasColumnName("Id")
                .HasColumnType("int")
                .IsRequired();

            builder.Property(t => t.WorkOrderId)
                .HasColumnName("WorkOrderId")
                .HasColumnType("int")
                .IsRequired();
            builder.HasOne(amg => amg.WOActivity)
                .WithMany(am => am.Activity)
                .HasForeignKey(amg => amg.WorkOrderId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(t => t.ActivityId)
                .HasColumnName("ActivityId")
                .HasColumnType("int")
                .IsRequired();  
             builder.HasOne(amg => amg.ActivityMaster)
                .WithMany(am => am.workOrderActivities)
                .HasForeignKey(amg => amg.ActivityId)
                .OnDelete(DeleteBehavior.Cascade);    
             
            builder.Property(t => t.EstimatedTime)
                .HasColumnName("EstimatedTime")
                .HasColumnType("decimal(5,2)")
                .IsRequired();   
            
            builder.Property(t => t.Description)
                .HasColumnName("Description")
                .HasColumnType("varchar(250)")
                .IsRequired();   

             builder.Property(b => b.IsActive)
                .HasColumnName("IsActive")
                .HasColumnType("bit")
                .HasConversion(statusConverter)
                .IsRequired();

            builder.Property(b => b.IsDeleted)
                .HasColumnName("IsDeleted")
                .HasColumnType("bit")
                .HasConversion(isDeleteConverter)
                .IsRequired();

            builder.Property(b => b.CreatedByName)
                .IsRequired()
                .HasColumnType("varchar(50)");
                    
            builder.Property(b => b.CreatedIP)
                .IsRequired()
                .HasColumnType("varchar(255)");

            builder.Property(b => b.ModifiedByName)
                .HasColumnType("varchar(50)");

            builder.Property(b => b.ModifiedIP)
                .HasColumnType("varchar(255)");                        
        }
    }
}