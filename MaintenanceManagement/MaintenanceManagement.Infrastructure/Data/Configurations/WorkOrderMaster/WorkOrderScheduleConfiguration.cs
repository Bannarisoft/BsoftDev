using Core.Domain.Entities.WorkOrderMaster;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MaintenanceManagement.Infrastructure.Data.Configurations.WorkOrderMaster
{
    public class WorkOrderScheduleConfiguration : IEntityTypeConfiguration<WorkOrderSchedule>
    {       
       public void Configure(EntityTypeBuilder<WorkOrderSchedule> builder)
        {             
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
                .HasColumnType("DateTimeOffset")
                .IsRequired();   
            
            builder.Property(t => t.RepairEndTime)
                .HasColumnName("RepairEndTime")                
                .IsRequired()
                .HasColumnType("DateTimeOffset");           
        }
    }
}