using System.Security.Cryptography;
using System.Text;
using Core.Application.Common.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging; // This is where the ILogger interface is defined
using Core.Application.Common.Interfaces.IUser;
using Core.Domain.Events;
using Serilog;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IUserSession;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Core.Domain.Entities;
using System.Collections.Concurrent;
using Hangfire;
using Core.Application.Common.Interfaces.ICompanySettings;
using static Core.Domain.Enums.Common.Enums;
namespace Core.Application.UserLogin.Commands.UserLogin
{
    public class UserLoginCommandHandler : IRequestHandler<UserLoginCommand, ApiResponseDTO<LoginResponse>>
    {
        private readonly IUserCommandRepository _userRepository;
        private readonly IUserQueryRepository _userQueryRepository;
        private readonly IJwtTokenHelper  _jwtTokenHelper;
        private readonly IUserSessionRepository _userSessionRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IIPAddressService _ipAddressService;
         private readonly JwtSettings _jwtSettings;
        private readonly ILogger<UserLoginCommandHandler> _logger;
        private readonly IMediator _mediator;
        private readonly ITimeZoneService _timeZoneService;
        private static readonly ConcurrentDictionary<string, UserLockoutInfo> _userLockoutInfo = new();
        private readonly ICompanyQuerySettings _companyQuerySettings;

        public UserLoginCommandHandler(IUserCommandRepository userRepository,  IJwtTokenHelper jwtTokenHelper, IUserQueryRepository userQueryRepository, IMediator mediator,ILogger<UserLoginCommandHandler> logger,IUserSessionRepository userSessionRepository, IHttpContextAccessor httpContextAccessor, IIPAddressService ipAddressService, IOptions<JwtSettings> jwtSettings, ITimeZoneService timeZoneService, ICompanyQuerySettings companyQuerySettings)
        {
            _userRepository = userRepository;
            _userQueryRepository = userQueryRepository;
            _jwtTokenHelper = jwtTokenHelper;
            _userSessionRepository = userSessionRepository;
            _httpContextAccessor = httpContextAccessor;
            _ipAddressService = ipAddressService;
            _jwtSettings = jwtSettings.Value;
             _mediator = mediator; 
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));   
            _timeZoneService = timeZoneService;  
            _companyQuerySettings = companyQuerySettings;       
        }

       public async Task<ApiResponseDTO<LoginResponse>> Handle(UserLoginCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling user login request for Username: {Username}", request.Username);

                        
            var systemTimeZoneId = _timeZoneService.GetSystemTimeZone();
            var currentTime = _timeZoneService.GetCurrentTime(systemTimeZoneId);
            
            var user = await _userQueryRepository.GetByUsernameAsync(request.Username);
          

            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
              
                _logger.LogWarning("Invalid login attempt for Username: {Username}", request.Username);

              var (remainingAttempts, lockoutTime) = await loginAttemptSession(user.UserName, currentTime);

                if (lockoutTime > 0)
                {
                    return new ApiResponseDTO<LoginResponse>
                    {
                        IsSuccess = false,
                        Message = $"User is locked. Try again after {lockoutTime:G}."
                    };
                }                
                return new ApiResponseDTO<LoginResponse>
                {
                    IsSuccess = false,
                    Message = $"Invalid username or password.You have {remainingAttempts} attempts remaining."
                };
            }

            // await _userSessionRepository.DeactivateUserSessionsAsync(user.UserId);   
                  
        

            
            // Generate JWT token            
 			var token = _jwtTokenHelper.GenerateToken(user.UserName,user.UserId,user.Mobile,user.EmailId,user.IsFirstTimeUser.ToString(),user.EntityId ?? 0,user.UserGroup.GroupCode,0,0,0, out var jti);
            var httpContext = _httpContextAccessor.HttpContext;
            var browserInfo = httpContext?.Request.Headers["User-Agent"].ToString();
            string broswerDetails = browserInfo != null ? _ipAddressService.GetUserBrowserDetails(browserInfo) : string.Empty;

            DateTime expirationTime = currentTime.AddMinutes(_jwtSettings.ExpiryMinutes);
            await _userSessionRepository.AddSessionAsync(new UserSessions
            {
                UserId = user.UserId,
                JwtId = jti,
                ExpiresAt =expirationTime, // Token expiry
                IsActive = 1,
                CreatedAt = currentTime,
                LastActivity =currentTime,
                BrowserInfo=broswerDetails
            });           
             _logger.LogInformation("JWT token generated for Username: {Username}", user.UserName);
            
            //Domain Event
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "Login",
                actionCode: user.UserName,
                actionName: "User logged in",
                details: $"User '{user.UserName}' logged in successfully with roles: {token}",
                module:"UserLogin"
            );
            await _mediator.Publish(domainEvent, cancellationToken);
            
            //Log login event via Serilog
            Log.Information("User {UserName} logged in successfully at {Time}. Roles: {Roles}", user.UserName, currentTime);

            _userLockoutInfo.TryRemove(request.Username, out _);

            return new ApiResponseDTO<LoginResponse>
            {
                IsSuccess = true,
                Message = "Login Successful.",
                Data = new LoginResponse
                {
                    Token = token,
                    IsFirstTimeUser = user.IsFirstTimeUser,
                    Message = "Login Successful."
                }
            };
        } 
      
        public async Task<(int,int)> loginAttemptSession(string username, DateTime currentTime)
        {
              
                if (!_userLockoutInfo.ContainsKey(username))
                {
                    _userLockoutInfo[username] = new UserLockoutInfo { Attempts = 0, IsLocked = false };
                }
                var userInfo = _userLockoutInfo[username];
                userInfo.Attempts++;
      
                       
                var companySettings = await _companyQuerySettings.BeforeLoginGetUserCompanySettings(username);
                
                
                int remainingAttempts = companySettings.FailedLoginAttempts - userInfo.Attempts;

                if (remainingAttempts > 0)
                {
                    return (remainingAttempts,0);
                }
                if (userInfo.Attempts >= companySettings.FailedLoginAttempts)
                {
                    // Lock the user
                    userInfo.IsLocked = true; 
                    userInfo.UnlockTime = currentTime.AddMinutes(companySettings.AutoReleaseTime);
                    _userRepository.lockUser(username);
                    // Schedule Hangfire job to unlock user
                    BackgroundJob.Schedule<IUserCommandRepository>(service =>service.UnlockUser(username), TimeSpan.FromMinutes(companySettings.AutoReleaseTime));

                    _logger.LogWarning("User {Username} is locked due to too many invalid login attempts.", username);

                    
                    return (0,companySettings.AutoReleaseTime);
                }
                 return (0,0);
        }        
    }
}
