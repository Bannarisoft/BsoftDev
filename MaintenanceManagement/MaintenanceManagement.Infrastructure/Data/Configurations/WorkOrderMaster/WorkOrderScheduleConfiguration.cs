using Core.Domain.Entities.WorkOrderMaster;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using static Core.Domain.Common.BaseEntity;

namespace MaintenanceManagement.Infrastructure.Data.Configurations.WorkOrderMaster
{
    public class WorkOrderScheduleConfiguration : IEntityTypeConfiguration<WorkOrderSchedule>
    {       
       public void Configure(EntityTypeBuilder<WorkOrderSchedule> builder)
        { 
            var statusConverter = new ValueConverter<Status, bool>(
                v => v == Status.Active,                   
                v => v ? Status.Active : Status.Inactive   
            );

                
            var isDeleteConverter = new ValueConverter<IsDelete, bool>(
                v => v == IsDelete.Deleted,                 
                v => v ? IsDelete.Deleted : IsDelete.NotDeleted 
            );
            
            builder.ToTable("WorkOrderSchedule", "Maintenance");

            builder.HasKey(t => t.Id);
            builder.Property(t => t.Id)
                .HasColumnName("Id")
                .HasColumnType("int")
                .IsRequired();

            builder.Property(t => t.WorkOrderId)
                .HasColumnName("WorkOrderId")
                .HasColumnType("int")
                .IsRequired();
            builder.HasOne(amg => amg.WOSchedule)
                .WithMany(am => am.Schedule)
                .HasForeignKey(amg => amg.WorkOrderId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(t => t.RepairStartTime)
                .HasColumnName("RepairStartTime")                
                .IsRequired()
                .HasConversion(v => v.ToTimeSpan(), v => TimeOnly.FromTimeSpan(v));     
            
            builder.Property(t => t.RepairEndTime)
                .HasColumnName("RepairEndTime")                
                .IsRequired()
                .HasConversion(v => v.ToTimeSpan(), v => TimeOnly.FromTimeSpan(v));     
               
            builder.Property(t => t.DownTimeStartTime)
                .HasColumnName("DownTimeStartTime")
                .IsRequired(false)
                .HasConversion(
                    v => v.HasValue ? v.Value.ToTimeSpan() : (TimeSpan?)null,
                    v => v.HasValue ? TimeOnly.FromTimeSpan(v.Value) : (TimeOnly?)null
                );

            builder.Property(t => t.DownTimeEndTime)
                .HasColumnName("DownTimeEndTime")
                .IsRequired(false)
                .HasConversion(
                    v => v.HasValue ? v.Value.ToTimeSpan() : (TimeSpan?)null,
                    v => v.HasValue ? TimeOnly.FromTimeSpan(v.Value) : (TimeOnly?)null
                ); 

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