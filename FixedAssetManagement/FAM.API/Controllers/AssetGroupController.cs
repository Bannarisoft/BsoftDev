using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.AssetGroup.Command.CreateAssetGroup;
using Core.Application.AssetGroup.Command.DeleteAssetGroup;
using Core.Application.AssetGroup.Command.UpdateAssetGroup;
using Core.Application.AssetGroup.Queries.GetAssetGroup;
using Core.Application.AssetGroup.Queries.GetAssetGroupAutoComplete;
using Core.Application.AssetGroup.Queries.GetAssetGroupById;
using FAM.Infrastructure.Data;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace FAM.API.Controllers
{
    [Route("[controller]")]
     [ApiController]
    public class AssetGroupController : ApiControllerBase
    {
        private readonly IValidator<CreateAssetGroupCommand> _createassetgroupcommandvalidator;
        private readonly IValidator<UpdateAssetGroupCommand> _updateassetgroupcommandvalidator;
        private readonly ILogger<AssetGroupController> _logger;
        
        private readonly ApplicationDbContext _dbContext;
        private readonly IMediator _mediator;

        public AssetGroupController(ILogger<AssetGroupController> logger, IMediator mediator, ApplicationDbContext dbContext, IValidator<CreateAssetGroupCommand> createassetgroupcommandvalidator, IValidator<UpdateAssetGroupCommand> updateassetgroupcommandvalidator)
        : base(mediator)
        {
            _logger = logger;
            _mediator = mediator;
            _dbContext = dbContext;
            _createassetgroupcommandvalidator = createassetgroupcommandvalidator;
            _updateassetgroupcommandvalidator = updateassetgroupcommandvalidator;
        }

[HttpPost]
public async Task<IActionResult> CreateAsync(CreateAssetGroupCommand createAssetGroupCommand)
{
    
    // Validate the incoming command
    var validationResult = await _createassetgroupcommandvalidator.ValidateAsync(createAssetGroupCommand);
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
    var CreatedAssetGroupId = await _mediator.Send(createAssetGroupCommand);

    if (CreatedAssetGroupId.IsSuccess)
    {
     _logger.LogInformation($"AssetGroup {createAssetGroupCommand.Code} created successfully.");
      return Ok(new
      {
          StatusCode = StatusCodes.Status201Created,
          message =CreatedAssetGroupId.Message,
          data = CreatedAssetGroupId.Data
      });
    }
     _logger.LogWarning($"AssetGroup {createAssetGroupCommand.Code} Creation failed.");
      return BadRequest(new
        {
            StatusCode = StatusCodes.Status400BadRequest,
            message = CreatedAssetGroupId.Message
        });
  
}
[HttpPut]
public async Task<IActionResult> UpdateAsync(UpdateAssetGroupCommand updateAssetGroupCommand)
{
  
        // Validate the incoming command
        var validationResult = await _updateassetgroupcommandvalidator.ValidateAsync(updateAssetGroupCommand);
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

        var updatedassetgroup = await _mediator.Send(updateAssetGroupCommand);

        if (updatedassetgroup.IsSuccess)
        {
            _logger.LogInformation($"AssetGroup {updateAssetGroupCommand.GroupName} updated successfully.");
           return Ok(new
            {
                message = updatedassetgroup.Message,
                statusCode = StatusCodes.Status200OK
            });
        }
        _logger.LogWarning($"AssetGroup {updateAssetGroupCommand.GroupName} Update failed.");
        return NotFound(new
        {
            message =updatedassetgroup.Message,
            statusCode = StatusCodes.Status404NotFound
        });   
}

[HttpDelete("{id}")]
public async Task<IActionResult> DeleteAssetGroupAsync(int id)
{

        // Process the delete command
        var result = await _mediator.Send(new DeleteAssetGroupCommand { Id = id });

        if (result.IsSuccess) 
        {
            _logger.LogInformation($"AssetGroup {id} deleted successfully.");
             return Ok(new
            {
                message = result.Message,
                statusCode = StatusCodes.Status200OK
            });
            
        }
         _logger.LogWarning($"AssetGroup {id} Not Found or Invalid AssetGroupId.");
        return NotFound(new
        {
            message = result.Message,
            statusCode = StatusCodes.Status404NotFound
        });
   
}
        [HttpGet]
        public async Task<IActionResult> GetAllAssetGroupAsync([FromQuery] int PageNumber,[FromQuery] int PageSize,[FromQuery] string? SearchTerm = null)
        {
           var assetgroups = await Mediator.Send(
            new GetAssetGroupQuery
            {
                PageNumber = PageNumber, 
                PageSize = PageSize, 
                SearchTerm = SearchTerm
            });
      
           
            return Ok( new 
            { 
                StatusCode=StatusCodes.Status200OK, 
                data = assetgroups.Data,
                TotalCount = assetgroups.TotalCount,
                PageNumber = assetgroups.PageNumber,
                PageSize = assetgroups.PageSize
                });
        }

        [HttpGet("by-name")]
        public async Task<IActionResult> GetAssetGroup([FromQuery] string? groupname)
        {
        var assetgroups = await Mediator.Send(new GetAssetGroupAutoCompleteQuery 
        { 
                SearchPattern = groupname ?? string.Empty 
        });

        return Ok(new { StatusCode = StatusCodes.Status200OK, data = assetgroups.Data });
        }

        [HttpGet("{id}")]
        [ActionName(nameof(GetByIdAsync))]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var assetgroup = await Mediator.Send(new GetAssetGroupByIdQuery() { Id = id});
          
            if(assetgroup.IsSuccess)
            {
                
              return Ok(new { StatusCode=StatusCodes.Status200OK, data = assetgroup.Data,message = assetgroup.Message });
            }
            return NotFound( new { StatusCode=StatusCodes.Status404NotFound, message = assetgroup.Message });
           
        }

    }
}