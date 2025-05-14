using Contracts.Events.Notifications;
using MediatR;
using Core.Application.Common.Interfaces;

namespace BackgroundService.Application.Email
{
    public class SendEmailCommandHandler  : IRequestHandler<SendEmailCommand, bool>
    {
        private readonly IEmailService _emailService;

        public SendEmailCommandHandler(IEmailService emailService)
        {
            _emailService = emailService;
        }
        public async Task<bool> Handle(SendEmailCommand request, CancellationToken cancellationToken)
        {
            //return await _emailService.SendEmailAsync(request.ToEmail!, request.Subject!, request.HtmlContent!, request.Provider ?? "Gmail");
            return await _emailService.SendEmailAsync(request);
        }   
    }
}