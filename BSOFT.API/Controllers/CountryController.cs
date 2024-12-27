using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Country.Commands;
using Core.Application.Country.Commands.CreateCountry;
using Core.Application.Country.Queries.GetCountries;
using Core.Application.Country.Queries.GetCountryAutoComplete;
using Core.Application.Country.Queries.GetcountryById;
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
         
       public CountryController(ISender mediator, IValidator<CreateCountryCommand> createCountryCommandValidator) 
            : base(mediator)
        {        
            _createCountryCommandValidator = createCountryCommandValidator;    
        }
        [HttpGet]
        public async Task<IActionResult> GetAllCountriesAsync()
        {
            var units = await Mediator.Send(new GetCountryQuery());
            return Ok(units);
        }

        [HttpGet("{countryid}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var country = await Mediator.Send(new GetCountryByIdQuery() {Id=id}); 
            if(country == null)
            {
                BadRequest("Invalid CountryID");
            }
            return Ok(country);
        }

        
    [HttpPost]
    public async Task<IActionResult> CreateAsync(CreateCountryCommand  command)
    { 

        var validationResult = await _createCountryCommandValidator.ValidateAsync(command);
        if (!validationResult.IsValid)
        {
        return BadRequest(validationResult.Errors);
        }

        // If validation passes, send the command
        var countryDto = await Mediator.Send(command);
        return Ok("Created successfully");
        // Return a 201 Created response with the correct route and ID
        return CreatedAtRoute(nameof(GetByIdAsync), new { countryid  = countryDto.Id }, countryDto);
    }
    [HttpPut("update/{countryid}")]
    public async Task<IActionResult> UpdateAsync(int countryid, UpdateCountryCommand command)
    {
        if (countryid    != command.Id)
        {
            return BadRequest("CountryId Mismatch");
        }

        await Mediator.Send(command);
        return NoContent();
    }
    [HttpPut("delete/{countryid}")]
    public async Task<IActionResult> DeleteAsync(int countryid,DeleteCountryCommand command)
    {
         if(countryid != command.Id)
        {
           return BadRequest("CountryID Mismatch"); 
        }

        await Mediator.Send(command);
        return Ok("Status Closed Successfully");
    }

       [HttpGet("GetCountrysearch")]
        public async Task<IActionResult> GetCountry([FromQuery] string searchPattern)
        {
           
            var countries = await Mediator.Send(new GetcountryAutoCompleteQuery {SearchPattern = searchPattern}); // Pass `searchPattern` to the constructor
            return Ok(countries);
        }


     
    }
}