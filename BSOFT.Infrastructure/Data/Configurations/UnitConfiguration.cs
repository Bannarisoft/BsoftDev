using Core.Domain.Entities;
using Core.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BSOFT.Infrastructure.Data.Configurations
{
    public class UnitConfiguration : IEntityTypeConfiguration<Unit>
    {
        public void Configure(EntityTypeBuilder<Unit> builder)
        {
        builder.ToTable("Unit", "AppData");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Id)
            .HasColumnName("Id")
            .HasColumnType("int")
            .IsRequired();

        builder.Property(u => u.UnitName)
            .HasColumnName("UnitName")
            .HasColumnType("varchar(50)")
            .IsRequired();

        builder.Property(u => u.ShortName)
            .HasColumnName("ShortName")
            .HasColumnType("varchar(10)")
            .IsRequired();

        builder.Property(u => u.CompanyId)
            .HasColumnName("CompanyId")
            .HasColumnType("int")
            .IsRequired();

        builder.Property(u => u.DivisionId)
            .HasColumnName("DivisionId")
            .HasColumnType("int")
            .IsRequired();

        builder.Property(u => u.UnitHeadName)
            .HasColumnName("UnitHeadName")
            .HasColumnType("varchar(50)")
            .IsRequired();

        builder.Property(u => u.CINNO)
            .HasColumnName("CINNO")
            .HasColumnType("varchar(50)")
            .IsRequired();

           builder.Property(c => c.IsActive)
                .HasColumnName("IsActive")
                .HasColumnType("bit")
                .HasConversion(
                        v => v == 1, // convert byte to bool
                        v => v ? (byte)1 : (byte)0 // convert bool to byte
                    )
                .IsRequired();
       
        builder.HasOne(u => u.UnitAddress)
            .WithOne(ua => ua.Unit)
            .HasForeignKey<UnitAddress>(ua => ua.UnitId);

        builder.HasOne(u => u.UnitContacts)
            .WithOne(uc => uc.Unit)
            .HasForeignKey<UnitContacts>(uc => uc.UnitId);
        }
    }
}