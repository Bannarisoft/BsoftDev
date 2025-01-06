using BSOFT.Infrastructure.Data;
using Core.Application.Country.Commands.CreateCountry;
using Core.Application.Country.Commands.DeleteCountry;
using Core.Application.Country.Commands.UpdateCountry;
using Core.Application.Country.Queries.GetCountries;
using Core.Application.Country.Queries.GetCountryAutoComplete;
using Core.Application.Country.Queries.GetCountryById;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BSOFT.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CountryController : ApiControllerBase
    {
         private readonly IValidator<CreateCountryCommand> _createCountryCommandValidator;
         private readonly IValidator<UpdateCountryCommand> _updateCountryCommandValidator;
         private readonly ApplicationDbContext _dbContext;
         
       public CountryController(ISender mediator, 
                             IValidator<CreateCountryCommand> createCountryCommandValidator, 
                             IValidator<UpdateCountryCommand> updateCountryCommandValidator,ApplicationDbContext dbContext) 
         : base(mediator)
        {        
            _createCountryCommandValidator = createCountryCommandValidator;    
            _updateCountryCommandValidator = updateCountryCommandValidator;    _dbContext = dbContext;  
             
        }
        [HttpGet]
        public async Task<IActionResult> GetAllCountriesAsync()
        {           
            var countries = await Mediator.Send(new GetCountryQuery());          
            return Ok(countries);
        }
        [HttpGet("{countryId}")]
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