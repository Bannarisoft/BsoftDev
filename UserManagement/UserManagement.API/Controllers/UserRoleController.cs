using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Core.Application.UserRole.Queries.GetRole;
using Core.Application.UserRole.Queries.GetRoleById;
using Core.Application.UserRole.Commands.CreateRole;
using Core.Application.UserRole.Commands.DeleteRole;
using Core.Application.UserRole.Commands.UpdateRole;
using Core.Application.UserRole.Queries.GetRolesAutocomplete;
using Core.Application.Common.Interfaces;
using FluentValidation;
using UserManagement.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Core.Application.Common.Interfaces.IUser;
using Core.Application.Common.HttpResponse;

namespace UserManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    
    public class UserRoleController : ApiControllerBase
    {
         private readonly IValidator<CreateRoleCommand> _createRoleCommandValidator;
        private readonly IValidator<UpdateRoleCommand> _updateRoleCommandValidator;
         private readonly ApplicationDbContext _dbContext;
         private readonly ILogger<UserRoleController> _logger;

         private readonly IUserCommandRepository  _userCommandRepository;
        public UserRoleController(ISender mediator    , IValidator<CreateRoleCommand> createRoleCommandValidator,
        IUserCommandRepository userCommandRepository,
        IValidator<UpdateRoleCommand> updateRoleCommandValidator, 
        ILogger<UserRoleController> logger,
        ApplicationDbContext dbContext ) : base(mediator)
        {
            _createRoleCommandValidator= createRoleCommandValidator;
            _updateRoleCommandValidator= updateRoleCommandValidator;
            _userCommandRepository= userCommandRepository;
             _dbContext = dbContext; 
             _logger = logger;

        }
       
        [HttpGet]
        public async Task<IActionResult> GetAllRoleAsync()
        {
             _logger.LogInformation("Fetching All userRole Request started.");
           
            var userRole =await Mediator.Send(new GetRoleQuery());
           if (userRole.Data == null || !userRole.Data.Any())
            {
               
                _logger.LogInformation($"No userRole records found in the database. Total count: {userRole?.Data?.Count ?? 0}" );
                 return NotFound(new
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        message = userRole.Message
                    });
             }           
          
            return Ok(new
            {
                StatusCode = StatusCodes.Status200OK,
                data = userRole.Data
            });
           
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {


            _logger.LogInformation($"Fetching role with ID: {id}");

             var userRole = await Mediator.Send(new GetRoleByIdQuery { Id = id });
              if (userRole == null || userRole.Data == null)
            {
                _logger.LogInformation($"userRole with ID {id} not found in the database.");

                return NotFound(new
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = userRole?.Message ?? "userRole not found."
                });
            }
             // Return success response
            return Ok(new
            {
                StatusCode = StatusCodes.Status200OK,
                Data = userRole.Data
            });

           

        }
        [HttpGet("by-name/{name}")] 
        public async Task<IActionResult> GetRoles(string name)
        {
          _logger.LogInformation($"Starting GetAllUserRoleAutoCompleteSearchAsync with search pattern: {name}");
             var query = new GetRolesAutocompleteQuery { SearchTerm = name };
                var result = await Mediator.Send(query);

                if (result.Data == null  || !result.Data.Any())
                {
                    _logger.LogWarning($"No User Role found for search pattern: {name}" );

                    return NotFound(new
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        Message = "No matching User Role found / Deleted."
                    });
                }

                _logger.LogInformation($"User Role found for search pattern: {name}. Returning data.");

                return Ok(new
                {
                    StatusCode = StatusCodes.Status200OK,
                    Data = result.Data
                });
     
  
        }

        [HttpPost]
        public async Task<IActionResult>CreateAsync(CreateRoleCommand createRoleCommand)
        {

                   _logger.LogInformation($"Create User Role request started with data: {createRoleCommand}");

            // Validate the command
            var validationResult = await _createRoleCommandValidator.ValidateAsync(createRoleCommand);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning($"Validation failed for Create User Role request. Errors: { validationResult.Errors}");

                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "Validation failed",
                    Errors = validationResult.Errors.Select(e => e.ErrorMessage).ToArray()
                });
            }

            // Process the command
            var createuserrole = await Mediator.Send(createRoleCommand);
            if (createuserrole.IsSuccess)
            {
                _logger.LogInformation($"Create User Role request succeeded. User Role created with ID: {createuserrole.Data.UserRoleId}");

                return Ok(new
                {
                    StatusCode = StatusCodes.Status201Created,
                    Message = createuserrole.Message
                    
                });
            }
            _logger.LogWarning($"Create User Role request failed. Reason: {createuserrole.Message}");

            return BadRequest(new
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Message = createuserrole.Message
            });
            
               
        }

         [HttpDelete]      
        public async Task<IActionResult> DeleteAsync( DeleteRoleCommand userroleDeleteCommand)
        {
              _logger.LogInformation($"Delete User Role request started with ID: {userroleDeleteCommand.Id}");

                // Check if the department exists
                var userRole = await Mediator.Send(new GetRoleByIdQuery { Id = userroleDeleteCommand.Id });
                if (userRole == null)
                {
                    _logger.LogWarning($"User Role with ID {userroleDeleteCommand.Id} not found.");

                    return NotFound(new
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        Message = "User Role not found"
                    });
                }

                _logger.LogInformation($"User Role with ID {userroleDeleteCommand.Id} found. Proceeding with deletion.");

                // Attempt to delete the department
                var result = await Mediator.Send(userroleDeleteCommand);

                if (result.IsSuccess)
                {
                    _logger.LogInformation($"User Role with ID {userroleDeleteCommand.Id} deleted successfully.");

                    return Ok(new
                    {
                        Message = result.Message,
                        StatusCode = StatusCodes.Status200OK
                      
                    });
                }

                _logger.LogWarning($"Failed to delete User Role with ID {userroleDeleteCommand.Id}. Reason: {result.Message}");

                return BadRequest(new
                {
                    Message = result.Message,
                    StatusCode = StatusCodes.Status400BadRequest
                });

        
        }
       
  

    [HttpPut]
    public async Task<IActionResult> UpdateAsync( UpdateRoleCommand updateRolecommand)
    {      
               _logger.LogInformation($"Update User Role  request started with data: {updateRolecommand}");

            // Check if the department exists
            var department = await Mediator.Send(new GetRoleByIdQuery { Id = updateRolecommand.Id });
            if (department == null)
            {
                _logger.LogWarning($"User Role with ID {updateRolecommand.Id} not found.");

                return NotFound(new
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = "User Role  not found"
                });
            }
                    // Validate the update command
            var validationResult = await _updateRoleCommandValidator.ValidateAsync(updateRolecommand);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning($"Validation failed for Update User Role  request. Errors: {validationResult.Errors}" );

                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "Validation failed",
                    Errors = validationResult.Errors.Select(e => e.ErrorMessage).ToArray()
                });
            }

          
            var updateResult = await Mediator.Send(updateRolecommand);
            if (updateResult.IsSuccess)
            {
                _logger.LogInformation($"User Role  with ID {updateRolecommand.Id} updated successfully." );

                return Ok(new
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "User Role  updated successfully"
                  
                });
            }

            _logger.LogWarning($"Failed to update User Role  with ID {updateRolecommand.Id}. Reason: {updateResult.Message}");

            return BadRequest(new
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Message = updateResult.Message
            });               
    }


    }
}