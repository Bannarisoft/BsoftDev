using Core.Application.Manufacture.Commands.CreateManufacture;
using Core.Application.Manufacture.Commands.UpdateManufacture;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using Core.Application.Manufacture.Queries.GetManufacture;
using Core.Application.Manufacture.Queries.GetManufactureById;
using Core.Application.Manufacture.Commands.DeleteManufacture;
using Core.Application.Manufacture.Queries.GetManufactureAutoComplete;

namespace FAM.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ManufactureController  : ApiControllerBase
    { 
         private readonly IValidator<CreateManufactureCommand> _createManufactureCommandValidator;
         private readonly IValidator<UpdateManufactureCommand> _updateManufactureCommandValidator;
         
         
       public ManufactureController(ISender mediator, 
                             IValidator<CreateManufactureCommand> createManufactureCommandValidator, 
                             IValidator<UpdateManufactureCommand> updateManufactureCommandValidator) 
        : base(mediator)
        {        
            _createManufactureCommandValidator = createManufactureCommandValidator;    
            _updateManufactureCommandValidator = updateManufactureCommandValidator;                 
        }

        [HttpGet]                
        public async Task<IActionResult> GetAllCitiesAsync([FromQuery] int PageNumber,[FromQuery] int PageSize,[FromQuery] string? SearchTerm = null)
        {            
            var manufactures = await Mediator.Send(
            new GetManufactureQuery
            {
                PageNumber = PageNumber, 
                PageSize = PageSize, 
                SearchTerm = SearchTerm
            });
            return Ok(new 
            { 
                StatusCode=StatusCodes.Status200OK, 
                message = manufactures.Message,
                data = manufactures.Data.ToList(),
                TotalCount = manufactures.TotalCount,
                PageNumber = manufactures.PageNumber,
                PageSize = manufactures.PageSize
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
                    message = "Invalid Manufacture ID" 
                });
            }
            var result = await Mediator.Send(new GetManufactureByIdQuery { Id = id });            
            if (result is null )
            {                
                return NotFound(new 
                { 
                    StatusCode=StatusCodes.Status404NotFound,
                    message = $"ManufactureId {id} not found", 
                });
            }
            return Ok(new 
            {
                StatusCode=StatusCodes.Status200OK,
                data = result.Data
            });   
        }

        [HttpPost("create")]               
        public async Task<IActionResult> CreateAsync(CreateManufactureCommand  command)
        { 
            var validationResult = await _createManufactureCommandValidator.ValidateAsync(command);
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
        public async Task<IActionResult> UpdateAsync(UpdateManufactureCommand command)
        {         
            var validationResult = await _updateManufactureCommandValidator.ValidateAsync(command);
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
              var result = await Mediator.Send(new DeleteManufactureCommand { Id = id });                 
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
                data =$"Manufacture ID {id} Deleted" ,
                message = result.Message
            });
        }
             
        [HttpGet("by-name")]  
        public async Task<IActionResult> GetManufacture([FromQuery] string? name)
        {          
            var result = await Mediator.Send(new GetManufactureAutoCompleteQuery {SearchPattern = name}); // Pass `searchPattern` to the constructor
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