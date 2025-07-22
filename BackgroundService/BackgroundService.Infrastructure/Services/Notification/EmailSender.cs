using BackgroundService.Application.Interfaces;
using BackgroundService.Application.Interfaces.Notification;
using BackgroundService.Application.Notification.Common.Interfaces;
using BackgroundService.Domain.Entities.Notification;
using BackgroundService.Infrastructure.Configurations;
using Contracts.Events.Notifications;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace BackgroundService.Infrastructure.Services.Notification
{
    public class EmailSender : IEmailSender
    {
        private readonly EmailSettings _emailSettings;
        private readonly ILogger<EmailSender> _logger;
        public EmailSender(EmailSettings emailSettings, ILogger<EmailSender> logger)
        {
            _emailSettings = emailSettings;
            _logger = logger;
        }
        
        public async Task<bool> SendEmailAsync(List<string> emails, string subject, string header,string message, string footer, List<string>? CcEmails = null, List<string>? BccEmails = null, int channelId = 0, int eventTypeId = 0, int eventRuleId = 0)
        {
            try
            {                   
                if (emails == null || !emails.Any())
                {
                    _logger.LogWarning("❌ No recipients provided. Email sending aborted.");
                    return false;
                }

                var provider = _emailSettings.Providers["Gmail"]; // or "Zimbra"

                using var smtpClient = new SmtpClient(provider.Host, provider.Port)
                {
                    Credentials = new NetworkCredential(provider.UserName, provider.Password),
                    EnableSsl = provider.EnableSsl
                };

                using var mailMessage = new MailMessage
                {
                    From = new MailAddress(provider.UserName),
                    Subject = subject,
                    Body = $"{header}<br/><br/>{message}<br/><br/>{footer}",
                    IsBodyHtml = true
                };

                foreach (var toEmail in emails.Where(e => !string.IsNullOrWhiteSpace(e)))
                {
                    mailMessage.To.Add(toEmail.Trim());
                }

                if (!mailMessage.To.Any())
                {
                    _logger.LogWarning("⚠️ After filtering, no valid 'To' addresses found. Skipping email.");
                    return false;
                }

                if (CcEmails != null)
                {
                    foreach (var cc in CcEmails.Where(e => !string.IsNullOrWhiteSpace(e)))
                        mailMessage.CC.Add(cc.Trim());
                }

                if (BccEmails != null)
                {
                    foreach (var bcc in BccEmails.Where(e => !string.IsNullOrWhiteSpace(e)))
                        mailMessage.Bcc.Add(bcc.Trim());
                }

                _logger.LogInformation("📤 Sending email to: {Recipients}", string.Join(",", mailMessage.To.Select(t => t.Address)));
                await smtpClient.SendMailAsync(mailMessage);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Failed to send email to: {Recipients}", string.Join(",", emails ?? new List<string>()));
                return false;
            }
        }        
    }
}
