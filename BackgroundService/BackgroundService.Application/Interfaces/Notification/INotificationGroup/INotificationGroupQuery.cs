using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackgroundService.Domain.Entities.Notification;

namespace BackgroundService.Application.Interfaces.Notification.INotificationGroup
{
    public interface INotificationGroupQuery
    {
        Task<(List<NotificationGroup>, int)> GetAllNotificationGroupAsync(int PageNumber, int PageSize, string? SearchTerm);
        Task<List<NotificationGroup>> GetNotificationGroupsAutoComplete(string searchPattern);
    }
}