using MediatR;
using BSOFT.Infrastructure.Data;
using Core.Application.Modules.Commands.UpdateModule;
using Core.Application.Modules.Commands.CreateModule;
using Core.Application.Modules.Queries.GetModules;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BSOFT.Application.Modules.Commands.DeleteModule;
using Microsoft.AspNetCore.Authorization;
using Core.Application.Modules.Queries.GetModuleById;

namespace BSOFT.API.Controllers
{
[ApiController]
[Route("api/[controller]")]

    public class ModulesController : ApiControllerBase
    {
    // public ModulesController(ISender mediator) : base(mediator)
    // {
    // }
    private readonly IValidator<CreateModuleCommand> _createModuleCommandValidator;
         private readonly IValidator<UpdateModuleCommand> _updateModuleCommandValidator;
         private readonly ApplicationDbContext _dbContext;
         private readonly ILogger<ModulesController> _logger;

         
       public ModulesController(ISender mediator, 
                             IValidator<CreateModuleCommand> createModuleCommandValidator, 
                             IValidator<UpdateModuleCommand> updateModuleCommandValidator, 
                             ApplicationDbContext dbContext,
                             ILogger<ModulesController> logger) 
         : base(mediator)
        {        
            _createModuleCommandValidator = createModuleCommandValidator;
            _updateModuleCommandValidator = updateModuleCommandValidator;    
            _dbContext = dbContext;  
            _logger = logger;

             
        }

    [HttpPost]
    public async Task<IActionResult> CreateModule([FromBody] CreateModuleCommand createModuleCommand)
    {
        var validationResult = await _createModuleCommandValidator.ValidateAsync(createModuleCommand);
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

        var response = await Mediator.Send(createModuleCommand);
        if (response.IsSuccess)
            {
                _logger.LogInformation($"Module {createModuleCommand.ModuleName} created successfully.");

                return Ok(new { StatusCode = StatusCodes.Status201Created, message = response.Message, data = response.Data });
            }
                _logger.LogWarning($"Failed to create module {createModuleCommand.ModuleName}.");

                return BadRequest(new { StatusCode = StatusCodes.Status400BadRequest, message = response.Message }); 
    }

    [HttpGet]
    public async Task<IActionResult> GetModules()
    {
        var modules = await Mediator.Send(new GetModulesQuery());
        _logger.LogInformation($"Total {modules.Count} modules listed successfully.");
        return Ok(new { StatusCode = StatusCodes.Status200OK, data = modules });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetByIdAsync(int id)
    {
            
        var module = await Mediator.Send(new GetModuleByIdQuery { Id = id });

        if (module is null)
        {
            _logger.LogWarning($"Module not found for ID {id}.");

           return NotFound(new { StatusCode = StatusCodes.Status404NotFound, message = $"Module ID {id} not found." });
        }
           _logger.LogWarning("Module Listed successfully: {Modulename}", module);

        return Ok(new { StatusCode = StatusCodes.Status200OK, data = module });
    }
    [HttpPut]
    public async Task<IActionResult> UpdateModule([FromBody] UpdateModuleCommand updateModuleCommand)
    {
        var validationResult = await _updateModuleCommandValidator.ValidateAsync(updateModuleCommand);
        _logger.LogWarning($"Validation failed: {string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage))}");

        if (!validationResult.IsValid)
        {
            return BadRequest(new
            {
                StatusCode = StatusCodes.Status400BadRequest,
                message = "Validation failed",
                errors = validationResult.Errors.Select(e => e.ErrorMessage)
            });
        }

        var response = await Mediator.Send(updateModuleCommand);
        if (response.IsSuccess)
            {
                _logger.LogInformation($"Module {updateModuleCommand.ModuleName} updated successfully.");

                return Ok(new { StatusCode = StatusCodes.Status200OK, message = response.Message });
            }
                _logger.LogWarning($"Failed to update module {updateModuleCommand.ModuleName}.");

                return BadRequest(new { StatusCode = StatusCodes.Status400BadRequest, message = response.Message });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteModule(DeleteModuleCommand deleteModuleCommand)
    {
        var deletedModule = await Mediator.Send(deleteModuleCommand);
        if(deletedModule.IsSuccess)
        {
            _logger.LogInformation($"Module {deleteModuleCommand.ModuleId} deleted successfully.");
            return Ok(new { StatusCode=StatusCodes.Status200OK, message = deletedModule.Message, errors = "" });
        }
            _logger.LogWarning($"Deletion failed for module {deleteModuleCommand.ModuleId}: {deletedModule?.Message ?? "Unknown error"}.");
            return BadRequest(new { StatusCode=StatusCodes.Status400BadRequest, message = deletedModule.Message, errors = "" });
    }


    }
}