using Core.Application.AssetMaster.AssetTransferIssue.Command.CreateAssetTransferIssue;
using Core.Application.AssetMaster.AssetTransferIssue.Command.UpdateAssetTransferIssue;
using Core.Application.AssetMaster.AssetTransferIssue.Queries.GetAssertByCategory;
using Core.Application.AssetMaster.AssetTransferIssue.Queries.GetAssetTranferedById;
using Core.Application.AssetMaster.AssetTransferIssue.Queries.GetAssetTransfered;
using Core.Application.Common.HttpResponse;
using Core.Domain.Entities.AssetMaster;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;



namespace FAM.API.Controllers.AssetMaster
{
    [Route("[controller]")]
    public class AssetTransferController : ApiControllerBase
    
    {

               private readonly IValidator<CreateAssetTransferIssueCommand> _createAssetTransferIssueCommandValidator;
               private readonly IValidator<UpdateAssetTransferIssueCommand> _UpdateAssetTransferIssueCommandValidator;

        public AssetTransferController(ISender mediator ,IValidator<CreateAssetTransferIssueCommand>  createAssetTransferIssueCommand ,IValidator<UpdateAssetTransferIssueCommand>  updateAssetTransferIssueCommand  )  : base(mediator)
        {
           _createAssetTransferIssueCommandValidator =  createAssetTransferIssueCommand;
           _UpdateAssetTransferIssueCommandValidator =  updateAssetTransferIssueCommand;
           
        }
         
        [HttpGet]
        public async Task<IActionResult> GetAllAsync([FromQuery] int PageNumber, [FromQuery] int PageSize, [FromQuery] string? SearchTerm = null)
        {
            var assetInsurances = await Mediator.Send(
                new AssetTransferQuery
                {
                    PageNumber = PageNumber,
                    PageSize = PageSize,
                    SearchTerm = SearchTerm
                });

            return Ok(new
            {
                StatusCode = StatusCodes.Status200OK,
                Data = assetInsurances.Data,
                TotalCount = assetInsurances.TotalCount,
                PageNumber = assetInsurances.PageNumber,
                PageSize = assetInsurances.PageSize
            });
        }

        [HttpPost("create")]
       // public async Task<ActionResult<ApiResponseDTO<AssetTransferIssueHdr>>> CreateAssetTransfer([FromBody] CreateAssetTransferIssueCommand command)
         public async Task<IActionResult> CreateAsync(CreateAssetTransferIssueCommand  command)
        {
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

       [HttpPut("Put")]
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
        

    }
}