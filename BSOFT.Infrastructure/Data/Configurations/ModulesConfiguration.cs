using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BSOFT.Infrastructure.Data.Configurations
{
    public class ModulesConfiguration : IEntityTypeConfiguration<Modules>
    {
        public void Configure(EntityTypeBuilder<Modules> builder)
        {
            // Set table name and schema (optional)
            builder.ToTable("Modules", "AppData");

            // Set primary key
            builder.HasKey(m => m.Id);

            // Configure properties
            builder.Property(m => m.ModuleName)
                .IsRequired() // Not null
                .HasMaxLength(100) // Set max length
                .HasColumnType("varchar(100)");

            // Configure relationships
            builder.HasMany(m => m.RoleEntitlements)
                .WithOne(re => re.Module) // Define navigation property in RoleEntitlement
                .HasForeignKey(re => re.ModuleId) // Foreign key in RoleEntitlement
                .OnDelete(DeleteBehavior.Restrict); // Restrict delete behavior

            builder.HasMany(m => m.Menus)
                .WithOne(menu => menu.Module) // Define navigation property in Menu
                .HasForeignKey(menu => menu.ModuleId) // Foreign key in Menu
                .OnDelete(DeleteBehavior.Cascade); // Cascade delete behavior
        }
    }
}