using Core.Application.AssetMaster.AssetTransferIssue.Command.CreateAssetTransferIssue;
using Core.Application.AssetMaster.AssetTransferIssue.Command.UpdateAssetTransferIssue;
using Core.Application.AssetMaster.AssetTransferIssue.Queries.GetAllAssetTransfer;
using Core.Application.AssetMaster.AssetTransferIssue.Queries.GetAssertByCategory;
using Core.Application.AssetMaster.AssetTransferIssue.Queries.GetAssetDtlToTransfer;
using Core.Application.AssetMaster.AssetTransferIssue.Queries.GetAssetTranferedById;
using Core.Application.AssetMaster.AssetTransferIssue.Queries.GetAssetTransfered;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IAssetMaster.IAssetTransferIssue;
using Core.Domain.Entities.AssetMaster;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;



namespace FAM.API.Controllers.AssetMaster
{
    [Route("api/[controller]")]
    public class AssetTransferController : ApiControllerBase
    
    {

               private readonly IValidator<CreateAssetTransferIssueCommand> _createAssetTransferIssueCommandValidator;
               private readonly IValidator<UpdateAssetTransferIssueCommand> _UpdateAssetTransferIssueCommandValidator;
               private readonly IAssetTransferQueryRepository _assetTransferQueryRepository;

        public AssetTransferController(ISender mediator ,IValidator<CreateAssetTransferIssueCommand>  createAssetTransferIssueCommand ,IValidator<UpdateAssetTransferIssueCommand>  updateAssetTransferIssueCommand ,IAssetTransferQueryRepository assetTransferQueryRepository  )  : base(mediator)
        {
           _createAssetTransferIssueCommandValidator =  createAssetTransferIssueCommand;
           _UpdateAssetTransferIssueCommandValidator =  updateAssetTransferIssueCommand;
           _assetTransferQueryRepository = assetTransferQueryRepository;
           
        }
         
        [HttpGet("GetAllAssetTransfers")]
        public async Task<IActionResult> GetAllAsync([FromQuery] int PageNumber, [FromQuery] int PageSize,[FromQuery] DateTimeOffset? FromDate = null, [FromQuery] DateTimeOffset? ToDate = null , [FromQuery] string? SearchTerm = null )
        {
            var assetTransferList = await Mediator.Send(
                new AssetTransferQuery
                {
                    PageNumber = PageNumber,
                    PageSize = PageSize,
                    SearchTerm = SearchTerm,
                    FromDate = FromDate,
                    ToDate = ToDate
                    
                });

            return Ok(new
            {
                StatusCode = StatusCodes.Status200OK,
                Data = assetTransferList.Data,
                TotalCount = assetTransferList.TotalCount,
                PageNumber = assetTransferList.PageNumber,
                PageSize = assetTransferList.PageSize
            });
        }
        
        [HttpGet("GetAllAssetTransfersByAssetTransferId/{id}")]       
              public async Task<IActionResult> GetAllAssetTransfersAsync(int id)
              {
                  var query = new GetAllTransferQuery { AssetTransferId = id };  
                  var result = await Mediator.Send(query);
                 if (result == null) 
                  {
                      return NotFound($"Asset Transfer with ID {id} not found.");
                  }
                 return Ok(result);
              }     

        [HttpPost]
       // public async Task<ActionResult<ApiResponseDTO<AssetTransferIssueHdr>>> CreateAssetTransfer([FromBody] CreateAssetTransferIssueCommand command)
         public async Task<IActionResult> CreateAsync(CreateAssetTransferIssueCommand  command)
        { 

            var validationResult = await _createAssetTransferIssueCommandValidator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                return BadRequest(new 
                { 
                    StatusCode = StatusCodes.Status400BadRequest, 
                    Message = "Validation Failed", 
                    Errors = validationResult.Errors.Select(e => e.ErrorMessage).ToArray()
                });
            }
            if (command == null)
                return BadRequest(new ApiResponseDTO<AssetTransferIssueHdr>
                {
                    IsSuccess = false,
                    Message = "Invalid request data"
                });

            var response = await Mediator.Send(command);

            if (!response.IsSuccess)
                return BadRequest(response);

            return Ok(response);
        }

        [HttpGet("{id}")]       
              public async Task<IActionResult> GetAssetTransferByIdAsync(int id)
              {
                  var query = new GetAssetTranferedByIdQuery { AssetTransferId = id };  
                  var result = await Mediator.Send(query);
                 if (result == null) 
                  {
                      return NotFound($"Asset Transfer with ID {id} not found.");
                  }
                 return Ok(result);
              }

        [HttpPut]
        public async Task<IActionResult> UpdateAssetTransferIssue([FromBody] UpdateAssetTransferIssueCommand command)
        {

              var validationResult = await _UpdateAssetTransferIssueCommandValidator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                return BadRequest(new 
                { 
                    StatusCode = StatusCodes.Status400BadRequest, 
                    Message = "Validation Failed", 
                    Errors = validationResult.Errors.Select(e => e.ErrorMessage).ToArray()
                });
            }
           
            var result = await Mediator.Send(command);

            if (!result.IsSuccess)
            {
                return NotFound(result);
            }

             return Ok(new 
                {
                    StatusCode=StatusCodes.Status200OK,
                    message = result.Message,
                    errors = ""
                });
        }

         [HttpGet("GetAssetsByCategory/{categoryId}")]
        public async Task<IActionResult> GetAssetsByCategoryAsync(int categoryId)
        {
            var query = new GetAssetsByCategoryQuery { AssetCategoryId = categoryId };
            var result = await Mediator.Send(query);
            return Ok(result);
        }
        [HttpGet("GetAssetDetailsToTransfer/{AssetId}")]
        public async Task<IActionResult> GetAssetDetailsToTransferByIdAsync(int AssetId )
        {

            // 🔹 Check if the asset is pending or approved (with AckStatus <> 1)
        bool isRestricted = await _assetTransferQueryRepository.IsAssetPendingOrApprovedAsync(AssetId);

        if (isRestricted)
        {
            return BadRequest($"Asset ID {AssetId} is in 'Pending' or 'Approved' state with unacknowledged status.");
        }

            var query = new GetAssetDetailsToTransferQuery { AssetId = AssetId };
            var result = await Mediator.Send(query);
            return Ok(result);
            
        }      
 
    }
}