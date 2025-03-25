using Core.Application.AssetMaster.AssetSpecification.Commands.CreateAssetSpecification;
using Core.Application.AssetMaster.AssetSpecification.Commands.DeleteAssetSpecification;
using Core.Application.AssetMaster.AssetSpecification.Commands.UpdateAssetSpecification;
using Core.Application.AssetMaster.AssetSpecification.Queries.GetAssetSpecification;
using Core.Application.AssetMaster.AssetSpecification.Queries.GetAssetSpecificationAutoComplete;
using Core.Application.AssetMaster.AssetSpecification.Queries.GetAssetSpecificationById;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FAM.API.Controllers.AssetMaster
{
    [ApiController]
    [Route("api/[controller]")]
    public class AssetSpecificationController : ApiControllerBase
    {
        private readonly IValidator<CreateAssetSpecificationCommand> _createAssetSpecificationCommandValidator;
        private readonly IValidator<UpdateAssetSpecificationCommand> _updateAssetSpecificationCommandValidator;
        private readonly IValidator<DeleteAssetSpecificationCommand> _deleteAssetSpecificationCommandValidator;
        
        
    public AssetSpecificationController(ISender mediator, 
                            IValidator<CreateAssetSpecificationCommand> createAssetSpecificationCommandValidator, 
                            IValidator<UpdateAssetSpecificationCommand> updateAssetSpecificationCommandValidator,
                            IValidator<DeleteAssetSpecificationCommand> deleteAssetSpecificationCommandValidator)
        : base(mediator)
        {        
            _createAssetSpecificationCommandValidator = createAssetSpecificationCommandValidator;    
            _updateAssetSpecificationCommandValidator = updateAssetSpecificationCommandValidator;                 
            _deleteAssetSpecificationCommandValidator = deleteAssetSpecificationCommandValidator;     
        }
        [HttpGet]                
        public async Task<IActionResult> GetAllAssetSpecificationAsync([FromQuery] int PageNumber,[FromQuery] int PageSize,[FromQuery] string? SearchTerm = null)
        {            
            var specificationMaster = await Mediator.Send(
            new GetAssetSpecificationQuery
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
            var result = await Mediator.Send(new GetAssetSpecificationByIdQuery { Id = id });            
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
        public async Task<IActionResult> CreateAsync(CreateAssetSpecificationCommand  command)
        { 
            var validationResult = await _createAssetSpecificationCommandValidator.ValidateAsync(command);
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
        public async Task<IActionResult> UpdateAsync(UpdateAssetSpecificationCommand command)
        {         
            var validationResult = await _updateAssetSpecificationCommandValidator.ValidateAsync(command);
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
            var command = new DeleteAssetSpecificationCommand { Id = id };
            var validationResult = await  _deleteAssetSpecificationCommandValidator.ValidateAsync(command);
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
            var result = await Mediator.Send(new DeleteAssetSpecificationCommand { Id = id });                 
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
                data =$"Asset Specification ID {id} Deleted" ,
                message = result.Message
            });
        }
            
        [HttpGet("by-name")]  
        public async Task<IActionResult> GetAssetSpecification([FromQuery] string? name)
        {          
            var result = await Mediator.Send(new GetAssetSpecificationAutoCompleteQuery {SearchPattern = name}); // Pass `searchPattern` to the constructor
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