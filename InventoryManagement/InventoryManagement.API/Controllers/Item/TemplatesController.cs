using Core.Application.Item.Templates.CreateTemplate;
using Core.Application.Item.Templates.SearchTemplates;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.API.Controllers.Item
{
    [ApiController]
    [Route("api/templates")]
    public sealed class TemplatesController : ControllerBase
    {
        private readonly IMediator _mediator;
        public TemplatesController(IMediator mediator) => _mediator = mediator;

        /// <summary>Create a new template</summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> Create([FromBody] CreateTemplateCommand cmd, CancellationToken ct)
        {
            var id = await _mediator.Send(cmd, ct);

            // Return 201 with a response wrapper (and a Location header via CreatedAtAction)
            return CreatedAtAction(
                nameof(Get),
                new { id },
                new { StatusCode = StatusCodes.Status201Created, id }
            );
        }

        /// <summary>Get a template by id</summary>
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get(int id, CancellationToken ct)
        {
            // If SearchTemplatesQuery is your read model, we can fetch and filter
            var list = await _mediator.Send(new SearchTemplatesQuery { Term = null, Take = 1_000_000 }, ct);
            var tpl = list.FirstOrDefault(x => x.Id == id);

            if (tpl is null)
                return NotFound(new { StatusCode = StatusCodes.Status404NotFound, message = "Template not found." });

            return Ok(new { StatusCode = StatusCodes.Status200OK, data = tpl });
        }

        /// <summary>Search templates (simple list)</summary>
        [HttpGet("search")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Search([FromQuery] string? q, [FromQuery] int take = 20, CancellationToken ct = default)
        {
            var res = await _mediator.Send(new SearchTemplatesQuery { Term = q, Take = take }, ct);
            return Ok(new
            {
                StatusCode = StatusCodes.Status200OK,
                data = res,
                count = res?.Count ?? 0,
                query = q,
                take
            });
        }
    }
}
