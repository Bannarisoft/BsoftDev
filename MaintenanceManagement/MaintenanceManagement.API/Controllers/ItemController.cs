using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Item.ItemGroup.Queries;
using Core.Application.Item.ItemMaster.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MaintenanceManagement.API.Controllers
{
     [Route("api/[controller]")]
    public class ItemController : ApiControllerBase
    {
        private readonly ILogger<ItemController> _logger;
         private readonly IMediator _mediator;

        public ItemController(ILogger<ItemController> logger, IMediator mediator)
        : base(mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }
         [HttpGet("GetGroupCode/{oldUnitId}")]
        public async Task<IActionResult> GetGroupCode(string oldUnitId)
        {
            if (oldUnitId == null)
            {
                return BadRequest(new { StatusCode = StatusCodes.Status400BadRequest, Message = "Invalid OldUnitId" });
            }
            var result = await _mediator.Send(new GetItemGroupQuery { OldUnitId = oldUnitId });

            if (result == null || !result.IsSuccess || result.Data == null)
            {
                return NotFound(new { StatusCode = StatusCodes.Status404NotFound, Message = "No ItemGroupCode details found" });
            }

            return Ok(new { StatusCode = StatusCodes.Status200OK, Data = result.Data });
        }
        [HttpGet("GetItemMasters/{oldUnitId}/{grpcode}")]
        public async Task<IActionResult> GetItemMasters(
        string oldUnitId,
        string grpcode,
        [FromQuery] string? itemCode = null,
        [FromQuery] string? itemName = null)
        {
        if (string.IsNullOrWhiteSpace(oldUnitId) || string.IsNullOrWhiteSpace(grpcode))
        {
            return BadRequest(new 
            { 
                StatusCode = StatusCodes.Status400BadRequest, 
                Message = "OldUnitId and Grpcode are required." 
            });
        }

        var result = await _mediator.Send(new GetItemMasterQuery 
        { 
            OldUnitId = oldUnitId, 
            Grpcode = grpcode, 
            ItemCode = itemCode, 
            ItemName = itemName 
        });

        if (result == null || !result.IsSuccess || result.Data == null || result.Data.Count == 0)
        {
            return NotFound(new 
            { 
                StatusCode = StatusCodes.Status404NotFound, 
                Message = "No ItemMasters found for the provided criteria." 
            });
        }

        return Ok(new 
        { 
            StatusCode = StatusCodes.Status200OK, 
            Data = result.Data 
        });
    }

        
    }
}