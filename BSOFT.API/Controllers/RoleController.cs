using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BSOFT.Application.Role.Queries.GetRole;
using BSOFT.Application.Role.Queries.GetRoleById;
using BSOFT.Application.Role.Commands.CreateRole;
using BSOFT.Application.Role.Commands.DeleteRole;
using BSOFT.Application.Role.Commands.UpdateRole;
using BSOFT.Domain.Interfaces;

namespace BSOFT.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoleController : ApiControllerBase
    {
       
        [HttpGet]
        public async Task<IActionResult> GetAllRoleAsync()
        {
           
            var role =await Mediator.Send(new GetRoleQuery());
            return Ok(role);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var  role = await Mediator.Send(new GetRoleByIdQuery() {RoleId=id});
            if(role ==null)
            {
                return  NotFound();                
            }
            return Ok(role);

        }
          [HttpPost]
        public async Task<IActionResult>CreateAsync(CreateRoleCommand command)
        {
            var createrole=await Mediator.Send(command);
            return Ok("Created successfully");
            return CreatedAtAction(nameof(GetByIdAsync),new {id=createrole.RoleId},createrole);
        }
         [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
       {
        var result = await Mediator.Send(new DeleteRoleCommand { RoleId = id });
        if (result ==null)
        {
             return NotFound();
        }

        return Ok("Deleted Successfully");
       }
       
     [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAsync(int id, UpdateRoleCommand command)
    {
       
        if (id != command.RoleId)
        {
            return BadRequest("UnitId Mismatch");
        }

        var UpdateRoleCommand = await Mediator.Send(command);
        return Ok("Updated Successfully");
    }

    }
}