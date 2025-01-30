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
        [HttpGet("get-by-id/{roleEntitlementId}")]        
        public async Task<IActionResult> GetByIdAsync(int roleEntitlementId)
        {
            if (roleEntitlementId <= 0)
            {
                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    message = "Invalid Country ID"
                });
            }            
            var result = await Mediator.Send(new GetRoleEntitlementByIdQuery { Id = roleEntitlementId });            
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
        var validationResult = await _updateRoleEntitlementCommandValidator.ValidateAsync(command);
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
        // var userExists = await Mediator.Send(new RoleEntitlementDto { RoleName = command.RoleName });
        // if (userExists == null)
        // {
        //     _logger.LogInformation("User {RoleName} not found for update.", command.RoleName);

        //     return NotFound(new { StatusCode = StatusCodes.Status404NotFound, message = $"RoleName {command.RoleName} not found." });
        // }

            var response = await Mediator.Send(command);
            if (response.IsSuccess)
            {
                _logger.LogInformation("RoleEntitlement {RoleName} updated successfully.", command.RoleName);

                return Ok(new { StatusCode = StatusCodes.Status200OK, message = response.Message });
            }
                _logger.LogWarning("RoleEntitlement update failed for RoleName: {RoleName}", command.RoleName);

                return BadRequest(new { StatusCode = StatusCodes.Status400BadRequest, message = response.Message }); 

        // try
        // {
        //     var result = await Mediator.Send(command);
        //     return Ok(new { Message = "Role Entitlement updated successfully.", Updated = result });
        // }
        // catch (ValidationException ex)
        // {
        //     return BadRequest(new { Message = ex.Message });
        // }
        // catch (Exception ex)
        // {
        //     return StatusCode(500, new { Message = "An unexpected error occurred.", Details = ex.Message });
        // }
    }
    [HttpDelete("delete")]        
        public async Task<IActionResult> DeleteAsync(DeleteRoleEntitlementCommand command)
        {
            var result = await Mediator.Send(command);
            if (result.IsSuccess)
            {
                return Ok(new 
                { 
                    StatusCode = StatusCodes.Status200OK,
                    Message = "RoleEntitlement Deleted successfully", 
                    data = result.Data 
                });
            }
            else
            {            
                return BadRequest(new 
                { 
                    StatusCode = StatusCodes.Status400BadRequest,
                    message = result.Message 
                });
            }
        }
    [HttpGet("get-by-role-name/{roleName}")]
    public async Task<IActionResult> GetRoleEntitlements(string roleName)
    {
        var query = new GetRoleEntitlementsQuery { RoleName = roleName };
            _logger.LogWarning("RoleEntitlement Listed successfully: {RoleName}", query);

        var result = await Mediator.Send(query);
        return Ok( new { StatusCode=StatusCodes.Status200OK, data = result });

    }   
}
}