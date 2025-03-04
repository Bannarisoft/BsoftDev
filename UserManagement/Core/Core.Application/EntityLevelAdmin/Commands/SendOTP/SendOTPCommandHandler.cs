using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces;
using Core.Application.Common.Interfaces.INotifications;
using Core.Application.Common.Utilities;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.EntityLevelAdmin.Commands.SendOTP
{
    public class SendOTPCommandHandler : IRequestHandler<SendOTPCommand, ApiResponseDTO<SendOTPDTO>>
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly IChangePassword _changePasswordService;
        private readonly INotificationsQueryRepository _notificationsQueryRepository;
        private readonly ITimeZoneService _timeZoneService;
        public SendOTPCommandHandler(IMediator mediator, IMapper mapper, IChangePassword changePasswordService, INotificationsQueryRepository notificationsQueryRepository, ITimeZoneService timeZoneService)
        {
            _mediator = mediator;
            _mapper = mapper;
            _changePasswordService = changePasswordService;
            _notificationsQueryRepository = notificationsQueryRepository;
            _timeZoneService = timeZoneService;
        }

        public async Task<ApiResponseDTO<SendOTPDTO>> Handle(SendOTPCommand request, CancellationToken cancellationToken)
        {
            string verificationCode = await _changePasswordService.GenerateVerificationCode(6);
            int expiryMinutes = await _notificationsQueryRepository.GetResetCodeExpiryMinutes();
            var systemTimeZoneId = _timeZoneService.GetSystemTimeZone();
            var currentTime = _timeZoneService.GetCurrentTime(systemTimeZoneId); 

             ForgotPasswordCache.CodeStorage[request.Email] = new VerificationCodeDetails
            {
                Code = verificationCode,
                ExpiryTime = currentTime.AddMinutes(expiryMinutes)
            };

             Hangfire.BackgroundJob.Schedule(
            () => ForgotPasswordCache.RemoveVerificationCode(request.Email), 
            TimeSpan.FromMinutes(expiryMinutes)
            );

             var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "SendOTP to Entity Admin",
                actionCode: "OTP",
                actionName: "Genrate OTP",
                details: $"Username '{request.Email}' requested a password reset. Verification Code: {verificationCode}",
                module: "User"
            );

            await _mediator.Publish(domainEvent, cancellationToken);

            return new ApiResponseDTO<SendOTPDTO>()
            {
                IsSuccess = true,
                Message = "OTP sent successfully.",
                Data = new SendOTPDTO 
                { 
                    Email = request.Email,
                    PasswordResetCodeExpiryMinutes = expiryMinutes,
                    VerificationCode = verificationCode
                }
            };
        }
    }
}