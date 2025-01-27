using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Core.Application.Common.HttpResponse;
using AutoMapper;
using Core.Application.Common.Interfaces.IUser;
using Core.Application.Users.Queries.GetUsers;
using Core.Application.Common.Interfaces;

namespace Core.Application.Users.Commands.ForgotUserPassword
{
    public class ForgotUserPasswordCommandHandler : IRequestHandler<ForgotUserPasswordCommand, List<ApiResponseDTO<UserDto>>>
    {
         private readonly IMapper _imapper;
         private readonly IUserQueryRepository _userQueryRepository;
         private readonly IChangePassword _ichangePassword;

        public ForgotUserPasswordCommandHandler(IUserQueryRepository userQueryRepository, IMapper imapper, IChangePassword ichangePassword)
        {
            _userQueryRepository = userQueryRepository;
            _imapper = imapper;
            _ichangePassword = ichangePassword;
        }
         public async Task<List<ApiResponseDTO<UserDto>>> Handle(ForgotUserPasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await _userQueryRepository.GetByUsernameAsync(request.UserName);

            if (user == null)
            {
                return new List<ApiResponseDTO<UserDto>>
                {
                    new ApiResponseDTO<UserDto>
                    {
                        IsSuccess = false,
                        Message = "Username does not exists.",
                       
                    }
                };
            }
            else if(user.IsActive==false)
            {
                return new List<ApiResponseDTO<UserDto>>
                {
                    new ApiResponseDTO<UserDto>
                    {
                        IsSuccess = false,
                        Message = "Current Username is inactive contact admin .",
                        
                    }
                };
            }
            else if(user.Mobile==null || user.EmailId==null)
            {
                return new List<ApiResponseDTO<UserDto>>
                {
                    new ApiResponseDTO<UserDto>
                    {
                        IsSuccess = false,
                        Message = "For Verfication Code Sending Mobile Number and Email Id is Required for Username.",
                        
                    }
                };
            }
             var userDto = _imapper.Map<UserDto>(user);
             var verificationCode = await _ichangePassword.GenerateVerificationCode(6);

            return new List<ApiResponseDTO<UserDto>>
            {
                new ApiResponseDTO<UserDto>
                {
                    IsSuccess = true,
                    Message = $"Verification code sent to your registered email address {userDto.EmailId} and mobile number {userDto.Mobile} and Verification code is {verificationCode}. Please check your email and SMS for the verification code.",
              
                }
            };
        }
}
}

