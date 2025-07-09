using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackgroundService.Domain.Entities.Notification;
using BackgroundService.Infrastructure.Data.Notification.Configurations;
using Microsoft.EntityFrameworkCore;

namespace BackgroundService.Infrastructure.Data.Notification
{
    public class NotificationDbContext : DbContext
    {
        public NotificationDbContext(DbContextOptions<NotificationDbContext> options) : base(options) { }
        
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
    }
}