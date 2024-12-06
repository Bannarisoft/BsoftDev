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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var user = await Mediator.Send(new GetUserByIdQuery() { UserId = id});
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
            return CreatedAtAction(nameof(GetByIdAsync), new { id = createdUser.UserId }, createdUser);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(int id, UpdateUserCommand command)
        {
            if(id != command.UserId)
            {
                return BadRequest();
            }
            await Mediator.Send(command);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var  result = await Mediator.Send(new DeleteUserCommand { UserId = id});
            if(result == 0)
            {
                return BadRequest();
            }
            return NoContent();
        }
    }
}

