using System.Net.Mail;
using System.Net;
using System.Text;
using Microsoft.Extensions.Options;
using Core.Application.Common.Interfaces;
using Core.Domain.Common;

public class EmailService : IEmailService
{
    private readonly EmailSettings _emailSettings;

    public EmailService(IOptions<EmailSettings> emailSettings)
    {
        _emailSettings = emailSettings.Value;
    }

   public async Task<bool> SendEmailAsync(string toEmail, string subject, string htmlContent, string provider)
    {    
        var emailConfig = _emailSettings.Providers[provider]; // Get the provider (Gmail/Zimbra)

        using var smtpClient = new SmtpClient(emailConfig.Host)
        {
            Port = emailConfig.Port,
            Credentials = new NetworkCredential(emailConfig.UserName, emailConfig.Password),
            EnableSsl = emailConfig.EnableSsl
        };

        var mailMessage = new MailMessage
        {
            From = new MailAddress(emailConfig.UserName??string.Empty),
            Subject = subject,
            Body = htmlContent,
            IsBodyHtml = true
        };

        mailMessage.To.Add(toEmail);

        await smtpClient.SendMailAsync(mailMessage);
        return true;
    }
}    

