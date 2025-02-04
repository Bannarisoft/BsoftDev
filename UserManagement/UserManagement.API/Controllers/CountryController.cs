using Core.Application.Country.Commands.CreateCountry;
using Core.Application.Country.Commands.DeleteCountry;
using Core.Application.Country.Commands.UpdateCountry;
using Core.Application.Country.Queries.GetCountries;
using Core.Application.Country.Queries.GetCountryAutoComplete;
using Core.Application.Country.Queries.GetCountryById;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace UserManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    
    public class CountryController : ApiControllerBase
    {
         private readonly IValidator<CreateCountryCommand> _createCountryCommandValidator;
         private readonly IValidator<UpdateCountryCommand> _updateCountryCommandValidator;         
         
       public CountryController(ISender mediator, 
                             IValidator<CreateCountryCommand> createCountryCommandValidator, 
                             IValidator<UpdateCountryCommand> updateCountryCommandValidator) 
         : base(mediator)
        {        
            _createCountryCommandValidator = createCountryCommandValidator;    
            _updateCountryCommandValidator = updateCountryCommandValidator;    
             
        }
        [HttpGet]        
        public async Task<IActionResult> GetAllCountriesAsync()
        {           
            var countries = await Mediator.Send(new GetCountryQuery());          
            return Ok(new
            {
                StatusCode = StatusCodes.Status200OK,
                message = countries.Message,
                data = countries.Data
            });
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
            var result = await Mediator.Send(new GetCountryByIdQuery { Id = id });            
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
        [HttpPost("create")]          
        public async Task<IActionResult> CreateAsync(CreateCountryCommand  command)
        { 
            var validationResult = await _createCountryCommandValidator.ValidateAsync(command);
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
            return BadRequest(new 
            { 
                StatusCode=StatusCodes.Status400BadRequest,
                message = result.Message 
            });
                       
        }
        [HttpPut("update")]      
        public async Task<IActionResult> UpdateAsync( UpdateCountryCommand command)
        {
            var validationResult = await _updateCountryCommandValidator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    message = "Validation failed",
                    errors = validationResult.Errors.Select(e => e.ErrorMessage).ToArray()
                });
            }
            if (command.Id<=0)
            {
                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    message = "Invalid Country ID"
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
              var result = await Mediator.Send(new DeleteCountryCommand { Id = id });                 
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
                data =$"Country ID {id} Deleted" 
            });
        }

        [HttpGet("by-name{name}")]     
        public async Task<IActionResult> GetCountry(string name)
        {
            var result = await Mediator.Send(new GetCountryAutoCompleteQuery { SearchPattern = name });
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
    }
}