using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.AssetMaster.AssetDisposal.Command.CreateAssetDisposal;
using Core.Application.AssetMaster.AssetDisposal.Command.UpdateAssetDisposal;
using Core.Application.AssetMaster.AssetDisposal.Queries.GetAssetDisposal;
using Core.Application.AssetMaster.AssetDisposal.Queries.GetAssetDisposalById;
using Core.Application.AssetMaster.AssetDisposal.Queries.GetDisposalType;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace FAM.API.Controllers.AssetMaster
{
     [Route("api/[controller]")]
    public class AssetDisposalController : ApiControllerBase
    {
        private readonly ILogger<AssetDisposalController> _logger;
        private readonly IMediator _mediator;
        private readonly IValidator<CreateAssetDisposalCommand> _createAssetDisposalCommand;
        private readonly IValidator<UpdateAssetDisposalCommand> _updateAssetDisposalCommand;

        public AssetDisposalController(ILogger<AssetDisposalController> logger, IMediator mediator,IValidator<CreateAssetDisposalCommand> createAssetDisposalCommand,IValidator<UpdateAssetDisposalCommand> updateAssetDisposalCommand)
        : base(mediator)
        {
            _logger = logger;
            _mediator = mediator;
            _createAssetDisposalCommand=createAssetDisposalCommand;
            _updateAssetDisposalCommand=updateAssetDisposalCommand;
        }
        [HttpGet("DisposalType")]
        public async Task<IActionResult> GetDisposalTypes()
        {
            var result = await Mediator.Send(new GetDisposalTypeQuery());

            if (result == null || result.Data == null || result.Data.Count == 0)
            {
                return NotFound(new
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    message = "No Disposal Types found."
                });
            }
            return Ok(new
            {
                StatusCode = StatusCodes.Status200OK,
                message = "Disposal Types fetched successfully.",
                data = result.Data
            });
        }

        [HttpGet("{id}")]
        [ActionName(nameof(GetByIdAsync))]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var assetdisposal = await Mediator.Send(new GetAssetDisposalByIdQuery() { Id = id});
          
            if(assetdisposal.IsSuccess)
            {
                
              return Ok(new { StatusCode=StatusCodes.Status200OK, data = assetdisposal.Data,message = assetdisposal.Message });
            }
            return NotFound( new { StatusCode=StatusCodes.Status404NotFound, message = assetdisposal.Message });
           
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAssetDisposalAsync([FromQuery] int PageNumber,[FromQuery] int PageSize,[FromQuery] string? SearchTerm = null)
        {
           var assetdisposal = await Mediator.Send(
            new GetAssetDisposalQuery
            {
                PageNumber = PageNumber, 
                PageSize = PageSize, 
                SearchTerm = SearchTerm
            });
      
           
            return Ok( new 
            { 
                StatusCode=StatusCodes.Status200OK, 
                data = assetdisposal.Data,
                TotalCount = assetdisposal.TotalCount,
                PageNumber = assetdisposal.PageNumber,
                PageSize = assetdisposal.PageSize
            });
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync(CreateAssetDisposalCommand createAssetDisposalCommand)
        {
            
            // Validate the incoming command
            var validationResult = await _createAssetDisposalCommand.ValidateAsync(createAssetDisposalCommand);
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
            var CreatedAssetDisposalId = await _mediator.Send(createAssetDisposalCommand);

            if (CreatedAssetDisposalId.IsSuccess)
            {
          
            return Ok(new
            {
                StatusCode = StatusCodes.Status201Created,
                message =CreatedAssetDisposalId.Message,
                data = CreatedAssetDisposalId.Data
            });
            }
            return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    message = CreatedAssetDisposalId.Message
                });
        
        }

        [HttpPut]
        public async Task<IActionResult> UpdateAsync(UpdateAssetDisposalCommand updateAssetDisposalCommand )
        {
        
                // Validate the incoming command
                var validationResult = await _updateAssetDisposalCommand.ValidateAsync(updateAssetDisposalCommand);
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

                var updatedassetdisposal = await _mediator.Send(updateAssetDisposalCommand);

                if (updatedassetdisposal.IsSuccess)
                {
                    
                return Ok(new
                    {
                        message = updatedassetdisposal.Message,
                        statusCode = StatusCodes.Status200OK
                    });
                }
               
                return NotFound(new
                {
                    message =updatedassetdisposal.Message,
                    statusCode = StatusCodes.Status404NotFound
                });   
        }

        
    }
}