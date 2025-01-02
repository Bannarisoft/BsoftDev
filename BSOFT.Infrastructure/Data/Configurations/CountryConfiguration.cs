using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BSOFT.Infrastructure.Data.Configurations
{
    public class CountryConfiguration : IEntityTypeConfiguration<Countries>
    {
        public void Configure(EntityTypeBuilder<Countries> builder)
        {
            builder.ToTable("Country", "AppData");
            builder.HasKey(u => u.Id);
            //builder.HasKey(u => new { u.Id, u.CountryCode });
            builder.Property(u => u.Id)
                        .HasColumnName("Id")
                        .HasColumnType("int")
                        .IsRequired();

             builder.Property(u => u.CountryCode)
            .HasColumnName("CountryCode")
            .HasColumnType("varchar(5)")
            .IsRequired();

             builder.Property(u => u.CountryName)
            .HasColumnName("CountryName")
            .HasColumnType("varchar(50)")
            .IsRequired();


            builder.Property(u => u.IsActive)                   
             .HasConversion(
                v => v == 1, // convert byte to bool
                v => v ? (byte)1 : (byte)0 // convert bool to byte
            ) 
            .HasColumnType("bit")
            .HasColumnName("IsActive")     
            .IsRequired();
            

            builder.Property(u => u.CreatedBy)
            .HasColumnName("CreatedBy")
            .HasColumnType("int");   

            builder.Property(u => u.CreatedAt)
            .HasColumnName("CreatedAt")
            .HasColumnType("datetime");

            builder.Property(u => u.CreatedByName)
            .HasColumnName("CreatedByName")
            .HasColumnType("varchar(50)");

            builder.Property(u => u.CreatedIP)
            .HasColumnName("CreatedIP")
            .HasColumnType("varchar(25)");

            builder.Property(u => u.ModifiedAt)
            .HasColumnName("ModifiedAt")
            .HasColumnType("datetime");

            builder.Property(u => u.ModifiedBy)
            .HasColumnName("ModifiedBy")
            .HasColumnType("int");

            builder.Property(u => u.ModifiedByName)
            .HasColumnName("ModifiedByName")
            .HasColumnType("varchar(50)");


            builder.Property(u => u.ModifiedIP)
            .HasColumnName("ModifiedIP")
            .HasColumnType("varchar(25)"); 
        }
    }
}