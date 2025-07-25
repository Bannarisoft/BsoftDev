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
         private readonly IValidator<DeleteRoleCommand> _deleteRoleCommandValidator;
         private readonly ILogger<UserRoleController> _logger;

         private readonly IUserCommandRepository  _userCommandRepository;
        public UserRoleController(ISender mediator    , IValidator<CreateRoleCommand> createRoleCommandValidator,
        IUserCommandRepository userCommandRepository,
        IValidator<UpdateRoleCommand> updateRoleCommandValidator, 
        ILogger<UserRoleController> logger,
        IValidator<DeleteRoleCommand> deleteRoleCommandValidator ) : base(mediator)
        {
            _createRoleCommandValidator= createRoleCommandValidator;
            _updateRoleCommandValidator= updateRoleCommandValidator;
            _userCommandRepository= userCommandRepository;
            _deleteRoleCommandValidator= deleteRoleCommandValidator;
             _logger = logger;

        }
       
        [HttpGet]
        public async Task<IActionResult> GetAllRoleAsync([FromQuery] int PageNumber, [FromQuery] int PageSize, [FromQuery] string? SearchTerm = null)
            {
                _logger.LogInformation("Fetching all user roles request started.");

                var userRoles = await Mediator.Send(new GetRoleQuery
                {
                    PageNumber = PageNumber,
                    PageSize = PageSize,
                    SearchTerm = SearchTerm
                });

                if (userRoles == null || userRoles.Data == null || !userRoles.Data.Any())
                {
                    _logger.LogWarning("No user role records found.");
                    return NotFound(new
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        Message = "No user roles found."
                    });
                }

                _logger.LogInformation("User roles retrieved successfully.");
                return Ok(new
                {
                    StatusCode = StatusCodes.Status200OK,
                    Data = userRoles.Data,
                    TotalCount = userRoles.TotalCount,
                    PageNumber = PageNumber,
                    PageSize = PageSize
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
        [HttpGet("by-name")] 
        public async Task<IActionResult> GetRoles([FromQuery] string? name)
            {
                _logger.LogInformation("Starting GetAllUserRoleAutoCompleteSearchAsync with search pattern: {SearchPattern}", name);

                var query = new GetRolesAutocompleteQuery { SearchTerm = name ?? string.Empty };
                var result = await Mediator.Send(query);

                if (result.IsSuccess)
                {
                    _logger.LogInformation("User Role found for search pattern: {SearchPattern}. Returning data.", name);

                    return Ok(new
                    {
                        StatusCode = StatusCodes.Status200OK,
                        Data = result.Data
                    });
                }

                _logger.LogWarning("No User Role found for search pattern: {SearchPattern}", name);

                return NotFound(new
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = "No matching User Role found / Deleted."
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

        [HttpDelete("{id}")]         
        public async Task<IActionResult> DeleteAsync( int id )
        {
              _logger.LogInformation($"Delete User Role request started with ID: {id}");
              
            var command = new DeleteRoleCommand { Id = id };
             var validationResult = await  _deleteRoleCommandValidator.ValidateAsync(command);
               if (!validationResult.IsValid)
                {
                    return BadRequest(new
                    {
                        message = validationResult.Errors.Select(e => e.ErrorMessage).FirstOrDefault(),
                        statusCode = StatusCodes.Status400BadRequest
                    });
                } 
                // Check if the department exists
                var userRole = await Mediator.Send(command);
                if (userRole == null)
                {
                    _logger.LogWarning($"User Role with ID {id} not found.");

                    return NotFound(new
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        Message = "User Role not found"
                    });
                }

                _logger.LogInformation($"User Role with ID {id} found. Proceeding with deletion.");

                // Attempt to delete the department
                var result = await Mediator.Send(new  DeleteRoleCommand {Id=id});

                if (result.IsSuccess)
                {
                    _logger.LogInformation($"User Role with ID {id} deleted successfully.");

                    return Ok(new
                    {
                        Message = result.Message,
                        StatusCode = StatusCodes.Status200OK
                      
                    });
                }

                _logger.LogWarning($"Failed to delete User Role with ID {id}. Reason: {result.Message}");

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