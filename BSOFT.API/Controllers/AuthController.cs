using Core.Application.UserLogin.Commands.UserLogin;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BSOFT.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {   
            
             try
            {
                // Check if the UserLoginCommand supports constructor arguments
                var command = new UserLoginCommand
                {
                    Username = request.Username,
                    Password = request.Password
                };

                var response = await _mediator.Send(command);
                return Ok(response);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = ex.Message });
            }
            // var response = await _mediator.Send(command);
            // if (response.Token == string.Empty)
            // {
            //     return Unauthorized(new { response.Message });
            // }

            // return Ok(new { response.Token, response.Message });
        }
    }
}

