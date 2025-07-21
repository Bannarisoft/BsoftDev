using BackgroundService.Domain.Entities.Notification;

namespace BackgroundService.Application.Interfaces.Notification
{
    public interface INotificationLogger
    {
         Task LogAsync(NotificationEventLog log);
    }
}