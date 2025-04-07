
namespace SagaOrchestrator.Application.Orchestration.Interfaces.IEmail
{
    public interface IEmailService
    {
         Task<bool> SendEmailAsync(string toEmail, string subject, string htmlContent);
    }
}