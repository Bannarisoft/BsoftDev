using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackgroundService.Domain.Entities.Notification;

namespace BackgroundService.Application.Notification.Common.Interfaces.INotificationGroupMembers
{
    public interface INotificationGroupMemberQuery
    {
         Task<(List<NotificationGroupMembers>, int)> GetAllNotificationGroupAsync(int PageNumber, int PageSize, string? SearchTerm);
        Task<bool> AlreadyExistsAsync(int GroupId,int UserId, int? id = null);
        Task<bool> NotFoundAsync(int id);
    }
}