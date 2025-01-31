using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using BSOFT.Infrastructure.Data;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Core.Application.Currency.Queries.GetCurrency;
using Core.Application.Currency.Queries.GetCurrencyById;
using Core.Application.Currency.Queries.GetCurrencyAutoComplete;
using Core.Application.Currency.Commands.CreateCurrency;
using Core.Application.Currency.Commands.UpdateCurrency;
using FluentValidation;
using Core.Application.AdminSecuritySettings.Commands.DeleteAdminSecuritySettings;
using Core.Application.Currency.Commands.DeleteCurrency;

namespace BSOFT.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CurrencyController : ApiControllerBase
    {
        private readonly IValidator<CreateCurrencyCommand> _createCurrencyCommandValidator;
        private readonly IValidator<UpdateCurrencyCommand> _updateCurrencyCommandValidator;
        private readonly ApplicationDbContext _dbContext;
        private readonly IMediator _mediator;
        private readonly ILogger<CurrencyController> _logger;

         public CurrencyController(IMediator mediator,ApplicationDbContext dbContext, 
                             ILogger<CurrencyController> logger, IValidator<CreateCurrencyCommand> createCurrencyCommandValidator, IValidator<UpdateCurrencyCommand> updateCurrencyCommandValidator) 
        : base(mediator)
        {
               
            _dbContext = dbContext; 
            _mediator = mediator; 
            _logger = logger;
            _createCurrencyCommandValidator = createCurrencyCommandValidator;   
            _updateCurrencyCommandValidator = updateCurrencyCommandValidator;
        }
        

        [HttpGet]
        public async Task<IActionResult> GetAllCurrencyAsync()
        {
        
        var result  = await Mediator.Send(new GetCurrencyQuery());
        if (result == null || result.Data == null || !result.Data.Any())
        {
            _logger.LogWarning("No Currency Record {Currency} not found in DB.", result.Data);
            return NotFound(new
            {
                message = result.Message,
                statusCode = StatusCodes.Status404NotFound
              
            });
        }
        _logger.LogInformation("Currency {Currencies} Listed successfully.", result.Data.Count);
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
            _logger.LogWarning("Currency {CurrencyId} not found.", id);
            return BadRequest(new
            {
                StatusCode = StatusCodes.Status400BadRequest,
                message = "Invalid Entity ID"
            });
        }

        var result = await Mediator.Send(new GetCurrencyByIdQuery { CurrencyId = id });

        if (result.IsSuccess)
        {
              _logger.LogInformation("Currency {CurrencyId} Listed successfully.", result.Data);
              return Ok(new
             {
                 message = result.Message,
                 statusCode = StatusCodes.Status200OK,
                 data = result.Data
             }); 
        }
        _logger.LogWarning("Currency {CurrencyId} Not found.", result.Data);
        return NotFound(new
        {
            message = result.Message,
            statusCode = StatusCodes.Status404NotFound
        });
   
}
       [HttpGet("GetCurrencysearch")]
        public async Task<IActionResult> GetCurrency([FromQuery] string searchPattern)
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
        var result = await Mediator.Send(new GetCurrencyAutocompleteQuery { SearchPattern = searchPattern });
        _logger.LogInformation("Search pattern: {SearchPattern}", searchPattern);
       if (result.IsSuccess)
        {
        _logger.LogInformation("Currency {Currencies} Listed successfully.", result.Data.Count);
         return Ok(new  
            {
                message = result.Message,
                statusCode = StatusCodes.Status200OK,
                data = result.Data
            });
        }
        _logger.LogInformation("No Currency Record {Currency} not found in DB.", result.Data);
        return NotFound(new
        {
            message = result.Message,
            statusCode = StatusCodes.Status404NotFound
        });                  
}
[HttpPost]
public async Task<IActionResult> CreateAsync(CreateCurrencyCommand command)
{
    
    // Validate the incoming command
   var validationResult = await _createCurrencyCommandValidator.ValidateAsync(command);
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
    var createdcurrencyId = await _mediator.Send(command);

    if (createdcurrencyId.IsSuccess)
    {
     _logger.LogInformation("Currency {Code} created successfully.", command.Code);
      return Ok(new
      {
          StatusCode = StatusCodes.Status201Created,
          message =createdcurrencyId.Message,
          data = createdcurrencyId.Data
      });
    }
     _logger.LogWarning("Currency {Code} Creation failed.", command.Code);
      return BadRequest(new
        {
            StatusCode = StatusCodes.Status400BadRequest,
            message = createdcurrencyId.Message
        });
  
}
[HttpPut("update")]
public async Task<IActionResult> UpdateAsync( UpdateCurrencyCommand command)
{
  
        // Validate the incoming command
        var validationResult = await _updateCurrencyCommandValidator.ValidateAsync(command);
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

        var updatedcurrency = await _mediator.Send(command);

        if (updatedcurrency.IsSuccess)
        {
            _logger.LogInformation("Currency {Code} updated successfully.", command.Code);
           return Ok(new
            {
                message = updatedcurrency.Message,
                statusCode = StatusCodes.Status200OK
            });
        }
        _logger.LogWarning("Currency {Code} Update failed.", command.Code);
        return NotFound(new
        {
            message ="Currency not found",
            statusCode = StatusCodes.Status404NotFound
        });

        
} 

[HttpDelete("delete")]
public async Task<IActionResult> DeleteCurrencyAsync( DeleteCurrencyCommand command)
{
        // Process the delete command
        var result = await _mediator.Send(command);

        if (result.IsSuccess) 
        {
            _logger.LogInformation("Currency {Code} deleted successfully.", command.Id);
             return Ok(new
            {
                message = result.Message,
                statusCode = StatusCodes.Status200OK
            });
            
        }
        _logger.LogWarning("Currency {Code} Not Found or Invalid EntityId.", command.Id);
        return NotFound(new
        {
            message = result.Message,
            statusCode = StatusCodes.Status404NotFound
        });
   
}



    }
}