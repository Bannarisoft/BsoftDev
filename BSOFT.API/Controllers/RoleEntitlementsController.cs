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
         
       public RoleEntitlementsController(ISender mediator, 
                             IValidator<CreateRoleEntitlementCommand> createRoleEntitlementCommandValidator, 
                             IValidator<UpdateRoleEntitlementCommand> updateRoleEntitlementCommandValidator, 
                             ApplicationDbContext dbContext) 
         : base(mediator)
        {        
            _createRoleEntitlementCommandValidator = createRoleEntitlementCommandValidator;
            _updateRoleEntitlementCommandValidator = updateRoleEntitlementCommandValidator;    
            _dbContext = dbContext;  
             
        }

    [HttpPost]
    public async Task<IActionResult> CreateRoleEntitlement(CreateRoleEntitlementCommand command)
    {
        var validationResult = await _createRoleEntitlementCommandValidator.ValidateAsync(command);
        if (!validationResult.IsValid)
        {
        return BadRequest(validationResult.Errors);
        }

        var result = await Mediator.Send(command);
        return Ok(new { Message = "Role Entitlement created successfully.", CreatedCount = result });
    }

    [HttpPut]
    public async Task<IActionResult> UpdateRoleEntitlement(UpdateRoleEntitlementCommand command)
    {
        var validationResult = await _updateRoleEntitlementCommandValidator.ValidateAsync(command);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        try
        {
            var result = await Mediator.Send(command);
            return Ok(new { Message = "Role Entitlement updated successfully.", Updated = result });
        }
        catch (ValidationException ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "An unexpected error occurred.", Details = ex.Message });
        }
    }

    [HttpGet("{roleName}")]
    public async Task<IActionResult> GetRoleEntitlements(string roleName)
    {
        var query = new GetRoleEntitlementsQuery { RoleName = roleName };
        var result = await Mediator.Send(query);
        return Ok(result);
    }   
}
}