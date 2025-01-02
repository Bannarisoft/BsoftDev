using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BSOFT.Infrastructure.Data;
using BSOFT.Infrastructure.Repositories;
using Core.Application.City.Commands.CreateCity;
using Core.Application.City.Commands.DeleteCity;
using Core.Application.City.Commands.UpdateCity;
using Core.Application.City.Queries.GetCities;
using Core.Application.City.Queries.GetCityAutoComplete;
using Core.Application.City.Queries.GetCityById;
using Core.Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BSOFT.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]


    public class CityController : ApiControllerBase
    {
         private readonly IValidator<CreateCityCommand> _createCityCommandValidator;
         private readonly IValidator<UpdateCityCommand> _updateCityCommandValidator;
         private readonly ApplicationDbContext _dbContext;
         
       public CityController(ISender mediator, 
                             IValidator<CreateCityCommand> createCityCommandValidator, 
                             IValidator<UpdateCityCommand> updateCityCommandValidator,ApplicationDbContext dbContext) 
         : base(mediator)
        {        
            _createCityCommandValidator = createCityCommandValidator;    
            _updateCityCommandValidator = updateCityCommandValidator;    _dbContext = dbContext;  
             
        }
        [HttpGet]
        public async Task<IActionResult> GetAllCitiesAsync()
        {            

            var cities = await Mediator.Send(new GetCityQuery());
            var activeCities = cities.Where(c => c.IsActive == 1).ToList(); 
            return Ok(activeCities);

        }

        [HttpGet("{cityId}")]
        public async Task<IActionResult> GetByIdAsync(int cityId)
        {
             if (cityId <= 0)
            {
                return BadRequest("Invalid City ID");
            }

            var city = await Mediator.Send(new GetCityByIdQuery() { Id = cityId });

            if (city == null || city.IsActive != 1) 
            {
                return NotFound("This CityID not Active");
            }

            return Ok(city);           
        }

        
    [HttpPost]
    public async Task<IActionResult> CreateAsync(CreateCityCommand  command)
    { 

        var validationResult = await _createCityCommandValidator.ValidateAsync(command);
        if (!validationResult.IsValid)
        {
        return BadRequest(validationResult.Errors);
        }        
        // If validation passes, send the command
        var result = await Mediator.Send(command);
        if (result.IsSuccess)
        {
        return Ok(new { Message = "City created successfully", City = result.Data });
        }
        else
        {
        // If the result is a failure, return BadRequest with the error message
        return BadRequest(result.ErrorMessage);
        }
        //return Ok("Created successfully");         
        
    }
    [HttpPut("{cityId}")]
    public async Task<IActionResult> UpdateAsync(int cityId, UpdateCityCommand command)
    {
         // Use the validator for UpdateCountryCommand
        var validationResult = await _updateCityCommandValidator.ValidateAsync(command);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }
        if (command.StateId<=0)
        {
            return BadRequest("Invalid StateID");
        }        
          
     /*    var stateExists  = await _dbContext.States
                                .FirstOrDefaultAsync(c => c.Id == command.StateId && c.IsActive == 1);        
    

        if (stateExists==null )
        {
            return BadRequest("The specified State ID does not exist or is inactive.");
        } 

        if (cityId != command.Id)
        {
            return BadRequest("CityId Mismatch");
        }   


        var city = await _dbContext.Cities
                                    .FirstOrDefaultAsync(c => c.Id == cityId && c.IsActive == 1);

        if (city == null)
        {
            return BadRequest("Only active cities (IsActive = 1) can be updated.");
        } */

        var result = await Mediator.Send(command);
        if (result.IsSuccess)
        {
        return Ok(new { Message = "City Updated successfully", City = result.Data });
        }
        else
        {
        // If the result is a failure, return BadRequest with the error message
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
       /*  
        var city = await _dbContext.Cities
                                        .FirstOrDefaultAsync(c => c.Id == cityId && c.IsActive == 1);
                                        

        if (city == null)
        {
            return BadRequest("Only active cities (IsActive = 1) can be deleted.");
        } */
      //  await Mediator.Send(command);
       // return Ok("Status Closed Successfully");

         var result = await Mediator.Send(command);
        if (result.IsSuccess)
        {
        return Ok(new { Message = "City Deleted successfully", City = result.Data });
        }
        else
        {
        // If the result is a failure, return BadRequest with the error message
        return BadRequest(result.ErrorMessage);
        }
       
    }

       [HttpGet("GetCitySearch")]
        public async Task<IActionResult> GetCountry([FromQuery] string searchPattern)
        {
           
            var cities = await Mediator.Send(new GetCityAutoCompleteQuery {SearchPattern = searchPattern}); // Pass `searchPattern` to the constructor
            var activeCities = cities.Where(c => c.IsActive == 1).ToList(); 
            return Ok(activeCities);
        }


     
    }
}