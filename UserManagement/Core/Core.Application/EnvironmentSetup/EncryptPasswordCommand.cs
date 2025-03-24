using MediatR;

namespace Core.Application.EnvironmentSetup
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