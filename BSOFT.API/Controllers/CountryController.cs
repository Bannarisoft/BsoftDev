using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BSOFT.Infrastructure.Data;
using Core.Application.Country.Commands;
using Core.Application.Country.Commands.CreateCountry;
using Core.Application.Country.Commands.DeleteCountry;
using Core.Application.Country.Commands.UpdateCountry;
using Core.Application.Country.Queries.GetCountries;
using Core.Application.Country.Queries.GetCountryAutoComplete;
using Core.Application.Country.Queries.GetcountryById;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
            //var countries = await Mediator.Send(new GetCountryQuery());
            //return Ok(countries);

            var countries = await Mediator.Send(new GetCountryQuery());
            var activeCountries = countries.Where(c => c.IsActive == 1).ToList(); // Filter active countries
            return Ok(activeCountries);

        }

        [HttpGet("{countryid}")]
        public async Task<IActionResult> GetByIdAsync(int countryid)
        {
             if (countryid <= 0)
            {
                return BadRequest("Invalid Country ID");
            }

            var country = await Mediator.Send(new GetCountryByIdQuery() { Id = countryid });

            if (country == null || country.IsActive != 1) // Check if the country is active
            {
                return NotFound("Active country not found.");
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
        //return CreatedAtAction(nameof(GetByIdAsync), new { id = countryDto.Id }, countryDto);
    }
    [HttpPut("{countryid}")]
    public async Task<IActionResult> UpdateAsync(int countryid, UpdateCountryCommand command)
    {
         // Use the validator for UpdateCountryCommand
        var validationResult = await _updateCountryCommandValidator.ValidateAsync(command);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        if (countryid != command.Id)
        {
            return BadRequest("CountryId Mismatch");
        }   
        var country = await _dbContext.Countries
                                    .FirstOrDefaultAsync(c => c.Id == countryid && c.IsActive == 1);

        if (country == null)
        {
            return BadRequest("Only active countries (IsActive = 1) can be updated.");
        }

        await Mediator.Send(command);
        return NoContent();
    }
    [HttpDelete("{countryid}")]
    public async Task<IActionResult> DeleteAsync(int countryid,DeleteCountryCommand command)
    {
         if(countryid != command.Id)
        {
           return BadRequest("CountryID Mismatch"); 
        }
        // Ensure the country is active before deletion
        var country = await _dbContext.Countries
                                        .FirstOrDefaultAsync(c => c.Id == countryid && c.IsActive == 1);

        if (country == null)
        {
            return BadRequest("Only active countries (IsActive = 1) can be deleted.");
        }
        await Mediator.Send(command);
        return Ok("Status Closed Successfully");
    }

       [HttpGet("GetCountrysearch")]
        public async Task<IActionResult> GetCountry([FromQuery] string searchPattern)
        {
           
            var countries = await Mediator.Send(new GetcountryAutoCompleteQuery {SearchPattern = searchPattern}); // Pass `searchPattern` to the constructor
            var activeCountries = countries.Where(c => c.IsActive == 1).ToList(); 
            return Ok(activeCountries);
        }


     
    }
}