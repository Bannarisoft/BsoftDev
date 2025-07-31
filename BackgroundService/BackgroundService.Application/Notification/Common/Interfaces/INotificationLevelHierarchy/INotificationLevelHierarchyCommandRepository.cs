namespace BackgroundService.Application.Notification.Common.Interfaces.INotificationLevelHierarchy
{
    public interface INotificationLevelHierarchyCommandRepository
    {
        Task<int> CreateAsync(Domain.Entities.Notification.NotificationLevelHierarchy NotificationLevelHierarchy);
        Task<int> UpdateAsync(int id, Domain.Entities.Notification.NotificationLevelHierarchy NotificationLevelHierarchy);
        Task<int> DeleteAsync(int id, Domain.Entities.Notification.NotificationLevelHierarchy NotificationLevelHierarchy);        
        Task<bool> IsNameDuplicateAsync(int notificationConfigId, int targetTypeId, int targetId);
                
    }
}