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
            return Ok(new 
            { 
                StatusCode=StatusCodes.Status200OK, 
                message = states.Message,
                data = states.Data 
            });
        }

        [HttpGet("{stateId}")]
        public async Task<IActionResult> GetByIdAsync(int stateId)
        {
            if (stateId <= 0)
            {                
                return BadRequest(new 
                { 
                    StatusCode=StatusCodes.Status400BadRequest,
                    message = "Invalid State ID" 
                });
            }
            var result = await Mediator.Send(new GetStateByIdQuery { Id = stateId });                         
            if(result == null)
            {                
                return NotFound(new 
                { 
                    StatusCode=StatusCodes.Status404NotFound,
                    message = "StateID {stateId} not found", 
                });
            }
            return Ok(new 
            {
                StatusCode=StatusCodes.Status200OK,
                data = result
            });   
        }        
        [HttpPost]
        public async Task<IActionResult> CreateAsync(CreateStateCommand  command)
        { 
            var validationResult = await _createStateCommandValidator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                return BadRequest(new 
                {
                    StatusCode=StatusCodes.Status400BadRequest,message = "Validation failed", 
                    errors = validationResult.Errors.Select(e => e.ErrorMessage).ToArray() 
                });
            }                
            var result = await Mediator.Send(command);
            if(result.IsSuccess)
            {                
                return Ok(new { StatusCode=StatusCodes.Status201Created, message = result.Message, errors = "", data = result.Data });
            }
            

            return BadRequest( new { StatusCode=StatusCodes.Status400BadRequest, message = result.Message, errors = "" }); 
            
        }
        [HttpPut("{stateId}")]
        public async Task<IActionResult> UpdateAsync(int stateId, UpdateStateCommand command)
        {   
            if (stateId != command.Id)
            {
                return BadRequest(new { Message = "State ID mismatch" });
            }
            var validationResult = await _updateStateCommandValidator.ValidateAsync(command);

            if (!validationResult.IsValid)
            {                
                return BadRequest(
                    new
                    {
                        StatusCode = StatusCodes.Status400BadRequest,
                        message = "Validation failed",
                        errors = validationResult.Errors.Select(e => e.ErrorMessage).ToArray()
                    }
                );
            }
            if (command.CountryId<=0)
            {
                return BadRequest(
                    new
                    {
                        StatusCode = StatusCodes.Status400BadRequest,
                        message = "Invalid StateID"
                    }
                );
            }

            var result = await Mediator.Send(command);
            if(result.IsSuccess)
            {                 
                return Ok(new 
                {   StatusCode=StatusCodes.Status200OK,
                    message = result.Message, 
                    City = result.Data
                });
            }
            return BadRequest( new { StatusCode=StatusCodes.Status400BadRequest, message = result.Message, errors = "" }); 
        }        
        [HttpDelete("{stateId}")]
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
            if(result.IsSuccess)
            {
                return Ok(new { StatusCode=StatusCodes.Status200OK, message = result.Message, errors = "" });                
            }
            return BadRequest(new { StatusCode=StatusCodes.Status400BadRequest, message = result.Message, errors = "" });
        }

        [HttpGet("GetStateSearch")]
            public async Task<IActionResult> GetState([FromQuery] string searchPattern)
            {
                if (string.IsNullOrWhiteSpace(searchPattern))
                {
                    return BadRequest(new { Message = "Search pattern is required" });
                }
                var result = await Mediator.Send(new GetStateAutoCompleteQuery {SearchPattern = searchPattern}); // Pass `searchPattern` to the constructor
                if (result.IsSuccess)
                {
                    return Ok(new 
                    {
                        StatusCode=StatusCodes.Status200OK,
                        message = result.Message,
                        data = result.Data
                    });
                }
                return Ok(new
                {
                    StatusCode = StatusCodes.Status200OK,
                    message = result.Message,
                    data = result.Data
                });
            }    
    }
}