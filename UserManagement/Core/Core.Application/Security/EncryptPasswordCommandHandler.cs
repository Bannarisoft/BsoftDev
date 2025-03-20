using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common;
using MediatR;

namespace Core.Application.Security
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