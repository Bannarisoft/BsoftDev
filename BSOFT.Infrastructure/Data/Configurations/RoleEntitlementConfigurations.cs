using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BSOFT.Domain.Entities;

namespace BSOFT.Infrastructure.Data.Configurations
{
    public class RoleEntitlementConfigurations : IEntityTypeConfiguration<RoleEntitlement>
    {
    public void Configure(EntityTypeBuilder<RoleEntitlement> builder)
    {
        builder.HasKey(re => re.Id);
        builder.Property(re => re.RoleName).IsRequired().HasMaxLength(100);
    }
    }
}