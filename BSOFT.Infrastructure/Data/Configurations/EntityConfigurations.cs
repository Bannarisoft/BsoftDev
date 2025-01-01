using Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BSOFT.Infrastructure.Data.Configurations
{
    public class EntityConfigurations : IEntityTypeConfiguration<Core.Domain.Entities.Entity>
    {
        public void Configure(EntityTypeBuilder<Entity> builder)
        {
        builder.ToTable("Entity", "AppData");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Id)
            .HasColumnName("Id")
            .HasColumnType("int")
            .IsRequired();

        builder.Property(u => u.EntityCode)
            .HasColumnName("EntityCode")
            .HasColumnType("varchar(20)")
            .IsRequired();

        builder.Property(u => u.EntityName)
            .HasColumnName("EntityName")
            .HasColumnType("varchar(100)")
            .IsRequired();

        builder.Property(u => u.EntityDescription)
            .HasColumnName("EntityDescription")
            .HasColumnType("varchar(250)")
            .IsRequired();

        builder.Property(u => u.Address)
            .HasColumnName("Address")
            .HasColumnType("varchar(200)")
            .IsRequired();

        builder.Property(u => u.Phone)
            .HasColumnName("Phone")
            .HasColumnType("varchar(40)")
            .IsRequired();

        builder.Property(u => u.Email)
            .HasColumnName("Email")
            .HasColumnType("varchar(200)")
            .IsRequired();

         builder.Property(u => u.IsActive)
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