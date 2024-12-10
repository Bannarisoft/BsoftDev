using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BSOFT.API.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    
    public class RoleEntitlementsController : ApiControllerBase
    {

    [HttpPost]
    public async Task<IActionResult> CreateRoleEntitlement([FromBody] CreateRoleEntitlementDto dto)
    {
        var command = new CreateRoleEntitlementCommand { RoleEntitlementDto = dto };
        var result = await Mediator.Send(command);
        return CreatedAtAction(nameof(GetRoleEntitlement), new { id = result }, dto);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllRoleEntitlements()
    {
        var query = new GetAllRoleEntitlementsQuery();
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    }
}
