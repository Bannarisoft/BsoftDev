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
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using BSOFT.Domain.Interfaces;
using BSOFT.Domain.Common;

namespace BSOFT.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        private readonly IIPAddressService _ipAddressService;
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> dbContextOptions, IIPAddressService ipAddressService) 
            : base(dbContextOptions) 
        {  
            _ipAddressService = ipAddressService;         
        }
        public DbSet<Unit> Unit { get; set; } 
        public DbSet<Entity> Entity { get; set; } 

        public DbSet<User> User { get; set; }
        public DbSet<RoleEntitlement> RoleEntitlement { get; set; }
        public DbSet<Department> Department { get; set; } 
        public DbSet<Role> Role { get; set; } 
        public DbSet<Company> Companies { get; set; }
        public DbSet<Division> Divisions { get; set; }
        public DbSet<MenuPermission> MenuPermission { get; set; }

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
            .HasMany(re => re.MenuPermissions)
            .WithOne(mp => mp.RoleEntitlement)
            .HasForeignKey(mp => mp.RoleEntitlementId);

            modelBuilder.Entity<MenuPermission>()
            .HasKey(mp => mp.MenuPermissionId); // Ensure primary key is defined


    
            base.OnModelCreating(modelBuilder);
        }
         public override int SaveChanges()
        {
            UpdateIpFields();
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateIpFields();
            return await base.SaveChangesAsync(cancellationToken);
        }

        private void UpdateIpFields()
        {
            string currentIp = _ipAddressService.GetSystemIPAddress();
            foreach (EntityEntry entry in ChangeTracker.Entries<BaseEntity>())
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Property("CreatedIP").CurrentValue = currentIp;
                    entry.Property("CreatedAt").CurrentValue = DateTime.UtcNow;
                    entry.Property("CreatedBy").CurrentValue = 1;
                    entry.Property("CreatedByName").CurrentValue = "Test";
                }
                if (entry.State == EntityState.Modified)
                {
                    entry.Property("ModifiedIP").CurrentValue = currentIp;
                    entry.Property("ModifiedAt").CurrentValue = DateTime.UtcNow;
                    entry.Property("ModifiedBy").CurrentValue = 1;
                    entry.Property("ModifiedByName").CurrentValue = "Test";
                }
            }
        }
    }
}
