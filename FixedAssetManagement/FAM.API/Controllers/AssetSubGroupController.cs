using Core.Application.AssetSubGroup.Command.CreateAssetSubGroup;
using Core.Application.AssetSubGroup.Command.DeleteAssetSubGroup;
using Core.Application.AssetSubGroup.Command.UpdateAssetSubGroup;
using Core.Application.AssetSubGroup.Queries.GetAssetSubGroup;
using Core.Application.AssetSubGroup.Queries.GetAssetSubGroupAutoComplete;
using Core.Application.AssetSubGroup.Queries.GetAssetSubGroupById;
using FAM.Infrastructure.Data;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FAM.API.Controllers
{
    [Route("api/[controller]")]
     [ApiController]
    public class AssetSubGroupController : ApiControllerBase
    {
        private readonly IValidator<CreateAssetSubGroupCommand> _createAssetSubGroupCommandValidator;
        private readonly IValidator<UpdateAssetSubGroupCommand> _updateAssetSubGroupCommandValidator;
        private readonly ILogger<AssetSubGroupController> _logger;
        
        private readonly ApplicationDbContext _dbContext;
        private readonly IMediator _mediator;

        public AssetSubGroupController(ILogger<AssetSubGroupController> logger, IMediator mediator, ApplicationDbContext dbContext, IValidator<CreateAssetSubGroupCommand> createAssetSubGroupCommandValidator, IValidator<UpdateAssetSubGroupCommand> updateAssetSubGroupCommandValidator)
        : base(mediator)
        {
            _logger = logger;
            _mediator = mediator;
            _dbContext = dbContext;
            _createAssetSubGroupCommandValidator = createAssetSubGroupCommandValidator;
            _updateAssetSubGroupCommandValidator = updateAssetSubGroupCommandValidator;
        }

[HttpPost]
public async Task<IActionResult> CreateAsync(CreateAssetSubGroupCommand createAssetSubGroupCommand)
{
      /*   if (createAssetSubGroupCommand.GroupId <= 0)
        {
            return BadRequest(new
            {
                StatusCode = StatusCodes.Status400BadRequest,
                message = "Enter valid GroupId"                                
            });
        } */

    // Validate the incoming command
            var validationResult = await _createAssetSubGroupCommandValidator.ValidateAsync(createAssetSubGroupCommand);
     _logger.LogWarning($"Validation failed: {string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage))}");
    if (!validationResult.IsValid)
    {
        
        return BadRequest(new
        {
            StatusCode = StatusCodes.Status400BadRequest,
            message = "Validation failed",
            errors = validationResult.Errors.Select(e => e.ErrorMessage)
        });
    }

    // Process the command
    var CreatedAssetSubGroupId = await _mediator.Send(createAssetSubGroupCommand);

    if (CreatedAssetSubGroupId.IsSuccess)
    {
     _logger.LogInformation($"AssetSubGroup {createAssetSubGroupCommand.Code} created successfully.");
      return Ok(new
      {
          StatusCode = StatusCodes.Status201Created,
          message =CreatedAssetSubGroupId.Message,
          data = CreatedAssetSubGroupId.Data
      });
    }
     _logger.LogWarning($"AssetSubGroup {createAssetSubGroupCommand.Code} Creation failed.");
      return BadRequest(new
        {
            StatusCode = StatusCodes.Status400BadRequest,
            message = CreatedAssetSubGroupId.Message
        });
  
}
[HttpPut]
public async Task<IActionResult> UpdateAsync(UpdateAssetSubGroupCommand updateAssetSubGroupCommand)
{

        var validationResult = await _updateAssetSubGroupCommandValidator.ValidateAsync(updateAssetSubGroupCommand);
        _logger.LogWarning($"Validation failed: {string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage))}");
        if (!validationResult.IsValid)
        {
           
            return BadRequest(new
            {
                StatusCode = StatusCodes.Status400BadRequest,
                message = "Validation failed",
                errors = validationResult.Errors.Select(e => e.ErrorMessage)
            });
        }

        var updatedAssetSubGroup = await _mediator.Send(updateAssetSubGroupCommand);

        if (updatedAssetSubGroup.IsSuccess)
        {
            _logger.LogInformation($"AssetSubGroup {updateAssetSubGroupCommand.SubGroupName} updated successfully.");
           return Ok(new
            {
                message = updatedAssetSubGroup.Message,
                statusCode = StatusCodes.Status200OK
            });
        }
        _logger.LogWarning($"AssetSubGroup {updateAssetSubGroupCommand.SubGroupName} Update failed.");
        return NotFound(new
        {
            message =updatedAssetSubGroup.Message,
            statusCode = StatusCodes.Status404NotFound
        });   
}

[HttpDelete("{id}")]
public async Task<IActionResult> DeleteAssetSubGroupAsync(int id)
{

        // Process the delete command
        var result = await _mediator.Send(new DeleteAssetSubGroupCommand { Id = id });

        if (result.IsSuccess) 
        {
            _logger.LogInformation($"AssetSubGroup {id} deleted successfully.");
             return Ok(new
            {
                message = result.Message,
                statusCode = StatusCodes.Status200OK
            });
            
        }
         _logger.LogWarning($"AssetSubGroup {id} Not Found or Invalid AssetSubGroupId.");
        return NotFound(new
        {
            message = result.Message,
            statusCode = StatusCodes.Status404NotFound
        });
   
}
        [HttpGet]
        public async Task<IActionResult> GetAllAssetSubGroupAsync([FromQuery] int PageNumber,[FromQuery] int PageSize,[FromQuery] string? SearchTerm = null)
        {
           var assetSubGroups = await Mediator.Send(
            new GetAssetSubGroupQuery
            {
                PageNumber = PageNumber, 
                PageSize = PageSize, 
                SearchTerm = SearchTerm
            });           
            return Ok( new 
            { 
                StatusCode=StatusCodes.Status200OK, 
                data = assetSubGroups.Data,
                TotalCount = assetSubGroups.TotalCount,
                PageNumber = assetSubGroups.PageNumber,
                PageSize = assetSubGroups.PageSize
                });
        }

        [HttpGet("by-name")]
        public async Task<IActionResult> GetAssetSubGroup([FromQuery] string? SubGroupName)
        {
        var assetSubGroups = await Mediator.Send(new GetAssetSubGroupAutoCompleteQuery 
        { 
                SearchPattern = SubGroupName ?? string.Empty 
        });

        return Ok(new { StatusCode = StatusCodes.Status200OK, data = assetSubGroups.Data });
        }

        [HttpGet("{id}")]
        [ActionName(nameof(GetByIdAsync))]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var assetSubGroup = await Mediator.Send(new GetAssetSubGroupByIdQuery() { Id = id});
          
            if(assetSubGroup.IsSuccess)
            {
                
              return Ok(new { StatusCode=StatusCodes.Status200OK, data = assetSubGroup.Data,message = assetSubGroup.Message });
            }
            return NotFound( new { StatusCode=StatusCodes.Status404NotFound, message = assetSubGroup.Message });
           
        }

    }
}