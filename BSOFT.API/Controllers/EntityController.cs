using MediatR;
using Microsoft.AspNetCore.Mvc;
using Core.Application.Entity.Queries.GetEntity;
using Core.Application.Entity.Queries.GetEntityById;
using Core.Application.Entity.Queries.GetEntityLastCode;
using Core.Application.Entity.Commands.CreateEntity;
using Core.Application.Entity.Commands.UpdateEntity;
using Core.Application.Entity.Commands.DeleteEntity;
using FluentValidation;
using BSOFT.Infrastructure.Data;
using Core.Application.Entity.Queries.GetEntityAutoComplete;


namespace BSOFT.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EntityController : ApiControllerBase
    {
        private readonly IValidator<CreateEntityCommand> _createEntityCommandValidator;
        private readonly IValidator<UpdateEntityCommand> _updateEntityCommandValidator;
        private readonly ApplicationDbContext _dbContext;
        public EntityController(ISender mediator, 
                             IValidator<CreateEntityCommand> createEntityCommandValidator, 
                             IValidator<UpdateEntityCommand> updateEntityCommandValidator,ApplicationDbContext dbContext) 
        : base(mediator)
        {
            _createEntityCommandValidator = createEntityCommandValidator;    
            _updateEntityCommandValidator = updateEntityCommandValidator;    
            _dbContext = dbContext;  
        }
        [HttpGet]
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

        [HttpGet("GenerateNewEntityCode")]
        public async Task<IActionResult> GenerateEntityCodeAsync()
        {
            var lastentitycode = await Mediator.Send(new GetEntityLastCodeQuery());
            return Ok(lastentitycode);
        }

    [HttpPost]
    public async Task<IActionResult> CreateAsync(CreateEntityCommand command)
    {
        var validationResult = await _createEntityCommandValidator.ValidateAsync(command);
        if (!validationResult.IsValid)
        {
        return BadRequest(validationResult.Errors);
        }
        var createdEntity = await Mediator.Send(command);
        return Ok("Created Successfully");
       
    }

     [HttpPut("update/{id}")]
    public async Task<IActionResult> UpdateAsync(int id, UpdateEntityCommand command)
    {
        var validationResult = await _updateEntityCommandValidator.ValidateAsync(command);
        if (!validationResult.IsValid)
        {
        return BadRequest(validationResult.Errors);
        }
        if (id != command.EntityId)
        {
            return BadRequest("EntityId Mismatch");
        }

        var updatedentity = await Mediator.Send(command);
        return Ok("Updated Successfully");
    }

    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> DeleteAsync(int id,DeleteEntityCommand command)
    {
        if(id != command.EntityId)
        {
           return BadRequest(); 
        }
        await Mediator.Send(command);
        return Ok("Status Closed Successfully");
    }

    [HttpGet("GetEntitysearch")]
        public async Task<IActionResult> GetEntity([FromQuery] string searchPattern)
        {
            var entities = await Mediator.Send(new GetEntityAutocompleteQuery {SearchPattern = searchPattern}); // Pass `searchPattern` to the constructor
            var activeentities = entities.Where(c => c.IsActive == 1).ToList(); 
            return Ok(activeentities);
        }
}
}