using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BSOFT.Infrastructure.Data;
using Core.Application.State.Commands;
using Core.Application.State.Commands.CreateState;
using Core.Application.State.Commands.DeleteState;
using Core.Application.State.Commands.UpdateState;
using Core.Application.State.Queries.GetStates;
using Core.Application.State.Queries.GetStateAutoComplete;
using Core.Application.State.Queries.GetStateById;
using Core.Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BSOFT.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StateController : ApiControllerBase
    {
         private readonly IValidator<CreateStateCommand> _createStateCommandValidator;
         private readonly IValidator<UpdateStateCommand> _updateStateCommandValidator;
         private readonly ApplicationDbContext _dbContext;
         
       public StateController(ISender mediator, 
                             IValidator<CreateStateCommand> createStateCommandValidator, 
                             IValidator<UpdateStateCommand> updateStateCommandValidator,ApplicationDbContext dbContext) 
         : base(mediator)
        {        
            _createStateCommandValidator = createStateCommandValidator;    
            _updateStateCommandValidator = updateStateCommandValidator;    _dbContext = dbContext;  
             
        }
        [HttpGet]
        public async Task<IActionResult> GetAllStatesAsync()
        {
            var states = await Mediator.Send(new GetStateQuery());
            var activeState = states.Where(c => c.IsActive == 1).ToList(); 
            return Ok(activeState);
        }

        [HttpGet("{stateId}")]
        public async Task<IActionResult> GetByIdAsync(int stateId)
        {
             if (stateId <= 0)
            {
                return BadRequest("Invalid State ID");
            }

            var state = await Mediator.Send(new GetStateByIdQuery() { Id = stateId });

            if (state == null || state.IsActive != 1) 
            {
                return NotFound("This StateID not Active");
            }

            return Ok(state);           
        }

        
    [HttpPost]
    public async Task<IActionResult> CreateAsync(CreateStateCommand  command)
    { 

       var validationResult = await _createStateCommandValidator.ValidateAsync(command);
        if (!validationResult.IsValid)
        {
        return BadRequest(validationResult.Errors);
        }        
        // If validation passes, send the command
        var result = await Mediator.Send(command);
        if (result.IsSuccess)
        {
        return Ok(new { Message = "State created successfully", State = result.Data });
        }
        else
        {
        // If the result is a failure, return BadRequest with the error message
        return BadRequest(result.ErrorMessage);
        }
    }
    [HttpPut("{stateId}")]
    public async Task<IActionResult> UpdateAsync(int stateId, UpdateStateCommand command)
    {         
        var validationResult = await _updateStateCommandValidator.ValidateAsync(command);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }
        if (command.CountryId<=0)
        {
            return BadRequest("Invalid CountryID");
        }
        var result = await Mediator.Send(command);
        if (result.IsSuccess)
        {
        return Ok(new { Message = "State Updated successfully", State = result.Data });
        }
        else
        {
        // If the result is a failure, return BadRequest with the error message
        return BadRequest(result.ErrorMessage);
        }
    }
    [HttpDelete("{stateId}")]
    public async Task<IActionResult> DeleteAsync(int stateId,DeleteStateCommand command)
    {
        if(stateId != command.Id)
        {
           return BadRequest("StateID Mismatch"); 
        }
        var result = await Mediator.Send(command);
        if (result.IsSuccess)
        {
        return Ok(new { Message = "State Deleted successfully", State = result.Data });
        }
        else
        {
        // If the result is a failure, return BadRequest with the error message
        return BadRequest(result.ErrorMessage);
        }
    }

       [HttpGet("GetStateSearch")]
        public async Task<IActionResult> GetState([FromQuery] string searchPattern)
        {
             var states = await Mediator.Send(new GetStateAutoCompleteQuery {SearchPattern = searchPattern}); // Pass `searchPattern` to the constructor
            var activeStates = states.Where(c => c.IsActive == 1).ToList(); 
            return Ok(activeStates);
        }    
    }
}