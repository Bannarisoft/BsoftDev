using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Contracts.Models.Email;
using MassTransit;
using Microsoft.Extensions.Options;

namespace SagaOrchestrator.Infrastructure.Consumers
{
    public class EmailEventConsumer  : IConsumer<EmailEventDto>
    {
        private readonly MailSettings _settings;
        public EmailEventConsumer(IOptions<MailSettings> options)
        {
            _settings = options.Value;
        }
        public async Task Consume(ConsumeContext<EmailEventDto> context)
        {
            var message = context.Message;

            if (_settings == null || _settings.Providers == null)
            {
                Console.WriteLine("MailSettings or Providers is NULL.");
                throw new InvalidOperationException("Mail settings not configured.");
            }

            if (!_settings.Providers.TryGetValue("Gmail", out var provider))
            {
                Console.WriteLine("Available Providers:");
                foreach (var key in _settings.Providers.Keys)
                {
                    Console.WriteLine($"- {key}");
                }

                throw new InvalidOperationException("Email provider 'Gmail' configuration not found.");
            }

            using var client = new SmtpClient(provider.Host, provider.Port)
            {
                Credentials = new NetworkCredential(provider.UserName, provider.Password),
                EnableSsl = provider.EnableSsl
            };

            var mail = new MailMessage(provider.UserName, message.ToEmail, message.Subject, message.Body);
            await client.SendMailAsync(mail);
        }
    }
}