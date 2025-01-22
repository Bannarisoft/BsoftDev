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
        public UserRoleController(ISender mediator    , IValidator<CreateRoleCommand> createRoleCommandValidator,
        IValidator<UpdateRoleCommand> updateRoleCommandValidator, 
        ILogger<UserRoleController> logger,
        ApplicationDbContext dbContext ) : base(mediator)
        {
            _createRoleCommandValidator= createRoleCommandValidator;
            _updateRoleCommandValidator= updateRoleCommandValidator;
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

                if (result.Data == null )
                {
                    _logger.LogWarning("No User Role found for search pattern: {SearchPattern}", searchTerm);

                    return NotFound(new
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        Message = "No matching User Role found."
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

         [HttpPut("delete/{id}")]
        public async Task<IActionResult> DeleteAsync(int id, DeleteRoleCommand command)
        {
              _logger.LogInformation("Delete Department request started with ID: {DepartmentId}", command.UserRoleId);

                // Check if the department exists
                var userRole = await Mediator.Send(new GetRoleByIdQuery { Id = command.UserRoleId });
                if (userRole == null)
                {
                    _logger.LogWarning("Department with ID {DepartmentId} not found.", command.UserRoleId);

                    return NotFound(new
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        Message = "Department not found"
                    });
                }

                _logger.LogInformation("Department with ID {DepartmentId} found. Proceeding with deletion.", command.UserRoleId);

                // Attempt to delete the department
                var result = await Mediator.Send(command);

                if (result.IsSuccess)
                {
                    _logger.LogInformation("Department with ID {DepartmentId} deleted successfully.", command.UserRoleId);

                    return Ok(new
                    {
                        Message = result.Message,
                        StatusCode = StatusCodes.Status200OK
                      
                    });
                }

                _logger.LogWarning("Failed to delete User Role with ID {Id}. Reason: {Message}", command.UserRoleId, result.Message);

                return BadRequest(new
                {
                    Message = result.Message,
                    StatusCode = StatusCodes.Status400BadRequest
                });

        
        }
       
     [HttpPut("update/{id}")]
    public async Task<IActionResult> UpdateAsync(int id, UpdateRoleCommand command)
    {      
               _logger.LogInformation("Update User Role  request started with data: {@Command}", command);

            // Check if the department exists
            var department = await Mediator.Send(new GetRoleByIdQuery { Id = command.Id });
            if (department == null)
            {
                _logger.LogWarning("User Role with ID {Id} not found.", command.Id);

                return NotFound(new
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = "User Role  not found"
                });
            }

            // Validate the update command
            var validationResult = await _updateRoleCommandValidator.ValidateAsync(command);
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
            var updateResult = await Mediator.Send(command);
            if (updateResult.IsSuccess)
            {
                _logger.LogInformation("User Role  with ID {Id} updated successfully.", command.Id);

                return Ok(new
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "User Role  updated successfully"
                  
                });
            }

            _logger.LogWarning("Failed to update User Role  with ID {Id}. Reason: {Message}", command.Id, updateResult.Message);

            return BadRequest(new
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Message = updateResult.Message
            });
        

        //   var validationResult = await _updateRoleCommandValidator.ValidateAsync(command);
        // if (!validationResult.IsValid)
        // {
        // return BadRequest(validationResult.Errors);
        // }
        // if (id != command.Id)
        // {
        //     return BadRequest("UserRole Id Mismatch");
        // }

        // var UpdateUserRole = await Mediator.Send(command);
        // return Ok("Updated Successfully");
   
    }



    }
}