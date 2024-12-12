using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BSOFT.Application.Users.Queries.GetUsers;
using BSOFT.Application.Users.Queries.GetUserById;
using BSOFT.Application.Users.Commands.CreateUser;
using BSOFT.Application.Users.Commands.UpdateUser;
using BSOFT.Application.Users.Commands.DeleteUser;


namespace BSOFT.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class UserController : ApiControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAllUsersAsync()
        {
            var users = await Mediator.Send(new GetUserQuery());
            return Ok(users);
        }

        [HttpGet("{userid}")]
        public async Task<IActionResult> GetByIdAsync(int userid)
        {
            var user = await Mediator.Send(new GetUserByIdQuery() { UserId = userid});
            if(user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync(CreateUserCommand command)
        {
            var createdUser = await Mediator.Send(command);
            return CreatedAtAction(nameof(GetByIdAsync), new { userid = createdUser.Id }, createdUser);
        }

        [HttpPut("update/{userid}")]
        public async Task<IActionResult> UpdateAsync(int userid, UpdateUserCommand command)
        {
            if(userid != command.UserId)
            {
                return BadRequest();
            }
            await Mediator.Send(command);
            return NoContent();
        }
        
        [HttpPut("delete/{userid}")]
        public async Task<IActionResult> DeleteAsync(int userid,DeleteUserCommand command)
        {
             if(userid != command.UserId)
            {
                return BadRequest();
            }
            await Mediator.Send(command);

            return NoContent();
        }

    }
}

