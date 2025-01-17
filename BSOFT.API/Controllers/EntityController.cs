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
        // Fetch all entities
        var result  = await Mediator.Send(new GetEntityQuery());
        // Access the list from the result
        // Adjust 'Data' based on your actual property name

        // Check if the result is empty
        if (result == null || result.Data == null || !result.Data.Any())
        {
            throw new CustomException(
                "No entities found.",
                new[] { "The database does not contain any entities." },
                CustomException.HttpStatus.NotFound
            );
        }

        // Return success response with 200 OK
        return Ok(new
        {
            message = "Entities retrieved successfully.",
            data = result,
            statusCode = StatusCodes.Status200OK
        });
   
}
    [HttpGet("{id}")]
    [ActionName(nameof(GetByIdAsync))]
public async Task<IActionResult> GetByIdAsync(int id)
{
        if (id <= 0)
        {
            throw new CustomException(
                "Validation failed",
                new[] { "The provided ID must be greater than zero." },
                CustomException.HttpStatus.BadRequest
            );
        }

        var result = await Mediator.Send(new GetEntityByIdQuery { EntityId = id });
        if (result == null || result.Data == null || !result.Data.Any())
        {
         throw new CustomException(
        "Entity not found",
        new[] { $"The entity with ID {id} does not exist." },
        CustomException.HttpStatus.NotFound
    );
        } 

        // Return success response
        return Ok(new
        {
            message = "Entity retrieved successfully.",
            statusCode = StatusCodes.Status200OK,
            data = result
        });
   
}
       
    [HttpGet("GenerateNewEntityCode")]
public async Task<IActionResult> GenerateEntityCodeAsync()
{
   
        // Fetch the last entity code using the mediator
        var lastEntityCode = await Mediator.Send(new GetEntityLastCodeQuery());

        // Check if the result is null or empty
        if (string.IsNullOrEmpty(lastEntityCode))
        {
            throw new CustomException(
                "No entity code found in the database.",
                new[] { "The database does not contain any entity code." },
                CustomException.HttpStatus.NotFound
            );
        }

        // Return the last entity code as a response
        return Ok(new
        {
            message = "Entity code retrieved successfully.",
            data = lastEntityCode,
            statusCode = StatusCodes.Status200OK
        });
   
}

[HttpPost]
public async Task<IActionResult> CreateAsync(CreateEntityCommand command)
{
    
    // Validate the incoming command
    var validationResult = await _createEntityCommandValidator.ValidateAsync(command);
    if (!validationResult.IsValid)
    {
        
        // If validation fails, throw ValidationException with detailed error messages
        throw new Core.Application.Common.Exceptions.ValidationException(
            "Validation failed",  // General message
            validationResult.Errors.Select(e => e.ErrorMessage).ToArray()  // Validation errors
        );
    }

    // Process the command
    var createdEntityId = await _mediator.Send(command);

    if (createdEntityId.Data <= 0)
    {
        throw new CustomException(
            "Failed to create entity.",
            new[] { "Entity creation failed due to an unknown error." },
            CustomException.HttpStatus.InternalServerError
        );
    }

    // Return success response with 201 Created
    return CreatedAtAction(
        nameof(GetByIdAsync),
        new { id = createdEntityId },
        new
        {
            message = "Entity created successfully.",
            data = new { id = createdEntityId },
            statusCode = StatusCodes.Status201Created
        }
    );
  
}

[HttpPut("update/{id}")]
public async Task<IActionResult> UpdateAsync(int id, UpdateEntityCommand command)
{
  
        // Validate the incoming command
        var validationResult = await _updateEntityCommandValidator.ValidateAsync(command);
        if (!validationResult.IsValid)
        {
           
              throw new Core.Application.Common.Exceptions.ValidationException(
            "Validation failed",  // General message
            validationResult.Errors.Select(e => e.ErrorMessage).ToArray()  // Validation errors
        );
        }

        // Check for EntityId mismatch
        if (id != command.EntityId)
        {
            throw new CustomException(
                "EntityId Mismatch",
                new[] { "The provided EntityId does not match the ID in the request URL." },
                CustomException.HttpStatus.BadRequest
            );
        }

        // Process the command to update the entity
        var updatedEntity = await _mediator.Send(command);

        // Check if the entity was updated successfully
        if (updatedEntity.Data <= 0)
        {
            throw new CustomException(
                "Entity not found",
                new[] { $"The entity with ID {id} does not exist." },
                CustomException.HttpStatus.NotFound
            );
        }

        // Return success response with 200 OK
        return Ok(new
        {
            message = "Entity updated successfully.",
            statusCode = StatusCodes.Status200OK
        });
    
}

[HttpDelete("delete/{id}")]
public async Task<IActionResult> DeleteEntityAsync(int id, DeleteEntityCommand command)
{
     // Validate if the EntityId matches the URL id
        if (id != command.EntityId)
        {
            throw new CustomException(
                "EntityId Mismatch",
                new[] { "The provided EntityId does not match the ID in the request URL." },
                CustomException.HttpStatus.BadRequest
            );
        }

        // Process the delete command
        var result = await _mediator.Send(command);

        // Check if the entity was found and deleted
        if (result.Data <= 0) // Assuming 0 or -1 indicates "not found"
        {
             throw new CustomException(
                "Entity not found",
                new[] { $"The entity with ID {id} does not exist." },
                CustomException.HttpStatus.NotFound
            );
        }

        // Return success response
        return Ok(new
        {
            message = "Entity deleted successfully.",
            statusCode = StatusCodes.Status200OK
        });
   
}

[HttpGet("GetEntitysearch")]
public async Task<IActionResult> GetEntity([FromQuery] string searchPattern)
{
      // Check if searchPattern is provided
        if (string.IsNullOrEmpty(searchPattern))
        {
            throw new CustomException(
                "Search pattern cannot be empty.",
                new[] { "Please provide a valid search pattern." },
                CustomException.HttpStatus.BadRequest
            );
        }

        // Fetch entities based on search pattern
        var entities = await Mediator.Send(new GetEntityAutocompleteQuery { SearchPattern = searchPattern });

       if (entities == null || entities.Data == null || !entities.Data.Any())
        {
         throw new CustomException(
        "Entity not found",
        new[] { $"No entities found for the search pattern: {searchPattern}" },
        CustomException.HttpStatus.NotFound
    );
        } 

        // Return success response with 200 OK and the entity data
        return Ok(new
        {
            message = "Entities retrieved successfully.",
            data = entities,
            statusCode = StatusCodes.Status200OK
        });
    
}
}
}