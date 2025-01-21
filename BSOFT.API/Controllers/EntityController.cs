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
        public EntityController(IMediator mediator, 
                             IValidator<CreateEntityCommand> createEntityCommandValidator, 
                             IValidator<UpdateEntityCommand> updateEntityCommandValidator,ApplicationDbContext dbContext) 
        : base(mediator)
        {
            _createEntityCommandValidator = createEntityCommandValidator;    
            _updateEntityCommandValidator = updateEntityCommandValidator;    
            _dbContext = dbContext; 
            _mediator = mediator; 
        }
        
[HttpGet]
public async Task<IActionResult> GetAllEntityAsync()
{
        
        var result  = await Mediator.Send(new GetEntityQuery());

        if (result.IsSuccess == false)
        {
            return NotFound(new
            {
                message = result.Message,
                statusCode = StatusCodes.Status404NotFound
            });
        }

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
            return BadRequest(new
            {
                StatusCode = StatusCodes.Status400BadRequest,
                message = "Invalid Entity ID"
            });
        }

        var result = await Mediator.Send(new GetEntityByIdQuery { EntityId = id });

        if (result.IsSuccess)
        {
              return Ok(new
             {
                 message = result.Message,
                 statusCode = StatusCodes.Status200OK,
                 data = result.Data
             }); 
        }

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
            return Ok(new
            {
                message = lastEntityCode.Message,
                statusCode = StatusCodes.Status200OK,
                data = lastEntityCode.Data
            });
        }

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
      return Ok(new
      {
          StatusCode = StatusCodes.Status201Created,
          message = createdEntityId.Message,
          data = createdEntityId
      });
    }

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
           return Ok(new
            {
                message = updatedEntity.Message,
                statusCode = StatusCodes.Status200OK
            });
        }

        return NotFound(new
        {
            message = updatedEntity.Message,
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
             return Ok(new
            {
                message = result.Message,
                statusCode = StatusCodes.Status200OK
            });
            
        }
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
            return BadRequest(new
            {
                StatusCode = StatusCodes.Status400BadRequest,
                message = "Search pattern cannot be empty."
            });
        }

        // Fetch entities based on search pattern
        var entities = await Mediator.Send(new GetEntityAutocompleteQuery { SearchPattern = searchPattern });

       if (entities.IsSuccess)
        {
         return Ok(new  
            {
                message = entities.Message,
                statusCode = StatusCodes.Status200OK,
                data = entities.Data
            });
        } 

        return NotFound(new
        {
            message = entities.Message,
            statusCode = StatusCodes.Status404NotFound
        });
        
    
}
}
}