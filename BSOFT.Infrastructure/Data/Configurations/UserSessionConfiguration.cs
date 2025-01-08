using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Core.Domain.Entities;

namespace BSOFT.Infrastructure.Data.Configurations
{
    public class UserSessionConfiguration : IEntityTypeConfiguration<UserSession>
    {
        public void Configure(EntityTypeBuilder<UserSession> builder)
        {
        builder.ToTable("UserSession", "AppSecurity");

        builder.HasKey(ua => ua.Id);

        builder.Property(ua => ua.Id)
            .HasColumnName("Id")
            .HasColumnType("int")
            .IsRequired();

        builder.Property(ua => ua.UserId)
            .HasColumnName("UserId")
            .HasColumnType("int")
            .IsRequired();

        builder.Property(ua => ua.UserName)
            .HasColumnName("UserName")
            .HasColumnType("varchar(50)")
            .IsRequired();

        builder.Property(ua => ua.SessionId)
            .HasColumnName("SessionId")
            .HasColumnType("nvarchar(-1)")
            .IsRequired();

        builder.Property(ua => ua.Token)
            .HasColumnName("Token")
            .HasColumnType("nvarchar(-1)")
            .IsRequired();

        builder.Property(ua => ua.CreatedAt)
            .HasColumnName("CreatedAt")
            .HasColumnType("datetime")
            .IsRequired();
        builder.Property(u => u.IsActive)
            .HasColumnName("IsActive")
            .HasColumnType("bit")
            .HasConversion(
            v => v == 1, // convert byte to bool
            v => v ? (byte)1 : (byte)0 // convert bool to byte
                )
            .IsRequired();

        builder.Property(ua => ua.Browser )
            .HasColumnName("Browser")
            .HasColumnType("varchar(250)");
           

        builder.Property(ua => ua.CreatedIP)
            .HasColumnName("CreatedIP")
            .HasColumnType("varchar(50)")
            .IsRequired();

        builder.Property(ua => ua.Status)
            .HasColumnName("Status")
            .HasColumnType("varchar(50)")
            .IsRequired();
        }
    }
    
}

