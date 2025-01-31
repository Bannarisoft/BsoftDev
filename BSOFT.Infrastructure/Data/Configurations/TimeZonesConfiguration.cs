using Core.Domain.Entities;
using Core.Domain.Enums;
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
                .IsRequired();

        builder.Property(u => u.Name)
            .HasColumnName("Name")
            .HasColumnType("varchar(50)")
            .IsRequired();

        builder.Property(u => u.Offset)
            .HasColumnName("Offset")
            .HasColumnType("varchar(20)")
            .IsRequired();

        builder.Property(u => u.Location)
            .HasColumnName("Location")
            .HasColumnType("varchar(100)")
            .IsRequired();

         builder.Property(u => u.IsActive)
            .HasColumnName("IsActive")
            .HasColumnType("bit")
            .HasConversion(
             v => v == TimeZonesEnum.TimeZonesStatus.Active, // convert enum to bool
             v => v ? TimeZonesEnum.TimeZonesStatus.Active : TimeZonesEnum.TimeZonesStatus.Inactive // convert bool to enum
             )
            .IsRequired();
        builder.Property(u => u.IsDeleted)
            .HasColumnName("IsDeleted")
            .HasColumnType("bit")
            .HasConversion(
             v => v == TimeZonesEnum.TimeZonesDelete.Deleted, // convert enum to bool
             v => v ? TimeZonesEnum.TimeZonesDelete.Deleted : TimeZonesEnum.TimeZonesDelete.NotDeleted // convert bool to enum
             )
            .IsRequired();
        }
    }
}