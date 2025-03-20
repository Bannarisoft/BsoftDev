using MediatR;

namespace Core.Application.Security
{
    public class EncryptPasswordCommand : IRequest<string>
    {
        public string Password { get; set; }
        public EncryptPasswordCommand(string password)
        {
            Password = password;
        }
    }
}