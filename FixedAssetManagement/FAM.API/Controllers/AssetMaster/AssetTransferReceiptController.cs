using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.AssetMaster.AssetTranferIssueApproval.Queries.GetAssetTransferIssueById;
using Core.Application.AssetMaster.AssetTransferReceipt.Command.CreateAssetTransferReceipt;
using Core.Application.AssetMaster.AssetTransferReceipt.Queries.GetAssetReceiptDetails;
using Core.Application.AssetMaster.AssetTransferReceipt.Queries.GetAssetReceiptDetailsById;
using Core.Application.AssetMaster.AssetTransferReceipt.Queries.GetAssetReceiptPending;
using Core.Application.Common.HttpResponse;
using Core.Domain.Entities.AssetMaster;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace FAM.API.Controllers.AssetMaster
{
    [Route("api/[controller]")]
    public class AssetTransferReceiptController : ApiControllerBase
    {
        private readonly ILogger<AssetTransferReceiptController> _logger;
         private readonly IMediator _mediator;
        private readonly IValidator<CreateAssetTransferReceiptCommand> _createAssetTransferReceiptCommandValidator;

        public AssetTransferReceiptController(ILogger<AssetTransferReceiptController> logger,IMediator mediator,IValidator<CreateAssetTransferReceiptCommand> createAssetTransferReceiptCommandValidator)
        :base(mediator)
        {
            _logger = logger;
            _mediator=mediator;
            _createAssetTransferReceiptCommandValidator=createAssetTransferReceiptCommandValidator;
        }

        [HttpGet("GetAssetTransferReceiptPending")]
        public async Task<IActionResult> GetAssetTransferReceiptPendingAsync([FromQuery] int PageNumber,[FromQuery] int PageSize,[FromQuery] string? AssetTransferId = null,[FromQuery] DateTimeOffset? FromDate = null,
        [FromQuery] DateTimeOffset? ToDate = null)
        {
           var assetamc = await Mediator.Send(
            new GetAssetReceiptPendingQuery
            {
                PageNumber = PageNumber, 
                PageSize = PageSize, 
                SearchTerm = AssetTransferId,
                FromDate = FromDate,
                ToDate = ToDate
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

        [HttpGet]
        public async Task<IActionResult> GetAssetTransferReceiptDetails([FromQuery] int PageNumber,[FromQuery] int PageSize,[FromQuery] string? AssetReceiptId = null,[FromQuery] DateTimeOffset? FromDate = null,
        [FromQuery] DateTimeOffset? ToDate = null)
        {
           var assetamc = await Mediator.Send(
            new GetAssetReceiptDetailsQuery
            {
                PageNumber = PageNumber, 
                PageSize = PageSize, 
                SearchTerm = AssetReceiptId,
                FromDate = FromDate,
                ToDate = ToDate
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
        public async Task<IActionResult> CreateAsync(CreateAssetTransferReceiptCommand  createAssetTransferReceiptCommand)
        {
             // Validate the incoming command
            var validationResult = await _createAssetTransferReceiptCommandValidator.ValidateAsync(createAssetTransferReceiptCommand);
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
            var CreatedAssetReceiptId = await _mediator.Send(createAssetTransferReceiptCommand);

            if (CreatedAssetReceiptId.IsSuccess)
            {
          
            return Ok(new
            {
                StatusCode = StatusCodes.Status201Created,
                message =CreatedAssetReceiptId.Message,
                data = CreatedAssetReceiptId.Data
            });
            }
            return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    message = CreatedAssetReceiptId.Message
                });
        }


        [HttpGet("{id}")]
        [ActionName(nameof(GetByAssetTransferReceiptIdAsync))]
        public async Task<IActionResult> GetByAssetTransferReceiptIdAsync(int id)
        {
            var assetreceipt = await Mediator.Send(new GetAssetReceiptDetailsByIdQuery() { AssetReceiptId = id});
          
            if(assetreceipt.IsSuccess)
            {
                
              return Ok(new { StatusCode=StatusCodes.Status200OK, data = assetreceipt.Data,message = assetreceipt.Message });
            }
            return NotFound( new { StatusCode=StatusCodes.Status404NotFound, message = assetreceipt.Message });
           
        }

      
    }
}