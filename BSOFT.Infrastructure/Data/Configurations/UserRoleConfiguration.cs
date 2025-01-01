using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Core.Domain.Entities;

namespace BSOFT.Infrastructure.Data.Configurations
{
    public class UserRoleConfiguration :IEntityTypeConfiguration<UserRole>
    {
         public void Configure(EntityTypeBuilder<UserRole> builder)
        {
               builder.ToTable("UserRole", "AppSecurity");

            builder.HasKey(u => u.Id);

              builder.Property(u => u.Id)
                        .HasColumnName("Id")
                        .HasColumnType("int")
                        .IsRequired();

             builder.Property(u => u.RoleName)
            .HasColumnName("RoleName")
            .HasColumnType("varchar(50)")
            .IsRequired();

             builder.Property(u => u.Description)
            .HasColumnName("Description")
            .HasColumnType("varchar(250)")
            .IsRequired();

              builder.Property(u => u.CompanyId)
            .HasColumnName("CompanyId")
            .HasColumnType("int")
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