using Core.Domain.Entities;
using Core.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BSOFT.Infrastructure.Data.Configurations
{
    public class CurrencyConfiguration : IEntityTypeConfiguration<Core.Domain.Entities.Currency>
    {
        public void Configure(EntityTypeBuilder<Currency> builder)
        {
            builder.ToTable("Currency", "AppData");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Id)
            .HasColumnName("Id")
            .HasColumnType("int")
            .IsRequired();

        builder.Property(u => u.Code)
            .HasColumnName("Code")
            .HasColumnType("varchar(6)")
            .IsRequired();

        builder.Property(u => u.Name)
            .HasColumnName("Name")
            .HasColumnType("varchar(50)")
            .IsRequired();

         builder.Property(u => u.IsActive)
            .HasColumnName("IsActive")
            .HasColumnType("bit")
            .HasConversion(
             v => v == CurrencyEnum.CurrencyStatus.Active, // convert enum to bool
             v => v ? CurrencyEnum.CurrencyStatus.Active : CurrencyEnum.CurrencyStatus.Inactive // convert bool to enum
             )
            .IsRequired();
        builder.Property(u => u.IsDeleted)
            .HasColumnName("IsDeleted")
            .HasColumnType("bit")
            .HasConversion(
             v => v == CurrencyEnum.CurrencyDelete.Deleted, // convert enum to bool
             v => v ? CurrencyEnum.CurrencyDelete.Deleted : CurrencyEnum.CurrencyDelete.NotDeleted // convert bool to enum
             )
            .IsRequired();
        }
    }
}