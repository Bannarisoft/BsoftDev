using Core.Application.UserLogin.Commands.UserLogin;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using FluentValidation;
using Core.Application.Common.Interfaces.IUserSession;
using Microsoft.AspNetCore.Authorization;
using UserManagement.Infrastructure.Services;
using System.Collections.Concurrent;
using Hangfire;
using Core.Application.AdminSecuritySettings.Queries.GetAdminSecuritySettingsById;
using Core.Application.Common.Interfaces.IUser;
using Infrastructure;
using Core.Application.Common.Interfaces;

namespace UserManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class AuthController : ControllerBase
    {
        private static readonly ConcurrentDictionary<string, UserLockoutInfo> _userLockoutInfo = new();
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;        
        private readonly IValidator<UserLoginCommand> _userLoginCommandValidator;
        private readonly ILogger<AuthController> _logger;
        private readonly IUserSessionRepository _userSessionRepository;        
        private readonly IUserQueryRepository _userQueryRepository;
        private readonly ITimeZoneService _timeZoneService;

        public AuthController(IMediator mediator,IValidator<UserLoginCommand> userLoginCommandValidator, IMapper mapper, ILogger<AuthController> logger,IUserSessionRepository userSessionRepository, IUserQueryRepository userQueryRepository, ITimeZoneService timeZoneService)
        {
            _mediator = mediator;
            _userLoginCommandValidator = userLoginCommandValidator;
            _mapper = mapper;
            _logger = logger;
            _userSessionRepository = userSessionRepository;            
            _userQueryRepository = userQueryRepository; 
            _timeZoneService = timeZoneService;           
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {  
            var systemTimeZoneId = _timeZoneService.GetSystemTimeZone();
            var currentTime = _timeZoneService.GetCurrentTime(systemTimeZoneId);
            string username = request.Username;            
   
            // Check if the user is locked
            if (_userLockoutInfo.TryGetValue(username, out var lockoutInfo) && lockoutInfo.IsLocked)
            {
                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = $"User is locked. Try again after {lockoutInfo.UnlockTime:G}."
                });
            }
            // Map the incoming request to a UserLoginCommand
            var command = _mapper.Map<UserLoginCommand>(request);         
            // Process the command using Mediator
            var response = await _mediator.Send(command);   
            if (response.IsSuccess)
            {                
                _logger.LogInformation("User {Username} authenticated successfully.", command.Username);

                // Reset lockout info on successful login
                _userLockoutInfo.TryRemove(username, out _);
                return Ok(new
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = response.Message,
                    Data = response.Data
                });
            }
            _logger.LogWarning("Authentication failed for user: {Username}. Reason: {Message}", 
                command.Username, response.Message);

            if (response.Message is "Invalid username or password.")
            {
               // Track invalid login attempts
                if (!_userLockoutInfo.ContainsKey(username))
                {
                    _userLockoutInfo[username] = new UserLockoutInfo { Attempts = 0, IsLocked = false };
                }
                var userInfo = _userLockoutInfo[username];
                userInfo.Attempts++;
      
                // Retrieve max login attempts from AdminSecuritySettings            
                var user = await _userQueryRepository.GetByUsernameAsync(username);
                const int adminSecuritySettingId = 18; // Replace with the actual ID for your settings
    //          var adminSettings = await _mediator.Send(new GetAdminSecuritySettingsByIdQuery { Id = companyId });         
                var adminSettings = await _mediator.Send(new GetAdminSecuritySettingsByIdQuery { Id = adminSecuritySettingId });                        
                int maxLoginAttempts = adminSettings.Data?.MaxFailedLoginAttempts ?? 5;
                int AutoLockMinutes = adminSettings.Data?.AccountAutoUnlockMinutes ?? 5;

                // Notify the user about the remaining attempts
                
                int remainingAttempts = maxLoginAttempts - userInfo.Attempts;

                if (remainingAttempts > 0)
                {
                    return Unauthorized(new
                    {
                        StatusCode = StatusCodes.Status400BadRequest,
                        Message = $"Invalid login attempt. You have {remainingAttempts} attempts remaining."
                    });
                }
                if (userInfo.Attempts >= maxLoginAttempts)
                {
                    // Lock the user
                    userInfo.IsLocked = true; 
                    userInfo.UnlockTime = currentTime .AddMinutes(AutoLockMinutes);

                    // Schedule Hangfire job to unlock user
                    BackgroundJob.Schedule(() => UnlockUser(username), TimeSpan.FromMinutes(AutoLockMinutes));

                    _logger.LogWarning("User {Username} is locked due to too many invalid login attempts.", username);

                    return Unauthorized(new
                    {
                        StatusCode = StatusCodes.Status400BadRequest,
                        Message = $"User is locked due to too many invalid attempts. Try again in {AutoLockMinutes} minutes."
                        
                    });
                }
            }
            return BadRequest(new
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Message = response.Message
            }); 
        }
          // Method to unlock user
        [NonAction]
        public void UnlockUser(string username)
        {
            if (_userLockoutInfo.TryGetValue(username, out var userInfo))
            {
                userInfo.IsLocked = false;
                userInfo.Attempts = 0;
                userInfo.UnlockTime = null;

                _logger.LogInformation("User {Username} has been unlocked automatically by Hangfire.", username);
            }
            else
            {
                _logger.LogWarning("Attempted to unlock user {Username}, but no lockout info was found.", username);
            }
        }      
       // Get session by JWT ID
        [HttpGet("session/{jwtId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetSessionByJwtId(string jwtId)
        {
            if (string.IsNullOrEmpty(jwtId))
            {
              //  return BadRequest(new { Message = "JWT ID cannot be null or empty." });
                 return Unauthorized(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "JWT ID cannot be null or empty."
                }); 
            }

            var session = await _userSessionRepository.GetSessionByJwtIdAsync(jwtId);

            if (session is null)
            {
                //return NotFound(new { Message = "Session not found for the provided JWT ID." });
                return NotFound(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "JWT ID cannot be null or empty."
                }); 
            }

            return Ok(new
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Session retrieved successfully.",
                Data = session
            });
        }

        // Deactivate expired sessions
        [HttpPost("deactivate-expired")]
        [AllowAnonymous]
        public async Task<IActionResult> DeactivateExpiredSessions()
        {
            await _userSessionRepository.DeactivateExpiredSessionsAsync();

            _logger.LogInformation("Expired sessions have been deactivated.");

            return Ok(new
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Expired sessions have been deactivated."
            });
        }
      
                        // Deactivate user sessions by User ID
        [HttpPost("deactivate-user-session/{userId}")]
        [AllowAnonymous]
        public async Task<IActionResult> DeactivateUserSessionsAsync(int userId)
        {
            await _userSessionRepository.DeactivateUserSessionsAsync(userId);

            _logger.LogInformation("All sessions for user {UserId} have been deactivated.", userId);

            return Ok(new
            {
                StatusCode = StatusCodes.Status200OK,
                Message = $"All sessions for user {userId} have been deactivated."
            });
        }
          // Helper class for lockout information
        public class UserLockoutInfo
        {
            public int Attempts { get; set; }
            public bool IsLocked { get; set; }
            public DateTime? UnlockTime { get; set; }
        }
    }
}

