using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces;
using Core.Domain.Common;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Serilog;

namespace BSOFT.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;
        private SmtpClient _smtpClient;
        private readonly string _templatePath;

        public EmailService(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value ?? throw new ArgumentNullException(nameof(emailSettings));
            _smtpClient = new SmtpClient();
            _templatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates", "EmailTemplate.html");
        }

        private void ConfigureSmtpClient(string provider)
        {
            if (string.IsNullOrWhiteSpace(provider))
                throw new ArgumentException("Email provider must be specified.", nameof(provider));

            if (!_emailSettings.Providers.TryGetValue(provider, out var settings) || settings == null)
                throw new InvalidOperationException($"Email settings for {provider} are not configured.");

            _smtpClient = new SmtpClient(settings.Host)
            {
                Port = settings.Port,
                EnableSsl = settings.EnableSsl,
                UseDefaultCredentials = false,
                DeliveryMethod = SmtpDeliveryMethod.Network
            };

            if (!string.IsNullOrWhiteSpace(settings.UserName) && !string.IsNullOrWhiteSpace(settings.Password))
            {
                _smtpClient.Credentials = new NetworkCredential(settings.UserName, settings.Password);
            }
        }

        public async Task<bool> SendEmailAsync(string to, string subject, string body, string provider = "Gmail")
        {
            try
            {
                ConfigureSmtpClient(provider);

                if (!_emailSettings.Providers.ContainsKey(provider))
                    throw new ArgumentException($"Invalid email provider specified: {provider}");

                var fromAddress = new MailAddress(_emailSettings.Providers[provider].UserName);
                string emailBody = await GetEmailBodyAsync(body);

                var mailMessage = new MailMessage
                {
                    From = fromAddress,
                    Subject = subject,
                    Body = emailBody,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(to);
                await _smtpClient.SendMailAsync(mailMessage);

                Log.Information("Email sent successfully using {Provider} to {Recipient}.", provider, to);
                return true;
            }
            catch (Exception ex)
            {
                Log.Error("Error sending email: {Message}", ex.Message);
                return false;
            }
        }

        private async Task<string> GetEmailBodyAsync(string bodyContent)
        {
            if (!File.Exists(_templatePath))
                throw new FileNotFoundException("Email template not found.");

            string template = await File.ReadAllTextAsync(_templatePath);
            return template.Replace("{{Content}}", bodyContent).Replace("{{Year}}", DateTime.UtcNow.Year.ToString());
        }
    }
}
