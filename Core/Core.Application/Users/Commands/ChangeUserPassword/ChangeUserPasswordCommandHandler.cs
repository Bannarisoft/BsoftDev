using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.Interfaces;
using Core.Application.Common.Interfaces.IUser;
using Core.Application.Users.Commands.CreateFirstTimeUserPassword;
using Core.Domain.Entities;
using MediatR;

namespace Core.Application.Users.Commands.ChangeUserPassword
{
    public class ChangeUserPasswordCommandHandler : IRequestHandler<ChangeUserPasswordCommand,string>
    {
        private readonly IMapper _imapper;
        private readonly IUserQueryRepository _userQueryRepository;
        private readonly IChangePassword _ichangePassword;
        public ChangeUserPasswordCommandHandler(IMapper imapper, IUserQueryRepository userQueryRepository, IChangePassword ichangePassword)
        {
            _imapper = imapper;
            _userQueryRepository = userQueryRepository;
            _ichangePassword = ichangePassword;
        }

        public async Task<string> Handle(ChangeUserPasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await _userQueryRepository.GetByIdAsync(request.UserId);
            var passwordLog = _imapper.Map<PasswordLog>(request);
            if ( BCrypt.Net.BCrypt.Verify(request.OldPassword, user.PasswordHash))
            {
                if (request.NewPassword == request.OldPassword)
                {
                    return "New password cannot be the same as the old password.";
                }
                passwordLog.PasswordHash = await _ichangePassword.PasswordEncode(request.NewPassword);
                var changedPassword = await _ichangePassword.ChangePassword(request.UserId,request.NewPassword,passwordLog);

                
                return changedPassword;
            }
            

            return "Old password is incorrect.";
        }
    }
}