using MediatR;
using Microsoft.AspNetCore.Mvc;
using Core.Application.Units.Queries.GetUnits;
using Core.Application.Units.Queries.GetUnitById;
using Core.Application.Units.Commands.CreateUnit;
using Core.Application.Units.Commands.DeleteUnit;
using Core.Application.Units.Commands.UpdateUnit;
using Core.Application.Units.Queries.GetUnitAutoComplete;
using FluentValidation;
using BSOFT.Infrastructure.Data;
using Core.Application.Common.Exceptions;

namespace BSOFT.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UnitController : ApiControllerBase
    {
        private readonly IValidator<CreateUnitCommand> _createUnitCommandValidator;
        private readonly IValidator<UpdateUnitCommand> _updateUnitCommandValidator;
        private readonly ApplicationDbContext _dbContext;
        public UnitController(ISender mediator,IValidator<CreateUnitCommand> createUnitCommandValidator,IValidator<UpdateUnitCommand> updateUnitCommandValidator,ApplicationDbContext dbContext) 
        : base(mediator)
        {
            _createUnitCommandValidator = createUnitCommandValidator;   
            _updateUnitCommandValidator = updateUnitCommandValidator; 
            _dbContext = dbContext;  
        }
        [HttpGet]
        public async Task<IActionResult> GetAllUnitsAsync()
        {
            var result = await Mediator.Send(new GetUnitQuery());
         
            if (result == null || result.Data == null || !result.Data.Any())
        {
            throw new CustomException(
                "No Units found.",
                new[] { "The database does not contain any units." },
                CustomException.HttpStatus.NotFound
            );
        }

        // Return success response with 200 OK
        return Ok(new
        {
            message = "Units retrieved successfully.",
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
            var unit = await Mediator.Send(new GetUnitByIdQuery() { Id = id});
           if (unit == null || unit.Data == null || !unit.Data.Any())
            {
               
                throw new CustomException(
                "Unit not found",
                new[] { $"The Unit with ID {id} does not exist." },
                CustomException.HttpStatus.NotFound
            );
            }
              // Return success response
        return Ok(new
        {
            message = "Unit retrieved successfully.",
            statusCode = StatusCodes.Status200OK,
            data = unit
        });
        }

    [HttpPost]
    public async Task<IActionResult> CreateUnitAsync(CreateUnitCommand command)
    {
        var validationResult = await _createUnitCommandValidator.ValidateAsync(command);
        if (!validationResult.IsValid)
        {
         // If validation fails, throw ValidationException with detailed error messages
        throw new Core.Application.Common.Exceptions.ValidationException(
            "Validation failed",  // General message
            validationResult.Errors.Select(e => e.ErrorMessage).ToArray()  // Validation errors
        );
        }
        var createdUnit = await Mediator.Send(command);
         if (createdUnit.Data <= 0)
    {
        throw new CustomException(
            "Failed to create Unit.",
            new[] { "Unit creation failed due to an unknown error." },
            CustomException.HttpStatus.InternalServerError
        );
    }
        // Return success response with 201 Created
        return CreatedAtAction(
        nameof(GetByIdAsync),
        new { id = createdUnit },
        new
        {
            message = "Unit created successfully.",
            data = new { id = createdUnit },
            statusCode = StatusCodes.Status201Created
        }
    );
       
    }


    [HttpPut("update/{id}")]
    public async Task<IActionResult> UpdateUnitAsync(int id, UpdateUnitCommand command)
    {
        var validationResult = await _updateUnitCommandValidator.ValidateAsync(command);
        if (!validationResult.IsValid)
        {
        return BadRequest(validationResult.Errors);
        }
        if (id != command.UnitId)
        {
            return BadRequest("UnitId Mismatch");
        }
        command.UnitId = id;
        var result = await Mediator.Send(command);
        return Ok("Updated Successfully");
    }


    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> DeleteUnitAsync(int id,DeleteUnitCommand command)
    {
         if(id != command.UnitId)
        {
           return BadRequest("UnitId Mismatch"); 
        }
        await Mediator.Send(command);

        return Ok("Status Closed Successfully");
    }

       [HttpGet("GetUnitSearch")]
        public async Task<IActionResult> GetUnit([FromQuery] string searchPattern)
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
            var units = await Mediator.Send(new GetUnitAutoCompleteQuery {SearchPattern = searchPattern});
        
            // Check if Units are returned
         if (units == null || units.Data == null || !units.Data.Any())
        {
            throw new CustomException(
                "No Units found matching the search pattern.",
                new[] { $"No entities found for the search pattern: {searchPattern}" },
                CustomException.HttpStatus.NotFound
            );
        }

        // Return success response with 200 OK and the entity data
        return Ok(new
        {
            message = "Units retrieved successfully.",
            data = units,
            statusCode = StatusCodes.Status200OK
        });
          
        }
     
    }
}