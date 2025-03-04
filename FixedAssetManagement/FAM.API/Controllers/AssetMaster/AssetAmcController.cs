using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.AssetMaster.AssetAmc.Command.CreateAssetAmc;
using Core.Application.AssetMaster.AssetAmc.Command.DeleteAssetAmc;
using Core.Application.AssetMaster.AssetAmc.Command.UpdateAssetAmc;
using Core.Application.AssetMaster.AssetAmc.Queries.GetAssetAmc;
using Core.Application.AssetMaster.AssetAmc.Queries.GetAssetAmcById;
using Core.Application.AssetMaster.AssetAmc.Queries.GetCoverageScope;
using Core.Application.AssetMaster.AssetAmc.Queries.GetExistingVendorDetails;
using Core.Application.AssetMaster.AssetAmc.Queries.GetRenewStatus;
using FAM.Infrastructure.Migrations;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace FAM.API.Controllers.AssetMaster
{
   [Route("api/[controller]")]
    public class AssetAmcController : ApiControllerBase
    {
        private readonly ILogger<AssetAmcController> _logger;
        private readonly IMediator _mediator;
        private readonly IValidator<CreateAssetAmcCommand> _createAssetAmcCommand;
        private readonly IValidator<UpdateAssetAmcCommand> _updateAmcCommand;

        public AssetAmcController(ILogger<AssetAmcController> logger, IMediator mediator,IValidator<CreateAssetAmcCommand> createAssetAmcCommand,IValidator<UpdateAssetAmcCommand> updateAmcCommand)
        : base(mediator)
        {
            _logger = logger;
            _mediator = mediator;
            _createAssetAmcCommand=createAssetAmcCommand;
            _updateAmcCommand=updateAmcCommand;
        }

        [HttpGet("GetExistingVendor/{oldUnitId}/{VendorCode}")]
        public async Task<IActionResult> GetExistingVendor(string oldUnitId, string VendorCode)
        {
            if (oldUnitId == null)
            {
                return BadRequest(new { StatusCode = StatusCodes.Status400BadRequest, Message = "Invalid OldUnitId" });
            }
            if (VendorCode =="0")
            {
                return BadRequest(new { StatusCode = StatusCodes.Status400BadRequest, Message = "Invalid VendorCode" });
            }

            var result = await _mediator.Send(new GetExistingVendorDetailsQuery { OldUnitCode = oldUnitId,VendorCode = VendorCode });

            if (result == null || !result.IsSuccess || result.Data == null)
            {
                return NotFound(new { StatusCode = StatusCodes.Status404NotFound, Message = "No Vendor details found" });
            }

            return Ok(new { StatusCode = StatusCodes.Status200OK, Data = result.Data });
        }

         [HttpGet("RenewStatus")]
        public async Task<IActionResult> GetRenewStatusTypes()
        {
            var result = await Mediator.Send(new GetRenewStatusQuery());

            if (result == null || result.Data == null || result.Data.Count == 0)
            {
                return NotFound(new
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    message = "No RenewStatus Types found."
                });
            }
            return Ok(new
            {
                StatusCode = StatusCodes.Status200OK,
                message = "RenewStatus fetched successfully.",
                data = result.Data
            });
        }

         [HttpGet("CoverageScope")]
        public async Task<IActionResult> GetCoverageScopeTypes()
        {
            var result = await Mediator.Send(new GetCoverageScopeQuery());

            if (result == null || result.Data == null || result.Data.Count == 0)
            {
                return NotFound(new
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    message = "No CoverageScope found."
                });
            }
            return Ok(new
            {
                StatusCode = StatusCodes.Status200OK,
                message = "CoverageScope fetched successfully.",
                data = result.Data
            });
        }

        [HttpGet("{id}")]
        [ActionName(nameof(GetByIdAsync))]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var assetamc = await Mediator.Send(new GetAssetAmcByIdQuery() { Id = id});
          
            if(assetamc.IsSuccess)
            {
                
              return Ok(new { StatusCode=StatusCodes.Status200OK, data = assetamc.Data,message = assetamc.Message });
            }
            return NotFound( new { StatusCode=StatusCodes.Status404NotFound, message = assetamc.Message });
           
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAssetAmcAsync([FromQuery] int PageNumber,[FromQuery] int PageSize,[FromQuery] string? SearchTerm = null)
        {
           var assetamc = await Mediator.Send(
            new GetAssetAmcQuery
            {
                PageNumber = PageNumber, 
                PageSize = PageSize, 
                SearchTerm = SearchTerm
            });
      
           
            return Ok( new 
            { 
                StatusCode=StatusCodes.Status200OK, 
                data = assetamc.Data,
                TotalCount = assetamc.TotalCount,
                PageNumber = assetamc.PageNumber,
                PageSize = assetamc.PageSize
            });
        }

         [HttpPost]
        public async Task<IActionResult> CreateAsync(CreateAssetAmcCommand createAssetAmcCommand)
        {
            
            // Validate the incoming command
            var validationResult = await _createAssetAmcCommand.ValidateAsync(createAssetAmcCommand);
            
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
            var CreatedAssetamcid = await _mediator.Send(createAssetAmcCommand);

            if (CreatedAssetamcid.IsSuccess)
            {
          
            return Ok(new
            {
                StatusCode = StatusCodes.Status201Created,
                message =CreatedAssetamcid.Message,
                data = CreatedAssetamcid.Data
            });
            }
            return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    message = CreatedAssetamcid.Message
                });
        
        }

        [HttpPut]
        public async Task<IActionResult> UpdateAsync(UpdateAssetAmcCommand updateAssetAmcCommand )
        {
        
                // Validate the incoming command
                var validationResult = await _updateAmcCommand.ValidateAsync(updateAssetAmcCommand);
       
                if (!validationResult.IsValid)
                {
                
                    return BadRequest(new
                    {
                        StatusCode = StatusCodes.Status400BadRequest,
                        message = "Validation failed",
                        errors = validationResult.Errors.Select(e => e.ErrorMessage)
                    });
                }

                var updatedassetamc = await _mediator.Send(updateAssetAmcCommand);

                if (updatedassetamc.IsSuccess)
                {
                    
                return Ok(new
                    {
                        message = updatedassetamc.Message,
                        statusCode = StatusCodes.Status200OK
                    });
                }
               
                return NotFound(new
                {
                    message =updatedassetamc.Message,
                    statusCode = StatusCodes.Status404NotFound
                });   
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAssetAmcAsync(int id)
        {

                // Process the delete command
                var result = await _mediator.Send(new DeleteAssetAmcCommand { Id = id });

                if (result.IsSuccess) 
                {
                    
                    return Ok(new
                    {
                        message = result.Message,
                        statusCode = StatusCodes.Status200OK
                    });
                    
                }
              
                return NotFound(new
                {
                    message = result.Message,
                    statusCode = StatusCodes.Status404NotFound
                });
        
        }

        
    }
}