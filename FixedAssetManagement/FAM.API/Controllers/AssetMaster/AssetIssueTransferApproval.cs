using Core.Application.AssetMaster.AssetAdditionalCost.Queries.GetAssetAdditionalCostById;
using Core.Application.AssetMaster.AssetTranferIssueApproval.Commands.UpdateAssetTranferIssueApproval;
using Core.Application.AssetMaster.AssetTranferIssueApproval.Queries.GetAssetTransferIssueApproval;
using Core.Application.AssetMaster.AssetTranferIssueApproval.Queries.GetAssetTransferIssueById;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FAM.API.Controllers.AssetMaster
{
    [Route("api/[controller]")]
    public class AssetIssueTransferApproval : ApiControllerBase
    {
        private readonly ILogger<AssetIssueTransferApproval> _logger;
        private readonly IMediator _mediator;
        private readonly IValidator<UpdateAssetTranferIssueApprovalCommand> _updateAssetTranferIssueApprovalCommand;

        public AssetIssueTransferApproval(ILogger<AssetIssueTransferApproval> logger, IMediator mediator, IValidator<UpdateAssetTranferIssueApprovalCommand> updateAssetTranferIssueApprovalCommand)
        : base(mediator)
        {
            _logger = logger;
            _mediator = mediator;
            _updateAssetTranferIssueApprovalCommand = updateAssetTranferIssueApprovalCommand;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllAssetIssueTransferPendingAsync([FromQuery] int PageNumber,[FromQuery] int PageSize,[FromQuery] string? TransferType = null,[FromQuery] DateTimeOffset? FromDate = null,
        [FromQuery] DateTimeOffset? ToDate = null)
        {
           var assetamc = await Mediator.Send(
            new GetAssetTranferIssueApprovalQuery
            {
                PageNumber = PageNumber, 
                PageSize = PageSize, 
                SearchTerm = TransferType,
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

        [HttpGet("{id}")]
        [ActionName(nameof(GetByAssetTransferIdAsync))]
        public async Task<IActionResult> GetByAssetTransferIdAsync(int id)
        {
            var assettransfer = await Mediator.Send(new GetAssetTransferIssueByIdQuery() { Id = id});
          
            if(assettransfer.IsSuccess)
            {
                
              return Ok(new { StatusCode=StatusCodes.Status200OK, data = assettransfer.Data,message = assettransfer.Message });
            }
            return NotFound( new { StatusCode=StatusCodes.Status404NotFound, message = assettransfer.Message });
           
        }
            [HttpPost("update-status")]
            public async Task<IActionResult> UpdateStatus([FromBody] UpdateAssetTranferIssueApprovalCommand command)
            {
                 // Validate the incoming command
                var validationResult = await _updateAssetTranferIssueApprovalCommand.ValidateAsync(command);
       
                if (!validationResult.IsValid)
                {
                    return BadRequest(new
                    {
                        StatusCode = StatusCodes.Status400BadRequest,
                        message = "Validation failed",
                        errors = validationResult.Errors.Select(e => e.ErrorMessage)
                    });
                }
                var response = await _mediator.Send(command);

                if (!response.IsSuccess)
                {
                    return BadRequest(new
                    {
                        message = response.Message,
                        statusCode = StatusCodes.Status400BadRequest
                    });
                }

                return Ok(new
                {
                    message = response.Message,
                    statusCode = StatusCodes.Status200OK,
                    data = response.Data
                });
            }
         
    }
}