using Core.Application.AssetMaster.AssetWarranty.Commands.CreateAssetWarranty;
using Core.Application.AssetMaster.AssetWarranty.Commands.DeleteAssetWarranty;
using Core.Application.AssetMaster.AssetWarranty.Commands.DeleteFileAssetWarranty;
using Core.Application.AssetMaster.AssetWarranty.Commands.UpdateAssetWarranty;
using Core.Application.AssetMaster.AssetWarranty.Commands.UploadAssetWarranty;
using Core.Application.AssetMaster.AssetWarranty.Queries.GetAssetWarranty;
using Core.Application.AssetMaster.AssetWarranty.Queries.GetAssetWarrantyAutoComplete;
using Core.Application.AssetMaster.AssetWarranty.Queries.GetAssetWarrantyById;
using Core.Application.AssetMaster.AssetWarranty.Queries.GetWarrantyClaimStatus;
using Core.Application.AssetMaster.AssetWarranty.Queries.GetWarrantyType;
using FAM.API.Validation.AssetMaster.AssetWarranty;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FAM.API.Controllers.AssetMaster
{
    public class AssetWarrantyController : ApiControllerBase
    {
        private readonly IValidator<CreateAssetWarrantyCommand> _createAssetWarrantyCommandValidator;
        private readonly IValidator<UpdateAssetWarrantyCommand> _updateAssetWarrantyCommandValidator;        
        private readonly IValidator<UploadFileAssetWarrantyCommand> _UploadFileCommandValidator;
        private readonly IValidator<DeleteAssetWarrantyCommand> _deleteAssetWarrantyCommandValidator;
        
    public AssetWarrantyController(ISender mediator, 
                            IValidator<CreateAssetWarrantyCommand> createAssetWarrantyCommandValidator, 
                            IValidator<UpdateAssetWarrantyCommand> updateAssetWarrantyCommandValidator,
                             IValidator<UploadFileAssetWarrantyCommand> UploadFileCommandValidator,
                             IValidator<DeleteAssetWarrantyCommand> deleteAssetWarrantyCommandValidator
                             ) 
        : base(mediator)
        {        
            _createAssetWarrantyCommandValidator = createAssetWarrantyCommandValidator;    
            _updateAssetWarrantyCommandValidator = updateAssetWarrantyCommandValidator;                 
            _UploadFileCommandValidator = UploadFileCommandValidator;     
            _deleteAssetWarrantyCommandValidator = deleteAssetWarrantyCommandValidator;  
        }
        [HttpGet]                
        public async Task<IActionResult> GetAllAssetWarrantyAsync([FromQuery] int PageNumber,[FromQuery] int PageSize,[FromQuery] string? SearchTerm = null)
        {            
            var WarrantyMaster = await Mediator.Send(
            new GetAssetWarrantyQuery
            {
                PageNumber = PageNumber, 
                PageSize = PageSize, 
                SearchTerm = SearchTerm
            });
            return Ok(new 
            { 
                StatusCode=StatusCodes.Status200OK, 
                message = WarrantyMaster.Message,
                data = WarrantyMaster.Data.ToList(),
                TotalCount = WarrantyMaster.TotalCount,
                PageNumber = WarrantyMaster.PageNumber,
                PageSize = WarrantyMaster.PageSize
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
                    message = "Invalid WarrantyMaster Id" 
                });
            }
            var result = await Mediator.Send(new GetAssetWarrantyByIdQuery { Id = id });            
            if (result is null )
            {                
                return NotFound(new 
                { 
                    StatusCode=StatusCodes.Status404NotFound,
                    message = $"Warranty Master {id} not found", 
                });
            }
            return Ok(new 
            {
                StatusCode=StatusCodes.Status200OK,
                data = result.Data
            });   
        }

        [HttpPost]               
        public async Task<IActionResult> CreateAsync(CreateAssetWarrantyCommand  command)
        { 
            var validationResult = await _createAssetWarrantyCommandValidator.ValidateAsync(command);
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
        public async Task<IActionResult> UpdateAsync(UpdateAssetWarrantyCommand command)
        {         
            var validationResult = await _updateAssetWarrantyCommandValidator.ValidateAsync(command);
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
            var command = new DeleteAssetWarrantyCommand { Id = id };
            var validationResult = await  _deleteAssetWarrantyCommandValidator.ValidateAsync(command);
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
            var result = await Mediator.Send(new DeleteAssetWarrantyCommand { Id = id });                 
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
                data =$"Asset Warranty ID {id} Deleted" ,
                message = result.Message
            });
        }
            
        [HttpGet("by-name")]  
        public async Task<IActionResult> GetAssetWarranty([FromQuery] string? name)
        {          
            var result = await Mediator.Send(new GetAssetWarrantyAutoCompleteQuery {SearchPattern = name}); // Pass `searchPattern` to the constructor
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
        [HttpPost("upload-logo")]
        public async Task<IActionResult> UploadLogo(UploadFileAssetWarrantyCommand uploadFileCommand)
        {
            var validationResult = await _UploadFileCommandValidator.ValidateAsync(uploadFileCommand);
            if (!validationResult.IsValid)
            {
                return BadRequest(new 
                { 
                    StatusCode=StatusCodes.Status400BadRequest, 
                    message = "Validation failed", 
                    errors = validationResult.Errors.Select(e => e.ErrorMessage).ToArray() 
                });
            }
            var file = await Mediator.Send(uploadFileCommand);
            if (!file.IsSuccess)
            {
                return BadRequest(new 
                { 
                    StatusCode=StatusCodes.Status400BadRequest, 
                    message = file.Message, 
                    errors = "" 
                });
            }
           
               
           return Ok(new 
            { 
                StatusCode=StatusCodes.Status200OK, 
                message = file.Message, 
                data = file.Data,
                errors = "" 
            });
              

        }
        [HttpDelete("delete-logo")]
        public async Task<IActionResult> DeleteLogo(DeleteFileAssetWarrantyCommand deleteFileCommand)
        {
            var file = await Mediator.Send(deleteFileCommand);
            if (!file.IsSuccess)
            {
                return BadRequest(new 
                { 
                    StatusCode=StatusCodes.Status400BadRequest, 
                    message = file.Message, 
                    errors = "" 
                });
            }
            return Ok(new 
            { 
                StatusCode=StatusCodes.Status200OK, 
                message = file.Message, 
                errors = "" 
            });
        }    
        [HttpGet("WarrantyType")]
        public async Task<IActionResult> GetManufactureTypes()
        {
            var result = await Mediator.Send(new GetWarrantyTypeQuery());

            if (result == null || result.Data == null || result.Data.Count == 0)
            {
                return NotFound(new
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    message = "No Warranty Type found."
                });
            }
            return Ok(new
            {
                StatusCode = StatusCodes.Status200OK,
                message = "Warranty Type fetched successfully.",
                data = result.Data
            });
        }    
        [HttpGet("WarrantyClaimStatus")]
        public async Task<IActionResult> GetWarrantyClaimStatus()
        {
            var result = await Mediator.Send(new GetWarrantyClaimStatusQuery());

            if (result == null || result.Data == null || result.Data.Count == 0)
            {
                return NotFound(new
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    message = "No Warranty Claim Status found."
                });
            }
            return Ok(new
            {
                StatusCode = StatusCodes.Status200OK,
                message = "Warranty Claim Status fetched successfully.",
                data = result.Data
            });
        }        
    }    
}