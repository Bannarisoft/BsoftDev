using Core.Application.UserLogin.Commands.UserLogin;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using BSOFT.API.Models;

namespace BSOFT.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IMediator mediator,IMapper mapper, ILogger<AuthController> logger)
        {
            _mediator = mediator;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {   
            if (!ModelState.IsValid)
            {
                 var validationError = new ApiErrorResponse(400, "Invalid request data.");
                _logger.LogWarning("Validation failed: {ErrorDetails}", validationError);
                 return BadRequest(validationError);
            }
            try
            {
                var command = _mapper.Map<UserLoginCommand>(request);
                var result = await _mediator.Send(command);

            if (result == null || !result.IsAuthenticated)
            {
                var authError = new ApiErrorResponse(401, "Invalid username or password.");
                _logger.LogWarning("Authentication failed for user: {Username}", request.Username);
                return Unauthorized(authError);
            }

                _logger.LogInformation("User {Username} authenticated successfully.", request.Username);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during login for user: {Username}", request.Username);
                return StatusCode(500, new ApiErrorResponse(500, "An unexpected error occurred."));
            }
        }
    }
}

