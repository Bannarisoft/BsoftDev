using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain.Entities;
using Core.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BSOFT.Infrastructure.Data.Configurations
{
    public class FinancialYearConfiguration  : IEntityTypeConfiguration<FinancialYear>
    {

  public void Configure(EntityTypeBuilder<FinancialYear> builder)
        {
          builder.ToTable("FinancialYear", "AppData");

            builder.HasKey(u => u.Id);

              builder.Property(u => u.Id)
                        .HasColumnName("Id")
                        .HasColumnType("int")
                        .IsRequired();

             
             builder.Property(u => u.StartYear)
            .HasColumnName("StartYear")
            .HasColumnType("varchar(50)")
            .IsRequired();

             builder.Property(u => u.StartDate)
            .HasColumnName("StartDate")
            .HasColumnType("datetime")
            .IsRequired();

             builder.Property(u => u.EndDate)
            .HasColumnName("EndDate")
            .HasColumnType("datetime")
            .IsRequired();


             builder.Property(u => u.FinYearName)
            .HasColumnName("FinYearName")
            .HasColumnType("varchar(50)")
            .IsRequired();

            builder.Property(u => u.IsActive)
            .HasColumnName("IsActive")
            .HasColumnType("bit")
            .HasConversion(
             v => v == FinancialYearEnum.FinancialYearStatus.Active, // convert enum to bool
             v => v ? FinancialYearEnum.FinancialYearStatus.Active : FinancialYearEnum.FinancialYearStatus.Inactive // convert bool to enum
             )
            .IsRequired();
        builder.Property(u => u.IsDeleted)
            .HasColumnName("IsDeleted")
            .HasColumnType("bit")
            .HasConversion(
             v => v == FinancialYearEnum.FinancialYearDelete.Deleted, // convert enum to bool
             v => v ? FinancialYearEnum.FinancialYearDelete.Deleted : FinancialYearEnum.FinancialYearDelete.NotDeleted // convert bool to enum
             )
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