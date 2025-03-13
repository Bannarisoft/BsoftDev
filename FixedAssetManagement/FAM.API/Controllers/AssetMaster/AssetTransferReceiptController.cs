using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.AssetMaster.AssetTranferIssueApproval.Queries.GetAssetTransferIssueById;
using Core.Application.AssetMaster.AssetTransferReceipt.Command.CreateAssetTransferReceipt;
using Core.Application.AssetMaster.AssetTransferReceipt.Queries.GetAssetReceiptPending;
using Core.Application.Common.HttpResponse;
using Core.Domain.Entities.AssetMaster;
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

        public AssetTransferReceiptController(ILogger<AssetTransferReceiptController> logger,IMediator mediator)
        :base(mediator)
        {
            _logger = logger;
            _mediator=mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAssetReceiptPendingAsync([FromQuery] int PageNumber,[FromQuery] int PageSize,[FromQuery] string? AssetTransferId = null,[FromQuery] DateTimeOffset? FromDate = null,
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

        [HttpPost]
        public async Task<IActionResult> CreateAsync(CreateAssetTransferReceiptCommand  createAssetTransferReceiptCommand)
        {
            if (createAssetTransferReceiptCommand == null)
                return BadRequest(new ApiResponseDTO<AssetTransferReceiptHdr>
                {
                    IsSuccess = false,
                    Message = "Invalid request data"
                });
            var response = await Mediator.Send(createAssetTransferReceiptCommand);
            if (!response.IsSuccess)
                return BadRequest(response);
            return Ok(response);
        }

      
    }
}