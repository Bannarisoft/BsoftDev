using Microsoft.EntityFrameworkCore;
using Core.Domain.Entities;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Core.Application.Common.Interfaces;
using Core.Domain.Common;
using FAM.Infrastructure.Data.Configurations;
using FAM.Infrastructure.Data.Configurations.AssetMaster;
using Core.Domain.Entities.AssetMaster;
using Core.Domain.Entities.AssetPurchase;
using FAM.Infrastructure.Data.Configurations.AssetPurchase;
namespace FAM.Infrastructure.Data
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
        
        public DbSet<AssetGroup> AssetGroup { get; set; } 
        public DbSet<AssetCategories> AssetCategories { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<SubLocation> SubLocations { get; set; } 
		public DbSet<MiscMaster> MiscMaster { get; set; } 
        public DbSet<MiscTypeMaster> MiscTypeMaster { get; set; }
        public DbSet<DepreciationGroups> DepreciationGroups { get; set; }
        public DbSet<AssetSubCategories> AssetSubCategories { get; set; }
		public DbSet<Manufactures> Manufactures { get; set; }
		public DbSet<AssetMasterGenerals> AssetMasterGenerals { get; set; }
        public DbSet<UOM> UOMs { get; set; }
		public DbSet<AssetSource> AssetSource { get; set; }
        public DbSet<SpecificationMasters> SpecificationMasters { get; set; }
		public DbSet<AssetLocation> AssetLocations { get; set; }
		public DbSet<AssetWarranties> AssetWarranties { get; set; }
        public DbSet<AssetPurchaseDetails> AssetPurchaseDetails { get; set; }
        public DbSet<AssetSpecifications> AssetSpecifications { get; set; }
        public DbSet<AssetAdditionalCost> AssetAdditionalCost { get; set; }
		public DbSet<AssetInsurance> AssetInsurance { get; set; }
        public DbSet<AssetAmc> AssetAmc { get; set; }        
        public DbSet<DepreciationDetails> DepreciationDetails { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new AssetGroupConfiguration());
            modelBuilder.ApplyConfiguration(new AssetCategoriesConfiguration());
            modelBuilder.ApplyConfiguration(new LocationConfiguration());
            modelBuilder.ApplyConfiguration(new SubLocationConfiguration());
			modelBuilder.ApplyConfiguration(new MiscMasterConfiguration());
            modelBuilder.ApplyConfiguration(new MiscTypeMasterConfiguration());
            modelBuilder.ApplyConfiguration(new DepreciationGroupConfiguration());
            modelBuilder.ApplyConfiguration(new AssetSubCategoriesConfiguration());
            modelBuilder.ApplyConfiguration(new ManufactureConfiguration());   
 			modelBuilder.ApplyConfiguration(new AssetMasterGeneralConfiguration()); 
            modelBuilder.ApplyConfiguration(new ManufactureConfiguration());
            modelBuilder.ApplyConfiguration(new UOMConfiguration());   
			modelBuilder.ApplyConfiguration(new AssetLocationConfiguration());
			modelBuilder.ApplyConfiguration(new AssetSourceConfiguration());
            modelBuilder.ApplyConfiguration(new SpecificationMasterConfiguration());   
 			modelBuilder.ApplyConfiguration(new AssetPurchaseDetailsConfiguration());
            modelBuilder.ApplyConfiguration(new AssetSpecificationConfiguration());   
			modelBuilder.ApplyConfiguration(new AssetWarrantyConfiguration());
            modelBuilder.ApplyConfiguration(new AssetAdditionalCostConfiguration());   
			modelBuilder.ApplyConfiguration(new AssetInsuranceConfiguration());
            modelBuilder.ApplyConfiguration(new AssetAmcConfiguration()); 
            modelBuilder.ApplyConfiguration(new DepreciationDetailConfiguration()); 

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
