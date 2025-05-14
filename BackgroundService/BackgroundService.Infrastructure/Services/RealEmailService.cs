
using System.Net;
using System.Net.Mail;
using BackgroundService.Infrastructure.Configurations;
using Contracts.Events.Notifications;
using Core.Application.Common.Interfaces;

namespace BackgroundService.Infrastructure.Services
{
    public class RealEmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;

        public RealEmailService(EmailSettings emailSettings)
        {
            _emailSettings = emailSettings;
        }

        public async Task<bool> SendEmailAsync(SendEmailCommand command)
        {
            var providerKey = string.IsNullOrEmpty(command.Provider) ? "Gmail" : command.Provider;
            if (!_emailSettings.Providers.TryGetValue(providerKey, out var provider))
            {
                Console.WriteLine($"Provider '{providerKey}' not found in EmailSettings.");
                return false;
            }
            try
            {
                var smtpClient = new SmtpClient(provider.Host)
                {
                    Port = provider.Port,
                    Credentials = new NetworkCredential(provider.UserName, provider.Password),
                    EnableSsl = provider.EnableSsl,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false
                };

                var mail = new MailMessage
                {
                    From = new MailAddress(provider.UserName ?? string.Empty),
                    Subject = command.Subject,
                    Body = command.HtmlContent,
                    IsBodyHtml = true
                };

                mail.To.Add(command.ToEmail!);

                await smtpClient.SendMailAsync(mail);
                Console.WriteLine($"✅ Email sent to {command.ToEmail} via {providerKey}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Failed to send email: {ex.Message}");
                return false;
            }
        }
    }
}
