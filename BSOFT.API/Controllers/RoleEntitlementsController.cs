using Newtonsoft.Json;
using MediatR;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BSOFT.Application.RoleEntitlements.Commands.CreateRoleEntitlement;
using BSOFT.Application.RoleEntitlements.Commands.UpdateRoleRntitlement;
using BSOFT.Application.RoleEntitlements.Queries.GetRoleEntitlements;

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
    public RoleEntitlementsController(ISender mediator) : base(mediator)
    {
    }

    [HttpPost]
    public async Task<IActionResult> CreateRoleEntitlement(CreateRoleEntitlementCommand command)
    {
        Console.WriteLine($"Request Payload: {JsonConvert.SerializeObject(command)}");
        var result = await Mediator.Send(command);
        return Ok(new { Message = "Role Entitlement created successfully.", CreatedCount = result });
    }

    [HttpPut]
    public async Task<IActionResult> UpdateRoleEntitlement(UpdateRoleEntitlementCommand command)
    {
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