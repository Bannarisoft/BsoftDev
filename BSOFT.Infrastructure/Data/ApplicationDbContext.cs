using Microsoft.EntityFrameworkCore;
using Core.Domain.Entities;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Core.Application.Common.Interfaces;
using Core.Domain.Common;
using BSOFT.Infrastructure.Data.Configurations;


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
        
        public DbSet<Entity> Entity { get; set; } 
        public DbSet<Unit> Unit { get; set; } 
        public DbSet<UnitAddress> UnitAddress { get; set; }
        public DbSet<UnitContacts> UnitContacts { get; set; }
        public DbSet<Department> Department { get; set; } 
        public DbSet<User> User { get; set; }
        public DbSet<UserRole> UserRole { get; set; } 
        public DbSet<Company> Companies { get; set; }
        public DbSet<CompanyAddress> companyAddresses { get; set; }
        public DbSet<CompanyContact> CompanyContacts { get; set; }
        public DbSet<Division> Divisions { get; set; }
        public DbSet<Modules> Modules { get; set; }
        public DbSet<Menu> Menus { get; set; }
        public DbSet<RoleEntitlement> RoleEntitlements { get; set; }
        public DbSet<Countries> Countries { get; set; }
        public DbSet<States> States { get; set; }
        public DbSet<Cities> Cities { get; set; }
        public DbSet<PasswordLog> PasswordLogs { get; set; }
        public DbSet<UserRoleAllocation> UserRoleAllocations { get; set; }
        public DbSet<UserSession> UserSession { get; set; }
        public DbSet<PasswordComplexityRule> PasswordComplexityRule { get; set; }
        public DbSet<AdminSecuritySettings> AdminSecuritySettings { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new UnitConfiguration());
            modelBuilder.ApplyConfiguration(new UnitAddressConfiguration());
            modelBuilder.ApplyConfiguration(new UnitContactsConfiguration());
            modelBuilder.ApplyConfiguration(new RoleEntitlementConfigurations());
            modelBuilder.ApplyConfiguration(new MenuConfiguration());
            modelBuilder.ApplyConfiguration(new ModulesConfiguration());
            modelBuilder.ApplyConfiguration(new CountryConfiguration());
            modelBuilder.ApplyConfiguration(new UnitConfiguration());
            modelBuilder.ApplyConfiguration(new CompanyConfiguration());
            modelBuilder.ApplyConfiguration(new CompanyAddressConfiguration());
			modelBuilder.ApplyConfiguration(new StateConfiguration());       
            modelBuilder.ApplyConfiguration(new CityConfiguration());  
            modelBuilder.ApplyConfiguration(new CompanyContactConfiguration());
            modelBuilder.ApplyConfiguration(new EntityConfigurations());
            modelBuilder.ApplyConfiguration(new DivisionConfiguration());
			modelBuilder.ApplyConfiguration(new DepartmentConfiguration());
            modelBuilder.ApplyConfiguration(new UserRoleConfiguration());
            modelBuilder.ApplyConfiguration(new UserRoleAllocationConfigurations());
            modelBuilder.ApplyConfiguration(new UserSessionConfiguration());
			modelBuilder.ApplyConfiguration(new PwdComplexityRuleConfiguration());
            modelBuilder.ApplyConfiguration(new AdminSecuritySettingsConfiguration());

            
            modelBuilder.ApplyConfiguration(new PasswordLogConfiguration());
               
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
