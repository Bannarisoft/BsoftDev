
namespace BackgroundService.Application.Notification.Common.Interfaces.INotificationConfig
{
    public interface INotificationConfigQueryRepository
    {
        Task<(IEnumerable<dynamic>, int)> GetAllNotificationConfigAsync(int PageNumber, int PageSize, string? SearchTerm);
        Task<Domain.Entities.Notification.NotificationConfig> GetByIdAsync(int ShiftMasterId);
        Task<List<Domain.Entities.Notification.NotificationConfig>> GetNotificationConfigAutoCompleteAsync(string searchPattern);
        Task<bool> SoftDeleteValidation(int Id);        
        Task<bool> NotFoundAsync(int Id );
    }
}