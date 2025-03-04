using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace UserManagement.Infrastructure.Data.Configurations
{
    public class UserGroupUsersConfiguration : IEntityTypeConfiguration<UserGroupUsers>
    {
        public void Configure(EntityTypeBuilder<UserGroupUsers> builder)
        {
            builder.ToTable("UserGroupUsers", "AppSecurity");
            builder.HasKey(ug => ug.Id);

            builder.Property(ug => ug.Id)
                .HasColumnName("Id")
                .HasColumnType("int")
                .IsRequired();

            builder.Property(ug => ug.UserGroupId)
                .HasColumnName("UserGroupId")
                .HasColumnType("int")
                .IsRequired();

            builder.Property(ug => ug.UserId)
                .HasColumnName("UserId")
                .HasColumnType("int")
                .IsRequired();

                     builder.HasOne(ug => ug.UserGroup)
            .WithMany(ug => ug.UserGroupUsers)
            .HasForeignKey(ug => ug.UserGroupId)
            .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(ug => ug.User)
                .WithOne(u => u.UserGroupUsers)
                .HasForeignKey<UserGroupUsers>(ug => ug.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}