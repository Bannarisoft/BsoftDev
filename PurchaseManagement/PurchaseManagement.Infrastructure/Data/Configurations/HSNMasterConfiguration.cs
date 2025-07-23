using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using static Core.Domain.Common.BaseEntity;

namespace PurchaseManagement.Infrastructure.Data.Configurations
{
    public class HSNMasterConfiguration : IEntityTypeConfiguration<HSNMaster>
    {

        public void Configure(EntityTypeBuilder<HSNMaster> builder)
        {


            var statusConverter = new ValueConverter<Status, bool>(
                   v => v == Status.Active,
                   v => v ? Status.Active : Status.Inactive
            );

            var isDeleteConverter = new ValueConverter<IsDelete, bool>(
                v => v == IsDelete.Deleted,
                v => v ? IsDelete.Deleted : IsDelete.NotDeleted
            );

            builder.ToTable("HSNMaster", "Purchase");

            builder.HasKey(t => t.Id);

            builder.Property(t => t.Id)
                .HasColumnName("Id")
                .HasColumnType("int")
                .IsRequired();

            builder.Property(t => t.Type)
                .HasColumnName("Type")
                .HasColumnType("int")
                .IsRequired();

            builder.Property(t => t.HSNCode)
                .HasColumnName("HSNCode")
                .HasColumnType("varchar(10)")
                .IsRequired();

            builder.Property(t => t.Description)
                .HasColumnName("Description")
                .HasColumnType("varchar(250)")
                .IsRequired();

            builder.Property(t => t.GstCategory)
                .HasColumnName("GstCategory")
                .HasColumnType("varchar(250)")
                .IsRequired();

            builder.Property(t => t.GstPercentage)
                .HasColumnName("GstPercentage")
                .HasColumnType("decimal(5,2)")
                .IsRequired();

            builder.Property(t => t.CgstPercentage)
                .HasColumnName("CgstPercentage")
                .HasColumnType("decimal(5,2)")
                .IsRequired();

            builder.Property(t => t.SgstPercentage)
                .HasColumnName("SgstPercentage")
                .HasColumnType("decimal(5,2)")
                .IsRequired();

            builder.Property(t => t.IgstPercentage)
                .HasColumnName("IgstPercentage")
                .HasColumnType("decimal(5,2)")
                .IsRequired();

            builder.Property(t => t.ValidFrom)
                .HasColumnName("ValidFrom")
                .HasColumnType("datetimeoffset")
                .IsRequired();      

            builder.Property(b => b.IsActive)
                .HasColumnName("IsActive")
                .HasColumnType("bit")
                .HasConversion(statusConverter)
                .IsRequired();

            builder.Property(b => b.IsDeleted)
                .HasColumnName("IsDeleted")
                .HasColumnType("bit")
                .HasConversion(isDeleteConverter)
                .IsRequired();
            
            builder.Property(b => b.CreatedByName)
                .IsRequired()
                .HasColumnType("varchar(50)");

    
            builder.Property(b => b.CreatedIP)
                .IsRequired()
                .HasColumnType("varchar(20)");

            builder.Property(b => b.ModifiedByName)
                .HasColumnType("varchar(50)");

            builder.Property(b => b.ModifiedIP)
                .HasColumnType("varchar(20)");                        


        }
        
    }
}