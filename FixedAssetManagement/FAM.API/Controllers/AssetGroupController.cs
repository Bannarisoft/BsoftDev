using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.AssetGroup.Command.CreateAssetGroup;
using FAM.Infrastructure.Data;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace FAM.API.Controllers
{
    [Route("[controller]")]
     [ApiController]
    public class AssetGroupController : ApiControllerBase
    {
        private readonly ILogger<AssetGroupController> _logger;
        
        private readonly ApplicationDbContext _dbContext;
        private readonly IMediator _mediator;

        public AssetGroupController(ILogger<AssetGroupController> logger, IMediator mediator, ApplicationDbContext dbContext)
        : base(mediator)
        {
            _logger = logger;
            _mediator = mediator;
            _dbContext = dbContext;
        }

        [HttpPost]
public async Task<IActionResult> CreateAsync(CreateAssetGroupCommand createAssetGroupCommand)
{
    
    // Validate the incoming command
    // var validationResult = await _createEntityCommandValidator.ValidateAsync(createEntityCommand);
    //  _logger.LogWarning($"Validation failed: {string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage))}");
    // if (!validationResult.IsValid)
    // {
        
    //     return BadRequest(new
    //     {
    //         StatusCode = StatusCodes.Status400BadRequest,
    //         message = "Validation failed",
    //         errors = validationResult.Errors.Select(e => e.ErrorMessage)
    //     });
    // }

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

    }
}