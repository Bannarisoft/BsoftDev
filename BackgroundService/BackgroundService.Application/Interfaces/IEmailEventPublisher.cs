
using Contracts.Models.Email;
namespace BackgroundService.Application.Interfaces
{
    public interface IEmailEventPublisher
    {
        Task PublishEmailAsync(EmailEventDto emailEvent);
    }
}