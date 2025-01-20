using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BSOFT.Infrastructure.Data.Configurations
{
    public class PwdComplexityRuleConfiguration : IEntityTypeConfiguration<PasswordComplexityRule>
    {

         public void Configure(EntityTypeBuilder<PasswordComplexityRule> builder)
        {
         builder.ToTable("PasswordComplexityRule", "AppSecurity");
           builder.HasKey(p => p.Id);

        builder.Property(p => p.Id).IsRequired();

            builder.Property(p => p.PwdComplexityRule)
            .HasColumnName("PwdComplexityRule")
            .HasColumnType("varchar(150)")
            .IsRequired();

        builder.Property(p => p.IsActive)
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