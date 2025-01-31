using Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using static Core.Domain.Enums.Common.Enums;

namespace BSOFT.Infrastructure.Data.Configurations
{
    public class UnitConfiguration : IEntityTypeConfiguration<Unit>
    {
        public void Configure(EntityTypeBuilder<Unit> builder)
        {
            var isActiveConverter = new ValueConverter<Status, bool>
               (
                    v => v == Status.Active,  
                    v => v ? Status.Active : Status.Inactive 
                );

                var isDeletedConverter = new ValueConverter<IsDelete, bool>
                (
                 v => v == IsDelete.Deleted,  
                 v => v ? IsDelete.Deleted : IsDelete.NotDeleted 
                );
                
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


        builder.Property(u => u.IsActive)
                .HasColumnName("IsActive")
                .HasColumnType("bit")
                .HasConversion(isActiveConverter)
                .IsRequired();

                 builder.Property(u => u.IsDeleted)
            .HasColumnName("IsDeleted")
            .HasColumnType("bit")
            .HasConversion(isDeletedConverter)
            .IsRequired();

            builder.Property(b => b.CreatedByName)
            .IsRequired()
            .HasColumnType("varchar(50)");

             builder.Property(b => b.CreatedIP)
            .IsRequired()
            .HasColumnType("varchar(255)");

            builder.Property(b => b.ModifiedByName)
            .HasColumnType("varchar(50)");

            builder.Property(b => b.ModifiedIP)
            .HasColumnType("varchar(255)");
            
        builder.HasMany(u => u.UnitAddress)
            .WithOne(ua => ua.Unit)
            .HasForeignKey(ua => ua.UnitId);

        builder.HasMany(u => u.UnitContacts)
            .WithOne(uc => uc.Unit)
            .HasForeignKey(uc => uc.UnitId);
        }
    }
}