using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.AssetMaster.AssetAdditionalCost.Commands.CreateAssetAdditionalCost;
using Core.Application.AssetMaster.AssetAdditionalCost.Commands.UpdateAssetAdditionalCost;
using Core.Application.AssetMaster.AssetAdditionalCost.Queries.GetAssetAdditionalCost;
using Core.Application.AssetMaster.AssetAdditionalCost.Queries.GetAssetAdditionalCostById;
using Core.Application.AssetMaster.AssetAdditionalCost.Queries.GetCostTypeQuery;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace FAM.API.Controllers.AssetMaster
{
    [Route("api/[controller]")]
    public class AssetAdditionalCostController : ApiControllerBase
    {
        private readonly ILogger<AssetAdditionalCostController> _logger;
        private readonly IMediator _mediator;
        private readonly IValidator<CreateAssetAdditionalCostCommand> _createAssetAdditionalCostCommand;
        private readonly IValidator<UpdateAssetAdditionalCostCommand> _updateAssetAdditionalCostCommand;


        public AssetAdditionalCostController(ILogger<AssetAdditionalCostController> logger, IMediator mediator,IValidator<CreateAssetAdditionalCostCommand> createAssetAdditionalCostCommand,IValidator<UpdateAssetAdditionalCostCommand> updateAssetAdditionalCostCommand)
        :base(mediator)
        {
            _logger = logger;
            _mediator = mediator;
            _createAssetAdditionalCostCommand=createAssetAdditionalCostCommand;
            _updateAssetAdditionalCostCommand=updateAssetAdditionalCostCommand;
        }

        [HttpGet("CostType")]
        public async Task<IActionResult> GetCostTypes()
        {
            var result = await Mediator.Send(new GetCostTypeQuery());

            if (result == null || result.Data == null || result.Data.Count == 0)
            {
                return NotFound(new
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    message = "No Cost Types found."
                });
            }
            return Ok(new
            {
                StatusCode = StatusCodes.Status200OK,
                message = "Cost Types fetched successfully.",
                data = result.Data
            });
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync(CreateAssetAdditionalCostCommand createAssetAdditionalCostCommand)
        {
            
            // Validate the incoming command
            var validationResult = await _createAssetAdditionalCostCommand.ValidateAsync(createAssetAdditionalCostCommand);
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
            var CreatedAssetGroupId = await _mediator.Send(createAssetAdditionalCostCommand);

            if (CreatedAssetGroupId.IsSuccess)
            {
          
            return Ok(new
            {
                StatusCode = StatusCodes.Status201Created,
                message =CreatedAssetGroupId.Message,
                data = CreatedAssetGroupId.Data
            });
            }
            return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    message = CreatedAssetGroupId.Message
                });
        
        }

        [HttpPut]
        public async Task<IActionResult> UpdateAsync(UpdateAssetAdditionalCostCommand updateAssetAdditionalCostCommand )
        {
        
                // Validate the incoming command
                var validationResult = await _updateAssetAdditionalCostCommand.ValidateAsync(updateAssetAdditionalCostCommand);
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

                var updatedassetgroup = await _mediator.Send(updateAssetAdditionalCostCommand);

                if (updatedassetgroup.IsSuccess)
                {
                    
                return Ok(new
                    {
                        message = updatedassetgroup.Message,
                        statusCode = StatusCodes.Status200OK
                    });
                }
               
                return NotFound(new
                {
                    message =updatedassetgroup.Message,
                    statusCode = StatusCodes.Status404NotFound
                });   
        }

        [HttpGet("{id}")]
        [ActionName(nameof(GetByIdAsync))]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var assetadditionalcost = await Mediator.Send(new GetAssetAdditionalCostByIdQuery() { Id = id});
          
            if(assetadditionalcost.IsSuccess)
            {
                
              return Ok(new { StatusCode=StatusCodes.Status200OK, data = assetadditionalcost.Data,message = assetadditionalcost.Message });
            }
            return NotFound( new { StatusCode=StatusCodes.Status404NotFound, message = assetadditionalcost.Message });
           
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAssetAdditionalCostAsync([FromQuery] int PageNumber,[FromQuery] int PageSize,[FromQuery] string? SearchTerm = null)
        {
           var assetadditionalcost = await Mediator.Send(
            new GetAssetAdditionalCostQuery
            {
                PageNumber = PageNumber, 
                PageSize = PageSize, 
                SearchTerm = SearchTerm
            });
      
           
            return Ok( new 
            { 
                StatusCode=StatusCodes.Status200OK, 
                data = assetadditionalcost.Data,
                TotalCount = assetadditionalcost.TotalCount,
                PageNumber = assetadditionalcost.PageNumber,
                PageSize = assetadditionalcost.PageSize
            });
        }



      
    }
}