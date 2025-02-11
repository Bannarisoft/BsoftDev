using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces;
using Core.Application.Common.Interfaces.IUser;
using Core.Application.Users.Commands.CreateFirstTimeUserPassword;
using Core.Domain.Entities;
using MediatR;

namespace Core.Application.Users.Commands.ChangeUserPassword
{
    public class ChangeUserPasswordCommandHandler : IRequestHandler<ChangeUserPasswordCommand,ApiResponseDTO<string>>
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

        public async Task<ApiResponseDTO<string>> Handle(ChangeUserPasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await _userQueryRepository.GetByIdAsync(request.UserId);
            var passwordLog = _imapper.Map<PasswordLog>(request);
            if ( BCrypt.Net.BCrypt.Verify(request.OldPassword, user.PasswordHash))
            {
                if (request.NewPassword == request.OldPassword)
                {
                    return new ApiResponseDTO<string> { IsSuccess = false, Message = "New password cannot be the same as the old password." };
                }
                passwordLog.PasswordHash = await _ichangePassword.PasswordEncode(request.NewPassword);
                var changedPassword = await _ichangePassword.ChangePassword(request.UserId,request.NewPassword,passwordLog);

                
                return new ApiResponseDTO<string> { IsSuccess = true, Message = "Password changed successfully.", Data = changedPassword };
            }
            

            return new ApiResponseDTO<string> { IsSuccess = false, Message = "Old password is incorrect." };
        }
    }
}