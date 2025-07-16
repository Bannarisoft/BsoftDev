
using System.Net;
using System.Net.Mail;
using BackgroundService.Infrastructure.Configurations;
using Contracts.Events.Notifications;
using Core.Application.Common.Interfaces;
using Serilog;

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
                Log.Information("Provider '{ProviderKey}' not found in EmailSettings.", providerKey);
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
                Log.Information("✅ Email sent to {ToEmail} via {ProviderKey}", command.ToEmail, providerKey);
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "❌ Failed to send email: {ErrorMessage}", ex.Message);
                return false;
            }
        }
    }
}
