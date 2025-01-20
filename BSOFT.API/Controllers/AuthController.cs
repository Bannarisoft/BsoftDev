using Core.Application.UserLogin.Commands.UserLogin;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;

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
        //[AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {   
            
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var command = _mapper.Map<UserLoginCommand>(request);
            
            var result = await _mediator.Send(command);

            if (result == null)
            {
                return Unauthorized("Invalid username or password.");
            }
            if (!result.IsAuthenticated)
            {
                return Unauthorized(result.Message);
            }

            return Ok(result);

            
        }
    }
}

