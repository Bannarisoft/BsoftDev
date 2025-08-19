using Core.Application.Item.ItemDetail.Commands.CreateItem;
using Core.Application.Item.ItemDetail.Commands.DeleteItemImage;
using Core.Application.Item.ItemDetail.Commands.UpdateItem;
using Core.Application.Item.ItemDetail.Commands.UploadItemImage;
using Core.Application.Item.ItemDetail.Queries.GetAllItems;
using Core.Application.Item.ItemDetail.Queries.GetItemById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.API.Controllers.Item
{
    [ApiController]
     [Route("api/[controller]")]
    public sealed class ItemMasterController : ControllerBase
    {
        private readonly IMediator _mediator;
        public ItemMasterController(IMediator mediator) => _mediator = mediator;

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string? search = null, [FromQuery] bool onlyActive = true)
        {
            var (items, total) = await _mediator.Send(new GetAllItemsQuery { PageNumber = pageNumber, PageSize = pageSize, SearchTerm = search, OnlyActive = onlyActive });
            return Ok(new { StatusCode = StatusCodes.Status200OK, data = items, totalCount = total, pageNumber, pageSize });
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var dto = await _mediator.Send(new GetItemByIdQuery { Id = id });
            if (dto is null) return NotFound(new { message = "Item not found." });
            return Ok(new { StatusCode = StatusCodes.Status200OK, data = dto });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateItemCommand command, CancellationToken ct)
        {
            var id = await _mediator.Send(command, ct);

            var body = new
            {
                StatusCode = StatusCodes.Status201Created,
                message = "Item created successfully.",
                data = new { id }
            };
            return CreatedAtAction(nameof(GetById), new { id }, body);
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Update([FromBody] UpdateItemCommand cmd, CancellationToken ct)
        {
            var command = new UpdateItemCommand { Payload = cmd.Payload };
            await _mediator.Send(command, ct);
            return Ok(new
            {
                statusCode = StatusCodes.Status200OK,
                message = "Item updated successfully.",
                data = new { cmd.Payload.Id }
            });
        }      
        [HttpPost("upload-logo")]
        public async Task<IActionResult> UploadLogo([FromForm] UploadFileCommand command, CancellationToken ct)
        {
            if (command.File is null || command.File.Length == 0)
            {
                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    message = "No file uploaded.",
                    errors = "File is required."
                });
            }

            var dto = await _mediator.Send(command, ct); // <-- instance, not type

            return Ok(new
            {
                StatusCode = StatusCodes.Status200OK,
                message = "Logo uploaded successfully.",
                data = dto,
                errors = ""
            });
        }        
        [HttpDelete("delete-logo")]
        public async Task<IActionResult> DeleteLogo([FromBody] DeleteFileCommand command, CancellationToken ct)
        {
            var result = await _mediator.Send(command, ct);

            // adjust response shape to your DeleteFileCommand result type
            return Ok(new
            {
                StatusCode = StatusCodes.Status200OK,
                message = "Logo deleted successfully.",
                data = result,
                errors = ""
            });
        }
    }
}
