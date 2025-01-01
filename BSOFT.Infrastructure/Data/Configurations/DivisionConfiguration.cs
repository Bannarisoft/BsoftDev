using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BSOFT.Infrastructure.Data.Configurations
{
    public class DivisionConfiguration : IEntityTypeConfiguration<Division>
    {
        public void Configure(EntityTypeBuilder<Division> builder)
        {
            builder.ToTable("Division", "AppData");
            builder.HasKey(d => d.Id);

            builder.Property(d => d.ShortName)
            .IsRequired()
            .HasColumnType("varchar(50)");

            builder.Property(d => d.Name)
            .IsRequired()
            .HasColumnType("varchar(100)");

            builder.Property(d => d.CompanyId)
            .IsRequired()
            .HasColumnType("int");

            builder.Property(u => u.IsActive)
            .HasColumnName("IsActive")
            .HasColumnType("bit")
            .HasConversion(
                v => v == 1, 
                v => v ? (byte)1 : (byte)0 
            )
            .IsRequired();
        }
    }
}