using Core.Application.UserLogin.Commands.UserLogin;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using FluentValidation;

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

        public AuthController(IMediator mediator,IValidator<UserLoginCommand> userLoginCommandValidator, IMapper mapper, ILogger<AuthController> logger)
        {
            _mediator = mediator;
            _userLoginCommandValidator = userLoginCommandValidator;
            _mapper = mapper;
            _logger = logger;
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
    }
}

