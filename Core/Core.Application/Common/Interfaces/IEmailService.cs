namespace Core.Application.Common.Interfaces
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(string to, string subject, string body, string provider = "Gmail");
    }
}