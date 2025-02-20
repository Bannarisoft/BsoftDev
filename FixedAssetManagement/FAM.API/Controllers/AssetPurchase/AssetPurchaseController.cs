using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.AssetCategories.Queries.GetAssetCategoriesAutoComplete;
using Core.Application.AssetCategories.Queries.GetAssetCategoriesById;
using Core.Application.AssetMaster.AssetPurchase.Queries.GetAssetGRN;
using Core.Application.AssetMaster.AssetPurchase.Queries.GetAssetGrnDetails;
using Core.Application.AssetMaster.AssetPurchase.Queries.GetAssetGRNItem;
using Core.Application.AssetMaster.AssetPurchase.Queries.GetAssetSourceAutoComplete;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace FAM.API.Controllers.AssetPurchase
{
   [Route("api/[controller]")]
    public class AssetPurchaseController : ApiControllerBase
    {
        private readonly ILogger<AssetPurchaseController> _logger;
        private readonly IMediator _mediator;

        public AssetPurchaseController(ILogger<AssetPurchaseController> logger, IMediator mediator)
         : base(mediator)
        {
            _logger = logger;
           _mediator = mediator;
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
    public async Task<IActionResult> GetGrnNo(int oldUnitId)
    {
        if (oldUnitId <= 0)
        {
            return BadRequest(new { StatusCode = StatusCodes.Status400BadRequest, Message = "Invalid OldUnitId" });
        }

        var result = await _mediator.Send(new GetAssetGrnQuery { OldUnitId = oldUnitId });

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


        
    }
}