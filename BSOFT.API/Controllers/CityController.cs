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
            return Ok(cities);
        }

        [HttpGet("{cityId}")]
        public async Task<IActionResult> GetByIdAsync(int cityId)
        {
             if (cityId <= 0)
            {
                return BadRequest("Invalid City ID");
            }
            var result = await Mediator.Send(new GetCityByIdQuery { Id = cityId });            
            if (!result.IsSuccess)
            {                
                return NotFound(new { Message = result.ErrorMessage });
            }
            return Ok(result.Data);   
        }

    [HttpPost]
    public async Task<IActionResult> CreateAsync(CreateCityCommand  command)
    { 
        var validationResult = await _createCityCommandValidator.ValidateAsync(command);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }        
        var result = await Mediator.Send(command);
        if (result.IsSuccess)
        {
            return Ok(new { Message = "City created successfully", City = result.Data });
        }
        else
        {
            return BadRequest(result.ErrorMessage);
        }        
    }
    [HttpPut("{cityId}")]
    public async Task<IActionResult> UpdateAsync(int cityId, UpdateCityCommand command)
    {         
        var validationResult = await _updateCityCommandValidator.ValidateAsync(command);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }
        if (command.StateId<=0)
        {
            return BadRequest("Invalid StateID");
        } 
        var result = await Mediator.Send(command);
        if (result.IsSuccess)
        {
            return Ok(new { Message = "City Updated successfully", City = result.Data });
        }
        else
        {        
            return BadRequest(result.ErrorMessage);
        }       
    }
    [HttpDelete("{cityId}")]
    public async Task<IActionResult> DeleteAsync(int cityId,DeleteCityCommand command)
    {
        if(cityId != command.Id)
        {
           return BadRequest("CityID Mismatch"); 
        }
        var result = await Mediator.Send(command);
        if (result.IsSuccess)
        {
            return Ok(new { Message = "City Deleted successfully", City = result.Data });
        }
        else
        {        
            return BadRequest(result.ErrorMessage);
        }       
    }

       [HttpGet("GetCitySearch")]
        public async Task<IActionResult> GetCity([FromQuery] string searchPattern)
        {           
            var result = await Mediator.Send(new GetCityAutoCompleteQuery {SearchPattern = searchPattern}); // Pass `searchPattern` to the constructor
            if (!result.IsSuccess)
            {
                return NotFound(new { Message = result.ErrorMessage });
            }
            return Ok(result.Data);
        }


     
    }
}