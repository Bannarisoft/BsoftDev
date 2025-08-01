using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackgroundService.Domain.Entities.Notification;

namespace BackgroundService.Application.Notification.Common.Interfaces.INotificationEventRule
{
    public interface INotificationEventRuleQuery
    {
        Task<(IEnumerable<dynamic>, int)> GetAllNotificationEventRuleAsync(int PageNumber, int PageSize, string? SearchTerm);
        Task<bool> AlreadyExistsAsync(int NotificationChannelId, int TemplateId, int NotificationLevelHierarchyId,  int RecipientTypeId, int? id = null);
        Task<bool> NotFoundAsync(int id);
    }
}