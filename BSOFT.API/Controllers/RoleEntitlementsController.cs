using BSOFT.Application.RoleEntitlements.Commands.CreateRoleEntitlement;
using BSOFT.Application.RoleEntitlements.Queries.GetRoles;
using BSOFT.Application.Role.Queries.GetRolesAutocomplete;
using BSOFT.Application.Role.Queries.GetRoleById;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BSOFT.API.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    
    public class RoleEntitlementsController : ApiControllerBase
    {

    [HttpPost]
    public async Task<IActionResult> CreateRoleEntitlement([FromBody] CreateRoleEntitlementVm dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var command = new CreateRoleEntitlementCommand { RoleEntitlementVm = dto };
        var result = await Mediator.Send(command);
        return CreatedAtAction(nameof(GetRoleEntitlement), new { id = result }, dto);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetRoleEntitlement(int id)
    {
            var query = new GetRoleByIdQuery { RoleId = id };
            var result = await Mediator.Send(query);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllRoleEntitlements()
    {
        var query = new GetAllRoleEntitlementsQuery();
        var result = await Mediator.Send(query);
        return Ok(result);
    }


    }
}
