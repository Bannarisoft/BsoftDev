using Core.Application.AssetMaster.AssetPurchase.Commands.CreateAssetPurchaseDetails;
using Core.Application.AssetMaster.AssetPurchase.Commands.UpdateAssetPurchaseDetails;
using Core.Application.AssetMaster.AssetPurchase.Queries.GetAssetGRN;
using Core.Application.AssetMaster.AssetPurchase.Queries.GetAssetGrnDetails;
using Core.Application.AssetMaster.AssetPurchase.Queries.GetAssetGRNItem;
using Core.Application.AssetMaster.AssetPurchase.Queries.GetAssetPurchase;
using Core.Application.AssetMaster.AssetPurchase.Queries.GetAssetPurchaseById;
using Core.Application.AssetMaster.AssetPurchase.Queries.GetAssetSourceAutoComplete;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;


namespace FAM.API.Controllers.AssetPurchase
{
   [Route("api/[controller]")]
    public class AssetPurchaseController : ApiControllerBase
    {
        private readonly ILogger<AssetPurchaseController> _logger;
        private readonly IMediator _mediator;
        private readonly IValidator<CreateAssetPurchaseDetailCommand> _createassetpurchasedetailcommandvalidator; 
        private readonly IValidator<UpdateAssetPurchaseDetailCommand> _updateassetpurchasedetailcommandvalidator; 

        public AssetPurchaseController(ILogger<AssetPurchaseController> logger, IMediator mediator, IValidator<CreateAssetPurchaseDetailCommand> createassetpurchasedetailcommandvalidator, IValidator<UpdateAssetPurchaseDetailCommand> updateassetpurchasedetailcommandvalidator)
         : base(mediator)
        {
            _logger = logger;
           _mediator = mediator;
            _createassetpurchasedetailcommandvalidator = createassetpurchasedetailcommandvalidator;
            _updateassetpurchasedetailcommandvalidator = updateassetpurchasedetailcommandvalidator;
        }

         [HttpGet("AssetSource/by-name")]
        public async Task<IActionResult> GetAssetSource([FromQuery] string? SourceName)
        {
        var assetsource = await Mediator.Send(new GetAssetSourceAutoCompleteQuery 
        { 
                SearchPattern = SourceName ?? string.Empty 
        });

        return Ok(new { StatusCode = StatusCodes.Status200OK, data = assetsource.Data });
        }
        [HttpGet("{userName}")]
        public async Task<IActionResult> GetAssetUnitByUser(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                return BadRequest(new 
                { 
                    StatusCode = StatusCodes.Status400BadRequest, 
                    Message = "Username is required."
                });
            }

            var assetUnits = await _mediator.Send(new GetAssetUnitAutoCompleteQuery 
            { 
                Username = userName
            });

            return Ok(new 
            { 
                StatusCode = StatusCodes.Status200OK, 
                Data = assetUnits.Data 
            });
        }


        [HttpGet("GetGrnNo/{oldUnitId}")]
        public async Task<IActionResult> GetGrnNo(int oldUnitId, [FromQuery] string? searchGrnNo)
        {
            if (oldUnitId <= 0)
            {
                return BadRequest(new { StatusCode = StatusCodes.Status400BadRequest, Message = "Invalid OldUnitId" });
            }

            var result = await _mediator.Send(new GetAssetGrnQuery { OldUnitId = oldUnitId, SearchGrnNo = searchGrnNo });

            if (result == null || !result.IsSuccess || result.Data == null)
            {
                return NotFound(new { StatusCode = StatusCodes.Status404NotFound, Message = "No GRN details found" });
            }

            return Ok(new { StatusCode = StatusCodes.Status200OK, Data = result.Data });
        }
            [HttpGet("GetGrnItems/{oldUnitId}/{grnNo}")]
            public async Task<IActionResult> GetGrnItems(int oldUnitId,  int grnNo)
            {
                var query = new GetAssetGrnItemQuery { OldUnitId = oldUnitId, GrnNo = grnNo };
                var result = await _mediator.Send(query);

                if (!result.IsSuccess)
                    return BadRequest(result);

                return Ok(result);
            }

            [HttpGet("GetGrnDetails/{oldUnitId}/{grnNo}/{grnSerialNo}")]
            public async Task<IActionResult> GetGrnDetails(int oldUnitId,int grnNo, int grnSerialNo)
            {
                var query = new GetAssetDetailsQuery { OldUnitId = oldUnitId, GrnNo = grnNo, GrnSerialNo = grnSerialNo };
                var result = await _mediator.Send(query);

                if (!result.IsSuccess)
                    return BadRequest(result);

                return Ok(result);
            }

            [HttpPost]
            public async Task<IActionResult> CreateAsync(CreateAssetPurchaseDetailCommand createAssetPurchaseDetailCommand)
            {
                
                // Validate the incoming command
                var validationResult = await _createassetpurchasedetailcommandvalidator.ValidateAsync(createAssetPurchaseDetailCommand);
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
                var CreatedAssetPurchaseDetailId = await _mediator.Send(createAssetPurchaseDetailCommand);

                if (CreatedAssetPurchaseDetailId.IsSuccess)
                {
                
                return Ok(new
                {
                    StatusCode = StatusCodes.Status201Created,
                    message =CreatedAssetPurchaseDetailId.Message,
                    data = CreatedAssetPurchaseDetailId.Data
                });
                }
                
                return BadRequest(new
                    {
                        StatusCode = StatusCodes.Status400BadRequest,
                        message = CreatedAssetPurchaseDetailId.Message
                    });
            
            }

            [HttpPut]
            public async Task<IActionResult> UpdateAsync(UpdateAssetPurchaseDetailCommand updateAssetPurchaseDetailCommand)
            {
            
                // Validate the incoming command
                    var validationResult = await _updateassetpurchasedetailcommandvalidator.ValidateAsync(updateAssetPurchaseDetailCommand);
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

                    var updatedassetpurchasedetail = await _mediator.Send(updateAssetPurchaseDetailCommand);

                    if (updatedassetpurchasedetail.IsSuccess)
                    {
                    
                    return Ok(new
                        {
                            message = updatedassetpurchasedetail.Message,
                            statusCode = StatusCodes.Status200OK
                        });
                    }
            
                    return NotFound(new
                    {
                        message =updatedassetpurchasedetail.Message,
                        statusCode = StatusCodes.Status404NotFound
                    });   
            }

        [HttpGet("AssetPurchase/{id}")]
        [ActionName(nameof(GetByIdAsync))]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var assetpurchase = await Mediator.Send(new GetAssetPurchaseByIdQuery() { Id = id});
          
            if(assetpurchase.IsSuccess)
            {
                
              return Ok(new { StatusCode=StatusCodes.Status200OK, data = assetpurchase.Data,message = assetpurchase.Message });
            }
            return NotFound( new { StatusCode=StatusCodes.Status404NotFound, message = assetpurchase.Message });
           
        }

         [HttpGet]
        public async Task<IActionResult> GetAllAssetPurchaseDetails([FromQuery] int PageNumber,[FromQuery] int PageSize,[FromQuery] string? SearchTerm = null)
        {
           var assetpurchase = await Mediator.Send(
            new GetAssetPurchaseQuery
            {
                PageNumber = PageNumber, 
                PageSize = PageSize, 
                SearchTerm = SearchTerm
            });
      
           
            return Ok( new 
            { 
                StatusCode=StatusCodes.Status200OK, 
                data = assetpurchase.Data,
                TotalCount = assetpurchase.TotalCount,
                PageNumber = assetpurchase.PageNumber,
                PageSize = assetpurchase.PageSize
                });
        }        
    }
}