using Core.Application.UserLogin.Commands.UserLogin;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using FluentValidation;
using Core.Application.Common.Interfaces.IUserSession;

namespace BSOFT.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
         private readonly IValidator<UserLoginCommand> _userLoginCommandValidator;

        private readonly ILogger<AuthController> _logger;
        private readonly IUserSessionRepository _userSessionRepository;

        public AuthController(IMediator mediator,IValidator<UserLoginCommand> userLoginCommandValidator, IMapper mapper, ILogger<AuthController> logger,IUserSessionRepository userSessionRepository)
        {
            _mediator = mediator;
            _userLoginCommandValidator = userLoginCommandValidator;
            _mapper = mapper;
            _logger = logger;
             _userSessionRepository = userSessionRepository;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {  
                  // Map the incoming request to a UserLoginCommand
            var command = _mapper.Map<UserLoginCommand>(request);

            // Validate the command
            var validationResult = await _userLoginCommandValidator.ValidateAsync(command);

            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Validation failed for login request: {Errors}", 
                    string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));

                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "Validation failed",
                    Errors = validationResult.Errors.Select(e => e.ErrorMessage)
                });
            }

            // Process the command using Mediator
            var response = await _mediator.Send(command);

            if (response.IsSuccess)
            {
                _logger.LogInformation("User {Username} authenticated successfully.", command.Username);

                return Ok(new
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = response.Message,
                    Data = response.Data
                });
            }

            _logger.LogWarning("Authentication failed for user: {Username}. Reason: {Message}", 
                command.Username, response.Message);

            return BadRequest(new
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Message = response.Message
            });
        }
       // Get session by JWT ID
        [HttpGet("session/{jwtId}")]
        
        public async Task<IActionResult> GetSessionByJwtId(string jwtId)
        {
            if (string.IsNullOrEmpty(jwtId))
            {
                return BadRequest(new { Message = "JWT ID cannot be null or empty." });
            }

            var session = await _userSessionRepository.GetSessionByJwtIdAsync(jwtId);

            if (session == null)
            {
                return NotFound(new { Message = "Session not found for the provided JWT ID." });
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
    }
}

