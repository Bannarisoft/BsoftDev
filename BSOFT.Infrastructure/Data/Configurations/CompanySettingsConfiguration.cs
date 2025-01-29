using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BSOFT.Infrastructure.Data.Configurations
{
    public class CompanySettingsConfiguration : IEntityTypeConfiguration<CompanySettings>
    {
        public void Configure(EntityTypeBuilder<CompanySettings> builder)
        {
            builder.ToTable("CompanySetting", "AppData");
            builder.HasKey(m => m.Id);

            builder.Property(c => c.Id)
                .HasColumnName("Id")
                .HasColumnType("int")
                .ValueGeneratedOnAdd()
                .IsRequired();

            builder.Property(c => c.CompanyId)
                .HasColumnName("CompanyId")
                .HasColumnType("int")
                .IsRequired();

            builder.Property(c => c.PasswordHistoryCount)
                .HasColumnName("PasswordHistoryCount")
                .HasColumnType("int")
                .IsRequired();

            builder.Property(c => c.SessionTimeout)
                .HasColumnName("SessionTimeout")
                .HasColumnType("int")
                .IsRequired();

            builder.Property(c => c.FailedLoginAttempts)
                .HasColumnName("FailedLoginAttempts")
                .HasColumnType("int")
                .IsRequired();

            builder.Property(c => c.AutoReleaseTime)
                .HasColumnName("AutoReleaseTime")
                .HasColumnType("int")
                .IsRequired();

            builder.Property(c => c.PasswordExpiryDays)
                .HasColumnName("PasswordExpiryDays")
                .HasColumnType("int")
                .IsRequired();

            builder.Property(c => c.PasswordExpiryAlert)
                .HasColumnName("PasswordExpiryAlert")
                .HasColumnType("int")
                .IsRequired();

            builder.Property(c => c.TwoFactorAuth)
                .HasColumnName("TwoFactorAuth")
                .HasColumnType("bit")
                .HasConversion(
                    v => v == 1, 
                    v => v ? (byte)1 : (byte)0 
                )
                .IsRequired();

            builder.Property(c => c.MaxConcurrentLogins)
                .HasColumnName("MaxConcurrentLogins")
                .HasColumnType("int")
                .IsRequired();

            builder.Property(c => c.ForgotPasswordCodeExpiry)
                .HasColumnName("ForgotPasswordCodeExpiry")
                .HasColumnType("int")
                .IsRequired();

            builder.Property(c => c.CaptchaOnLogin)
                .HasColumnName("CaptchaOnLogin")
                .HasColumnType("bit")
                .HasConversion(
                    v => v == 1, 
                    v => v ? (byte)1 : (byte)0 
                )
                .IsRequired();

            builder.Property(c => c.Currency)
                .HasColumnName("Currency")
                .HasColumnType("int")
                .IsRequired();

            builder.Property(c => c.Language)
                .HasColumnName("Language")
                .HasColumnType("int")
                .IsRequired();

            builder.Property(c => c.TimeZone)
                .HasColumnName("TimeZone")
                .HasColumnType("int")
                .IsRequired();

            builder.Property(c => c.FinancialYear)
                .HasColumnName("FinancialYear")
                .HasColumnType("int")
                .IsRequired();

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