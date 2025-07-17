namespace BackgroundService.Application.Interfaces.Notification
{
    public interface IInAppNotifier
    {
        Task<bool> SendInAppNotificationAsync(List<int> userIds, string message, string title);
    }
}