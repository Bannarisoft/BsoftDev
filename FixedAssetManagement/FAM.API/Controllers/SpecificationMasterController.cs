using Core.Application.SpecificationMaster.Commands.CreateSpecificationMaster;
using Core.Application.SpecificationMaster.Commands.DeleteSpecificationMaster;
using Core.Application.SpecificationMaster.Commands.UpdateSpecificationMaster;
using Core.Application.SpecificationMaster.Queries.GetSpecificationMaster;
using Core.Application.SpecificationMaster.Queries.GetSpecificationMasterAutoComplete;
using Core.Application.SpecificationMaster.Queries.GetSpecificationMasterById;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FAM.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SpecificationMasterController  : ApiControllerBase
    {
        private readonly IValidator<CreateSpecificationMasterCommand> _createSpecificationMasterCommandValidator;
        private readonly IValidator<UpdateSpecificationMasterCommand> _updateSpecificationMasterCommandValidator;
        private readonly IValidator<DeleteSpecificationMasterCommand> _deleteSpecificationMasterCommandValidator;
        
        
    public SpecificationMasterController(ISender mediator, 
                            IValidator<CreateSpecificationMasterCommand> createSpecificationMasterCommandValidator, 
                            IValidator<UpdateSpecificationMasterCommand> updateSpecificationMasterCommandValidator,
                            IValidator<DeleteSpecificationMasterCommand> deleteSpecificationMasterCommandValidator) 
        : base(mediator)
        {        
            _createSpecificationMasterCommandValidator = createSpecificationMasterCommandValidator;    
            _updateSpecificationMasterCommandValidator = updateSpecificationMasterCommandValidator;                 
            _deleteSpecificationMasterCommandValidator = deleteSpecificationMasterCommandValidator;
        }
        [HttpGet]                
        public async Task<IActionResult> GetAllSpecificationMasterAsync([FromQuery] int PageNumber,[FromQuery] int PageSize,[FromQuery] string? SearchTerm = null)
        {            
            var specificationMaster = await Mediator.Send(
            new GetSpecificationMasterQuery
            {
                PageNumber = PageNumber, 
                PageSize = PageSize, 
                SearchTerm = SearchTerm
            });
            return Ok(new 
            { 
                StatusCode=StatusCodes.Status200OK, 
                message = specificationMaster.Message,
                data = specificationMaster.Data.ToList(),
                TotalCount = specificationMaster.TotalCount,
                PageNumber = specificationMaster.PageNumber,
                PageSize = specificationMaster.PageSize
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
                    message = "Invalid SpecificationMaster Id" 
                });
            }
            var result = await Mediator.Send(new GetSpecificationMasterByIdQuery { Id = id });            
            if (result is null )
            {                
                return NotFound(new 
                { 
                    StatusCode=StatusCodes.Status404NotFound,
                    message = $"SpecificationMaster {id} not found", 
                });
            }
            return Ok(new 
            {
                StatusCode=StatusCodes.Status200OK,
                data = result.Data
            });   
        }

        [HttpPost]               
        public async Task<IActionResult> CreateAsync(CreateSpecificationMasterCommand  command)
        { 
            var validationResult = await _createSpecificationMasterCommandValidator.ValidateAsync(command);
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
        public async Task<IActionResult> UpdateAsync(UpdateSpecificationMasterCommand command)
        {         
            var validationResult = await _updateSpecificationMasterCommandValidator.ValidateAsync(command);
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
            var command = new DeleteSpecificationMasterCommand { Id = id };
            var validationResult = await  _deleteSpecificationMasterCommandValidator.ValidateAsync(command);
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
            var result = await Mediator.Send(new DeleteSpecificationMasterCommand { Id = id });                 
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
                data =$"SpecificationMaster ID {id} Deleted" ,
                message = result.Message
            });
        }
            
        [HttpGet("by-name")]  
        public async Task<IActionResult> GetSpecificationMaster([FromQuery] int assetGroupId,string? name)
        {          
            var result = await Mediator.Send(new GetSpecificationMasterAutoCompleteQuery {AssetGroupId=assetGroupId,SearchPattern = name}); // Pass `searchPattern` to the constructor
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