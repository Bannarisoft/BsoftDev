using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BSOFT.Application.Role.Queries.GetRole;
using BSOFT.Application.Role.Queries.GetRoleById;
using BSOFT.Application.Role.Commands.CreateRole;
using BSOFT.Application.Role.Commands.DeleteRole;
using BSOFT.Application.Role.Commands.UpdateRole;
using BSOFT.Application.Role.Queries.GetRolesAutocomplete;
using BSOFT.Application.Common.Interfaces;

namespace BSOFT.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoleController : ApiControllerBase
    {
        public RoleController(ISender mediator) : base(mediator)
        {
        }
       
        [HttpGet]
        public async Task<IActionResult> GetAllRoleAsync()
        {
           
            var roles =await Mediator.Send(new GetRoleQuery());
            return Ok(roles);
        }

        [HttpGet("{roleid}")]
        public async Task<IActionResult> GetByIdAsync(int roleid)
        {
            if (roleid <= 0)
            {
                return BadRequest("Invalid role ID");
            }
            var  role = await Mediator.Send(new GetRoleByIdQuery() {RoleId=roleid});
            if(role ==null)
            {
                return  NotFound();                
            }
            return Ok(role);

        }
        [HttpPost]
        public async Task<IActionResult>CreateAsync(CreateRoleCommand command)
        {
            var createrole = await Mediator.Send(command);
            return CreatedAtAction(nameof(GetByIdAsync),new {roleid = createrole.RoleId},createrole);
        }

         [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteAsync(int roleid, DeleteRoleCommand command)
        {
            if (roleid <= 0)
            {
                return BadRequest("Invalid role ID");
            }
            if (roleid != command.RoleId)
            {
                return BadRequest();
            }
            await Mediator.Send(command);

            return Ok("Deleted Successfully");
        }
       
     [HttpPut("update/{roleid}")]
    public async Task<IActionResult> UpdateAsync(int roleid, UpdateRoleCommand command)
    {      
       if (roleid <= 0)
        {
            return BadRequest("Invalid role ID");
        }
            if (roleid != command.RoleId)
        {
            return BadRequest();
        }
            await Mediator.Send(command);
        return Ok("Updated Successfully");
    }

    [HttpGet("roles")]
    public async Task<IActionResult> GetRoles([FromQuery] string searchTerm)
    {
        var roles = await Mediator.Send(new GetRolesAutocompleteQuery { SearchTerm = searchTerm });
        return Ok(roles);
    }

    }
}