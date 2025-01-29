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
using Microsoft.AspNetCore.Authorization;

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
         private readonly ILogger<UserController> _logger;
         private readonly IHttpClientFactory _httpClientFactory;
         
       public UserController(ISender mediator, 
                             IValidator<CreateUserCommand> createUserCommandValidator, 
                             IValidator<UpdateUserCommand> updateUserCommandValidator, 
                             ApplicationDbContext dbContext, 
                             IValidator<FirstTimeUserPasswordCommand> firstTimeUserPasswordCommandValidator, 
                             IValidator<ChangeUserPasswordCommand> changeUserPasswordCommandValidator,
                             ILogger<UserController> logger,
                             IHttpClientFactory httpClientFactory) 
         : base(mediator)
        {        
            _createUserCommandValidator = createUserCommandValidator;
            _updateUserCommandValidator = updateUserCommandValidator;    
            _dbContext = dbContext;  
            _firstTimeUserPasswordCommandValidator = firstTimeUserPasswordCommandValidator;
            _changeUserPasswordCommandValidator = changeUserPasswordCommandValidator;
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }
        
        [HttpGet]
        public async Task<IActionResult> GetAllUsersAsync()
        {
            var users = await Mediator.Send(new GetUserQuery());
            var activeUsers = users.Where(c => c.IsActive).ToList();
            _logger.LogInformation("Users Listed successfully.", activeUsers.Count);

            return Ok(new { StatusCode = StatusCodes.Status200OK, data = activeUsers });
        }

        [HttpGet("{userid}")]
        public async Task<IActionResult> GetByIdAsync(int userid)
        {
        //     var client = _httpClientFactory.CreateClient("ResilientHttpClient");

        // try
        // {
        //     var response = await client.GetAsync("https://httpstat.us/500");
        //     response.EnsureSuccessStatusCode();

        //     var user = await response.Content.ReadAsStringAsync();
        //     _logger.LogInformation("Retrieved user: {User}", user);

        //     return Ok(new { StatusCode = 200, Data = user });
        // }
        // catch (Exception ex)
        // {
        //     _logger.LogError(ex, "Error retrieving user with ID {UserId}.", userid);
        //     return StatusCode(500, "An error occurred while processing your request.");
        // }
    
            
            var user = await Mediator.Send(new GetUserByIdQuery { UserId = userid });

            if (user == null)
            {
                _logger.LogWarning("User Not Found for ID : {UserId}", userid);

                return NotFound(new { StatusCode = StatusCodes.Status404NotFound, message = $"User ID {userid} not found." });
            }
                _logger.LogWarning("User Listed successfully: {Username}", user);
                return Ok(new { StatusCode = StatusCodes.Status200OK, data = user });
        }

        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> CreateAsync([FromBody] CreateUserCommand command)
        {
            var validationResult = await _createUserCommandValidator.ValidateAsync(command);
                _logger.LogWarning("Validation failed: {ErrorDetails}", validationResult);


            if (!validationResult.IsValid)
            {
                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    message = "Validation failed",
                    errors = validationResult.Errors.Select(e => e.ErrorMessage)
                });
            }

            var response = await Mediator.Send(command);
            if (response.IsSuccess)
            {
                _logger.LogInformation("User {Username} created successfully.", command.UserName);

                return Ok(new { StatusCode = StatusCodes.Status201Created, message = response.Message, data = response.Data });
            }
                _logger.LogWarning("User creation failed for user: {Username}", command.UserName);

                return BadRequest(new { StatusCode = StatusCodes.Status400BadRequest, message = response.Message }); 
        }        
        [HttpPut]
        [Route("Update")]
        public async Task<IActionResult> UpdateAsync([FromBody] UpdateUserCommand command)
        {
            var validationResult = await _updateUserCommandValidator.ValidateAsync(command);
                _logger.LogWarning("Validation failed: {ErrorDetails}", validationResult);

            if (!validationResult.IsValid)
            {
                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    message = "Validation failed",
                    errors = validationResult.Errors.Select(e => e.ErrorMessage)
                });
            }

            var userExists = await Mediator.Send(new GetUserByIdQuery { UserId = command.UserId });
            if (userExists == null)
            {
                _logger.LogInformation("User {Username} not found for update.", command.UserId);

                return NotFound(new { StatusCode = StatusCodes.Status404NotFound, message = $"User ID {command.UserId} not found." });
            }

            var response = await Mediator.Send(command);
            if (response.IsSuccess)
            {
                _logger.LogInformation("User {Username} updated successfully.", command.UserName);

                return Ok(new { StatusCode = StatusCodes.Status200OK, message = response.Message });
            }
                _logger.LogWarning("User update failed for user: {Username}", command.UserName);

                return BadRequest(new { StatusCode = StatusCodes.Status400BadRequest, message = response.Message }); 
        }      

        [HttpDelete]
        [Route("Delete/{userid}")]
        public async Task<IActionResult> DeleteAsync(int userId,DeleteUserCommand deleteUserCommand)
        {
            if(userId != deleteUserCommand.UserId)
            {
                return BadRequest();
            }

            var deleteUser = await Mediator.Send(deleteUserCommand);

            if(deleteUser.IsSuccess)
            {
                _logger.LogInformation("User {UserId} deleted successfully.", deleteUserCommand.UserId);

                return Ok(new { StatusCode=StatusCodes.Status200OK, message = deleteUser.Message, errors = "" });
              
            }
                _logger.LogInformation("User {UserId} failed to delete.", deleteUserCommand.UserId);

                return BadRequest(new { StatusCode=StatusCodes.Status400BadRequest, message = deleteUser.Message, errors = "" });
        }

        [HttpGet]
        [Route("GetUsersByName")]
        public async Task<IActionResult> GetByUsernameAsync([FromQuery] string searchPattern)
        {
           
            var users = await Mediator.Send(new GetUserAutoCompleteQuery {SearchPattern = searchPattern});
            _logger.LogWarning("User Listed successfully: {Username}", users);

            return Ok( new { StatusCode=StatusCodes.Status200OK, data = users });
        }
        [HttpPut]
        [Route("FirstTimeUserChangePassword")]
        public async Task<IActionResult> FirstTimeUserChangePassword([FromBody] FirstTimeUserPasswordCommand command)
        {
            var validationResult = await _firstTimeUserPasswordCommandValidator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    message = "Validation failed",
                    errors = validationResult.Errors.Select(e => e.ErrorMessage)
                });
            }

            var response = await Mediator.Send(command);
            _logger.LogInformation("First Time User and Password changed successfully.", command.UserName);

            return Ok(new { StatusCode = StatusCodes.Status200OK, message = response });
        //     var validationResult = await _firstTimeUserPasswordCommandValidator.ValidateAsync(command);
        //     if (!validationResult.IsValid)
        //     {
        //         return BadRequest(validationResult.Errors);
        //     }
        //    var response = await Mediator.Send(command);
        //     return Ok(new { Message = response });
        }
        [HttpPut]
        [Route("ChangePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangeUserPasswordCommand command)
        {
            var validationResult = await _changeUserPasswordCommandValidator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    message = "Validation failed",
                    errors = validationResult.Errors.Select(e => e.ErrorMessage)
                });
            }

            var response = await Mediator.Send(command);
            _logger.LogInformation("User and Password changed successfully.", command.UserName);

            return Ok(new { StatusCode = StatusCodes.Status200OK, message = response });
        //     var validationResult = await _changeUserPasswordCommandValidator.ValidateAsync(command);
        //     if (!validationResult.IsValid)
        //     {
        //         return BadRequest(validationResult.Errors);
        //     }
        //    var response = await Mediator.Send(command);
        //     return Ok(new { Message = response });
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