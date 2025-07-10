using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackgroundService.Domain.Entities.Notification;

namespace BackgroundService.Application.Interfaces.Notification.INotificationGroup
{
    public interface INotificationGroupCommand
    {
        Task<int> CreateAsync(NotificationGroup notificationGroup);     
        Task<bool> UpdateAsync(NotificationGroup notificationGroup);
        Task<bool> DeleteAsync(int id,NotificationGroup notificationGroup); 
    }
}