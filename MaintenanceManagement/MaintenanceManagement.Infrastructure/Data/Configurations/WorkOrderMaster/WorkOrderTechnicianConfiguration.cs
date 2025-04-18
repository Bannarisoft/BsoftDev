
using Core.Domain.Entities.WorkOrderMaster;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace MaintenanceManagement.Infrastructure.Data.Configurations.WorkOrderMaster
{
    public class WorkOrderTechnicianConfiguration  : IEntityTypeConfiguration<WorkOrderTechnician>
    {       
       public void Configure(EntityTypeBuilder<WorkOrderTechnician> builder)
        {            
            builder.ToTable("WorkOrderTechnician", "Maintenance");

            builder.HasKey(t => t.Id);
            builder.Property(t => t.Id)
                .HasColumnName("Id")
                .HasColumnType("int")
                .IsRequired();

            builder.Property(t => t.WorkOrderId)
                .HasColumnName("WorkOrderId")
                .HasColumnType("int")
                .IsRequired();
            builder.HasOne(amg => amg.WOTechnician)
                .WithMany(am => am.Technicians)
                .HasForeignKey(amg => amg.WorkOrderId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(t => t.TechnicianId)
                .HasColumnName("TechnicianId")
                .HasColumnType("smallint")
                .IsRequired();  
            
             builder.Property(t => t.OldTechnicianId)
                .HasColumnName("OldTechnicianId")
                .HasColumnType("smallint")
                .IsRequired();
            
             builder.Property(t => t.SourceId)
                .HasColumnName("SourceId")
                .HasColumnType("smallint")
                .IsRequired();

            builder.Property(t => t.TechnicianName)
                .HasColumnName("TechnicianName")
                .HasColumnType("varchar(100)")
                .IsRequired();  
            
            builder.Property(t => t.HoursSpent)
                .HasColumnName("HoursSpent")
                .HasColumnType("decimal(10,2)")
                .IsRequired();                     
          
        }
    }
}