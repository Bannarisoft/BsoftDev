
using System.Text;
using Core.Application.Common.Interfaces;
using MediatR;

namespace Core.Application.Email.Commands
{
   public class SendEmailCommandHandler : IRequestHandler<SendEmailCommand, bool>
{
    private readonly IEmailService _emailService;

    public SendEmailCommandHandler(IEmailService emailService)
    {
        _emailService = emailService;
    }

    public async Task<bool> Handle(SendEmailCommand request, CancellationToken cancellationToken)
    {        
        if (string.IsNullOrWhiteSpace(request.HtmlContent))
        {
            throw new ArgumentException("Email content cannot be empty");
        }
        await _emailService.SendEmailAsync(
            request.ToEmail ?? string.Empty,
            request.Subject ?? string.Empty,
            request.HtmlContent ,
            request.Provider ?? string.Empty            
        );
        return true;
    }
}

}