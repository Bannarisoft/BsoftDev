
using FAM.Application.Common;
using MediatR;

namespace Core.Application.Common.EnvironmentSetup
{
    public class EncryptPasswordCommandHandler : IRequestHandler<EncryptPasswordCommand, string>
    {
        private readonly AesEncryptionService _encryptionService;

        public EncryptPasswordCommandHandler()
        {
            _encryptionService = new AesEncryptionService();
        }

        public Task<string> Handle(EncryptPasswordCommand request, CancellationToken cancellationToken)
        {
            var encryptedPassword = _encryptionService.Encrypt(request.Password);
            return Task.FromResult(encryptedPassword);
        }
    }
}