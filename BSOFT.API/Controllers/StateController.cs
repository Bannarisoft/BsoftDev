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
using Core.Application.State.Queries.GetStateByCountryId;

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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            if (id <= 0)
            {                
                return BadRequest(new 
                { 
                    StatusCode=StatusCodes.Status400BadRequest,
                    message = "Invalid State ID" 
                });
            }
            var result = await Mediator.Send(new GetStateByIdQuery { Id = id });                         
            if(result is null)
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
        [HttpPost("create")]   
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
        [HttpPut("update")]
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
        [HttpDelete("delete{id}")]   
        public async Task<IActionResult> DeleteAsync(int id)
        {          
            if (id <= 0)
            {
                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    message = "Invalid Country ID"
                });
            }            
              var result = await Mediator.Send(new DeleteStateCommand { Id = id });                 
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
                data =$"State ID {id} Deleted" 
            });
        }

        [HttpGet("by-name{name}")]  
        public async Task<IActionResult> GetState(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return BadRequest(new { Message = "Search pattern is required" });
            }
            var result = await Mediator.Send(new GetStateAutoCompleteQuery {SearchPattern = name}); // Pass `searchPattern` to the constructor
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
        [HttpGet("by-country/{countryid}")]
        public async Task<IActionResult> GetStateByCountryId(int countryid)
        {
            if (countryid <= 0)
            {                
                return BadRequest(new 
                { 
                    StatusCode=StatusCodes.Status400BadRequest,
                    message = "Invalid State ID" 
                });
            }
            var result = await Mediator.Send(new GetStateByCountryIdQuery { Id = countryid });                         
            if(result is null)
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
    }
}