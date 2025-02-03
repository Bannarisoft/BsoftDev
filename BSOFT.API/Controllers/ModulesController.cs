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
    public async Task<IActionResult> CreateModule([FromBody] CreateModuleCommand command)
    {
        var validationResult = await _createModuleCommandValidator.ValidateAsync(command);
        _logger.LogWarning("Validation failed: {ErrorDetails}", validationResult);
        if (!validationResult.IsValid)
        {
            return BadRequest(new
            {
                StatusCode = StatusCodes.Status400BadRequest,
                message = "Validation failed",
                errors = validationResult.Errors.Select(e => e.ErrorMessage)
            });
        }

        var response = await Mediator.Send(command);
        if (response.IsSuccess)
            {
                _logger.LogInformation("Module {ModuleName} created successfully.", command.ModuleName);

                return Ok(new { StatusCode = StatusCodes.Status201Created, message = response.Message, data = response.Data });
            }
                _logger.LogWarning("Module creation failed for Module: {Username}", command.ModuleName);

                return BadRequest(new { StatusCode = StatusCodes.Status400BadRequest, message = response.Message }); 
    }

    [HttpGet]
    public async Task<IActionResult> GetModules()
    {
        var modules = await Mediator.Send(new GetModulesQuery());
        _logger.LogInformation("Module Listed successfully.", modules.Count);
        return Ok(new { StatusCode = StatusCodes.Status200OK, data = modules });
    }

    [HttpPut]
    public async Task<IActionResult> UpdateModule([FromBody] UpdateModuleCommand command)
    {
        var validationResult = await _updateModuleCommandValidator.ValidateAsync(command);
        _logger.LogWarning("Validation failed: {ErrorDetails}", validationResult);


        if (!validationResult.IsValid)
        {
            return BadRequest(new
            {
                StatusCode = StatusCodes.Status400BadRequest,
                message = "Validation failed",
                errors = validationResult.Errors.Select(e => e.ErrorMessage)
            });
        }
            // var moduleExists = await Mediator.Send(new GetModuleByIdAsync { ModuleId = command.ModuleId });
            // if (moduleExists == null)
            // {
            //     _logger.LogInformation("Module {ModuleId} not found for update.", command.ModuleId);

            //     return NotFound(new { StatusCode = StatusCodes.Status404NotFound, message = $"Module ID {command.ModuleId} not found." });
            // }


        var response = await Mediator.Send(command);
        if (response.IsSuccess)
            {
                _logger.LogInformation("Module {moduleName} updated successfully.", command.ModuleName);

                return Ok(new { StatusCode = StatusCodes.Status200OK, message = response.Message });
            }
                _logger.LogWarning("Module update failed for Module: {ModuleName}", command.ModuleName);

                return BadRequest(new { StatusCode = StatusCodes.Status400BadRequest, message = response.Message });
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteModule(DeleteModuleCommand deleteModuleCommand)
    {
        var updatedModule = await Mediator.Send(deleteModuleCommand);
        if(updatedModule.IsSuccess)
        {
          return Ok(new { StatusCode=StatusCodes.Status200OK, message = updatedModule.Message, errors = "" });
        }

        return BadRequest(new { StatusCode=StatusCodes.Status400BadRequest, message = updatedModule.Message, errors = "" });
    }


    }
}