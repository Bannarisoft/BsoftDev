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
using Core.Domain.Enums.Common;

namespace Core.Application.Users.Commands.ForgotUserPassword
{
    public class ForgotUserPasswordCommandHandler : IRequestHandler<ForgotUserPasswordCommand, ApiResponseDTO<ForgotPasswordResponse>>
    {
        private readonly IMapper _mapper;
        private readonly IUserQueryRepository _userQueryRepository;
        private readonly IChangePassword _changePasswordService;
        private readonly IMediator _mediator;
        private readonly INotificationsQueryRepository _notificationsQueryRepository;
        private readonly ILogger<ForgotUserPasswordCommandHandler> _logger;
           private readonly IEmailService _emailService;
        private readonly ISmsService _smsService;
        private readonly ITimeZoneService _timeZoneService;

        public ForgotUserPasswordCommandHandler(
            IUserQueryRepository userQueryRepository,
            IMapper mapper,
            IChangePassword changePasswordService,
            ILogger<ForgotUserPasswordCommandHandler> logger,
            INotificationsQueryRepository notificationsQueryRepository,
            IMediator mediator,IEmailService emailService,ISmsService smsService, ITimeZoneService timeZoneService)
        {
            _userQueryRepository = userQueryRepository;
            _mapper = mapper;
            _changePasswordService = changePasswordService;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _notificationsQueryRepository = notificationsQueryRepository;
            _mediator = mediator;
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            _smsService = smsService ?? throw new ArgumentNullException(nameof(smsService));
            _timeZoneService = timeZoneService;    
        }

        public async Task<ApiResponseDTO<ForgotPasswordResponse>> Handle(ForgotUserPasswordCommand request, CancellationToken cancellationToken)
        {
           

            // Fetch user details
            var user = await _userQueryRepository.GetByUsernameAsync(request.UserName);
           

            // Generate verification code
            string verificationCode = await _changePasswordService.GenerateVerificationCode(6);
            int expiryMinutes = await _notificationsQueryRepository.GetResetCodeExpiryMinutes();
            var systemTimeZoneId = _timeZoneService.GetSystemTimeZone();
            var currentTime = _timeZoneService.GetCurrentTime(systemTimeZoneId); 

            // Store verification code in memory
            ForgotPasswordCache.CodeStorage[request.UserName] = new VerificationCodeDetails
            {
                Code = verificationCode,
                ExpiryTime = currentTime.AddMinutes(expiryMinutes)
            };

            // Schedule Hangfire job to remove the code after expiry
            Hangfire.BackgroundJob.Schedule(
            () => ForgotPasswordCache.RemoveVerificationCode(request.UserName), 
            TimeSpan.FromMinutes(expiryMinutes)
            );

        
            bool smsSent = false;          
            smsSent = await _smsService.SendSmsAsync(
                user.EmailId,                     
                $"Dear {request.UserName}, We received a request to reset your password. Use the verification code below to proceed:Code:{verificationCode}, This code is valid for {expiryMinutes} minutes."
            );          

            if (smsSent)
            {
                _logger.LogInformation("Login notification SMS sent to {Mobile}.",  user.Mobile);
            }
            else
            {
                _logger.LogWarning("Failed to send login notification SMS to {Mobile}.", user.Mobile);
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
            return new ApiResponseDTO<ForgotPasswordResponse> { IsSuccess = true, Data = responseDto };
        }      
    }
}
