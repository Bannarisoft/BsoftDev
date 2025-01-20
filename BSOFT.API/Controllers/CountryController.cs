using BSOFT.Infrastructure.Data;
using Core.Application.Country.Commands.CreateCountry;
using Core.Application.Country.Commands.DeleteCountry;
using Core.Application.Country.Commands.UpdateCountry;
using Core.Application.Country.Queries.GetCountries;
using Core.Application.Country.Queries.GetCountryAutoComplete;
using Core.Application.Country.Queries.GetCountryById;
using Core.Application.UserSession;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BSOFT.API.Controllers
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
            return Ok(countries);
        }
        [HttpGet("{countryId}")]
        [Authorize]
        public async Task<IActionResult> GetByIdAsync(int countryId)
        {
            if (countryId <= 0)
            {
                return BadRequest("Invalid Country ID");
            }            
            var result = await Mediator.Send(new GetCountryByIdQuery { Id = countryId });            
            if (!result.IsSuccess)
            {                
                return NotFound(new { Message = result.ErrorMessage });
            }
            return Ok(result.Data);
        }
          [HttpGet("user-session/{userId}")]
    public async Task<IActionResult> GetUserSession(int userId)
    {
        var query = new GetUserSessionQuery { UserId = userId };
        var session = await Mediator.Send(query);

        if (session == null)
        {
            return NotFound("No active session found for the user.");
        }

        return Ok(session);
    }
        [HttpPost]
        public async Task<IActionResult> CreateAsync(CreateCountryCommand  command)
        { 
            var validationResult = await _createCountryCommandValidator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }                    
            var result = await Mediator.Send(command);
            if (result.IsSuccess)
            {
                return Ok(new { Message = "Country created successfully", Country = result.Data });
            }
            else
            {            
                return BadRequest(result.ErrorMessage);
            }            
        }
        [HttpPut("{countryId}")]
        public async Task<IActionResult> UpdateAsync(int countryId, UpdateCountryCommand command)
        {
            var validationResult = await _updateCountryCommandValidator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }
            if (command.Id<=0)
            {
                return BadRequest("Invalid CountryId");
            }        
            var result = await Mediator.Send(command);
            if (result.IsSuccess)
            {
                return Ok(new { Message = "Country Updated successfully", Country = result.Data });
            }
            else
            {            
                return BadRequest(result.ErrorMessage);
            }
        }
        [HttpDelete("{countryId}")]
        public async Task<IActionResult> DeleteAsync(int countryId,DeleteCountryCommand command)
        {
            if(countryId != command.Id)
            {
                return BadRequest("CountryId Mismatch"); 
            }
            var result = await Mediator.Send(command);
            if (result.IsSuccess)
            {
                return Ok(new { Message = "Country Deleted successfully", Country = result.Data });
            }
            else
            {            
                return BadRequest(result.ErrorMessage);
            }
        }

        [HttpGet("GetCountrySearch")]
        public async Task<IActionResult> GetCountry([FromQuery] string searchPattern)
        {
            var result = await Mediator.Send(new GetCountryAutoCompleteQuery { SearchPattern = searchPattern });
            if (!result.IsSuccess)
            {
                return NotFound(new { Message = result.ErrorMessage }); 
            }
            return Ok(result.Data);
        } 
    }
}