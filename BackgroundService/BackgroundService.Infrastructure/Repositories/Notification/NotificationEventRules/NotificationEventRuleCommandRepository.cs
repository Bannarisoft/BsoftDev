using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackgroundService.Application.Notification.Common.Interfaces.INotificationEventRule;
using BackgroundService.Domain.Entities.Notification;
using BackgroundService.Infrastructure.Data.Notification;
using Microsoft.EntityFrameworkCore;

namespace BackgroundService.Infrastructure.Repositories.Notification.NotificationEventRules
{
    public class NotificationEventRuleCommandRepository : INotificationEventRuleCommand
    {
        private readonly NotificationDbContext _notificationDbContext;
        public NotificationEventRuleCommandRepository(NotificationDbContext notificationDbContext)
        {
            _notificationDbContext = notificationDbContext;
        }
        public async Task<int> CreateAsync(NotificationEventRule notificationEventRule)
        {
             _notificationDbContext.Entry(notificationEventRule);
            await _notificationDbContext.NotificationEventRule.AddAsync(notificationEventRule);
            await _notificationDbContext.SaveChangesAsync();

            return notificationEventRule.Id;
        }

        public async Task<bool> DeleteAsync(int id, NotificationEventRule notificationEventRule)
        {
             var NotificationDelete = await _notificationDbContext.NotificationEventRule.FirstOrDefaultAsync(u => u.Id == id);
            if (NotificationDelete != null)
            {
                NotificationDelete.IsDeleted = notificationEventRule.IsDeleted;
                return await _notificationDbContext.SaveChangesAsync() >0;
            }
            return false; 
        }

        public async Task<bool> UpdateAsync(NotificationEventRule notificationEventRule)
        {
             var existingNotification = await _notificationDbContext.NotificationEventRule
            .AsNoTracking().FirstOrDefaultAsync(u => u.Id == notificationEventRule.Id);
            
            if (existingNotification != null)
            {
                existingNotification.NotificationChannelId = notificationEventRule.NotificationChannelId;
                existingNotification.NotificationLevelHierarchyId = notificationEventRule.NotificationLevelHierarchyId;
                existingNotification.RecipientTypeId = notificationEventRule.RecipientTypeId;
                existingNotification.TemplateId = notificationEventRule.TemplateId;
                existingNotification.IsActive = notificationEventRule.IsActive;
                _notificationDbContext.NotificationEventRule.Update(existingNotification);

                return await _notificationDbContext.SaveChangesAsync() >0;
            }
            
            return false; 
        }
    }
}