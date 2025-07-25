using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackgroundService.Domain.Entities.Notification;

namespace BackgroundService.Application.Notification.Common.Interfaces.INotificationEventRule
{
    public interface INotificationEventRuleCommand
    {
        Task<int> CreateAsync(NotificationEventRule notificationEventRule);     
        Task<bool> UpdateAsync(NotificationEventRule notificationEventRule);
        Task<bool> DeleteAsync(int id,NotificationEventRule notificationEventRule); 
    }
}