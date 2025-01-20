using BSOFT.Infrastructure.Data;
using Core.Application.State.Commands.CreateState;
using Core.Application.State.Commands.DeleteState;
using Core.Application.State.Commands.UpdateState;
using Core.Application.State.Queries.GetStates;
using Core.Application.State.Queries.GetStateAutoComplete;
using Core.Application.State.Queries.GetStateById;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace BSOFT.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StateController : ApiControllerBase
    {
         private readonly IValidator<CreateStateCommand> _createStateCommandValidator;
         private readonly IValidator<UpdateStateCommand> _updateStateCommandValidator;         
         
       public StateController(ISender mediator, 
                             IValidator<CreateStateCommand> createStateCommandValidator, 
                             IValidator<UpdateStateCommand> updateStateCommandValidator) 
         : base(mediator)
        {        
            _createStateCommandValidator = createStateCommandValidator;    
            _updateStateCommandValidator = updateStateCommandValidator;     
             
        }
        [HttpGet]
        public async Task<IActionResult> GetAllStatesAsync()
        {
            var states = await Mediator.Send(new GetStateQuery());            
            return Ok(states);
        }

        [HttpGet("{stateId}")]
        [Authorize]
        public async Task<IActionResult> GetByIdAsync(int stateId)
        {
             if (stateId <= 0)
            {
                return BadRequest("Invalid State ID");
            }
            var result = await Mediator.Send(new GetStateByIdQuery { Id = stateId });            
            return result.IsSuccess
                ? Ok(result.Data)
                : NotFound(new { Message = result.ErrorMessage });    
            }        
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateAsync(CreateStateCommand  command)
        { 

        var validationResult = await _createStateCommandValidator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }                
            var result = await Mediator.Send(command);
             return result.IsSuccess
                ? CreatedAtAction(nameof(GetState), new { id = result.Data.Id }, new { Message = "State created successfully", State = result.Data })
                : BadRequest(new { Message = result.ErrorMessage }); 
             //return CreatedAtAction(nameof(GetState), new { id = result.Data.Id }, result);
            
        }
        [HttpPut("{stateId}")]
        [Authorize]
        public async Task<IActionResult> UpdateAsync(int stateId, UpdateStateCommand command)
        {   
            if (stateId != command.Id)
            {
                return BadRequest(new { Message = "State ID mismatch" });
            }
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
              return result.IsSuccess
                ? Ok(new { Message = "State updated successfully", State = result.Data })
                : BadRequest(new { Message = result.ErrorMessage });
        }        
        [HttpDelete("{stateId}")]
        [Authorize]
        public async Task<IActionResult> DeleteAsync(int stateId,DeleteStateCommand command)
        {
            if(stateId != command.Id)
            {
            return BadRequest("StateID Mismatch"); 
            }
            if (stateId <= 0)
            {
                return BadRequest(new { Message = "Invalid State ID" });
            }
            var result = await Mediator.Send(command);
            return result.IsSuccess
                ? Ok(new { Message = "State deleted successfully" })
                : BadRequest(new { Message = result.ErrorMessage });
        }

        [HttpGet("GetStateSearch")]
        [Authorize]
        public async Task<IActionResult> GetState([FromQuery] string searchPattern)
        {
            if (string.IsNullOrWhiteSpace(searchPattern))
            {
                return BadRequest(new { Message = "Search pattern is required" });
            }
            var result = await Mediator.Send(new GetStateAutoCompleteQuery {SearchPattern = searchPattern}); // Pass `searchPattern` to the constructor
            return result.IsSuccess
            ? Ok(result.Data)
            : NotFound(new { Message = result.ErrorMessage });
        }    
    }
}