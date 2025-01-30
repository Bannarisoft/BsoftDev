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
using Microsoft.Extensions.Logging;
using Core.Application.Common.Interfaces.INotifications;
using Core.Domain.Events;


namespace Core.Application.Users.Commands.ForgotUserPassword
{
    public class ForgotUserPasswordCommandHandler : IRequestHandler<ForgotUserPasswordCommand, List<ApiResponseDTO<ForgotPasswordResponse>>>
    {
         private readonly IMapper _imapper;
         private readonly IUserQueryRepository _userQueryRepository;
         private readonly IChangePassword _ichangePassword;
         private readonly IMediator _Imediator;
         private readonly INotificationsQueryRepository _INotificationsQueryRepository;

         
        private readonly ILogger<ForgotUserPasswordCommandHandler> _logger;

        public ForgotUserPasswordCommandHandler(IUserQueryRepository userQueryRepository, IMapper imapper, IChangePassword ichangePassword,ILogger<ForgotUserPasswordCommandHandler> logger,INotificationsQueryRepository INotificationsQueryRepository,IMediator Imediator)
        {
            _userQueryRepository = userQueryRepository;
            _imapper = imapper;
            _ichangePassword = ichangePassword;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _INotificationsQueryRepository = INotificationsQueryRepository;
            _Imediator = Imediator;
        }

        public async Task<List<ApiResponseDTO<ForgotPasswordResponse>>> Handle(ForgotUserPasswordCommand request, CancellationToken cancellationToken) 
        {
           if (string.IsNullOrWhiteSpace(request.UserName) || string.IsNullOrEmpty(request.UserName) || string.IsNullOrEmpty(request.UserName.Trim()))
            {
            _logger.LogWarning("Username required." + request.UserName);   
            return new List<ApiResponseDTO<ForgotPasswordResponse>>
            {
            new ApiResponseDTO<ForgotPasswordResponse>
            {
                IsSuccess = false,
                Message = "Username required.",
            }
            };
            }
            var user = await _userQueryRepository.GetByUsernameAsync(request.UserName);
            if (user == null)
            {
            _logger.LogWarning("Username does not exists." + request.UserName);
            return new List<ApiResponseDTO<ForgotPasswordResponse>>
            {
            new ApiResponseDTO<ForgotPasswordResponse>
            {
                IsSuccess = false,
                Message = "Username does not exists.",
            }
            };
        }
            else if(user.IsActive==0)
            {
                _logger.LogWarning("Current Username is inactive contact admin ." + request.UserName);
                return new List<ApiResponseDTO<ForgotPasswordResponse>>
            {
            new ApiResponseDTO<ForgotPasswordResponse>
            {
                IsSuccess = false,
                Message = "Current Username is inactive contact admin .",
                
            }
        };
    }
    else if(user.Mobile==null || user.EmailId==null)
    {
        _logger.LogWarning("For Verfication Code Sending Mobile Number and Email Id is Required for Username." + request.UserName);
        return new List<ApiResponseDTO<ForgotPasswordResponse>>
        {
            new ApiResponseDTO<ForgotPasswordResponse>
            {
                IsSuccess = false,
                Message = "For Verfication Code Sending Mobile Number and Email Id is Required for Username.",
            }
        };
    }
    else
    {

        var userDto = _imapper.Map<UserDto>(user);
        var verificationCode = await _ichangePassword.GenerateVerificationCode(6);
        var PasswordResetCodeExpiryMinutes= await _INotificationsQueryRepository.GetResetCodeExpiryMinutes();

          //Domain Event
                  var domainEvent = new AuditLogsDomainEvent(
                      actionDetail: "ResetUserPassword",
                      actionCode: verificationCode,
                      actionName: request.UserName,
                      details: $"UserName '{request.UserName}' has requested for password reset. Verfication Code Generated: {verificationCode}",
                      module:"ResetUserPassword"
                  );
                  await _Imediator.Publish(domainEvent, cancellationToken);
        
          // Create the response DTO
    var responseDto = new ForgotPasswordResponse
    {

        Message = $"Verification code sent to your registered email address {userDto.EmailId} and mobile number {userDto.Mobile}.",
        Email = userDto.EmailId,
        Mobile = userDto.Mobile,
        VerificationCode = verificationCode,
        PasswordResetCodeExpiryMinutes=PasswordResetCodeExpiryMinutes
    };

     _logger.LogInformation("Verification Code sent successfully." + userDto.UserName);
     return new List<ApiResponseDTO<ForgotPasswordResponse>>
    {
        ApiResponseDTO<ForgotPasswordResponse>.Success(responseDto)
    };
       
    }
}        
    }
}

