using MediatR;
using Microsoft.AspNetCore.Mvc;
using Core.Application.Units.Queries.GetUnits;
using Core.Application.Units.Queries.GetUnitById;
using Core.Application.Units.Commands.CreateUnit;
using Core.Application.Units.Commands.DeleteUnit;
using Core.Application.Units.Commands.UpdateUnit;
using Core.Application.Units.Queries.GetUnitAutoComplete;
using FluentValidation;
using UserManagement.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Core.Application.Units.Queries.GetUnitByUserId;

namespace UserManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class UnitController : ApiControllerBase
    {
        private readonly IValidator<CreateUnitCommand> _createUnitCommandValidator;
        private readonly IValidator<UpdateUnitCommand> _updateUnitCommandValidator;
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<UnitController> _logger;
        public UnitController(ISender mediator,IValidator<CreateUnitCommand> createUnitCommandValidator,IValidator<UpdateUnitCommand> updateUnitCommandValidator,ApplicationDbContext dbContext, ILogger<UnitController> logger) 
        : base(mediator)
        {
            _createUnitCommandValidator = createUnitCommandValidator;   
            _updateUnitCommandValidator = updateUnitCommandValidator; 
            _dbContext = dbContext;  
            _logger = logger;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllUnitsAsync([FromQuery] int PageNumber,[FromQuery] int PageSize,[FromQuery] string? SearchTerm = null)
        {
            var result = await Mediator.Send(
            new GetUnitQuery
             {
                PageNumber = PageNumber, 
                PageSize = PageSize, 
                SearchTerm = SearchTerm
                
            });
            
        if (result is null || result.Data is null || !result.Data.Any())
        {
            _logger.LogInformation($"No Unit Record {result.Data} not found in DB.");
            return NotFound(new
            {
                message = result.Message,
                statusCode = StatusCodes.Status404NotFound
                
            });
        }
         
        _logger.LogInformation($"Unit {result.Data.Count} Active Listed successfully.");
        return Ok(new
        {
            message = "Units retrieved successfully.",
            data = result.Data,
            statusCode = StatusCodes.Status200OK,
            TotalCount = result.TotalCount,
            PageNumber = result.PageNumber,
            PageSize = result.PageSize
        });
        }


        [HttpGet("{id}")]
        [ActionName(nameof(GetByIdAsync))]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
             if (id <= 0)
        {
            _logger.LogWarning($"UnitId {id} not found.");
            
            return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    message ="Invalid Unit ID"
                });
        }
            var unit = await Mediator.Send(new GetUnitByIdQuery() { Id = id});
           if (unit.IsSuccess)
            {
               _logger.LogInformation($"UnitId {unit.Data} Listed successfully.");
                return Ok(new
                {
                    message = unit.Message,
                    statusCode = StatusCodes.Status200OK,
                    data = unit.Data
                });
            }
            _logger.LogWarning($"UnitId {unit.Data} Not found.");
           return NotFound(new
            {
                message = unit.Message,
                statusCode = StatusCodes.Status404NotFound
            });
        
        }

    [HttpPost]
    public async Task<IActionResult> CreateUnitAsync(CreateUnitCommand createUnitCommand)
    {
        var validationResult = await _createUnitCommandValidator.ValidateAsync(createUnitCommand);
        _logger.LogWarning($"Validation failed: {string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage))}");
        if (!validationResult.IsValid)
        {
          return BadRequest(
            new
            {
                StatusCode = StatusCodes.Status400BadRequest,
                message = "Validation failed",
                errors = validationResult.Errors.Select(e => e.ErrorMessage)
            }
          );
        }
        var createdUnit = await Mediator.Send(createUnitCommand);
         if(createdUnit.IsSuccess)
         {
            _logger.LogInformation($"UnitId {createdUnit.Data} created successfully.");
             return Ok(new
             {
                 message = createdUnit.Message,
                 statusCode = StatusCodes.Status201Created,
                 data = createdUnit.Data
             });
         }
         _logger.LogWarning($"UnitId {createdUnit.Data} Creation failed.");
        return BadRequest(new
        {
            message = createdUnit.Message,
            statusCode = StatusCodes.Status400BadRequest
        });
       
    }


    [HttpPut("update")]
    public async Task<IActionResult> UpdateUnitAsync( UpdateUnitCommand updateUnitCommand)
    {
        var validationResult = await _updateUnitCommandValidator.ValidateAsync(updateUnitCommand);
        if (!validationResult.IsValid)
        { 
            _logger.LogWarning($"Validation failed: {string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage))}");
             return BadRequest(new
             {
                 StatusCode = StatusCodes.Status400BadRequest,
                 message = "Validation failed",
                 errors = validationResult.Errors.Select(e => e.ErrorMessage)
             });
        }
       
        var result = await Mediator.Send(updateUnitCommand);
        if(result.IsSuccess)
        {
            _logger.LogInformation($"UnitId {result.Data} updated successfully.");
            return Ok(new
            {
                message = result.Message,
                statusCode = StatusCodes.Status200OK
             
            });
        }
         _logger.LogWarning($"UnitId {result.Data} updated Failed.");
        return BadRequest(new
        {
            message = result.Message,
            statusCode = StatusCodes.Status400BadRequest
        });
    }


    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUnitAsync(int id)
    {
        
       var result = await Mediator.Send(new DeleteUnitCommand { UnitId = id });

        if (result.IsSuccess)
        {
            _logger.LogInformation($"UnitId {id} deleted successfully.");
            return Ok(new
            {
                message = result.Message,
                statusCode = StatusCodes.Status200OK,
                
            });
        }
        _logger.LogWarning($"UnitId {id} deleted Failed.");
        return BadRequest(new
        {
            message = result.Message,
            statusCode = StatusCodes.Status400BadRequest
        });
    }

       [HttpGet("by-name")]
        public async Task<IActionResult> GetUnit([FromQuery] string? unitname,int? CompanyId)
        {
            var units = await Mediator.Send(new GetUnitAutoCompleteQuery {SearchPattern = unitname??string.Empty, CompanyId = CompanyId??0});
             _logger.LogInformation("Search pattern: {SearchPattern}", unitname);
            if(units.IsSuccess)
            {
                _logger.LogInformation($"Unit {units.Data.Count} Listed successfully.");
                return Ok(new
                {
                    message = units.Message,
                    statusCode = StatusCodes.Status200OK,
                    data = units.Data
                });
            }
            _logger.LogWarning($"No Unit Record in the {unitname} not found in DB.");
            return NotFound(new
            {
                message = units.Message,
                statusCode = StatusCodes.Status404NotFound
            });
          
        }
      [HttpGet("by-userid")]
        public async Task<IActionResult> GetUnitByUserId([FromQuery] int? CompanyId,int UserId)
        {
            var units = await Mediator.Send(new GetUnitByUserIdQuery { CompanyId = CompanyId??0 ,UserId=UserId});
             
            if(units.IsSuccess)
            {
                _logger.LogInformation($"Unit {units.Data.Count} Listed successfully.");
                return Ok(new
                {
                    message = units.Message,
                    statusCode = StatusCodes.Status200OK,
                    data = units.Data
                });
            }
            _logger.LogWarning($"No Unit Record not found in DB.");
            return NotFound(new
            {
                message = units.Message,
                statusCode = StatusCodes.Status404NotFound
            });
          
        }
    }
}