using BSOFT.Infrastructure.Data;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Core.Application.Users.Queries.GetUsers;
using Core.Application.Users.Queries.GetUserById;
using Core.Application.Users.Commands.CreateUser;
using Core.Application.Users.Commands.UpdateUser;
using Core.Application.Users.Commands.DeleteUser;

namespace BSOFT.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class UserController : ApiControllerBase
    {
        // public UserController(ISender mediator) : base(mediator)
        // {
        // }

         private readonly IValidator<CreateUserCommand> _createUserCommandValidator;
         private readonly IValidator<UpdateUserCommand> _updateUserCommandValidator;
         private readonly ApplicationDbContext _dbContext;
         
       public UserController(ISender mediator, 
                             IValidator<CreateUserCommand> createUserCommandValidator, 
                             IValidator<UpdateUserCommand> updateUserCommandValidator, 
                             ApplicationDbContext dbContext) 
         : base(mediator)
        {        
            _createUserCommandValidator = createUserCommandValidator;
            _updateUserCommandValidator = updateUserCommandValidator;    
            _dbContext = dbContext;  
             
        }
        
        [HttpGet]
        public async Task<IActionResult> GetAllUsersAsync()
        {
            var users = await Mediator.Send(new GetUserQuery());
            return Ok(users);
        }

        [HttpGet("{userid}")]
        public async Task<IActionResult> GetByIdAsync(int userid)
        {
            if (userid <= 0)
            {
                return BadRequest("Invalid user ID");
            }

            var user = await Mediator.Send(new GetUserByIdQuery() { UserId = userid });
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> CreateAsync([FromBody] CreateUserCommand command)
        {
            var validationResult = await _createUserCommandValidator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
            return BadRequest(validationResult.Errors);
            }

            var createdUser = await Mediator.Send(command);
            return Ok(new { Message = "User created successfully.", userid = createdUser.Id });
        }        
        [HttpPut]
        [Route("Update")]
        public async Task<IActionResult> UpdateAsync([FromBody] UpdateUserCommand command)
        {
            var validationResult = await _updateUserCommandValidator.ValidateAsync(command);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }
            await Mediator.Send(command);
            return Ok(new { Message = "User updated successfully." });
        }      
        [HttpDelete]
        [Route("Delete/{userid}")]
        public async Task<IActionResult> DeleteAsync(int userid)
        {
            await Mediator.Send(new DeleteUserCommand { UserId = userid });
            return Ok(new { Message = "User deleted successfully." });
        }

        // [HttpPut("{userid}")]
        // public async Task<IActionResult> UpdateAsync(int userid, UpdateUserCommand command)
        // {
        //     if (userid <= 0)
        //     {
        //         return BadRequest("Invalid user ID");
        //     }

        //     if (userid != command.UserId)
        //     {
        //         return BadRequest();
        //     }
        //     await Mediator.Send(command);
        //     return NoContent();
        // }

        // [HttpDelete("{userid}")]
        // public async Task<IActionResult> DeleteAsync(int userid, DeleteUserCommand command)
        // {
        //     if (userid <= 0)
        //     {
        //         return BadRequest("Invalid user ID");
        //     }

        //     if (userid != command.UserId)
        //     {
        //         return BadRequest();
        //     }
        //     await Mediator.Send(command);

        //     return NoContent();
        // }
    }
}