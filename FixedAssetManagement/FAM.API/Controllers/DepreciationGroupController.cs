using Core.Application.DepreciationGroup.Commands.CreateDepreciationGroup;
using Core.Application.DepreciationGroup.Commands.DeleteDepreciationGroup;
using Core.Application.DepreciationGroup.Commands.UpdateDepreciationGroup;
using Core.Application.DepreciationGroup.Queries.GetBookTypeQuery;
using Core.Application.DepreciationGroup.Queries.GetDepreciationGroup;
using Core.Application.DepreciationGroup.Queries.GetDepreciationGroupAutoComplete;
using Core.Application.DepreciationGroup.Queries.GetDepreciationGroupById;
using Core.Application.DepreciationGroup.Queries.GetDepreciationMethodQuery;
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
        private readonly IValidator<DeleteDepreciationGroupCommand> _deleteDepreciationGroupCommandValidator;
         
         
       public DepreciationGroupController(ISender mediator, 
                             IValidator<CreateDepreciationGroupCommand> createDepreciationGroupCommandValidator, 
                             IValidator<UpdateDepreciationGroupCommand> updateDepreciationGroupCommandValidator,
                             IValidator<DeleteDepreciationGroupCommand> deleteDepreciationGroupCommandValidator) 
        : base(mediator)
        {        
            _createDepreciationGroupCommandValidator = createDepreciationGroupCommandValidator;    
            _updateDepreciationGroupCommandValidator = updateDepreciationGroupCommandValidator;                 
            _deleteDepreciationGroupCommandValidator = deleteDepreciationGroupCommandValidator;     
        }
        [HttpGet]                
        public async Task<IActionResult> GetAllDepreciationGroupsAsync([FromQuery] int PageNumber,[FromQuery] int PageSize,[FromQuery] string? SearchTerm = null)
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
        [ActionName(nameof(GetByIdAsync))]        
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
                    message = $"DepreciationGroupId {id} not found", 
                });
            }
            return Ok(new 
            {
                StatusCode=StatusCodes.Status200OK,
                data = result.Data
            });   
        }

        [HttpPost]               
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
        [HttpPut]        
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
                    asset = result.Data
                });
            }
                
                return BadRequest( new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    message = result.Message
                });
                
        }
        [HttpDelete("{id}")]        
        public async Task<IActionResult> DeleteAsync(int id)
        {      
            var command = new DeleteDepreciationGroupCommand { Id = id };
            var validationResult = await  _deleteDepreciationGroupCommandValidator.ValidateAsync(command);
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
                    message = "Invalid Asset ID"
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
        public async Task<IActionResult> GetDepreciationGroup([FromQuery] string? name)
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
       [HttpGet("bookType")]
        public async Task<IActionResult> GetBookTypes()
        {
            var result = await Mediator.Send(new GetBookTypeQuery());

            if (result == null || result.Data == null || result.Data.Count == 0)
            {
                return NotFound(new
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    message = "No Book Types found."
                });
            }
            return Ok(new
            {
                StatusCode = StatusCodes.Status200OK,
                message = "Book Types fetched successfully.",
                data = result.Data
            });
        }
        [HttpGet("DepMethod")]
        public async Task<IActionResult> GetDepreciationMethods()
        {
            var result = await Mediator.Send(new GetDepreciationMethodQuery());

            if (result == null || result.Data == null || result.Data.Count == 0)
            {
                return NotFound(new
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    message = "No Depreciation Method Types found."
                });
            }
            return Ok(new
            {
                StatusCode = StatusCodes.Status200OK,
                message = "Depreciation Method  fetched successfully.",
                data = result.Data
            });
        }
    }
}