using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BSOFT.Infrastructure.Data.Configurations
{
    public class CompanyConfiguration : IEntityTypeConfiguration<Company>
    {
        public void Configure(EntityTypeBuilder<Company> builder)
        {
            builder.ToTable("Company", "AppData");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Id)
                .HasColumnName("Id")
                .HasColumnType("int")
                .IsRequired();

            builder.Property(c => c.CompanyName)
                .HasColumnName("CompanyName")
                .HasColumnType("varchar(50)")
                .IsRequired();

            builder.Property(c => c.LegalName)
                .HasColumnName("LegalName")
                .HasColumnType("varchar(50)")
                .IsRequired();

            builder.Property(c => c.GstNumber)
                .HasColumnName("GstNumber")
                .HasColumnType("varchar(50)")
                .IsRequired();

            builder.Property(c => c.TIN)
                .HasColumnName("TIN")
                .HasColumnType("varchar(50)");

            builder.Property(c => c.TAN)
                .HasColumnName("TAN")
                .HasColumnType("varchar(50)");

            builder.Property(c => c.CSTNo)
                .HasColumnName("CSTNo")
                .HasColumnType("varchar(50)");

            builder.Property(c => c.YearOfEstablishment)
                .HasColumnName("YearOfEstablishment")
                .HasColumnType("int")
                .IsRequired();

            builder.Property(c => c.Website)
                .HasColumnName("Website")
                .HasColumnType("varchar(50)")
                .IsRequired();

            builder.Property(ca => ca.Logo)
                .HasColumnName("Logo")
                .HasColumnType("nvarchar(255)");

            builder.Property(c => c.EntityId)
                .HasColumnName("EntityId")
                .HasColumnType("int")
                .IsRequired();

            builder.Property(c => c.IsActive)
                .HasColumnName("IsActive")
                .HasColumnType("bit")
                .HasConversion(
        v => v == 1, // convert byte to bool
        v => v ? (byte)1 : (byte)0 // convert bool to byte
    )
                .IsRequired();

            builder.HasMany(c => c.CompanyAddress)
                .WithOne(ca => ca.Company)
                .HasForeignKey(ca => ca.CompanyId);

            builder.HasMany(c => c.CompanyContact)
                .WithOne(cc => cc.Company)
                .HasForeignKey(cc => cc.CompanyId);
        }
    }
}