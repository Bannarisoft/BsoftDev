using MediatR;

namespace Core.Application.Common.Security
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