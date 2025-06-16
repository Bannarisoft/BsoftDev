using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.StockLedger.Queries.GetCurrentStock;
using Core.Application.StockLedger.Queries.GetCurrentStockItemsById;
using Core.Application.StockLedger.Queries.GetStockLegerReport;
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
        public async Task<IActionResult> GetSubStoresCurrentStock([FromQuery] string oldUnitcode, [FromQuery] string itemcode, [FromQuery] int departmentId)
        {
            // Manual null/empty check for mandatory parameters
            if (string.IsNullOrWhiteSpace(oldUnitcode) || string.IsNullOrWhiteSpace(itemcode) || departmentId == 0)
            {
                return BadRequest(new
                {
                    statusCode = StatusCodes.Status400BadRequest,
                    message = "Both 'oldUnitcode' and 'itemcode' and 'departmentId' query parameters are required."
                });
            }

            var stock = await _mediator.Send(new GetCurrentStockQuery
            {
                OldUnitId = oldUnitcode,
                ItemCode = itemcode,
                DepartmentId = departmentId
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

         [HttpGet("item-codes/{oldUnitCode}/{departmentId}")]
        [ActionName(nameof(GetStockItemCodesAsync))]
        public async Task<IActionResult> GetStockItemCodesAsync(string oldUnitCode, int departmentId)
        {
            var result = await Mediator.Send(new GetCurrentStockItemsByIdQuery { OldUnitcode = oldUnitCode, DepartmentId = departmentId });

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