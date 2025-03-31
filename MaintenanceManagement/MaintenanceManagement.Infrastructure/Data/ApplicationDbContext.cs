using Microsoft.EntityFrameworkCore;
using Core.Domain.Entities;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Core.Application.Common.Interfaces;
using Core.Domain.Common;
using MaintenanceManagement.Infrastructure.Data.Configurations;



namespace MaintenanceManagement.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        private readonly IIPAddressService _ipAddressService;
        private readonly ITimeZoneService _timeZoneService; 

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> dbContextOptions, IIPAddressService ipAddressService, ITimeZoneService timeZoneService) 
            : base(dbContextOptions)
        {  
            _ipAddressService = ipAddressService; 
            _timeZoneService = timeZoneService;              
               
        }
        
         public DbSet<CostCenter> CostCenter { get; set; } 
         public DbSet<WorkCenter> WorkCenter { get; set; } 
        public DbSet<MachineGroup> MachineGroup { get ; set; }
        public DbSet<MiscTypeMaster> MiscTypeMaster { get; set; }
        public DbSet<MiscMaster> MiscMaster { get; set; }
        public DbSet<ShiftMaster> ShiftMaster { get; set; }
        public DbSet<ShiftMasterDetail> ShiftMasterDetail { get; set; }
        public DbSet<MaintenanceType> MaintenanceType { get; set; }
        public DbSet<MaintenanceCategory> MaintenanceCategory { get; set; }

        public DbSet<ActivityMaster> ActivityMaster { get; set; }

        public DbSet<ActivityMachineGroup>  ActivityMachineGroup { get; set; }
        public DbSet<MachineGroupUser>  MachineGroupUser { get; set; }
       

        protected override void OnModelCreating(ModelBuilder modelBuilder)

        {            
         //  modelBuilder.ApplyConfiguration(new MachineGroupConfiguration());
           modelBuilder.ApplyConfiguration( new MachineGroupConfiguration());
           modelBuilder.ApplyConfiguration(new MiscTypeMasterConfiguration());
           modelBuilder.ApplyConfiguration(new MiscMasterConfiguration());
            modelBuilder.ApplyConfiguration(new ShiftMasterConfiguration());
            modelBuilder.ApplyConfiguration(new ShiftMasterDetailsConfiguration());
        
            modelBuilder.ApplyConfiguration(new CostCenterConfiguration());
            modelBuilder.ApplyConfiguration(new WorkCenterConfiguration());
            modelBuilder.ApplyConfiguration(new MaintenanceTypeConfiguration());
            modelBuilder.ApplyConfiguration(new MaintenanceCategoryConfiguration());
            modelBuilder.ApplyConfiguration(new ActivityMasterConfiguration());
            modelBuilder.ApplyConfiguration( new ActivityMachineGroupConfiguration());
            modelBuilder.ApplyConfiguration( new MachineGroupUserConfiguration());


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
            int userId = _ipAddressService.GetUserId(); 
            string username = _ipAddressService.GetUserName();
            var systemTimeZoneId = _timeZoneService.GetSystemTimeZone();
            var currentTime = _timeZoneService.GetCurrentTime(systemTimeZoneId);  
            
            foreach (EntityEntry entry in ChangeTracker.Entries<BaseEntity>())
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Property("CreatedIP").CurrentValue = currentIp;
                    entry.Property("CreatedDate").CurrentValue = currentTime;
                    entry.Property("CreatedBy").CurrentValue = userId;
                    entry.Property("CreatedByName").CurrentValue = username;
                }
                if (entry.State == EntityState.Modified)
                {
                    entry.Property("ModifiedIP").CurrentValue = currentIp;
                    entry.Property("ModifiedDate").CurrentValue = currentTime;
                    entry.Property("ModifiedBy").CurrentValue = userId;
                    entry.Property("ModifiedByName").CurrentValue = username;
                }
            }
        }
    }
}
