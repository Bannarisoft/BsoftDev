using UserManagement.Infrastructure.Data;
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

namespace UserManagement.API.Controllers
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
        public async Task<IActionResult> GetAllStatesAsync([FromQuery] int PageNumber,[FromQuery] int PageSize,[FromQuery] string? SearchTerm = null)
        {
            var states = await Mediator.Send(
            new GetStateQuery
           {
                PageNumber = PageNumber, 
                PageSize = PageSize, 
                SearchTerm = SearchTerm
            });            
            return Ok(new 
            { 
                StatusCode=StatusCodes.Status200OK, 
                message = states.Message,
                data = states.Data.ToList(),
                TotalCount = states.TotalCount,
                PageNumber = states.PageNumber,
                PageSize = states.PageSize
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
        [HttpPut]
        public async Task<IActionResult> UpdateAsync(UpdateStateCommand command)
        {           
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
        [HttpDelete("{id}")]   
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
                data =$"State ID {id} Deleted" ,
                message = result.Message
            });
        }

        [HttpGet("by-name")]  
        public async Task<IActionResult> GetState([FromQuery] string? name)
        {           
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