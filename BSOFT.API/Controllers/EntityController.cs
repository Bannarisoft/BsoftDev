using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BSOFT.Application.Entity.Queries.GetEntity;
using BSOFT.Application.Entity.Queries.GetEntityById;
using BSOFT.Application.Entity.Queries.GetEntityLastCode;
using BSOFT.Application.Entity.Commands.CreateEntity;
using BSOFT.Application.Entity.Commands.UpdateEntity;
using BSOFT.Application.Entity.Commands.DeleteEntity;
using BSOFT.Application.Common.Interfaces;
using System.IO;
using System.Linq;
using System.Threading.Tasks;


namespace BSOFT.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EntityController : ApiControllerBase
    {
        public EntityController(ISender mediator) : base(mediator)
        {
        }
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAllEntityAsync()
        {
            var entity = await Mediator.Send(new GetEntityQuery());
            return Ok(entity);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var entitybyid = await Mediator.Send(new GetEntityByIdQuery() {EntityId = id});
            if(entitybyid == null)
            {
                BadRequest("ID in the URL does not match the command Unit.");
            }
            return Ok(entitybyid);
        }

        [HttpGet("GenerateEntityCode")]
        public async Task<IActionResult> GenerateEntityCodeAsync()
        {
            var lastentitycode = await Mediator.Send(new GetEntityLastCodeQuery());
            return Ok(lastentitycode);
        }

    [HttpPost]
    public async Task<IActionResult> CreateAsync(CreateEntityCommand command)
    {
        var createdEntity = await Mediator.Send(command);
        return Ok("Created Successfully");
        return CreatedAtAction(nameof(GetByIdAsync), new { id = createdEntity.EntityId}, createdEntity);
    }

     [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAsync(int id, UpdateEntityCommand command)
    {
        if (id != command.EntityId)
        {
            return BadRequest("EntityId Mismatch");
        }

        var updatedentity = await Mediator.Send(command);
        return Ok("Updated Successfully");
    }

    [HttpPut("delete/{id}")]
    public async Task<IActionResult> DeleteAsync(int id,DeleteEntityCommand command)
    {
        if(id != command.EntityId)
        {
           return BadRequest(); 
        }
        await Mediator.Send(command);
        return Ok("Status Closed Successfully");
   }
    }
}