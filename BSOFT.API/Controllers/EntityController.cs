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
using Core.Application.Common.Exceptions;
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
        if (result == null || result.Data == null || !result.Data.Any())
        {
            _logger.LogWarning("No Entity Record {Entity} not found in DB.", result.Data);
            return NotFound(new
            {
                message = result.Message,
                statusCode = StatusCodes.Status404NotFound
              
            });
        }
        _logger.LogInformation("Entity {Entities} Listed successfully.", result.Data.Count);
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
            _logger.LogWarning("Entity {EntityId} not found.", id);
            return BadRequest(new
            {
                StatusCode = StatusCodes.Status400BadRequest,
                message = "Invalid Entity ID"
            });
        }

        var result = await Mediator.Send(new GetEntityByIdQuery { EntityId = id });

        if (result.IsSuccess)
        {
              _logger.LogInformation("Entity {EntityId} Listed successfully.", result.Data);
              return Ok(new
             {
                 message = result.Message,
                 statusCode = StatusCodes.Status200OK,
                 data = result.Data
             }); 
        }
        _logger.LogWarning("Entity {EntityId} Not found.", result.Data);
        return NotFound(new
        {
            message = result.Message,
            statusCode = StatusCodes.Status404NotFound
        });
   
}
       
    [HttpGet("GenerateNewEntityCode")]
public async Task<IActionResult> GenerateEntityCodeAsync()
{
   
        
        var lastEntityCode = await Mediator.Send(new GetEntityLastCodeQuery());

        if (lastEntityCode.IsSuccess)
        {
            _logger.LogInformation("Entity {EntityCode} Generated successfully.", lastEntityCode.Data);
            return Ok(new
            {
                message = lastEntityCode.Message,
                statusCode = StatusCodes.Status200OK,
                data = lastEntityCode.Data
            });
        }
        _logger.LogInformation("Entity {EntityCode} Not found.", lastEntityCode.Data);
        return BadRequest(new
        {
            message = lastEntityCode.Message,
            statusCode = StatusCodes.Status400BadRequest
        });
   
}

[HttpPost]
public async Task<IActionResult> CreateAsync(CreateEntityCommand command)
{
    
    // Validate the incoming command
    var validationResult = await _createEntityCommandValidator.ValidateAsync(command);
    _logger.LogWarning("Validation failed: {ErrorDetails}", validationResult);
    if (!validationResult.IsValid)
    {
        
        return BadRequest(new
        {
            StatusCode = StatusCodes.Status400BadRequest,
            message = "Validation failed",
            errors = validationResult.Errors.Select(e => e.ErrorMessage).ToArray()
        });
    }

    // Process the command
    var createdEntityId = await _mediator.Send(command);

    if (createdEntityId.IsSuccess)
    {
     _logger.LogInformation("Entity {EntityName} created successfully.", command.EntityName);
      return Ok(new
      {
          StatusCode = StatusCodes.Status201Created,
          message =createdEntityId.Message,
          data = createdEntityId.Data
      });
    }
     _logger.LogWarning("Entity {EntityName} Creation failed.", command.EntityName);
      return BadRequest(new
        {
            StatusCode = StatusCodes.Status400BadRequest,
            message = createdEntityId.Message
        });
  
}

[HttpPut("update")]
public async Task<IActionResult> UpdateAsync( UpdateEntityCommand command)
{
  
        // Validate the incoming command
        var validationResult = await _updateEntityCommandValidator.ValidateAsync(command);
        _logger.LogWarning("Validation failed: {ErrorDetails}", validationResult);
        if (!validationResult.IsValid)
        {
           
            return BadRequest(new
            {
                StatusCode = StatusCodes.Status400BadRequest,
                message = "Validation failed",
                errors = validationResult.Errors.Select(e => e.ErrorMessage).ToArray()
            });
        }

        var updatedEntity = await _mediator.Send(command);

        if (updatedEntity.IsSuccess)
        {
            _logger.LogInformation("Entity {EntityName} updated successfully.", command.EntityName);
           return Ok(new
            {
                message = updatedEntity.Message,
                statusCode = StatusCodes.Status200OK
            });
        }
        _logger.LogWarning("Entity {EntityName} Update failed.", command.EntityName);
        return NotFound(new
        {
            message ="Entity not found",
            statusCode = StatusCodes.Status404NotFound
        });

        
}

[HttpDelete("delete")]
public async Task<IActionResult> DeleteEntityAsync( DeleteEntityCommand command)
{

        // Process the delete command
        var result = await _mediator.Send(command);

        if (result.IsSuccess) 
        {
            _logger.LogInformation("Entity {EntityName} deleted successfully.", command.EntityId);
             return Ok(new
            {
                message = result.Message,
                statusCode = StatusCodes.Status200OK
            });
            
        }
         _logger.LogWarning("Entity {EntityName} Not Found or Invalid EntityId.", command.EntityId);
        return NotFound(new
        {
            message = result.Message,
            statusCode = StatusCodes.Status404NotFound
        });
   
}

[HttpGet("GetEntitysearch")]
public async Task<IActionResult> GetEntity([FromQuery] string searchPattern)
{
      // Check if searchPattern is provided
        if (string.IsNullOrEmpty(searchPattern))
        {
            _logger.LogInformation("Search pattern cannot be empty.");
            return BadRequest(new
            {
                StatusCode = StatusCodes.Status400BadRequest,
                message = "Search pattern cannot be empty."
            });
        }

        // Fetch entities based on search pattern
        var entities = await Mediator.Send(new GetEntityAutocompleteQuery { SearchPattern = searchPattern });
        _logger.LogInformation("Search pattern: {SearchPattern}", searchPattern);
       if (entities.IsSuccess)
        {
        _logger.LogInformation("Entity {Entities} Listed successfully.", entities.Data.Count);
         return Ok(new  
            {
                message = entities.Message,
                statusCode = StatusCodes.Status200OK,
                data = entities.Data
            });
        }
        _logger.LogInformation("No Entity Record {Entity} not found in DB.", entities.Data);
        return NotFound(new
        {
            message = entities.Message,
            statusCode = StatusCodes.Status404NotFound
        });              
    
}
}
}