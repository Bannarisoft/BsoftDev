using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackgroundService.Domain.Common;
using BackgroundService.Domain.Entities.Notification;
using BackgroundService.Infrastructure.Data.Notification.Configurations;
using BackgroundService.Infrastructure.Services;
using Core.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace BackgroundService.Infrastructure.Data.Notification
{
    public class NotificationDbContext : DbContext
    {
       
        public NotificationDbContext(DbContextOptions<NotificationDbContext> options)
        : base(options)
        {
         
        }

        public DbSet<NotificationConfig> NotificationConfig { get; set; }
        public DbSet<NotificationGroup> NotificationGroup { get; set; }
        public DbSet<NotificationGroupMembers> NotificationGroupMembers { get; set; }
        public DbSet<NotificationLevelHierarchy> NotificationLevelHierarchy { get; set; }
        public DbSet<NotificationEventRule> NotificationEventRule { get; set; }
        public DbSet<NotificationEventLog> NotificationEventLog { get; set; }
        public DbSet<NotificationTemplate> NotificationTemplate { get; set; }
        public DbSet<MiscTypeMaster> MiscTypeMaster { get; set; }
        public DbSet<MiscMaster> MiscMaster { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new NotificationConfigConfiguration());
            modelBuilder.ApplyConfiguration(new NotificationGroupConfiguration());
            modelBuilder.ApplyConfiguration(new NotificationGroupMembersConfiguration());
            modelBuilder.ApplyConfiguration(new NotificationLevelHierarchyConfiguration());
            modelBuilder.ApplyConfiguration(new NotificationEventRuleConfiguration());
            modelBuilder.ApplyConfiguration(new NotificationEventLogConfiguration());
            modelBuilder.ApplyConfiguration(new NotificationTemplateConfiguration());
            modelBuilder.ApplyConfiguration(new MiscTypeMasterConfiguration());
            modelBuilder.ApplyConfiguration(new MiscMasterConfiguration());
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
            string currentIp = IPAddressService.GetSystemIPAddress();
            int userId = IPAddressService.GetUserId(); 
            string username = IPAddressService.GetUserName();
            var systemTimeZoneId = TimeZoneService.GetSystemTimeZone();
            var currentTime = TimeZoneService.GetCurrentTime(systemTimeZoneId);  
            
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