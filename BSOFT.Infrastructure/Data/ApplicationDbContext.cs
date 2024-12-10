using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;
using BSOFT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BSOFT.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> dbContextOptions) 
            : base(dbContextOptions) 
        {           
        }

        public DbSet<User> User { get; set; }
        public DbSet<RoleEntitlement> RoleEntitlement { get; set; }
        public DbSet<Department> Department { get; set; } 
        public DbSet<Role> Role { get; set; } 
        public DbSet<Company> Companies { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // User entity configuration
            modelBuilder.Entity<User>()
                .HasKey(u => u.UserId); // Primary key

            // Department entity configuration
            modelBuilder.Entity<Department>()
                .HasKey(d => d.DeptId); // Primary key

            // RoleEntitlement entity configuration
            modelBuilder.Entity<RoleEntitlement>()
                .HasMany(x => x.MenuPermissions)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade); // Cascade delete behavior

            base.OnModelCreating(modelBuilder);
        }
    }
}
