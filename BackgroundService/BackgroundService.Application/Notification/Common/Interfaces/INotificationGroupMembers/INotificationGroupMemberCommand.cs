using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackgroundService.Domain.Entities.Notification;

namespace BackgroundService.Application.Notification.Common.Interfaces.INotificationGroupMembers
{
    public interface INotificationGroupMemberCommand
    {
        Task<int> CreateAsync(NotificationGroupMembers notificationGroupMembers);     
        Task<bool> UpdateAsync(NotificationGroupMembers notificationGroupMembers);
        Task<bool> DeleteAsync(int id,NotificationGroupMembers notificationGroupMembers); 
    }
}