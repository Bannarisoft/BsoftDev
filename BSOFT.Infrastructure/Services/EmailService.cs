using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Core.Domain.Common;
using Hangfire;
using Microsoft.Extensions.Options;
using Serilog;

namespace BSOFT.Infrastructure.Services
{
    public class EmailService
    {
        private readonly SmtpClient _smtpClient;
        private readonly string _templatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates", "EmailTemplate.html");
        public EmailService(IOptions<EmailSettings> emailSettings)
        {        
            var settings = emailSettings.Value;
            _smtpClient = new SmtpClient(settings.Host)  //Gmail
            {                
                 Port = settings.Port,
                EnableSsl = settings.EnableSsl,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(settings.UserName, settings.Password),
                DeliveryMethod = SmtpDeliveryMethod.Network 
            };
        }

        public async Task<bool> SendEmailAsync(string to, string subject, string body)
        {
            try
            {
                string emailBody = await GetEmailBodyAsync(body);
                var mailMessage = new MailMessage
                {
                    From = new MailAddress("automails@bannarimills.com"),
                    //From = new MailAddress("ushadevi@bannarimills.co.in"),
                    Subject = subject,
                    Body = emailBody,
                    IsBodyHtml = true
                };
                mailMessage.To.Add(to);
                await _smtpClient.SendMailAsync(mailMessage);
                // Schedule a Hangfire job to invalidate the content after 5 minutes
                BackgroundJob.Schedule(() => InvalidateEmailContent(body), TimeSpan.FromMinutes(5));

                 return true;
             }           
            catch (Exception ex)
            {                
                Console.WriteLine($"General Error: {ex.Message}");                
                return false;         
            }
        }
        private async Task<string> GetEmailBodyAsync(string bodyContent)
        {
            if (!File.Exists(_templatePath))
                throw new FileNotFoundException("Email template not found.");

            string template = await File.ReadAllTextAsync(_templatePath);

            // Replace placeholders in the template
            return template
                .Replace("{{Content}}", bodyContent)
                .Replace("{{Year}}", DateTime.UtcNow.Year.ToString());
        }       
        public void InvalidateEmailContent(string bodyContent)
        {
           // Logic to invalidate the email content
            Console.WriteLine($"Email content invalidated: {bodyContent}");
            Log.Information("Email content invalidated: {BodyContent} at {Time}", bodyContent, DateTime.UtcNow);
        } 
    }
} 


