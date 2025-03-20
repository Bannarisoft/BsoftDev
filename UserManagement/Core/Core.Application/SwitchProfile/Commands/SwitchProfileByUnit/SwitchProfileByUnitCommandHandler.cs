using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces;
using Core.Application.Common.Interfaces.IUser;
using Core.Application.Common.Interfaces.IUserSession;
using Core.Domain.Entities;
using Core.Domain.Events;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using static Core.Domain.Enums.Common.Enums;

namespace Core.Application.SwitchProfile.Commands.SwitchProfileByUnit
{
    public class SwitchProfileByUnitCommandHandler : IRequestHandler<SwitchProfileByUnitCommand, ApiResponseDTO<SwitchProfileByUnitDTO>>
    {
        private readonly IJwtTokenHelper  _jwtTokenHelper;
        private readonly IUserSessionRepository _userSessionRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ITimeZoneService _timeZoneService;
        private readonly IMediator _mediator;
        private readonly IUserQueryRepository _userQueryRepository;
        private readonly IIPAddressService _ipAddressService;
        private readonly JwtSettings _jwtSettings;
        public SwitchProfileByUnitCommandHandler( IJwtTokenHelper jwtTokenHelper,IUserSessionRepository userSessionRepository,IHttpContextAccessor httpContextAccessor,ITimeZoneService timeZoneService,IMediator mediator,IUserQueryRepository userQueryRepository,IIPAddressService ipAddressService,IOptions<JwtSettings> jwtSettings)
        {
            _jwtTokenHelper = jwtTokenHelper;
            _userSessionRepository = userSessionRepository;
            _httpContextAccessor = httpContextAccessor;
            _timeZoneService = timeZoneService;
            _mediator = mediator;
            _userQueryRepository = userQueryRepository;
            _ipAddressService = ipAddressService;
            _jwtSettings = jwtSettings.Value;
        }
        public async Task<ApiResponseDTO<SwitchProfileByUnitDTO>> Handle(SwitchProfileByUnitCommand request, CancellationToken cancellationToken)
        {
            var userId = _ipAddressService.GetUserId();
            var groupCode = _ipAddressService.GetGroupcode();
            
            var user = await _userQueryRepository.GetByUserByUnit(userId,request.UnitId);
            if (user == null)
            {
                return new ApiResponseDTO<SwitchProfileByUnitDTO>
                {
                    IsSuccess = false,
                    Message = "User does not exist."
                };
            }
            var token = _jwtTokenHelper.GenerateToken(user.UserName,userId,user.Mobile,user.EmailId,user.IsFirstTimeUser.ToString(),user.EntityId ?? 0,groupCode,request.CompanyId,request.DivisionId,request.UnitId , out var jti);   
           
            var httpContext = _httpContextAccessor.HttpContext;
            var browserInfo = httpContext?.Request.Headers["User-Agent"].ToString();
            string broswerDetails = browserInfo != null ? _ipAddressService.GetUserBrowserDetails(browserInfo) : string.Empty;
            var systemTimeZoneId = _timeZoneService.GetSystemTimeZone();
            var currentTime = _timeZoneService.GetCurrentTime(systemTimeZoneId);  
           
            DateTime expirationTime = currentTime.AddMinutes(_jwtSettings.ExpiryMinutes);
            await _userSessionRepository.DeactivateUserSessionsAsync(userId);
            await _userSessionRepository.AddSessionAsync(new UserSessions
            {
                UserId = user.UserId,
                JwtId = jti,
                ExpiresAt =expirationTime, 
                IsActive = 1,
                CreatedAt = currentTime,
                LastActivity =currentTime,
                BrowserInfo=broswerDetails
            }); 
            //Domain Event
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "Profile",
                actionCode: "Profile",
                actionName: "User logged in",
                details: $"User Profile",
                module:"User"
            );
            await _mediator.Publish(domainEvent, cancellationToken);
            return new ApiResponseDTO<SwitchProfileByUnitDTO>
            {
                IsSuccess = true,
                Message = "Success",
                Data = new SwitchProfileByUnitDTO
                {
                    Token = token
                }
            };
        }
    }
}