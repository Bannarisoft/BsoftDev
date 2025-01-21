using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Core.Application.UserRole.Queries.GetRole;
using Core.Application.UserRole.Queries.GetRoleById;
using Core.Application.UserRole.Commands.CreateRole;
using Core.Application.UserRole.Commands.DeleteRole;
using Core.Application.UserRole.Commands.UpdateRole;
using Core.Application.UserRole.Queries.GetRolesAutocomplete;
using Core.Application.Common.Interfaces;
using FluentValidation;
using BSOFT.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;

namespace BSOFT.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    
    public class UserRoleController : ApiControllerBase
    {
         private readonly IValidator<CreateRoleCommand> _createRoleCommandValidator;
        private readonly IValidator<UpdateRoleCommand> _updateRoleCommandValidator;
         private readonly ApplicationDbContext _dbContext;
        public UserRoleController(ISender mediator    , IValidator<CreateRoleCommand> createRoleCommandValidator,
        IValidator<UpdateRoleCommand> updateRoleCommandValidator, 
        ApplicationDbContext dbContext ) : base(mediator)
        {
            _createRoleCommandValidator= createRoleCommandValidator;
            _updateRoleCommandValidator= updateRoleCommandValidator;
             _dbContext = dbContext; 

        }
       
        [HttpGet]
        public async Task<IActionResult> GetAllRoleAsync()
        {
           
            var roles =await Mediator.Send(new GetRoleQuery());
            return Ok(roles);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid role ID");
            }
            var  role = await Mediator.Send(new GetRoleByIdQuery() {Id=id});
            if(role ==null)
            {
                return  NotFound();                
            }
            return Ok(role);

        }
        [HttpPost]
        public async Task<IActionResult>CreateAsync(CreateRoleCommand command)
        {
                    var validationResult = await _createRoleCommandValidator.ValidateAsync(command);
                if (!validationResult.IsValid)
                {
                return BadRequest(validationResult.Errors);
                }
                var createuserrole = await Mediator.Send(command);
                return Ok("Created Successfully");
            //  Console.WriteLine("UserRole Create");
            // var createrole=await Mediator.Send(command);                    
            //  return Ok(new { Message = "UserRole created successfully.", id = createrole.Id });
        }

         [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteAsync(int id, DeleteRoleCommand command)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid role ID");
            }
            if (id != command.UserRoleId)
            {
                return BadRequest();
            }
            await Mediator.Send(command);

            return Ok("Deleted Successfully");
        }
       
     [HttpPut("update/{id}")]
    public async Task<IActionResult> UpdateAsync(int id, UpdateRoleCommand command)
    {      
          var validationResult = await _updateRoleCommandValidator.ValidateAsync(command);
        if (!validationResult.IsValid)
        {
        return BadRequest(validationResult.Errors);
        }
        if (id != command.Id)
        {
            return BadRequest("UserRole Id Mismatch");
        }

        var UpdateUserRole = await Mediator.Send(command);
        return Ok("Updated Successfully");
    //    if (id <= 0)
    //     {
    //         return BadRequest("Invalid role ID");
    //     }
    //         if (id != command.Id)
    //     {
    //         return BadRequest();
    //     }
    //         await Mediator.Send(command);
    //     return Ok("Updated Successfully");
    }

    [HttpGet("roles")]
    public async Task<IActionResult> GetRoles([FromQuery] string searchTerm)
    {
        var roles = await Mediator.Send(new GetRolesAutocompleteQuery { SearchPattern = searchTerm });
        return Ok(roles);
    }

    }
}