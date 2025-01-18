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
         
        
        return Ok(new
        {
            message = "Units retrieved successfully.",
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
                    message ="Invalid Unit ID"
                });
        }
            var unit = await Mediator.Send(new GetUnitByIdQuery() { Id = id});
           if (unit.IsSuccess)
            {
               
                return Ok(new
                {
                    message = unit.Message,
                    statusCode = StatusCodes.Status200OK,
                    data = unit.Data
                });
            }
           return NotFound(new
            {
                message = unit.Message,
                statusCode = StatusCodes.Status404NotFound
            });
        
        }

    [HttpPost]
    public async Task<IActionResult> CreateUnitAsync(CreateUnitCommand command)
    {
        var validationResult = await _createUnitCommandValidator.ValidateAsync(command);
        if (!validationResult.IsValid)
        {
          return BadRequest(
            new
            {
                StatusCode = StatusCodes.Status400BadRequest,
                message = "Validation failed",
                errors = validationResult.Errors.Select(e => e.ErrorMessage).ToArray()
            }
          );
        }
        var createdUnit = await Mediator.Send(command);
         if(createdUnit.IsSuccess)
         {
             return Ok(new
             {
                 message = createdUnit.Message,
                 statusCode = StatusCodes.Status201Created,
                 data = createdUnit.Data
             });
         }
        
        return BadRequest(new
        {
            message = createdUnit.Message,
            statusCode = StatusCodes.Status400BadRequest
        });
       
    }


    [HttpPut("update")]
    public async Task<IActionResult> UpdateUnitAsync( UpdateUnitCommand command)
    {
        var validationResult = await _updateUnitCommandValidator.ValidateAsync(command);
        if (!validationResult.IsValid)
        {
             return BadRequest(new
             {
                 StatusCode = StatusCodes.Status400BadRequest,
                 message = "Validation failed",
                 errors = validationResult.Errors.Select(e => e.ErrorMessage).ToArray()
             });
        }
       
        var result = await Mediator.Send(command);
        if(result.IsSuccess)
        {
            return Ok(new
            {
                message = result.Message,
                statusCode = StatusCodes.Status200OK,
                data = result.Data
            });
        }
        
        return BadRequest(new
        {
            message = result.Message,
            statusCode = StatusCodes.Status400BadRequest
        });
    }


    [HttpDelete("delete")]
    public async Task<IActionResult> DeleteUnitAsync(DeleteUnitCommand command)
    {
        
       var result = await Mediator.Send(command);

        if (result.IsSuccess)
        {
            return Ok(new
            {
                message = result.Message,
                statusCode = StatusCodes.Status200OK,
                data = result.Data
            });
        }
        
        return BadRequest(new
        {
            message = result.Message,
            statusCode = StatusCodes.Status400BadRequest
        });
    }

       [HttpGet("GetUnitSearch")]
        public async Task<IActionResult> GetUnit([FromQuery] string searchPattern)
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
            var units = await Mediator.Send(new GetUnitAutoCompleteQuery {SearchPattern = searchPattern});
        
            if(units.IsSuccess)
            {
                return Ok(new
                {
                    message = units.Message,
                    statusCode = StatusCodes.Status200OK,
                    data = units.Data
                });
            }

            return NotFound(new
            {
                message = units.Message,
                statusCode = StatusCodes.Status404NotFound
            });
          
        }
     
    }
}