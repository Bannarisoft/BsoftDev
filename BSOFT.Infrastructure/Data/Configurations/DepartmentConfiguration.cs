using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Core.Domain.Entities;

namespace BSOFT.Infrastructure.Data.Configurations
{
    public class DepartmentConfiguration :IEntityTypeConfiguration<Department>
    {
         public void Configure(EntityTypeBuilder<Department> builder)
        {
              builder.ToTable("Department", "AppData");

            builder.HasKey(u => u.Id);

              builder.Property(u => u.Id)
                        .HasColumnName("Id")
                        .HasColumnType("int")
                        .IsRequired();

             
             builder.Property(u => u.ShortName)
            .HasColumnName("ShortName")
            .HasColumnType("varchar(10)")
            .IsRequired();

             builder.Property(u => u.DeptName)
            .HasColumnName("DeptName")
            .HasColumnType("varchar(50)")
            .IsRequired();

              builder.Property(u => u.IsActive)                   
            .HasConversion(
                v => v == 1, // convert byte to bool
                v => v ? (byte)1 : (byte)0 // convert bool to byte
            )
            .HasColumnType("bit")
            .HasColumnName("IsActive")     
            .IsRequired();

            

           



        }


    }
}