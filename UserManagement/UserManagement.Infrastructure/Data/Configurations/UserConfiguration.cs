using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Core.Domain.Entities;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using static Core.Domain.Enums.Common.Enums;

namespace UserManagement.Infrastructure.Data.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
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

                var isfirstTimeUser = new ValueConverter<FirstTimeUserStatus, bool>
                (
                 v => v == FirstTimeUserStatus.Yes,  
                 v => v ? FirstTimeUserStatus.Yes : FirstTimeUserStatus.No 
                );
    
            builder.ToTable("Users", "AppSecurity");

            builder.HasKey(u => u.UserId);

            builder.Property(u => u.Id)
                .HasColumnName("Id")
                .HasColumnType("uniqueidentifier")
                .IsRequired()
                .HasDefaultValueSql("NEWID()");

            builder.Property(u => u.UserId)
                .HasColumnName("UserId")
                .HasColumnType("int")
                .IsRequired()
                .ValueGeneratedOnAdd();

            builder.Property(u => u.FirstName)
                .HasColumnName("FirstName")
                .HasColumnType("varchar(50)")
                .IsRequired(false);

            builder.Property(u => u.LastName)
                .HasColumnName("LastName")
                .HasColumnType("varchar(50)")
                .IsRequired(false);

            builder.Property(u => u.UserName)
                .HasColumnName("UserName")
                .HasColumnType("varchar(50)")
                .IsRequired(false);

            builder.Property(u => u.IsActive)
                .HasColumnName("IsActive")
                .HasColumnType("bit")
                .HasConversion(isActiveConverter)
                .IsRequired();

            builder.Property(u => u.IsFirstTimeUser)
                .HasColumnName("IsFirstTimeUser")
                .HasColumnType("bit")
                .HasConversion(isfirstTimeUser)
                .IsRequired();

            builder.Property(u => u.PasswordHash)
                .HasColumnName("PasswordHash")
                .HasColumnType("varchar(255)")
                .IsRequired(false);

            builder.Property(u => u.UserType)
                .HasColumnName("UserType")
                .HasColumnType("int")
                .IsRequired(false);

            builder.Property(u => u.Mobile)
                .HasColumnName("Mobile")
                .HasColumnType("varchar(20)")
                .IsRequired(false);

            builder.Property(u => u.EmailId)
                .HasColumnName("EmailId")
                .HasColumnType("varchar(100)")
                .IsRequired();

                  builder.Property(u => u.UserGroupId)
                .HasColumnName("UserGroupId")
                .HasColumnType("int")
                .IsRequired(false);


            builder.Property(u => u.IsDeleted)
            .HasColumnName("IsDeleted")
            .HasColumnType("bit")
            .HasConversion(isDeletedConverter)
            .IsRequired();

             builder.Property(u => u.IsLocked)
            .HasColumnName("IsLocked")
            .HasColumnType("bit")
            .HasConversion(
                    v => v == 1, 
                    v => v ? (byte)1 : (byte)0 
                )
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


                builder.HasOne(ug => ug.UserGroup)
            .WithMany(ug => ug.Users)
            .HasForeignKey(ug => ug.UserGroupId)
            .OnDelete(DeleteBehavior.Restrict);

            

        }
    }
}