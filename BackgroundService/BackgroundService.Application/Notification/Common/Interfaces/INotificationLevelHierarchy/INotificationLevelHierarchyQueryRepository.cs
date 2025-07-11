
using BackgroundService.Application.Notification.NotificationLevelHierarchy.Queries.GetAllNotificationLevelHierarchy;

namespace BackgroundService.Application.Notification.Common.Interfaces.INotificationLevelHierarchy
{
    public interface INotificationLevelHierarchyQueryRepository
    {
        Task<(IEnumerable<dynamic>, int)> GetAllNotificationLevelHierarchyAsync(int PageNumber, int PageSize, string? SearchTerm);
        Task<NotificationLevelHierarchyDto> GetByIdAsync(int id);        
        Task<bool> SoftDeleteValidation(int Id);        
        Task<bool> NotFoundAsync(int Id );
    }
}