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

        public AuthController(IMediator mediator,IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {   
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiErrorResponse(400, "Invalid request data."));
            }

            var command = _mapper.Map<UserLoginCommand>(request);
            var result = await _mediator.Send(command);

            if (result == null || !result.IsAuthenticated)
            {
                return Unauthorized(new ApiErrorResponse(401, "Invalid username or password."));
            }

            return Ok(result);
            
            // if (!ModelState.IsValid)
            // {
            //     return BadRequest(ModelState);
                
            // }

            // var command = _mapper.Map<UserLoginCommand>(request);
            // var result = await _mediator.Send(command);

            // if (result == null || !result.IsAuthenticated)
            // {
            //     return Unauthorized(result?.Message ?? "Invalid username or password.");
            // }

            // return Ok(result);

            
        }
    }
}

