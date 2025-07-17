
    namespace BackgroundService.Application.Interfaces.Notification
    {
        public interface IEmailSender
        {
             Task<bool> SendEmailAsync(List<string> emails, string subject, string message, string footer,List<string>? CcEmails = null, List<string>? BccEmails = null);
        }
    }