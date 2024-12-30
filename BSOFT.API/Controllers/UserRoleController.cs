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

namespace BSOFT.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserRoleController : ApiControllerBase
    {
        public UserRoleController(ISender mediator) : base(mediator)
        {
        }
       
        [HttpGet]
        public async Task<IActionResult> GetAllRoleAsync()
        {
           
            var roles =await Mediator.Send(new GetRoleQuery());
            return Ok(roles);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid role ID");
            }
            var  role = await Mediator.Send(new GetRoleByIdQuery() {Id=id});
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
            return CreatedAtAction(nameof(GetByIdAsync),new {roleid = createrole.Id},createrole);
        }

         [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteAsync(int id, DeleteRoleCommand command)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid role ID");
            }
            if (id != command.Id)
            {
                return BadRequest();
            }
            await Mediator.Send(command);

            return Ok("Deleted Successfully");
        }
       
     [HttpPut("update/{id}")]
    public async Task<IActionResult> UpdateAsync(int id, UpdateRoleCommand command)
    {      
       if (id <= 0)
        {
            return BadRequest("Invalid role ID");
        }
            if (id != command.Id)
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