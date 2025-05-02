using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.MainStoreStock.Queries.GetItemStockbyId;
using Core.Application.MainStoreStock.Queries.GetMainStoreStock;
using Core.Application.MainStoreStock.Queries.GetMainStoreStockItems;
using MassTransit.Futures.Contracts;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MaintenanceManagement.API.Controllers
{
    [Route("api/[controller]")]
    public class MainStoreStockController : ApiControllerBase
    {
        private readonly ILogger<MainStoreStockController> _logger;
        private readonly IMediator _mediator;

        public MainStoreStockController(ILogger<MainStoreStockController> logger, IMediator mediator)
         : base(mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [HttpGet("MainStore-stock")]
        [ActionName(nameof(GetMainStoresCurrentStock))]
        public async Task<IActionResult> GetMainStoresCurrentStock([FromQuery] string oldUnitcode, [FromQuery] string GroupCode)
        {
            // Manual null/empty check for mandatory parameters
            if (string.IsNullOrWhiteSpace(oldUnitcode) || string.IsNullOrWhiteSpace(GroupCode))
            {
                return BadRequest(new
                {
                    statusCode = StatusCodes.Status400BadRequest,
                    message = "Both 'oldUnitcode' and 'GroupCode' query parameters are required."
                });
            }

            var stock = await _mediator.Send(new GetMainStoreStockQuery
            {
                OldUnitcode = oldUnitcode,
                GroupCode = GroupCode
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

         [HttpGet("MainStore-StockItems")]
        [ActionName(nameof(GetMainStoresCurrentStockItems))]
        public async Task<IActionResult> GetMainStoresCurrentStockItems([FromQuery] string oldUnitcode, [FromQuery] string GroupCode)
        {
            // Manual null/empty check for mandatory parameters
            if (string.IsNullOrWhiteSpace(oldUnitcode) || string.IsNullOrWhiteSpace(GroupCode))
            {
                return BadRequest(new
                {
                    statusCode = StatusCodes.Status400BadRequest,
                    message = "Both 'oldUnitcode' and 'GroupCode' query parameters are required."
                });
            }

            var stock = await _mediator.Send(new GetMainStoreStockItemsQuery
            {
                OldUnitcode = oldUnitcode,
                GroupCode = GroupCode
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

        [HttpGet("{oldUnitcode}/{itemCode}")]
        [ActionName(nameof(GetByItemCodeIdAsync))]
        public async Task<IActionResult> GetByItemCodeIdAsync(string oldUnitcode, string itemCode)
        {
            var stockItem = await _mediator.Send(new GetItemStockbyIdQuery
            {
                OldUnitcode = oldUnitcode,
                ItemCode = itemCode
            });

            if (stockItem.IsSuccess)
            {
                return Ok(new
                {
                    StatusCode = StatusCodes.Status200OK,
                    data = stockItem.Data,
                    message = stockItem.Message
                });
            }

            return NotFound(new
            {
                StatusCode = StatusCodes.Status404NotFound,
                message = stockItem.Message
            });
        }

      
    }
}