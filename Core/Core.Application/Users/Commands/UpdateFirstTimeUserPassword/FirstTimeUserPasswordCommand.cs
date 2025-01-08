using MediatR;

namespace Core.Application.Users.Commands.UpdateFirstTimeUserPassword
{
    public class FirstTimeUserPasswordCommand : IRequest<string>
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}