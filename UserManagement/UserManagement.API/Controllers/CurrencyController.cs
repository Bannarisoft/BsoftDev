using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using UserManagement.Infrastructure.Data;
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

namespace UserManagement.API.Controllers
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
        if (result is null  || result.Data is null || !result.Data.Any())
        {
            _logger.LogWarning($"No Currency Record {result.Data} not found in DB.");
            return NotFound(new
            {
                message = result.Message,
                statusCode = StatusCodes.Status404NotFound
              
            });
        }
        _logger.LogInformation($"Total {result.Data.Count} active Currencies Listed successfully.");
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
            _logger.LogWarning($"CurrencyId {id} not found.");
            return BadRequest(new
            {
                StatusCode = StatusCodes.Status400BadRequest,
                message = "Invalid Entity ID"
            });
        }

        var result = await Mediator.Send(new GetCurrencyByIdQuery { CurrencyId = id });

        if (result.IsSuccess)
        {
              _logger.LogInformation($"CurrencyId {result.Data} Listed successfully.");
              return Ok(new
             {
                 message = result.Message,
                 statusCode = StatusCodes.Status200OK,
                 data = result.Data
             }); 
        }
        _logger.LogWarning($"CurrencyId {result.Data} Not found.");
        return NotFound(new
        {
            message = result.Message,
            statusCode = StatusCodes.Status404NotFound
        });
   
}
       [HttpGet("by-name/{name}")]
        public async Task<IActionResult> GetCurrency([FromRoute] string name)
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
        var result = await Mediator.Send(new GetCurrencyAutocompleteQuery { SearchPattern = name });
        _logger.LogInformation($"Search pattern: {name}");
       if (result.IsSuccess)
        {
        _logger.LogInformation($"Currency {result.Data.Count} Listed successfully.");
         return Ok(new  
            {
                message = result.Message,
                statusCode = StatusCodes.Status200OK,
                data = result.Data
            });
        }
        _logger.LogInformation($"No Currency Record {name} of {result.Data} not found in DB.");
        return NotFound(new
        {
            message = result.Message,
            statusCode = StatusCodes.Status404NotFound
        });                  
}
[HttpPost]
public async Task<IActionResult> CreateAsync(CreateCurrencyCommand createCurrencyCommand)
{
    
    // Validate the incoming command
   var validationResult = await _createCurrencyCommandValidator.ValidateAsync(createCurrencyCommand);
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
    var createdcurrencyId = await _mediator.Send(createCurrencyCommand);

    if (createdcurrencyId.IsSuccess)
    {
     _logger.LogInformation($"Currency {createCurrencyCommand.Code} created successfully.");
      return Ok(new
      {
          StatusCode = StatusCodes.Status201Created,
          message =createdcurrencyId.Message,
          data = createdcurrencyId.Data
      });
    }
     _logger.LogWarning($"Currency {createCurrencyCommand.Code} Creation failed.");
      return BadRequest(new
        {
            StatusCode = StatusCodes.Status400BadRequest,
            message = createdcurrencyId.Message
        });
  
}
[HttpPut]
public async Task<IActionResult> UpdateAsync( UpdateCurrencyCommand updateCurrencyCommand)
{
  
        // Validate the incoming command
        var validationResult = await _updateCurrencyCommandValidator.ValidateAsync(updateCurrencyCommand);
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

        var updatedcurrency = await _mediator.Send(updateCurrencyCommand);

        if (updatedcurrency.IsSuccess)
        {
            _logger.LogInformation($"Currency {updateCurrencyCommand.Name} updated successfully.");
           return Ok(new
            {
                message = updatedcurrency.Message,
                statusCode = StatusCodes.Status200OK
            });
        }

  
        _logger.LogWarning($"Currency {updateCurrencyCommand.Name} Update failed.");
        return NotFound(new
        {
            message =updatedcurrency.Message,
            statusCode = StatusCodes.Status404NotFound
        });

        
} 

[HttpDelete("{id}")]
public async Task<IActionResult> DeleteCurrencyAsync(int id)
{
        // Process the delete command
        var result = await _mediator.Send(new DeleteCurrencyCommand { Id = id });

        if (result.IsSuccess) 
        {
            _logger.LogInformation($"CurrencyId {id} deleted successfully.");
             return Ok(new
            {
                message = result.Message,
                statusCode = StatusCodes.Status200OK
            });
            
        }
        _logger.LogWarning($"CurrencyId {id} Not Found or Invalid CurrencyId.");
        return NotFound(new
        {
            message = result.Message,
            statusCode = StatusCodes.Status404NotFound
        });
   
}



    }
}