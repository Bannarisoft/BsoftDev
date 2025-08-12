using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Core.Application.Common.Interfaces;
using Core.Domain.Common;
using Core.Domain.Entities.Item;
using InventoryManagement.Infrastructure.Data.Configurations.Item;
using Core.Domain.Entities;
using InventoryManagement.Infrastructure.Data.Configurations;
using InventoryManagement.Infrastructure.Data.Configurations.Budget;
using Core.Domain.Entities.Budget;
using Microsoft.Identity.Client;
using Core.Domain.Entities.Item.ItemDetail;
using Core.Domain.Entities.Item.ItemDetail.Variant;

namespace InventoryManagement.Infrastructure.Data
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

         public DbSet<ItemGroup> ItemGroup { get; set; } 
         public DbSet<ItemCategory> ItemCategory { get; set; } 

		 public DbSet<MiscTypeMaster> MiscTypeMaster { get; set; }
         public DbSet<MiscMaster> MiscMaster { get; set; }        
         public DbSet<HSNMaster> HSNMaster { get; set; }
         public DbSet<UOM> UOMs { get; set; }
         public DbSet<UOMConversion> UOMConversions { get; set; }
        public DbSet<BudgetMaster> BudgetMaster { get; set; }
        public DbSet<BudgetDetail> BudgetDetail { get; set; }
        public DbSet<BudgetLog> BudgetLog { get; set; }
        // Item related DbSets
        public DbSet<ItemMaster> ItemMaster { get; set; }        
        public DbSet<ItemSupplier> ItemSupplier => Set<ItemSupplier>();
        public DbSet<ItemManufacture> ItemManufacture => Set<ItemManufacture>();
        public DbSet<ItemPurchase> ItemPurchase => Set<ItemPurchase>();
        public DbSet<ItemInventory> ItemInventory => Set<ItemInventory>();
        public DbSet<ItemQuality> ItemQuality => Set<ItemQuality>();
        public DbSet<ItemVariantDef> ItemVariantDef => Set<ItemVariantDef>();
        public DbSet<ItemVariantDefOption> ItemVariantDefOption => Set<ItemVariantDefOption>();
        public DbSet<ItemVariantValue> ItemVariantValue => Set<ItemVariantValue>();
        public DbSet<ItemLog> ItemLog => Set<ItemLog>();
        public DbSet<ItemUOM> ItemUOMs => Set<ItemUOM>();
        



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new ItemGroupConfiguration());
            modelBuilder.ApplyConfiguration(new ItemCategoryConfiguration());

            modelBuilder.ApplyConfiguration(new MiscTypeMasterConfiguration());
            modelBuilder.ApplyConfiguration(new MiscMasterConfiguration());
            modelBuilder.ApplyConfiguration(new BudgetMasterConfiguration());
            modelBuilder.ApplyConfiguration(new BudgetDetailConfiguration());
            modelBuilder.ApplyConfiguration(new BudgetLogConfiguration());
            modelBuilder.ApplyConfiguration(new HSNMasterConfiguration());
            modelBuilder.ApplyConfiguration(new UOMConfiguration());
            modelBuilder.ApplyConfiguration(new UOMConversionConfiguration());


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

