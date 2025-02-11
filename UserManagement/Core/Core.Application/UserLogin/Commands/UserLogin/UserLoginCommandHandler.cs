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

        public UserLoginCommandHandler(IUserCommandRepository userRepository,  IJwtTokenHelper jwtTokenHelper, IUserQueryRepository userQueryRepository, IMediator mediator,ILogger<UserLoginCommandHandler> logger,IUserSessionRepository userSessionRepository, IHttpContextAccessor httpContextAccessor, IIPAddressService ipAddressService, IOptions<JwtSettings> jwtSettings, ITimeZoneService timeZoneService)
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
        }

       public async Task<ApiResponseDTO<LoginResponse>> Handle(UserLoginCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling user login request for Username: {Username}", request.Username);
                        // Validate request input
            if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
            {
                _logger.LogWarning("Invalid login attempt with missing credentials.");
                return new ApiResponseDTO<LoginResponse>
                {
                    IsSuccess = false,
                    Message = "Username and password are required."
                };
            }          
            // Fetch user details
            var user = await _userQueryRepository.GetByUsernameAsync(request.Username);
            if (user == null )
            {
                _logger.LogWarning("Invalid login attempt for Username: {Username}", request.Username);
                
                return new ApiResponseDTO<LoginResponse>
                {
                    IsSuccess = false,
                    Message = "User does not exist."
                };
            }

            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                _logger.LogWarning("Invalid login attempt for Username: {Username}", request.Username);
                
                return new ApiResponseDTO<LoginResponse>
                {
                    IsSuccess = false,
                    Message = "Invalid username or password."
                };
            }


            _logger.LogInformation("User {Username} found. Retrieving roles...", request.Username);
            // Check if the user already has an active session
            var activeSession = await _userSessionRepository.GetSessionByUserIdAsync(user.UserId);

            if (activeSession != null)
            {
                // Step 2: Restrict login and notify the user
                return new ApiResponseDTO<LoginResponse>
                {
                    IsSuccess = false,
                    Message = "This username is already logged in on another machine. Please log out first."
                };
            }
        
        
           // Invalidate existing sessions if required (optional)
            await _userSessionRepository.DeactivateUserSessionsAsync(user.UserId);   
                      // Get user roles            
            var roles = await _userQueryRepository.GetUserRolesAsync(user.UserId);
            if (roles == null || roles.Count == 0)
            {
                _logger.LogWarning("No roles found for user {UserId}.", user.UserId);
                return new ApiResponseDTO<LoginResponse>
                {
                    IsSuccess = false,
                    Message = "User does not have any assigned roles."
                };
            }        
        

            _logger.LogInformation("Roles retrieved for user {UserId}: {Roles}", user.UserId, string.Join(", ", roles));
            var userlist = await _userQueryRepository.GetByIdAsync(user.UserId);
            // Generate JWT token            
 			var token = _jwtTokenHelper.GenerateToken(user.UserName,user.UserId,user.UserType, roles,user.Mobile,user.EmailId,user.UnitId,userlist.UserCompanies.ToList() , out var jti);
            var httpContext = _httpContextAccessor.HttpContext;
            var browserInfo = httpContext?.Request.Headers["User-Agent"].ToString();
            string broswerDetails = browserInfo != null ? _ipAddressService.GetUserBrowserDetails(browserInfo) : string.Empty;
            var systemTimeZoneId = _timeZoneService.GetSystemTimeZone();
            var currentTime = _timeZoneService.GetCurrentTime(systemTimeZoneId);  
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
                details: $"User '{user.UserName}' logged in successfully with roles: {token}, Roles: {roles}",
                module:"UserLogin"
            );
            await _mediator.Publish(domainEvent, cancellationToken);
            
            //Log login event via Serilog
            Log.Information("User {UserName} logged in successfully at {Time}. Roles: {Roles}", user.UserName, currentTime, string.Join(", ", roles));
            return new ApiResponseDTO<LoginResponse>
            {
                IsSuccess = true,
                Message = "Login Successful.",
                Data = new LoginResponse
                {
                    Token = token,
                    UserName = user.UserName,
                    UserRole = roles,
                    IsAuthenticated = true,
                    IsFirstTimeUser = user.IsFirstTimeUser,
                    Message = "Login Successful.",
                    CompanyId = user.CompanyId 
                }
            };
        }         
    }
}
