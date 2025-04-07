using Core.Domain.Entities.WorkOrderMaster;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using static Core.Domain.Common.BaseEntity;

namespace MaintenanceManagement.Infrastructure.Data.Configurations.WorkOrderMaster
{
    public class WorkOrderItemConfiguration  : IEntityTypeConfiguration<WorkOrderItem>
    {       
       public void Configure(EntityTypeBuilder<WorkOrderItem> builder)
        { 
            var statusConverter = new ValueConverter<Status, bool>(
                v => v == Status.Active,                   
                v => v ? Status.Active : Status.Inactive   
            );
            var isDeleteConverter = new ValueConverter<IsDelete, bool>(
                v => v == IsDelete.Deleted,                 
                v => v ? IsDelete.Deleted : IsDelete.NotDeleted 
            );
            
            builder.ToTable("WorkOrderItem", "Maintenance");

            builder.HasKey(t => t.Id);
            builder.Property(t => t.Id)
                .HasColumnName("Id")
                .HasColumnType("int")
                .IsRequired();

            builder.Property(t => t.WorkOrderId)
                .HasColumnName("WorkOrderId")
                .HasColumnType("int")
                .IsRequired();
            builder.HasOne(amg => amg.WOItem)
                .WithMany(am => am.Item)
                .HasForeignKey(amg => amg.WorkOrderId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(t => t.DepartmentId)
                .HasColumnName("DepartmentId")
                .HasColumnType("int")
                .IsRequired();  
            
            builder.Property(t => t.ItemCode)
                .HasColumnName("ItemCode")
                .HasColumnType("nvarchar(20))")
                .IsRequired();
            
            builder.Property(t => t.ItemName)
                .HasColumnName("ItemName")
                .HasColumnType("varchar(100))")
                .IsRequired();

            builder.Property(t => t.AvailableQty)
                .HasColumnName("AvailableQty")
                .HasColumnType("decimal(10,2)")
                .IsRequired();

            builder.Property(t => t.UsedQty)
                .HasColumnName("UsedQty")
                .HasColumnType("decimal(10,2)")
                .IsRequired();       
            
            builder.Property(t => t.Image)
                .HasColumnName("Image")
                .HasColumnType("varchar(250)")
                .IsRequired(false);

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