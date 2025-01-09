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
using Core.Application.Users.Queries.GetUserAutoComplete;
using Core.Application.Users.Commands.UpdateFirstTimeUserPassword;
using Core.Application.Users.Commands.ChangeUserPassword;

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
         private readonly IValidator<FirstTimeUserPasswordCommand> _firstTimeUserPasswordCommandValidator;
         private readonly IValidator<ChangeUserPasswordCommand> _changeUserPasswordCommandValidator;
         
       public UserController(ISender mediator, 
                             IValidator<CreateUserCommand> createUserCommandValidator, 
                             IValidator<UpdateUserCommand> updateUserCommandValidator, 
                             ApplicationDbContext dbContext, 
                             IValidator<FirstTimeUserPasswordCommand> firstTimeUserPasswordCommandValidator, 
                             IValidator<ChangeUserPasswordCommand> changeUserPasswordCommandValidator) 
         : base(mediator)
        {        
            _createUserCommandValidator = createUserCommandValidator;
            _updateUserCommandValidator = updateUserCommandValidator;    
            _dbContext = dbContext;  
            _firstTimeUserPasswordCommandValidator = firstTimeUserPasswordCommandValidator;
            _changeUserPasswordCommandValidator = changeUserPasswordCommandValidator;
             
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

        [HttpGet]
        [Route("GetUsersByName")]
        public async Task<IActionResult> GetByUsernameAsync([FromQuery] string searchPattern)
        {
           
            var users = await Mediator.Send(new GetUserAutoCompleteQuery {SearchPattern = searchPattern});
            return Ok(users);
        }
        [HttpPut]
        [Route("FirstTimeUserChangePassword")]
        public async Task<IActionResult> FirstTimeUserChangePassword([FromBody] FirstTimeUserPasswordCommand command)
        {
            var validationResult = await _firstTimeUserPasswordCommandValidator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }
           var response = await Mediator.Send(command);
            return Ok(new { Message = response });
        }
        [HttpPut]
        [Route("ChangePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangeUserPasswordCommand command)
        {
            var validationResult = await _changeUserPasswordCommandValidator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }
           var response = await Mediator.Send(command);
            return Ok(new { Message = response });
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