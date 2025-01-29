using Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace BSOFT.Infrastructure.Data.Configurations
{
    public class TimeZonesConfiguration : IEntityTypeConfiguration<TimeZones>
    {
        public void Configure(EntityTypeBuilder<TimeZones> builder)
        {
            builder.ToTable("TimeZones", "AppData");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Id)
            .HasColumnName("Id")
            .HasColumnType("int")
            .IsRequired();

         // Code field configuration
            builder.Property(u => u.Code)
                .HasColumnName("Code")
                .HasColumnType("varchar(20)") // Adjust length as per requirements
                .IsRequired()
                .HasDefaultValueSql("CONCAT('TZ-', LEFT(NEWID(), 8))") // Generate unique alphanumeric Code
                .ValueGeneratedOnAdd(); 

        builder.Property(u => u.Name)
            .HasColumnName("Name")
            .HasColumnType("varchar(50)")
            .IsRequired();

         builder.Property(u => u.IsActive)
            .HasColumnName("IsActive")
            .HasColumnType("bit")
            .HasConversion(
        v => v == 1, // convert byte to bool
        v => v ? (byte)1 : (byte)0 // convert bool to byte
    )
    .IsRequired();
        }
    }
}