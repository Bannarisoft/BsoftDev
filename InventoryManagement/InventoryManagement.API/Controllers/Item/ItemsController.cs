using Core.Application.Item.ItemDetail.Commands.CreateItem;
using Core.Application.Item.ItemDetail.Commands.UpdateItem;
using Core.Application.Item.ItemDetail.Queries.GetAllItems;
using Core.Application.Item.ItemDetail.Queries.GetItemById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.API.Controllers.Item
{
    [ApiController]
    [Route("api/items")]
   public sealed class ItemsController : ControllerBase
    {
        private readonly IMediator _mediator;
        public ItemsController(IMediator mediator) => _mediator = mediator;

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string? search = null, [FromQuery] bool onlyActive = true)
        {
            var (items, total) = await _mediator.Send(new GetAllItemsQuery { PageNumber = pageNumber, PageSize = pageSize, SearchTerm = search, OnlyActive = onlyActive });
            return Ok(new { data = items, totalCount = total, pageNumber, pageSize });
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var dto = await _mediator.Send(new GetItemByIdQuery { Id = id });
            if (dto is null) return NotFound(new { message = "Item not found." });
            return Ok(new { data = dto });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ItemDto payload)
        {
            var id = await _mediator.Send(new CreateItemCommand { Payload = payload });
            return CreatedAtAction(nameof(GetById), new { id }, new { id });
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] ItemDto payload)
        {
            await _mediator.Send(new UpdateItemCommand { Id = id, Payload = payload });
            return Ok(new { message = "Item updated." });
        }
    }
}
