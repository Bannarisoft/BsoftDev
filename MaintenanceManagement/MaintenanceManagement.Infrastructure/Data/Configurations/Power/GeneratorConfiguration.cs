using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain.Entities.Power;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using static Core.Domain.Common.BaseEntity;

namespace MaintenanceManagement.Infrastructure.Data.Configurations.Power
{
    public class GeneratorConfiguration : IEntityTypeConfiguration<Generator>
    {

        public void Configure(EntityTypeBuilder<Generator> builder)
        {
            var statusConverter = new ValueConverter<Status, bool>(
                     v => v == Status.Active,
                     v => v ? Status.Active : Status.Inactive
                 );
            // ValueConverter for IsDelete (enum to bit)
            var isDeleteConverter = new ValueConverter<IsDelete, bool>(
                v => v == IsDelete.Deleted,
                v => v ? IsDelete.Deleted : IsDelete.NotDeleted
            );

             builder.ToTable("Generator", "Maintenance");

             builder.HasKey(x => x.Id);
             builder.Property(t => t.Id)
                 .HasColumnName("Id")
                 .HasColumnType("int")
                 .IsRequired();

             builder.Property(x => x.Code)
                 .HasMaxLength(50)
                 .HasColumnType("varchar(50)")
                 .IsRequired();

             builder.Property(x => x.GenSetName)
                 .HasMaxLength(100)
                 .HasColumnType("varchar(100)")
                 .IsRequired();

             builder.Property(x => x.UnitId)
                 .HasColumnType("int")
                 .IsRequired();

             builder.Property(x => x.Serialnumber)
                 .HasColumnType("int")
                 .IsRequired();

             builder.Property(x => x.KVA)
                 .HasColumnType("decimal(18,2)")
                 .IsRequired();

             builder.Property(x => x.Current)
                 .HasColumnType("decimal(18,3)")
                 .IsRequired();

             builder.Property(x => x.Voltage)
                 .HasColumnType("decimal(18,3)")
                 .IsRequired();

             builder.Property(x => x.Power)
                 .HasColumnType("decimal(18,3)")
                 .IsRequired();

             builder.Property(x => x.OpeningEnergyReading)
                 .HasColumnType("decimal(18,3)")
                 .IsRequired();

             builder.Property(x => x.RPM)
                 .HasColumnType("int")
                 .IsRequired();

             builder.Property(x => x.PowerFactor)
                 .HasColumnType("decimal(18,3)")
                 .IsRequired();

             builder.Property(x => x.MultiplicationFactor)
                 .HasColumnType("int") // Corrected type
                 .IsRequired();

             builder.Property(x => x.Frequency)
                 .HasColumnType("decimal(18,3)")
                 .IsRequired();

             builder.Property(x => x.FuelTankCapacity) // Corrected name
                 .HasColumnType("decimal(18,2)")
                 .IsRequired();

             builder.Property(x => x.GensetStatusId)
                .HasColumnType("int")
                .IsRequired();         
    }




    }
}