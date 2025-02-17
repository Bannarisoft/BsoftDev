
using Core.Application.AssetMaster.AssetMasterGeneral.Commands.CreateAssetMasterGeneral;
using Core.Application.AssetMaster.AssetMasterGeneral.Commands.DeleteAssetMasterGeneral;
using Core.Application.AssetMaster.AssetMasterGeneral.Commands.UpdateAssetMasterGeneral;
using Core.Application.AssetMaster.AssetMasterGeneral.Queries.GetAssetMasterGeneral;
using Core.Application.AssetMaster.AssetMasterGeneral.Queries.GetAssetMasterGeneralAutoComplete;
using Core.Application.AssetMaster.AssetMasterGeneral.Queries.GetAssetMasterGeneralById;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FAM.API.Controllers.AssetMaster
{
    [ApiController]
    [Route("api/[controller]")]
    public class AssetMasterGeneralController : ApiControllerBase
    {
        private readonly IValidator<CreateAssetMasterGeneralCommand> _createAssetMasterGeneralCommandValidator;
         private readonly IValidator<UpdateAssetMasterGeneralCommand> _updateAssetMasterGeneralCommandValidator;
         
         
       public AssetMasterGeneralController(ISender mediator, 
                             IValidator<CreateAssetMasterGeneralCommand> createAssetMasterGeneralCommandValidator, 
                             IValidator<UpdateAssetMasterGeneralCommand> updateAssetMasterGeneralCommandValidator) 
        : base(mediator)
        {        
            _createAssetMasterGeneralCommandValidator = createAssetMasterGeneralCommandValidator;    
            _updateAssetMasterGeneralCommandValidator = updateAssetMasterGeneralCommandValidator;                 
        }

         [HttpGet]                
        public async Task<IActionResult> GetAllAssetMasterGeneralAsync([FromQuery] int PageNumber,[FromQuery] int PageSize,[FromQuery] string? SearchTerm = null)
        {            
            var assetMaster = await Mediator.Send(
            new GetAssetMasterGeneralQuery
            {
                PageNumber = PageNumber, 
                PageSize = PageSize, 
                SearchTerm = SearchTerm
            });
            return Ok(new 
            { 
                StatusCode=StatusCodes.Status200OK, 
                message = assetMaster.Message,
                data = assetMaster.Data.ToList(),
                TotalCount = assetMaster.TotalCount,
                PageNumber = assetMaster.PageNumber,
                PageSize = assetMaster.PageSize
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
                    message = "Invalid Asset ID" 
                });
            }
            var result = await Mediator.Send(new GetAssetMasterGeneralByIdQuery { Id = id });            
            if (result is null )
            {                
                return NotFound(new 
                { 
                    StatusCode=StatusCodes.Status404NotFound,
                    message = $"AssetId {id} not found", 
                });
            }
            return Ok(new 
            {
                StatusCode=StatusCodes.Status200OK,
                data = result.Data
            });   
        }

        [HttpPost]               
        public async Task<IActionResult> CreateAsync(CreateAssetMasterGeneralCommand  command)
        { 
            var validationResult = await _createAssetMasterGeneralCommandValidator.ValidateAsync(command);
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
        public async Task<IActionResult> UpdateAsync(UpdateAssetMasterGeneralCommand command)
        {         
            var validationResult = await _updateAssetMasterGeneralCommandValidator.ValidateAsync(command);
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
        [HttpDelete("   {id}")]        
        public async Task<IActionResult> DeleteAsync(int id)
        {             
            if (id <= 0)
            {
                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    message = "Invalid Asset ID"
                });
            }            
              var result = await Mediator.Send(new DeleteAssetMasterGeneralCommand { Id = id });                 
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
        public async Task<IActionResult> GetAssetName([FromQuery] string? name)
        {          
            var result = await Mediator.Send(new GetAssetMasterGeneralAutoCompleteQuery {SearchPattern = name}); // Pass `searchPattern` to the constructor
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