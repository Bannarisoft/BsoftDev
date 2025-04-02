using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using BackgroundService.Application.Interfaces;
using BackgroundService.Infrastructure.Configurations;

namespace BackgroundService.Infrastructure.Services
{
    public class NotificationService : INotificationService
    {
        private readonly EmailSettings _emailSettings;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(EmailSettings emailSettings, ILogger<NotificationService> logger)
        {
            _emailSettings = emailSettings;
            _logger = logger;
        }

        public async Task<bool> SendEmailAsync(string toEmail, string subject, string htmlContent, string provider = "Gmail")
        {
            try
            {
                // ✅ Check if provider exists
                if (!_emailSettings.Providers.TryGetValue(provider, out var emailConfig))
                {
                    _logger.LogError($"Email provider '{provider}' is not configured.");
                    return false;
                }

                using var smtpClient = new SmtpClient(emailConfig.Host)
                {
                    Port = emailConfig.Port,
                    Credentials = new NetworkCredential(emailConfig.UserName, emailConfig.Password),
                    EnableSsl = emailConfig.EnableSsl
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(emailConfig.UserName),
                    Subject = subject,
                    Body = htmlContent,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(toEmail);
                await smtpClient.SendMailAsync(mailMessage);

                _logger.LogInformation($"Email sent successfully to {toEmail} via {provider}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to send email to {toEmail} via {provider}. Error: {ex.Message}");
                return false;
            }
        }

        public Task SendSmsAsync(string phoneNumber, string message)
        {
            throw new NotImplementedException();
        }
    }
}
