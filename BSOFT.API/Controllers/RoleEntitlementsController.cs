using Newtonsoft.Json;
using MediatR;
using BSOFT.Infrastructure.Data;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Core.Application.RoleEntitlements.Commands.CreateRoleEntitlement;
using Core.Application.RoleEntitlements.Commands.UpdateRoleRntitlement;
using Core.Application.RoleEntitlements.Queries.GetRoleEntitlements;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Core.Application.RoleEntitlements.Commands.DeleteRoleEntitlement;
using Core.Application.RoleEntitlements.Queries.GetRoleEntitlementById;

namespace BSOFT.API.Controllers
{
[ApiController]
[Route("api/[controller]")]

public class RoleEntitlementsController : ApiControllerBase
{
    // public RoleEntitlementsController(ISender mediator) : base(mediator)
    // {
    // }

    private readonly IValidator<CreateRoleEntitlementCommand> _createRoleEntitlementCommandValidator;
         private readonly IValidator<UpdateRoleEntitlementCommand> _updateRoleEntitlementCommandValidator;
         private readonly ApplicationDbContext _dbContext;
         private readonly ILogger<RoleEntitlementsController> _logger;

         
       public RoleEntitlementsController(ISender mediator, 
                             IValidator<CreateRoleEntitlementCommand> createRoleEntitlementCommandValidator, 
                             IValidator<UpdateRoleEntitlementCommand> updateRoleEntitlementCommandValidator, 
                             ApplicationDbContext dbContext,
                             ILogger<RoleEntitlementsController> logger) 
         : base(mediator)
        {        
            _createRoleEntitlementCommandValidator = createRoleEntitlementCommandValidator;
            _updateRoleEntitlementCommandValidator = updateRoleEntitlementCommandValidator;    
            _dbContext = dbContext;  
            _logger = logger;
        }
        [HttpGet("{id}")]        
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    message = "Invalid Country ID"
                });
            }            
            var result = await Mediator.Send(new GetRoleEntitlementByIdQuery { Id = id });            
            if (!result.IsSuccess)
            {                
                return NotFound(new 
                { 
                    StatusCode = StatusCodes.Status404NotFound,
                    message = result.Message
                });
            }
            return Ok(new
            {
                StatusCode = StatusCodes.Status200OK,
                data = result.Data
            });
        }

    [HttpPost]
    public async Task<IActionResult> CreateRoleEntitlement(CreateRoleEntitlementCommand command)
    {
        var validationResult = await _createRoleEntitlementCommandValidator.ValidateAsync(command);
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
                _logger.LogInformation("RoleEntitement {RoleName} created successfully.", command.RoleName);

                return Ok(new { StatusCode = StatusCodes.Status201Created, message = response.Message, data = response.Data });
            }
                _logger.LogWarning("RoleEntitement creation failed for user: {RoleName}", command.RoleName);

                return BadRequest(new { StatusCode = StatusCodes.Status400BadRequest, message = response.Message }); 
             
    }

    [HttpPut]
    public async Task<IActionResult> UpdateRoleEntitlement(UpdateRoleEntitlementCommand command)
    {
        if (command == null)
        {
            _logger.LogError("UpdateRoleEntitlementCommand is null.");
            return BadRequest(new
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Message = "Invalid request. Command cannot be null."
            });
        }

        //  Validate request
        var validationResult = await _updateRoleEntitlementCommandValidator.ValidateAsync(command);

        if (!validationResult.IsValid)
        {
            _logger.LogWarning("Validation failed: {ErrorDetails}", string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage)));

            return BadRequest(new
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Message = "Validation failed",
                Errors = validationResult.Errors.Select(e => e.ErrorMessage)
            });
        }

        try
        {
            _logger.LogInformation("Processing UpdateRoleEntitlementCommand for Role: {RoleName}", command.RoleName);
            
            var response = await Mediator.Send(command);

            if (response == null)
            {
                _logger.LogWarning("RoleEntitlement update failed: No response received for RoleName: {RoleName}", command.RoleName);
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = "Unexpected error: No response from service."
                });
            }

            if (!response.IsSuccess)
            {
                if (response.Message.Contains("not found", StringComparison.OrdinalIgnoreCase))
                {
                    _logger.LogWarning("RoleEntitlement update failed: RoleName {RoleName} not found.", command.RoleName);
                    return NotFound(new
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        Message = response.Message
                    });
                }

                _logger.LogWarning("RoleEntitlement update failed for RoleName: {RoleName}", command.RoleName);
                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = response.Message
                });
            }

            _logger.LogInformation("RoleEntitlement {RoleName} updated successfully.", command.RoleName);
            return Ok(new
            {
                StatusCode = StatusCodes.Status200OK,
                Message = response.Message
            });
        }
        catch (ValidationException ex)
        {
            _logger.LogError(ex, "Validation error while updating RoleEntitlement for RoleName: {RoleName}", command.RoleName);
            return BadRequest(new
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Message = "Validation error",
                Details = ex.Message
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred while updating RoleEntitlement for RoleName: {RoleName}", command.RoleName);
            return StatusCode(StatusCodes.Status500InternalServerError, new
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = "An unexpected error occurred.",
                Details = ex.Message
            });
        }
    }
    [HttpDelete("{id}")]        
        public async Task<IActionResult> DeleteAsync(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new
            {
                    StatusCode = StatusCodes.Status400BadRequest,
                    message = "Invalid RoleEntitlement ID"
                });
            }
            var result = await Mediator.Send(new DeleteRoleEntitlementCommand { Id= id });                 
            if (!result.IsSuccess)
            {          
                 _logger.LogWarning($"Deletion failed for RoleEntitlement {id}: {result?.Message ?? "Unknown error"}.");
    
                return NotFound(new 
                { 
                    StatusCode = StatusCodes.Status404NotFound,
                    message = result.Message
                });
            }
            _logger.LogInformation($"RoleEntitlement {id} deleted successfully.");

            return Ok(new
            {
                StatusCode = StatusCodes.Status200OK,
                data =$"RoleEntitlement ID {id} Deleted" 
            });
 
        }
    [HttpGet("by-name/{name}")]
    public async Task<IActionResult> GetRoleEntitlements(string name)
    {
        var query = new GetRoleEntitlementsQuery { RoleName = name };
            _logger.LogWarning("RoleEntitlement Listed successfully: {RoleName}", query);

        var result = await Mediator.Send(query);
        return Ok( new { StatusCode=StatusCodes.Status200OK, data = result });

    }   
}
}