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
                .HasColumnType("smallint")
                .IsRequired();  
            
            builder.Property(t => t.ItemId)
                .HasColumnName("ItemId")
                .HasColumnType("smallint")
                .IsRequired(false);
            
            builder.Property(t => t.OldItemId)
                .HasColumnName("OldItemId")
                .HasColumnType("smallint")
                .IsRequired(false);

            builder.Property(t => t.ItemName)
                .HasColumnName("ItemName")
                .HasColumnType("varchar(250))")
                .IsRequired(false);

             builder.Property(t => t.SourceId)
                .HasColumnName("SourceId")
                .HasColumnType("smallint)")
                .IsRequired(false);

            builder.Property(t => t.AvailableQty)
                .HasColumnName("AvailableQty")
                .HasColumnType("int")
                .IsRequired();

            builder.Property(t => t.UsedQty)
                .HasColumnName("UsedQty")
                .HasColumnType("int")
                .IsRequired();       
            
            builder.Property(t => t.Image)
                .HasColumnName("Image")
                .HasColumnType("varchar(250)")
                .IsRequired(false);                    
        }
    }
}