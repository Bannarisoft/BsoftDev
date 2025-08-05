using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PurchaseManagement.Infrastructure.Data.Configurations
{
    public class IndentDepartmentMappingConfiguration : IEntityTypeConfiguration<IndentDepartmentMapping>
    {
        public void Configure(EntityTypeBuilder<IndentDepartmentMapping> builder)
        {
            builder.ToTable("IndentDepartmentMapping", "Purchase");

            builder.HasKey(t => t.Id);
            builder.Property(t => t.Id)
                .HasColumnName("Id")
                .HasColumnType("int")
                .IsRequired();


            builder.Property(m => m.IndentHeaderId)
                .HasColumnName("IndentHeaderId")
                .HasColumnType("int")
                .IsRequired();

            builder.Property(m => m.DepartmentId)
                   .HasColumnName("DepartmentId")
                   .HasColumnType("int")
                   .IsRequired();
                   
                   builder.HasOne(ac => ac.IndentHeader)
                .WithMany(am => am.IndentDepartmentMappings)
                .HasForeignKey(ac => ac.IndentHeaderId)
                .OnDelete(DeleteBehavior.NoAction);

        }
    }
}