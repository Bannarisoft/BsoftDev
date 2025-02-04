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
using System.Net;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace BSOFT.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class EntityController : ApiControllerBase
    {
        private readonly IValidator<CreateEntityCommand> _createEntityCommandValidator;
        private readonly IValidator<UpdateEntityCommand> _updateEntityCommandValidator;
        private readonly ApplicationDbContext _dbContext;
        private readonly IMediator _mediator;

        private readonly ILogger<EntityController> _logger;
        public EntityController(IMediator mediator, 
                             IValidator<CreateEntityCommand> createEntityCommandValidator, 
                             IValidator<UpdateEntityCommand> updateEntityCommandValidator,ApplicationDbContext dbContext, 
                             ILogger<EntityController> logger) 
        : base(mediator)
        {
            _createEntityCommandValidator = createEntityCommandValidator;    
            _updateEntityCommandValidator = updateEntityCommandValidator;    
            _dbContext = dbContext; 
            _mediator = mediator; 
            _logger = logger;
        }
        
[HttpGet]
public async Task<IActionResult> GetAllEntityAsync()
{
        
        var result  = await Mediator.Send(new GetEntityQuery());
        if (result is null || result.Data is null || !result.Data.Any())
        {
            _logger.LogWarning($"No Entity Record {result.Data} not found in DB.");
            return NotFound(new
            {
                message = result.Message,
                statusCode = StatusCodes.Status404NotFound
              
            });
        }
        _logger.LogInformation($"Entity {result.Data.Count} Listed successfully.");
        return Ok(new
        {
            
            message = result.Message,
            data = result.Data,
            statusCode = StatusCodes.Status200OK
        });
   
}
    [HttpGet("{id}")]
    [ActionName(nameof(GetByIdAsync))]
public async Task<IActionResult> GetByIdAsync(int id)
{
        
        if (id <= 0)
        {
            _logger.LogWarning($"EntityId {id} not found.");
            return BadRequest(new
            {
                StatusCode = StatusCodes.Status400BadRequest,
                message = "Invalid Entity ID"
            });
        }

        var result = await Mediator.Send(new GetEntityByIdQuery { EntityId = id });

        if (result.IsSuccess)
        {
              _logger.LogInformation($"EntityId {result.Data} Listed successfully.");
              return Ok(new
             {
                 message = result.Message,
                 statusCode = StatusCodes.Status200OK,
                 data = result.Data
             }); 
        }
        _logger.LogWarning($"EntityId {result.Data} Not found.");
        return NotFound(new
        {
            message = result.Message,
            statusCode = StatusCodes.Status404NotFound
        });
   
}
       
    [HttpGet("entity/new-code")]
public async Task<IActionResult> GenerateEntityCodeAsync()
{
   
        
        var lastEntityCode = await Mediator.Send(new GetEntityLastCodeQuery());

        if (lastEntityCode.IsSuccess)
        {
            _logger.LogInformation($"EntityCode {lastEntityCode.Data} Generated successfully.");
            return Ok(new
            {
                message = lastEntityCode.Message,
                statusCode = StatusCodes.Status200OK,
                data = lastEntityCode.Data
            });
        }
        _logger.LogInformation($"EntityCode {lastEntityCode.Data} Not found.");
        return BadRequest(new
        {
            message = lastEntityCode.Message,
            statusCode = StatusCodes.Status400BadRequest
        });
   
}

[HttpPost]
public async Task<IActionResult> CreateAsync(CreateEntityCommand createEntityCommand)
{
    
    // Validate the incoming command
    var validationResult = await _createEntityCommandValidator.ValidateAsync(createEntityCommand);
     _logger.LogWarning($"Validation failed: {string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage))}");
    if (!validationResult.IsValid)
    {
        
        return BadRequest(new
        {
            StatusCode = StatusCodes.Status400BadRequest,
            message = "Validation failed",
            errors = validationResult.Errors.Select(e => e.ErrorMessage)
        });
    }

    // Process the command
    var createdEntityId = await _mediator.Send(createEntityCommand);

    if (createdEntityId.IsSuccess)
    {
     _logger.LogInformation($"EntityName {createEntityCommand.EntityName} created successfully.");
      return Ok(new
      {
          StatusCode = StatusCodes.Status201Created,
          message =createdEntityId.Message,
          data = createdEntityId.Data
      });
    }
     _logger.LogWarning($"EntityName {createEntityCommand.EntityName} Creation failed.");
      return BadRequest(new
        {
            StatusCode = StatusCodes.Status400BadRequest,
            message = createdEntityId.Message
        });
  
}

[HttpPut]
public async Task<IActionResult> UpdateAsync( UpdateEntityCommand updateEntityCommand)
{
  
        // Validate the incoming command
        var validationResult = await _updateEntityCommandValidator.ValidateAsync(updateEntityCommand);
        _logger.LogWarning($"Validation failed: {string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage))}");
        if (!validationResult.IsValid)
        {
           
            return BadRequest(new
            {
                StatusCode = StatusCodes.Status400BadRequest,
                message = "Validation failed",
                errors = validationResult.Errors.Select(e => e.ErrorMessage)
            });
        }

        var updatedEntity = await _mediator.Send(updateEntityCommand);

        if (updatedEntity.IsSuccess)
        {
            _logger.LogInformation($"EntityName {updateEntityCommand.EntityName} updated successfully.");
           return Ok(new
            {
                message = updatedEntity.Message,
                statusCode = StatusCodes.Status200OK
            });
        }
        _logger.LogWarning($"EntityName {updateEntityCommand.EntityName} Update failed.");
        return NotFound(new
        {
            message =updatedEntity.Message,
            statusCode = StatusCodes.Status404NotFound
        });

        
}

[HttpDelete]
public async Task<IActionResult> DeleteEntityAsync( DeleteEntityCommand deleteEntityCommand)
{

        // Process the delete command
        var result = await _mediator.Send(deleteEntityCommand);

        if (result.IsSuccess) 
        {
            _logger.LogInformation($"EntityId {deleteEntityCommand.EntityId} deleted successfully.");
             return Ok(new
            {
                message = result.Message,
                statusCode = StatusCodes.Status200OK
            });
            
        }
         _logger.LogWarning($"EntityId {deleteEntityCommand.EntityId} Not Found or Invalid EntityId.");
        return NotFound(new
        {
            message = result.Message,
            statusCode = StatusCodes.Status404NotFound
        });
   
}

[HttpGet("entity/by-name/{name}")]
public async Task<IActionResult> GetEntity([FromRoute] string name)
{
      // Check if searchPattern is provided
        if (string.IsNullOrEmpty(name))
        {
            _logger.LogInformation($"Search pattern {name} cannot be empty.");
            return BadRequest(new
            {
                StatusCode = StatusCodes.Status400BadRequest,
                message = "Search pattern cannot be empty."
            });
        }

        // Fetch entities based on search pattern
        var entities = await Mediator.Send(new GetEntityAutocompleteQuery { SearchPattern = name });
        _logger.LogInformation("Search pattern: {SearchPattern}", name);
       if (entities.IsSuccess)
        {
        _logger.LogInformation($"Entity {entities.Data.Count} Listed successfully.");
         return Ok(new  
            {
                message = entities.Message,
                statusCode = StatusCodes.Status200OK,
                data = entities.Data
            });
        }
        _logger.LogInformation($"No Entity Record in the DB {name} not found.");
        return NotFound(new
        {
            message = entities.Message,
            statusCode = StatusCodes.Status404NotFound
        });              
    
}
}
}