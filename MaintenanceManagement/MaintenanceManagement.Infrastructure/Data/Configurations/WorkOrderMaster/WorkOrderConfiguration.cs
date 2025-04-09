using Core.Domain.Entities.WorkOrderMaster;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using static Core.Domain.Common.BaseEntity;

namespace MaintenanceManagement.Infrastructure.Data.Configurations.WorkOrderMaster
{
    public class WorkOrderConfiguration  : IEntityTypeConfiguration<WorkOrder>
    {       
       public void Configure(EntityTypeBuilder<WorkOrder> builder)
        { 
            var statusConverter = new ValueConverter<Status, bool>(
                v => v == Status.Active,                   
                v => v ? Status.Active : Status.Inactive   
            );
            var isDeleteConverter = new ValueConverter<IsDelete, bool>(
                v => v == IsDelete.Deleted,                 
                v => v ? IsDelete.Deleted : IsDelete.NotDeleted 
            );
            
            builder.ToTable("WorkOrder", "Maintenance");

            builder.HasKey(t => t.Id);
            builder.Property(t => t.Id)
                .HasColumnName("Id")
                .HasColumnType("int")
                .IsRequired();
            builder.Property(t => t.CompanyId)
                .HasColumnName("CompanyId")
                .HasColumnType("int")
                .IsRequired();
            builder.Property(t => t.UnitId)
                .HasColumnName("UnitId")
                .HasColumnType("int")
                .IsRequired();

            builder.Property(t => t.WorkOrderTypeId)
                .HasColumnName("WorkOrderTypeId")
                .HasColumnType("int")
                .IsRequired();
            builder.HasOne(amg => amg.CategoryType)
                .WithMany(am => am.WorkOrderType)
                .HasForeignKey(amg => amg.WorkOrderTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(t => t.RequestId)
                .HasColumnName("RequestId")
                .HasColumnType("varchar(25)")
                .IsRequired();  
            
             builder.Property(t => t.RequestTypeId)
                .HasColumnName("RequestTypeId")
                .HasColumnType("int")
                .IsRequired();   
            builder.HasOne(amg => amg.MiscRequestType)
                .WithMany(mg => mg.WorkOrderRequestType)
                .HasForeignKey(amg => amg.RequestTypeId)
                .OnDelete(DeleteBehavior.Restrict);
                       
             builder.Property(t => t.MachineCode)
                .HasColumnName("MachineCode")
                .HasColumnType("varchar(20)")
                .IsRequired(false);   
            
            builder.Property(t => t.PriorityId)
                .HasColumnName("PriorityId")
                .HasColumnType("int")
                .IsRequired();   
            builder.HasOne(amg => amg.MiscPriority)
                .WithMany(mg => mg.WorkOrderPriority)
                .HasForeignKey(amg => amg.PriorityId)
                .OnDelete(DeleteBehavior.Restrict);
               
            builder.Property(t => t.Remarks)
                .HasColumnName("Remarks")
                .HasColumnType("varchar(1000)")
                .IsRequired(false);     

            builder.Property(t => t.Image)
                .HasColumnName("Image")
                .HasColumnType("varchar(250)")
                .IsRequired(false);

            builder.Property(t => t.StatusId)
                .HasColumnName("StatusId")
                .HasColumnType("int")
                .IsRequired();           
            builder.HasOne(amg => amg.MiscStatus)
                .WithMany(mg => mg.WorkOrderStatus)
                .HasForeignKey(amg => amg.StatusId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(t => t.VendorId)
                .HasColumnName("VendorId")
                .HasColumnType("int")
                .IsRequired(false); 
            builder.Property(t => t.OldVendorId)
                .HasColumnName("OldVendorId")
                .HasColumnType("nvarchar(20)")
                .IsRequired(false); 
            builder.Property(t => t.VendorName)
                .HasColumnName("VendorName")
                .HasColumnType("nvarchar(250)")
                .IsRequired(false); 
            
              builder.Property(t => t.RootCauseId)
                .HasColumnName("RootCauseId")
                .HasColumnType("int")
                .IsRequired();     
             builder.HasOne(amg => amg.MiscRootCause)
                .WithMany(am => am.WorkOrderRootCause)
                .HasForeignKey(amg => amg.RootCauseId)
                .OnDelete(DeleteBehavior.Restrict);    


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