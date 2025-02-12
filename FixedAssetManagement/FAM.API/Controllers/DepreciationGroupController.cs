using Core.Application.DepreciationGroup.Commands.CreateDepreciationGroup;
using Core.Application.DepreciationGroup.Commands.DeleteDepreciationGroup;
using Core.Application.DepreciationGroup.Commands.UpdateDepreciationGroup;
using Core.Application.DepreciationGroup.Queries.GetDepreciationGroup;
using Core.Application.DepreciationGroup.Queries.GetDepreciationGroupAutoComplete;
using Core.Application.DepreciationGroup.Queries.GetDepreciationGroupById;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FAM.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DepreciationGroupController  : ApiControllerBase
    {
         private readonly IValidator<CreateDepreciationGroupCommand> _createDepreciationGroupCommandValidator;
         private readonly IValidator<UpdateDepreciationGroupCommand> _updateDepreciationGroupCommandValidator;
         
         
       public DepreciationGroupController(ISender mediator, 
                             IValidator<CreateDepreciationGroupCommand> createDepreciationGroupCommandValidator, 
                             IValidator<UpdateDepreciationGroupCommand> updateDepreciationGroupCommandValidator) 
        : base(mediator)
        {        
            _createDepreciationGroupCommandValidator = createDepreciationGroupCommandValidator;    
            _updateDepreciationGroupCommandValidator = updateDepreciationGroupCommandValidator;                 
        }
     [HttpGet]                
        public async Task<IActionResult> GetAllCitiesAsync([FromQuery] int PageNumber,[FromQuery] int PageSize,[FromQuery] string? SearchTerm = null)
        {            
            var depreciationGroups = await Mediator.Send(
            new GetDepreciationGroupQuery
            {
                PageNumber = PageNumber, 
                PageSize = PageSize, 
                SearchTerm = SearchTerm
            });
            return Ok(new 
            { 
                StatusCode=StatusCodes.Status200OK, 
                message = depreciationGroups.Message,
                data = depreciationGroups.Data.ToList(),
                TotalCount = depreciationGroups.TotalCount,
                PageNumber = depreciationGroups.PageNumber,
                PageSize = depreciationGroups.PageSize
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
                    message = "Invalid DepreciationGroup ID" 
                });
            }
            var result = await Mediator.Send(new GetDepreciationGroupByIdQuery { Id = id });            
            if (result is null )
            {                
                return NotFound(new 
                { 
                    StatusCode=StatusCodes.Status404NotFound,
                    message = "DepreciationGroupId {id} not found", 
                });
            }
            return Ok(new 
            {
                StatusCode=StatusCodes.Status200OK,
                data = result.Data
            });   
        }

        [HttpPost("create")]               
        public async Task<IActionResult> CreateAsync(CreateDepreciationGroupCommand  command)
        { 
            var validationResult = await _createDepreciationGroupCommandValidator.ValidateAsync(command);
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
        public async Task<IActionResult> UpdateAsync(UpdateDepreciationGroupCommand command)
        {         
            var validationResult = await _updateDepreciationGroupCommandValidator.ValidateAsync(command);
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
              var result = await Mediator.Send(new DeleteDepreciationGroupCommand { Id = id });                 
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
                data =$"DepreciationGroup ID {id} Deleted" ,
                message = result.Message
            });
        }
             
        [HttpGet("by-name")]  
        public async Task<IActionResult> GetCity([FromQuery] string? name)
        {          
            var result = await Mediator.Send(new GetDepreciationGroupAutoCompleteQuery {SearchPattern = name}); // Pass `searchPattern` to the constructor
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