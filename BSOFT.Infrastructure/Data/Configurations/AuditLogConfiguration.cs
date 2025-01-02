using Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BSOFT.Infrastructure.Data.Configurations
{
    public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLogs>
    {
        public void Configure(EntityTypeBuilder<AuditLogs> builder)
        {
            builder.ToTable("AuditLog");  // Set the table name in schema
            builder.HasKey(a => a.Id);  // Set the primary key for the table

            builder.Property(a => a.Id)
                .HasColumnName("Id")
                .HasColumnType("int") // MongoDB typically uses 24-char ObjectId as string
                .IsRequired();

            builder.Property(a => a.CreatedAt)
                .HasColumnName("CreatedAt")
                .HasColumnType("datetime")
                .IsRequired();

            builder.Property(a => a.UserId)
                .HasColumnName("UserId")
                .HasColumnType("int")
                .IsRequired();

            builder.Property(a => a.UserName)
                .HasColumnName("UserName")
                .HasColumnType("varchar(50)")
                .IsRequired();

            builder.Property(a => a.IPAddress)
                .HasColumnName("IPAddress")
                .HasColumnType("varchar(25)")
                .IsRequired();

            builder.Property(a => a.OS)
                .HasColumnName("OS")
                .HasColumnType("varchar(50)")
                .IsRequired();

            builder.Property(a => a.Browser)
                .HasColumnName("Browser")
                .HasColumnType("varchar(50)")
                .IsRequired();

            builder.Property(a => a.Action)
                .HasColumnName("Action")
                .HasColumnType("varchar(255)")
                .IsRequired();

            builder.Property(a => a.Details)
                .HasColumnName("Details")
                .HasColumnType("varchar(2000)");

            builder.Property(a => a.Module)
                .HasColumnName("Module")
                .HasColumnType("varchar(50)");       
        }
    }
}
