using BSOFT.Application.UserLogin.Commands.UserLogin;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BSOFT.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginCommand command)
        {
            var response = await _mediator.Send(command);
            if (response.Token == string.Empty)
            {
                return Unauthorized(new { response.Message });
            }

            return Ok(new { response.Token, response.Message });
        }
    }
}

