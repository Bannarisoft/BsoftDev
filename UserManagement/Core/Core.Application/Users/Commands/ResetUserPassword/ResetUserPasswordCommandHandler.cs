using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces;
using Core.Application.Common.Interfaces.IUser;
using Core.Application.Common.Utilities;
using Core.Application.Users.Queries.GetUsers;
using Core.Domain.Entities;
using MediatR;

namespace Core.Application.Users.Commands.ResetUserPassword
{
    public class ResetUserPasswordCommandHandler : IRequestHandler<ResetUserPasswordCommand, ApiResponseDTO<string>>
    {
        private readonly IMapper _mapper;
        private readonly IChangePassword _changePassword;
        private readonly IUserQueryRepository _userQueryRepository;
        private readonly ITimeZoneService _timeZoneService;

        public ResetUserPasswordCommandHandler(
            IMapper mapper,
            IChangePassword changePassword,
            IUserQueryRepository userRepository, ITimeZoneService timeZoneService)
        {
            _mapper = mapper;
            _changePassword = changePassword;
            _userQueryRepository = userRepository;
            _timeZoneService = timeZoneService;
        }

        public async Task<ApiResponseDTO<string>> Handle(ResetUserPasswordCommand request, CancellationToken cancellationToken)
        {
            var systemTimeZoneId = _timeZoneService.GetSystemTimeZone();
            var currentTime = _timeZoneService.GetCurrentTime(systemTimeZoneId); 
            // Check if the verification code exists in ForgotPasswordCache
            if (!ForgotPasswordCache.CodeStorage.TryGetValue(request.UserName, out var verificationCodeDetails))
            {
                return new ApiResponseDTO<string> { IsSuccess = false, Message = "Verification code is invalid or has expired."};
            }

            // Validate the provided code
            if (verificationCodeDetails.Code != request.VerificationCode)
            {
                return new ApiResponseDTO<string> { IsSuccess = false, Message = "Invalid verification code."};
            }

            // Check if the verification code has expired
            if (verificationCodeDetails.ExpiryTime < currentTime)
            {
                // Remove expired code from the cache
                ForgotPasswordCache.CodeStorage.Remove(request.UserName);
                return new ApiResponseDTO<string> { IsSuccess = false, Message = "Verification code has expired."};
            }

            // Fetch the user from the database
            var user = await _userQueryRepository.GetByUsernameAsync(request.UserName);
            if (user == null)
            {
                return new ApiResponseDTO<string> { IsSuccess = false, Message = "User not found."};
            }

            // Ensure the new password does not match the old password
            if (BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return new ApiResponseDTO<string> { IsSuccess = false, Message = "Your new password cannot be the same as the old password. Please choose a different password."};
            }
           
            // Update the user's password
            var passwordLog = new PasswordLogDTO
            {
                UserId = user.UserId,
                UserName = user.UserName,
                PasswordHash = await _changePassword.PasswordEncode(request.Password),
                CreatedAt = currentTime
            };
            var passwordLogMap = _mapper.Map<PasswordLog>(passwordLog);
            var result = await _changePassword.ResetUserPassword(user.UserId,passwordLogMap);
            bool log = await _changePassword.PasswordLog(passwordLogMap);
            if (!string.IsNullOrEmpty(result) && log)
            {
                // Remove the verification code after successful password reset
                ForgotPasswordCache.CodeStorage.Remove(request.UserName);

                return new ApiResponseDTO<string> { IsSuccess = true, Message = "Password reset successfully."};
            }

            return new ApiResponseDTO<string> { IsSuccess = false, Message = "Failed to reset the password. Please try again."};
        }
    }
}
