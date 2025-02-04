using MediatR;

namespace Core.Application.Email.Commands
{
    public class SendEmailCommand : IRequest<bool>
    {
        public string? Provider { get; set; } 
        public string? ToEmail { get; set; }
        public string? Subject { get; set; }
        public string? HtmlContent { get; set; }        
    }
}