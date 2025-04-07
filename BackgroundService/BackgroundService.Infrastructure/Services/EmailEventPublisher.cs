using MassTransit;
using Contracts.Models.Email;
using BackgroundService.Application.Interfaces;
namespace BackgroundService.Infrastructure.Services
{
    public class EmailEventPublisher  : IEmailEventPublisher
    {
        private readonly IPublishEndpoint _publishEndpoint;
        public EmailEventPublisher(IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }
        public async Task PublishEmailAsync(EmailEventDto emailEvent)
        {
            await _publishEndpoint.Publish(emailEvent);
        }
    }
}