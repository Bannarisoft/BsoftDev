using Newtonsoft.Json;
using MediatR;
using UserManagement.Infrastructure.Data;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Core.Application.RoleEntitlements.Commands.CreateRoleEntitlement;
using Core.Application.RoleEntitlements.Commands.UpdateRoleRntitlement;
using Core.Application.RoleEntitlements.Queries.GetRoleEntitlements;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Core.Application.RoleEntitlements.Commands.DeleteRoleEntitlement;
using Core.Application.RoleEntitlements.Queries.GetRoleEntitlementById;

namespace UserManagement.API.Controllers
{
[ApiController]
[Route("api/[controller]")]

public class RoleEntitlementsController : ApiControllerBase
{
    // public RoleEntitlementsController(ISender mediator) : base(mediator)
    // {
    // }

    private readonly IValidator<CreateRoleEntitlementCommand> _createRoleEntitlementCommandValidator;
         private readonly IValidator<UpdateRoleEntitlementCommand> _updateRoleEntitlementCommandValidator;
         private readonly ApplicationDbContext _dbContext;
         private readonly ILogger<RoleEntitlementsController> _logger;

         
       public RoleEntitlementsController(ISender mediator, 
                             IValidator<CreateRoleEntitlementCommand> createRoleEntitlementCommandValidator, 
                             IValidator<UpdateRoleEntitlementCommand> updateRoleEntitlementCommandValidator, 
                             ApplicationDbContext dbContext,
                             ILogger<RoleEntitlementsController> logger) 
         : base(mediator)
        {        
            _createRoleEntitlementCommandValidator = createRoleEntitlementCommandValidator;
            _updateRoleEntitlementCommandValidator = updateRoleEntitlementCommandValidator;    
            _dbContext = dbContext;  
            _logger = logger;
        }
        [HttpGet("{id}")]        
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    message = "Invalid Country ID"
                });
            }            
            var result = await Mediator.Send(new GetRoleEntitlementByIdQuery { Id = id });            
            if (!result.IsSuccess)
            {                
                return NotFound(new 
                { 
                    StatusCode = StatusCodes.Status404NotFound,
                    message = result.Message
                });
            }
            return Ok(new
            {
                StatusCode = StatusCodes.Status200OK,
                data = result.Data
            });
        }

    [HttpPost]
    public async Task<IActionResult> CreateRoleEntitlement(CreateRoleEntitlementCommand command)
    {
   
         var response = await Mediator.Send(command);
       if (response.IsSuccess)
            {
                
                return Ok(new 
                { 
                    StatusCode = StatusCodes.Status201Created, 
                    message = response.Message, 
                    data = response.Data 
                });
            }
                
        return BadRequest(new 
        { 
            StatusCode = StatusCodes.Status400BadRequest, 
            message = response.Message
        }); 
             
    }

    [HttpPut]
    public async Task<IActionResult> UpdateRoleEntitlement(UpdateRoleEntitlementCommand command)
    {
          if (command.RoleId != command.RoleId)
         {
             return BadRequest(new 
             {
                 StatusCode = StatusCodes.Status400BadRequest,
                 Message = "Invalid request. All ModuleMenus must have the same RoleId as the provided RoleId."
             });
         }
          var response = await Mediator.Send(command);
       if (response.IsSuccess)
            {
                
                return Ok(new 
                { 
                    StatusCode = StatusCodes.Status200OK, 
                    message = response.Message
                });
            }
                
        return BadRequest(new 
        { 
            StatusCode = StatusCodes.Status400BadRequest, 
            message = response.Message
        }); 
    }
    // [HttpDelete("{id}")]        
    //     public async Task<IActionResult> DeleteAsync(int id)
    //     {
    //         if (id <= 0)
    //         {
    //             return BadRequest(new
    //         {
    //                 StatusCode = StatusCodes.Status400BadRequest,
    //                 message = "Invalid RoleEntitlement ID"
    //             });
    //         }
    //         var result = await Mediator.Send(new DeleteRoleEntitlementCommand { Id= id });                 
    //         if (!result.IsSuccess)
    //         {          
    //              _logger.LogWarning($"Deletion failed for RoleEntitlement {id}: {result?.Message ?? "Unknown error"}.");
    
    //             return NotFound(new 
    //             { 
    //                 StatusCode = StatusCodes.Status404NotFound,
    //                 message = result.Message
    //             });
    //         }
    //         _logger.LogInformation($"RoleEntitlement {id} deleted successfully.");

    //         return Ok(new
    //         {
    //             StatusCode = StatusCodes.Status200OK,
    //             data =$"RoleEntitlement ID {id} Deleted" 
    //         });
 
    //     }
    // [HttpGet("by-name/{name}")]
    // public async Task<IActionResult> GetRoleEntitlements(string name)
    // {
    //     var query = new GetRoleEntitlementsQuery { RoleName = name };
    //         _logger.LogWarning("RoleEntitlement Listed successfully: {RoleName}", query);

    //     var result = await Mediator.Send(query);
    //     return Ok( new { StatusCode=StatusCodes.Status200OK, data = result });

    // }   
}
}