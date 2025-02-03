using Core.Application.City.Commands.CreateCity;
using Core.Application.City.Commands.DeleteCity;
using Core.Application.City.Commands.UpdateCity;
using Core.Application.City.Queries.GetCities;
using Core.Application.City.Queries.GetCityAutoComplete;
using Core.Application.City.Queries.GetCityById;
using Core.Application.City.Queries.GetCityByStateId;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BSOFT.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    
    public class CityController : ApiControllerBase
    {
         private readonly IValidator<CreateCityCommand> _createCityCommandValidator;
         private readonly IValidator<UpdateCityCommand> _updateCityCommandValidator;
         
         
       public CityController(ISender mediator, 
                             IValidator<CreateCityCommand> createCityCommandValidator, 
                             IValidator<UpdateCityCommand> updateCityCommandValidator) 
         : base(mediator)
        {        
            _createCityCommandValidator = createCityCommandValidator;    
            _updateCityCommandValidator = updateCityCommandValidator;    
             
        }
        [HttpGet]                
        public async Task<IActionResult> GetAllCitiesAsync()
        {  
            var cities = await Mediator.Send(new GetCityQuery());    
             if (cities is null )
            {                
                return NotFound(new 
                { 
                    StatusCode=StatusCodes.Status404NotFound,
                    message = "City details not found", 
                });
            }        
            return Ok(new 
            { 
                StatusCode=StatusCodes.Status200OK, 
                message = cities.Message,
                data = cities.Data 
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
                    message = "Invalid City ID" 
                });
            }
            var result = await Mediator.Send(new GetCityByIdQuery { Id = id });            
            if (result is null )
            {                
                return NotFound(new 
                { 
                    StatusCode=StatusCodes.Status404NotFound,
                    message = "CityId {id} not found", 
                });
            }
            return Ok(new 
            {
                StatusCode=StatusCodes.Status200OK,
                data = result.Data
            });   
        }

        [HttpPost("create")]               
        public async Task<IActionResult> CreateAsync(CreateCityCommand  command)
        { 
            var validationResult = await _createCityCommandValidator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                return BadRequest(new
                {
                    StatusCode=StatusCodes.Status400BadRequest,
                    message = "Validation failed", 
                    errors = validationResult.Errors.Select(e => e.ErrorMessage).ToArray()
                });
            }        
            var result = await Mediator.Send(command);
            if (result.IsSuccess)
            {
                return Ok(new 
                { 
                    StatusCode=StatusCodes.Status201Created,
                    message = result.Message, 
                    data = result.Data
                });
            }  
            else
            {      
                return BadRequest(new 
                { 
                    StatusCode=StatusCodes.Status400BadRequest,
                    message = result.Message
                });
            } 
        }
        [HttpPut("update")]        
        public async Task<IActionResult> UpdateAsync(UpdateCityCommand command)
        {         
            var validationResult = await _updateCityCommandValidator.ValidateAsync(command);
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
            if (command.StateId<=0)
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
            if (result.IsSuccess)
            {
                return Ok(new 
                {   StatusCode=StatusCodes.Status200OK,
                    message = result.Message, 
                    City = result.Data
                });
            }
                
                return BadRequest( new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    message = result.Message
                });
                
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
              var result = await Mediator.Send(new DeleteCityCommand { Id = id });                 
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
                data =$"City ID {id} Deleted" 
            });
        }
             
        [HttpGet("by-name{name}")]  
        public async Task<IActionResult> GetCity(string name)
        {    
            if (string.IsNullOrWhiteSpace(name))
            {
                 return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    message = "Search pattern is required"
                });                
            }       
            var result = await Mediator.Send(new GetCityAutoCompleteQuery {SearchPattern = name}); // Pass `searchPattern` to the constructor
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
                message = result.Message,
                data = result.Data
            });
        }    
        [HttpGet("by-state/{stateid}")]
        public async Task<IActionResult> GetStateByCountryId(int stateid)
        {
            if (stateid <= 0)
            {                
                return BadRequest(new 
                { 
                    StatusCode=StatusCodes.Status400BadRequest,
                    message = "Invalid State ID" 
                });
            }
            var result = await Mediator.Send(new GetCityByStateIdQuery { Id = stateid });                         
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