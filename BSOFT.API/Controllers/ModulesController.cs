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
         
       public ModulesController(ISender mediator, 
                             IValidator<CreateModuleCommand> createModuleCommandValidator, 
                             IValidator<UpdateModuleCommand> updateModuleCommandValidator, 
                             ApplicationDbContext dbContext) 
         : base(mediator)
        {        
            _createModuleCommandValidator = createModuleCommandValidator;
            _updateModuleCommandValidator = updateModuleCommandValidator;    
            _dbContext = dbContext;  
             
        }

    [HttpPost]
    [Route("Create")]
    public async Task<IActionResult> CreateModule([FromBody] CreateModuleCommand command)
    {
        var validationResult = await _createModuleCommandValidator.ValidateAsync(command);
        if (!validationResult.IsValid)
        {
        return BadRequest(validationResult.Errors);
        }

        var moduleId = await Mediator.Send(command);
        return Ok(new { Message = "Module created successfully.", ModuleId = moduleId });
    }

    [HttpGet]
    [Route("GetAll")]
    public async Task<IActionResult> GetModules()
    {
        var modules = await Mediator.Send(new GetModulesQuery());
        return Ok(modules);
    }

    [HttpPut]
    [Route("Update")]
    public async Task<IActionResult> UpdateModule([FromBody] UpdateModuleCommand command)
    {
        var validationResult = await _updateModuleCommandValidator.ValidateAsync(command);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        await Mediator.Send(command);
        return Ok(new { Message = "Module updated successfully." });
    }

    [HttpDelete]
    [Route("Delete/{moduleId}")]
    public async Task<IActionResult> DeleteModule(int moduleId)
    {
        await Mediator.Send(new DeleteModuleCommand { ModuleId = moduleId });
        return Ok(new { Message = "Module deleted successfully." });
    }


    }
}