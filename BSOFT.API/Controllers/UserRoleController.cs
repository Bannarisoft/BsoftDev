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
using BSOFT.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Core.Application.Common.Interfaces.IUser;
using Core.Application.Common.HttpResponse;

namespace BSOFT.API.Controllers
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
               
                _logger.LogInformation("No userRole records found in the database. Total count: {Count}", userRole?.Data?.Count ?? 0);
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
            // var roles =await Mediator.Send(new GetRoleQuery());
            // return Ok(roles);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {


            _logger.LogInformation("Fetching role with ID: {Id}", id);

             var userRole = await Mediator.Send(new GetRoleByIdQuery { Id = id });
              if (userRole == null || userRole.Data == null)
            {
                _logger.LogInformation("userRole with ID {Id} not found in the database.", id);

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
            [HttpGet("RolesSearch")]
    public async Task<IActionResult> GetRoles([FromQuery] string searchTerm)
    {
          _logger.LogInformation("Starting GetAllUserRoleAutoCompleteSearchAsync with search pattern: {SearchPattern}", searchTerm);
             var query = new GetRolesAutocompleteQuery { SearchTerm = searchTerm };
                var result = await Mediator.Send(query);

                if (result.Data == null  || !result.Data.Any())
                {
                    _logger.LogWarning("No User Role found for search pattern: {SearchPattern}", searchTerm);

                    return NotFound(new
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        Message = "No matching User Role found / Deleted."
                    });
                }

                _logger.LogInformation("User Role found for search pattern: {SearchPattern}. Returning data.", searchTerm);

                return Ok(new
                {
                    StatusCode = StatusCodes.Status200OK,
                    Data = result.Data
                });
     
  
    }
        [HttpPost]
        public async Task<IActionResult>CreateAsync(CreateRoleCommand command)
        {

                   _logger.LogInformation("Create User Role request started with data: {@Command}", command);

            // Validate the command
            var validationResult = await _createRoleCommandValidator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Validation failed for Create User Role request. Errors: {@Errors}", validationResult.Errors);

                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "Validation failed",
                    Errors = validationResult.Errors.Select(e => e.ErrorMessage).ToArray()
                });
            }

            // Process the command
            var createuserrole = await Mediator.Send(command);
            if (createuserrole.IsSuccess)
            {
                _logger.LogInformation("Create User Role request succeeded. User Role created with ID: {Id}", createuserrole.Data.UserRoleId);

                return Ok(new
                {
                    StatusCode = StatusCodes.Status201Created,
                    Message = createuserrole.Message
                    
                });
            }
            _logger.LogWarning("Create User Role request failed. Reason: {Message}", createuserrole.Message);

            return BadRequest(new
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Message = createuserrole.Message
            });
            
               
        }

         [HttpPut("Delete")]
        public async Task<IActionResult> DeleteAsync( DeleteRoleCommand userroleDeleteCommand)
        {
              _logger.LogInformation("Delete User Role request started with ID: {DepartmentId}", userroleDeleteCommand.Id);

                // Check if the department exists
                var userRole = await Mediator.Send(new GetRoleByIdQuery { Id = userroleDeleteCommand.Id });
                if (userRole == null)
                {
                    _logger.LogWarning("User Role with ID {DepartmentId} not found.", userroleDeleteCommand.Id);

                    return NotFound(new
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        Message = "User Role not found"
                    });
                }

                _logger.LogInformation("User Role with ID {Id} found. Proceeding with deletion.", userroleDeleteCommand.Id);

                // Attempt to delete the department
                var result = await Mediator.Send(userroleDeleteCommand);

                if (result.IsSuccess)
                {
                    _logger.LogInformation("User Role with ID {Id} deleted successfully.", userroleDeleteCommand.Id);

                    return Ok(new
                    {
                        Message = result.Message,
                        StatusCode = StatusCodes.Status200OK
                      
                    });
                }

                _logger.LogWarning("Failed to delete User Role with ID {Id}. Reason: {Message}", userroleDeleteCommand.Id, result.Message);

                return BadRequest(new
                {
                    Message = result.Message,
                    StatusCode = StatusCodes.Status400BadRequest
                });

        
        }
       
  

    [HttpPut("Update")]
    public async Task<IActionResult> UpdateAsync( UpdateRoleCommand updateRolecommand)
    {      
               _logger.LogInformation($"Update User Role  request started with data: {updateRolecommand}");

            // Check if the department exists
            var department = await Mediator.Send(new GetRoleByIdQuery { Id = updateRolecommand.Id });
            if (department == null)
            {
                _logger.LogWarning("User Role with ID {Id} not found.", updateRolecommand.Id);

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
                _logger.LogWarning("Validation failed for Update User Role  request. Errors: {@Errors}", validationResult.Errors);

                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "Validation failed",
                    Errors = validationResult.Errors.Select(e => e.ErrorMessage).ToArray()
                });
            }

            // Update the department
            var updateResult = await Mediator.Send(updateRolecommand);
            if (updateResult.IsSuccess)
            {
                _logger.LogInformation("User Role  with ID {Id} updated successfully.", updateRolecommand.Id);

                return Ok(new
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "User Role  updated successfully"
                  
                });
            }

            _logger.LogWarning("Failed to update User Role  with ID {Id}. Reason: {Message}", updateRolecommand.Id, updateResult.Message);

            return BadRequest(new
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Message = updateResult.Message
            });               
    }



   



    }
}