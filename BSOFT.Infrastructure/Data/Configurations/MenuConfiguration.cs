using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BSOFT.Infrastructure.Data.Configurations
{
    public class MenuConfiguration : IEntityTypeConfiguration<Menu>
    {
    public void Configure(EntityTypeBuilder<Menu> builder)
    {
        builder.ToTable("Menus", "AppData");
        builder.HasKey(m => m.Id);
        builder.Property(m => m.MenuName).IsRequired()
        .HasColumnType("varchar(100)");
        builder.HasOne(m => m.Module)
               .WithMany(module => module.Menus)
               .HasForeignKey(m => m.ModuleId);
    }
    }
}