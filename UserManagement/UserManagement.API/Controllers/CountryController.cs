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
         private readonly IValidator<DeleteCountryCommand> _deleteCountryCommandValidator;       
         
       public CountryController(ISender mediator, 
                             IValidator<CreateCountryCommand> createCountryCommandValidator, 
                             IValidator<UpdateCountryCommand> updateCountryCommandValidator, 
                             IValidator<DeleteCountryCommand> deleteCountryCommandValidator) 
         : base(mediator)
        {        
            _createCountryCommandValidator = createCountryCommandValidator;    
            _updateCountryCommandValidator = updateCountryCommandValidator;   
            _deleteCountryCommandValidator = deleteCountryCommandValidator;              
        }
        [HttpGet]        
        public async Task<IActionResult> GetAllCountriesAsync([FromQuery] int PageNumber,[FromQuery] int PageSize,[FromQuery] string? SearchTerm = null)
        {   
            var countries = await Mediator.Send(
            new GetCountryQuery
           {
                PageNumber = PageNumber, 
                PageSize = PageSize, 
                SearchTerm = SearchTerm
            });

                  
            return Ok(new
            {
                StatusCode = StatusCodes.Status200OK,
                message = countries.Message,
                data = countries.Data.ToList(),
                TotalCount = countries.TotalCount,
                PageNumber = countries.PageNumber,
                PageSize = countries.PageSize
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
        [HttpPost]          
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
        [HttpPut]      
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
        [HttpDelete("{id}")]   
        public async Task<IActionResult> DeleteAsync(int id)
        {
             var command = new DeleteCountryCommand { Id = id };
             var validationResult = await  _deleteCountryCommandValidator.ValidateAsync(command);
               if (!validationResult.IsValid)
                {
                    return BadRequest(new
                    {
                        message = validationResult.Errors.Select(e => e.ErrorMessage).FirstOrDefault(),
                        statusCode = StatusCodes.Status400BadRequest
                    });
                } 
            if (id <= 0)
            {
                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    message = "Invalid Country ID"
                });
            }            
              var result = await Mediator.Send(command);                 
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
                data =$"Country ID {id} Deleted" ,
                message = result.Message
            });
        }

        [HttpGet("by-name")]     
        public async Task<IActionResult> GetCountry([FromQuery] string? name)
        {
            var result = await Mediator.Send(new GetCountryAutoCompleteQuery { SearchPattern = name });
            if (!result.IsSuccess)
            {
                return NotFound(new 
                { 
                    StatusCode = StatusCodes.Status404NotFound,
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
    }
}