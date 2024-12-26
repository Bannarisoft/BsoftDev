using MediatR;
using BSOFT.Application.Modules.Commands.DeleteModule;
using BSOFT.Application.Modules.Commands.UpdateModule;
using BSOFT.Application.Modules.Commands.CreateModule;
using BSOFT.Application.Modules.Queries.GetModules;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BSOFT.API.Controllers
{
[ApiController]
[Route("api/[controller]")]
    public class ModulesController : ApiControllerBase
    {
    public ModulesController(ISender mediator) : base(mediator)
    {
    }

    [HttpPost]
    [Route("Create")]
    public async Task<IActionResult> CreateModule([FromBody] CreateModuleCommand command)
    {
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
        await Mediator.Send(command);
        return Ok(new { Message = "Module updated successfully." });
    }

    [HttpDelete]
    [Route("Delete/{moduleId}")]
    public async Task<IActionResult> SoftDeleteModule(int moduleId)
    {
        await Mediator.Send(new DeleteModuleCommand { ModuleId = moduleId });
        return Ok(new { Message = "Module deleted successfully." });
    }


    }
}