namespace BackgroundService.Application.Interfaces.Notification
{
    public interface ISmsSender
    {
        Task<bool> SendSmsAsync(List<string> mobileNumbers, string message);
    }
}