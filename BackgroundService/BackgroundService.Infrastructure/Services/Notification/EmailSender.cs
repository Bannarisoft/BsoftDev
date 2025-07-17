using BackgroundService.Application.Interfaces;
using BackgroundService.Application.Interfaces.Notification;
using BackgroundService.Infrastructure.Configurations;
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

        public async Task<bool> SendEmailAsync(List<string> toEmails, string subject, string body, string footer, List<string>? ccEmails, List<string>? bccEmails)
        {
            try
            {
                if (toEmails == null || !toEmails.Any())
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
                    Body = $"{body}<br/><br/>{footer}",
                    IsBodyHtml = true
                };

                foreach (var toEmail in toEmails.Where(e => !string.IsNullOrWhiteSpace(e)))
                {
                    mailMessage.To.Add(toEmail.Trim());
                }

                if (!mailMessage.To.Any())
                {
                    _logger.LogWarning("⚠️ After filtering, no valid 'To' addresses found. Skipping email.");
                    return false;
                }

                if (ccEmails != null)
                {
                    foreach (var cc in ccEmails.Where(e => !string.IsNullOrWhiteSpace(e)))
                    {
                        mailMessage.CC.Add(cc.Trim());
                    }
                }

                if (bccEmails != null)
                {
                    foreach (var bcc in bccEmails.Where(e => !string.IsNullOrWhiteSpace(e)))
                    {
                        mailMessage.Bcc.Add(bcc.Trim());
                    }
                }

                _logger.LogInformation("📤 Sending email to: {Recipients}", string.Join(",", mailMessage.To.Select(t => t.Address)));

                await smtpClient.SendMailAsync(mailMessage);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Failed to send email to: {Recipients}", string.Join(",", toEmails ?? new List<string>()));
                return false;
            }
        }
    }
}
