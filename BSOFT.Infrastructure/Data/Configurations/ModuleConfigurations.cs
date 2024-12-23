using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BSOFT.Domain.Entities;
public class ModuleConfigurations : IEntityTypeConfiguration<Modules>
{
    public void Configure(EntityTypeBuilder<Modules> builder)
    {
        builder.HasKey(m => m.Id);
        builder.Property(m => m.Name).IsRequired().HasMaxLength(100);
        builder.HasMany(m => m.Menus).WithOne(menu => menu.Module).HasForeignKey(menu => menu.ModuleId);
        // builder.HasData(new Modules { Id = 1, Name = "User Management" },
        //     new Modules { Id = 2, Name = "Reports" });
    }
}