using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.StockLedger.Queries.GetCurrentAllStockItems;
using Core.Application.StockLedger.Queries.GetCurrentStock;
using Core.Application.StockLedger.Queries.GetCurrentStockItemsById;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MaintenanceManagement.API.Controllers
{
    [Route("api/[controller]")]
    public class StockLedgerController : ApiControllerBase
    {
         private readonly ILogger<StockLedgerController> _logger;
         private readonly IMediator _mediator;


        public StockLedgerController(ILogger<StockLedgerController> logger, IMediator mediator)
        : base(mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }
        [HttpGet("current-stock")]
        [ActionName(nameof(GetSubStoresCurrentStock))]
        public async Task<IActionResult> GetSubStoresCurrentStock([FromQuery] string oldUnitcode, [FromQuery] string itemcode)
        {
            // Manual null/empty check for mandatory parameters
            if (string.IsNullOrWhiteSpace(oldUnitcode) || string.IsNullOrWhiteSpace(itemcode))
            {
                return BadRequest(new
                {
                    statusCode = StatusCodes.Status400BadRequest,
                    message = "Both 'oldUnitcode' and 'itemcode' query parameters are required."
                });
            }

            var stock = await _mediator.Send(new GetCurrentStockQuery
            {
                OldUnitId = oldUnitcode,
                ItemCode = itemcode
            });

            if (stock.IsSuccess && stock.Data != null)
            {
                return Ok(new
                {
                    statusCode = StatusCodes.Status200OK,
                    data = stock.Data,
                    message = "Success"
                });
            }

            return NotFound(new
            {
                statusCode = StatusCodes.Status404NotFound,
                message = stock.Message ?? "Stock not found"
            });
        }

         [HttpGet("item-codes/{oldUnitCode}")]
        [ActionName(nameof(GetStockItemCodesAsync))]
        public async Task<IActionResult> GetStockItemCodesAsync(string oldUnitCode)
        {
            var result = await Mediator.Send(new GetCurrentStockItemsByIdQuery { OldUnitcode = oldUnitCode });

            if (result.IsSuccess)
            {
                return Ok(new 
                { 
                    StatusCode = StatusCodes.Status200OK, 
                    data = result.Data, 
                    message = result.Message 
                });
            }

            return NotFound(new 
            { 
                StatusCode = StatusCodes.Status404NotFound, 
                message = result.Message 
            });
        }

        [HttpGet("CurrentStock/{oldUnitCode}")]
        [ActionName(nameof(GetAllStockItemDetails))]
        public async Task<IActionResult> GetAllStockItemDetails(string oldUnitCode)
        {
            var result = await Mediator.Send(new GetCurrentAllStockItemsQuery { OldUnitcode = oldUnitCode });

            if (result.IsSuccess)
            {
                return Ok(new 
                { 
                    StatusCode = StatusCodes.Status200OK, 
                    data = result.Data, 
                    message = result.Message 
                });
            }

            return NotFound(new 
            { 
                StatusCode = StatusCodes.Status404NotFound, 
                message = result.Message 
            });
        }

        
    }
}