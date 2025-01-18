using BSOFT.Infrastructure.Data;
using Core.Application.City.Commands.CreateCity;
using Core.Application.City.Commands.DeleteCity;
using Core.Application.City.Commands.UpdateCity;
using Core.Application.City.Queries.GetCities;
using Core.Application.City.Queries.GetCityAutoComplete;
using Core.Application.City.Queries.GetCityById;
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
            return Ok(new 
            { 
                StatusCode=StatusCodes.Status200OK, 
                message = cities.Message,
                data = cities.Data 
            });
        }

        [HttpGet("{cityId}")]
        public async Task<IActionResult> GetByIdAsync(int cityId)
        {
             if (cityId <= 0)
            {
                return BadRequest(new 
                { 
                    StatusCode=StatusCodes.Status400BadRequest,
                    message = "Invalid City ID" 
                });
            }
            var result = await Mediator.Send(new GetCityByIdQuery { Id = cityId });            
            if (result == null )
            {                
                return NotFound(new 
                { 
                    StatusCode=StatusCodes.Status404NotFound,
                    message = "City not found", 
                });
            }
            return Ok(new 
            {
                StatusCode=StatusCodes.Status200OK,
                data = result
            });   
        }

    [HttpPost]
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
                data = result.Data.Id 
            });
        }
       
            return BadRequest(new 
            { 
                StatusCode=StatusCodes.Status400BadRequest,
                message = result.Message

            });
               
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
    [HttpDelete("delete")]
    public async Task<IActionResult> DeleteAsync(DeleteCityCommand command)
    {
        
        var result = await Mediator.Send(command);
        if (result.IsSuccess)
        {
            return Ok(new 
            { 
                StatusCode=StatusCodes.Status200OK,
                message = result.Message
            });
        }
        else
        {        
            return BadRequest(new
            {
                StatusCode = StatusCodes.Status400BadRequest,
                message = result.Message
            });
        }       
    }

       [HttpGet("GetCitySearch")]
        public async Task<IActionResult> GetCity([FromQuery] string searchPattern)
        {           
            var result = await Mediator.Send(new GetCityAutoCompleteQuery {SearchPattern = searchPattern}); // Pass `searchPattern` to the constructor
            if (result.IsSuccess)
            {
                return Ok(new 
                {
                    StatusCode=StatusCodes.Status200OK,
                    message = result.Message,
                    data = result.Data
                });
            }
            return Ok(result.Data);
        }


     
    }
}