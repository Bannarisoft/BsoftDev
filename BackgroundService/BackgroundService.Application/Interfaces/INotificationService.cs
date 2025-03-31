
namespace Notification.Application.Interfaces
{
    public interface INotificationService
    {
        Task<bool> SendEmailAsync(string toEmail, string subject, string htmlContent, string provider = "Gmail");
        Task SendSmsAsync(string phoneNumber, string message);
    }
}