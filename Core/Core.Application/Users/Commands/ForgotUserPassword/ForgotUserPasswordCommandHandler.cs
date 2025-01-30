using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces;
using Core.Application.Common.Interfaces.INotifications;
using Core.Application.Common.Interfaces.IUser;
using Core.Application.Common.Utilities;
using Core.Application.Users.Commands.ForgotUserPassword;
using Core.Domain.Events;
using Core.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using Core.Application.Users.Queries.GetUsers;

namespace Core.Application.Users.Commands.ForgotUserPassword
{
    public class ForgotUserPasswordCommandHandler : IRequestHandler<ForgotUserPasswordCommand, List<ApiResponseDTO<ForgotPasswordResponse>>>
    {
        private readonly IMapper _mapper;
        private readonly IUserQueryRepository _userQueryRepository;
        private readonly IChangePassword _changePasswordService;
        private readonly IMediator _mediator;
        private readonly INotificationsQueryRepository _notificationsQueryRepository;
        private readonly ILogger<ForgotUserPasswordCommandHandler> _logger;
           private readonly IEmailService _emailService;
        private readonly ISmsService _smsService;

        public ForgotUserPasswordCommandHandler(
            IUserQueryRepository userQueryRepository,
            IMapper mapper,
            IChangePassword changePasswordService,
            ILogger<ForgotUserPasswordCommandHandler> logger,
            INotificationsQueryRepository notificationsQueryRepository,
            IMediator mediator,IEmailService emailService,ISmsService smsService)
        {
            _userQueryRepository = userQueryRepository;
            _mapper = mapper;
            _changePasswordService = changePasswordService;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _notificationsQueryRepository = notificationsQueryRepository;
            _mediator = mediator;
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            _smsService = smsService ?? throw new ArgumentNullException(nameof(smsService));
        }

        public async Task<List<ApiResponseDTO<ForgotPasswordResponse>>> Handle(ForgotUserPasswordCommand request, CancellationToken cancellationToken)
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(request.UserName))
            {
                _logger.LogWarning("Username is required.");
                return CreateErrorResponse("Username is required.");
            }

            // Fetch user details
            var user = await _userQueryRepository.GetByUsernameAsync(request.UserName);
            if (user == null)
            {
                _logger.LogWarning($"Username '{request.UserName}' does not exist.");
                return CreateErrorResponse("Username does not exist.");
            }

            if (!user.IsActive)
            {
                _logger.LogWarning($"Username '{request.UserName}' is inactive. Contact admin.");
                return CreateErrorResponse("The account is inactive. Contact admin.");
            }

            if (string.IsNullOrEmpty(user.Mobile) || string.IsNullOrEmpty(user.EmailId))
            {
                _logger.LogWarning($"Username '{request.UserName}' does not have a registered email or mobile.");
                return CreateErrorResponse("Mobile number and email address are required for verification code.");
            }

            // Generate verification code
            string verificationCode = await _changePasswordService.GenerateVerificationCode(6);
            int expiryMinutes = await _notificationsQueryRepository.GetResetCodeExpiryMinutes();

            // Store verification code in memory
            ForgotPasswordCache.CodeStorage[user.UserName] = new VerificationCodeDetails
            {
                Code = verificationCode,
                ExpiryTime = DateTime.UtcNow.AddMinutes(expiryMinutes)
            };

            // Schedule Hangfire job to remove the code after expiry
            Hangfire.BackgroundJob.Schedule(
            () => ForgotPasswordCache.RemoveVerificationCode(user.UserName), 
            TimeSpan.FromMinutes(expiryMinutes)
            );

            //Email
            bool emailSent = false;
            emailSent = await _emailService.SendEmailAsync(
            request.Email,
            "Password Reset Verification Code",                
            $"Dear {request.UserName},<br/><br/>We have received a request to reset your password.<br/><br/>To proceed with resetting your password, please use the verification code below: <br/><strong>Username:</strong>{request.UserName} <br/><strong>Verification Code:</strong>  {verificationCode} <br/><br/><br/><strong>Note:Verification Code will expire in {expiryMinutes} minutes</strong> <br/><br/><p>Regards,<br/>Bannari Mills Team </p>",
            "Gmail"
            );
            if (emailSent)
            {
                _logger.LogInformation("Verification Code email sent to {Email}.", request.Email);
            }
            else
            {
                _logger.LogWarning("Failed to send Verification Code notification email to {Email}.", request.Email);
            }
            _logger.LogInformation("Verification Code sent successfully.", request.UserName);

            //SMS
            bool smsSent = false;
            try
            {
                smsSent = await _smsService.SendSmsAsync(
                    request.Mobile,                     
                    $"Dear {request.UserName}, We received a request to reset your password. Use the verification code below to proceed:Code:{verificationCode}, This code is valid for {expiryMinutes} minutes."
                );
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred while sending the SMS: {ErrorMessage}", ex.Message);
            }

            if (smsSent)
            {
                _logger.LogInformation("Login notification SMS sent to {Mobile}.", request.Mobile);
            }
            else
            {
                _logger.LogWarning("Failed to send login notification SMS to {Mobile}.", request.Mobile);
            } 
            
            // Publish domain event for logging purposes
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "ResetUserPassword",
                actionCode: verificationCode,
                actionName: request.UserName,
                details: $"Username '{request.UserName}' requested a password reset. Verification Code: {verificationCode}",
                module: "ResetUserPassword"
            );
            await _mediator.Publish(domainEvent, cancellationToken);

            // Create response
            var userDto = _mapper.Map<UserDto>(user);
            var responseDto = new ForgotPasswordResponse
            {
                Message = $"Verification code sent to your registered email address {userDto.EmailId} and mobile number {userDto.Mobile}.",
                Email = userDto.EmailId,
                Mobile = userDto.Mobile,
                VerificationCode = verificationCode,
                PasswordResetCodeExpiryMinutes = expiryMinutes
            };

            _logger.LogInformation($"Verification code sent successfully for username '{userDto.UserName}'.");
            return new List<ApiResponseDTO<ForgotPasswordResponse>>
            {
                ApiResponseDTO<ForgotPasswordResponse>.Success(responseDto)
            };
        }

        private static List<ApiResponseDTO<ForgotPasswordResponse>> CreateErrorResponse(string message)
        {
            return new List<ApiResponseDTO<ForgotPasswordResponse>>
            {
                new ApiResponseDTO<ForgotPasswordResponse>
                {
                    IsSuccess = false,
                    Message = message
                }
            };
        }
    }
}
