using System.Security.Cryptography;
using System.Text;
using Core.Application.Common.Interfaces;
using MediatR;
using Core.Application.Common.Interfaces.IUser;
using Core.Application.Common.Interfaces.IUserSession;
using Core.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Core.Application.UserLogin.Commands.UserLogin
{
    public class UserLoginCommandHandler : IRequestHandler<UserLoginCommand, LoginResponse>
    {
        private readonly IUserCommandRepository _userRepository;
        private readonly IUserQueryRepository _userQueryRepository;
        private readonly IJwtTokenHelper  _jwtTokenHelper;
        private readonly IUserSessionRepository _userSessionRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IIPAddressService _ipAddressService;
         private readonly JwtSettings _jwtSettings;
        public UserLoginCommandHandler(IUserCommandRepository userRepository,  IJwtTokenHelper jwtTokenHelper, IUserQueryRepository userQueryRepository, IUserSessionRepository userSessionRepository, IHttpContextAccessor httpContextAccessor, IIPAddressService ipAddressService, IOptions<JwtSettings> jwtSettings)
        {
            _userRepository = userRepository;
            _userQueryRepository = userQueryRepository;
            _jwtTokenHelper = jwtTokenHelper;
            _userSessionRepository = userSessionRepository;
            _httpContextAccessor = httpContextAccessor;
            _ipAddressService = ipAddressService;
            _jwtSettings = jwtSettings.Value; 
        }

       public async Task<LoginResponse> Handle(UserLoginCommand request, CancellationToken cancellationToken)
        {            
            var user = await _userQueryRepository.GetByUsernameAsync(request.Username);

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return new LoginResponse
                {
                    IsAuthenticated = false,
                    IsFirstTimeUser = false,
                    Message = "Invalid username or password."
                };
                // throw new UnauthorizedAccessException("Invalid username or password.");
            }

            // Check if the user already has an active session
                var activeSession = await _userSessionRepository.GetSessionByUserIdAsync(user.UserId);

                if (activeSession != null)
                {
                    // Step 2: Restrict login and notify the user
                    return new LoginResponse
                    {
                        IsAuthenticated = false,
                        Message = "This username is already logged in on another machine. Please log out first."
                    };
                }


            // Invalidate existing sessions if required (optional)
            await _userSessionRepository.DeactivateUserSessionsAsync(user.UserId);
             // Get user roles
            var roles = await _userQueryRepository.GetUserRolesAsync(user.UserId);
            // Generate JWT token
            var token = _jwtTokenHelper.GenerateToken(user.UserName,user.UserId,user.UserType, roles, out var jti);

            var httpContext = _httpContextAccessor.HttpContext;
            var browserInfo = httpContext?.Request.Headers["User-Agent"].ToString();
            string broswerDetails = browserInfo != null ? _ipAddressService.GetUserBrowserDetails(browserInfo) : string.Empty;
            DateTime utcNow = DateTime.UtcNow;
            TimeZoneInfo indianZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");            
            DateTime indianTime = TimeZoneInfo.ConvertTimeFromUtc(utcNow, indianZone);
            DateTime expirationTime = indianTime.AddMinutes(_jwtSettings.ExpiryMinutes);
            await _userSessionRepository.AddSessionAsync(new UserSessions
            {
                UserId = user.UserId,
                JwtId = jti,
                ExpiresAt =expirationTime, // Token expiry
                IsActive = 1,
                CreatedAt = indianTime,
                LastActivity =indianTime,
                BrowserInfo=broswerDetails
            });


            return new LoginResponse
            {
                Token = token,
                UserName = user.UserName,
                UserRole = roles,
                IsAuthenticated = true,
                IsFirstTimeUser = user.IsFirstTimeUser,
                Message = "Login Successful."
            };

        }

        private bool VerifyPassword(string password, string storedHash)
        {
            using var sha256 = SHA256.Create();
            var hashedPassword = Convert.ToBase64String(sha256.ComputeHash(Encoding.UTF8.GetBytes(password)));
            return hashedPassword == storedHash;
        }
    }
}
