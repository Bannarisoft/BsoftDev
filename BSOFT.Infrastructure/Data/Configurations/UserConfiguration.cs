using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Core.Domain.Entities;

namespace BSOFT.Infrastructure.Data.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users", "AppSecurity");

            builder.HasKey(u => u.UserId);

            builder.Property(u => u.Id)
                .HasColumnName("Id")
                .HasColumnType("uniqueidentifier")
                .IsRequired()
                .HasDefaultValueSql("NEWID()");

            builder.Property(u => u.UserId)
                .HasColumnName("UserId")
                .HasColumnType("int")
                .IsRequired()
                .ValueGeneratedOnAdd();

            builder.Property(u => u.FirstName)
                .HasColumnName("FirstName")
                .HasColumnType("varchar(50)")
                .IsRequired();

            builder.Property(u => u.LastName)
                .HasColumnName("LastName")
                .HasColumnType("varchar(50)")
                .IsRequired();

            builder.Property(u => u.UserName)
                .HasColumnName("UserName")
                .HasColumnType("varchar(50)")
                .IsRequired();

            builder.Property(u => u.IsActive)
                .HasColumnName("IsActive")
                .HasColumnType("bit")
                // .HasConversion(
                //     v => v == 1, // convert byte to bool
                //     v => v ? (byte)1 : (byte)0 // convert bool to byte
                // )
                .IsRequired();

            builder.Property(u => u.IsFirstTimeUser)
                .HasColumnName("IsFirstTimeUser")
                .HasColumnType("bit")
                // .HasConversion(
                //     v => v == 1, // convert byte to bool
                //     v => v ? (byte)1 : (byte)0 // convert bool to byte
                // )
                .IsRequired();

            builder.Property(u => u.PasswordHash)
                .HasColumnName("PasswordHash")
                .HasColumnType("varchar(255)")
                .IsRequired();

            builder.Property(u => u.UserType)
                .HasColumnName("UserType")
                .HasColumnType("int")
                .IsRequired();

            builder.Property(u => u.Mobile)
                .HasColumnName("Mobile")
                .HasColumnType("varchar(20)")
                .IsRequired();

            builder.Property(u => u.EmailId)
                .HasColumnName("EmailId")
                .HasColumnType("varchar(100)")
                .IsRequired();

            builder.Property(u => u.CompanyId)
                .HasColumnName("CompanyId")
                .HasColumnType("int")
                .IsRequired();

            builder.Property(u => u.UnitId)
                .HasColumnName("UnitId")
                .HasColumnType("int")
                .IsRequired();

            builder.Property(u => u.DivisionId)
                .HasColumnName("DivisionId")
                .HasColumnType("int")
                .IsRequired();

            builder.Property(u => u.UserRoleId)
                .HasColumnName("UserRoleId")
                .HasColumnType("int")
                .IsRequired();

            // builder.HasOne(u => u.UserRole)
            //     .WithMany(ur => ur.Users)
            //     .HasForeignKey(u => u.UserRoleId)
            //     .OnDelete(DeleteBehavior.Cascade);

                 builder.HasMany(u => u.UserRoles)
               .WithOne(ur => ur.User)
               .HasForeignKey(ur => ur.UserId)
               .OnDelete(DeleteBehavior.Cascade);
        }
    }
}