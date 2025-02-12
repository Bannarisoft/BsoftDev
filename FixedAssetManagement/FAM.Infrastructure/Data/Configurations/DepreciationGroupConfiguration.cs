using Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using static Core.Domain.Common.BaseEntity;


namespace FAM.Infrastructure.Data.Configurations
{
    public class DepreciationGroupConfiguration  : IEntityTypeConfiguration<DepreciationGroups>
    {
        public void Configure(EntityTypeBuilder<DepreciationGroups> builder)
        {
                // ValueConverter for Status (enum to bit)
                var statusConverter = new ValueConverter<Status, bool>(
                    v => v == Status.Active,                    
                    v => v ? Status.Active : Status.Inactive    
                );
            // ValueConverter for IsDelete (enum to bit)
                var isDeleteConverter = new ValueConverter<IsDelete, bool>(
                    v => v == IsDelete.Deleted,                 
                    v => v ? IsDelete.Deleted : IsDelete.NotDeleted
                );

                builder.ToTable("DepreciationGroup", "FixedAsset");
                // Primary Key
                builder.HasKey(b => b.Id);
                builder.Property(b => b.Id)
                .HasColumnName("Id")
                .HasColumnType("int")
                .IsRequired();

                builder.Property(dg => dg.Code)                
                .HasColumnType("varchar(10)")
                .IsRequired();                
      
                builder.Property(dg => dg.BookType)                
                .HasColumnType("varchar(50)")
                .IsRequired();                

                builder.Property(dg => dg.DepreciationGroupName)                
                .HasColumnType("varchar(50)")
                .IsRequired(); 

                builder.Property(dg => dg.AssetGroupId)
                .HasColumnType("int")
                .IsRequired(); // Ensure non-nullable foreign key

                // Configure Foreign Key Relationship
                builder.HasOne(dg => dg.AssetGroup)
                .WithMany(ag => ag.DepreciationGroups)
                .HasForeignKey(dg => dg.AssetGroupId)                
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascading delete

                builder.Property(b => b.UsefulLife)                
                .HasColumnType("int")
                .IsRequired();

                 builder.Property(b => b.DepreciationMethod)                
                .HasColumnType("varchar(50)")
                .IsRequired();
                
                 builder.Property(b => b.ResidualValue)                
                .HasColumnType("int")
                .IsRequired();

                builder.Property(ag => ag.SortOrder)
                .HasColumnName("SortOrder")
                .HasColumnType("int")
                .IsRequired(); 

                builder.Property(b => b.IsActive)                
                .HasColumnType("bit")
                .HasConversion(statusConverter)
                .IsRequired();

                 builder.Property(b => b.IsDeleted)                
                .HasColumnType("bit")
                .HasConversion(isDeleteConverter)
                .IsRequired();

                builder.Property(b => b.CreatedByName)
                .IsRequired()
                .HasColumnType("varchar(50)");
    
                builder.Property(b => b.CreatedIP)
                .IsRequired()
                .HasColumnType("varchar(50)");

                builder.Property(b => b.ModifiedByName)
                .HasColumnType("varchar(50)");

                builder.Property(b => b.ModifiedIP)
                .HasColumnType("varchar(50)");              
        }
    }
}