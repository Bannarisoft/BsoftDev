
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

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> Create([FromBody] CreateTemplateCommand cmd, CancellationToken ct)
        {
            var id = await _mediator.Send(cmd, ct);
            return CreatedAtAction(nameof(Get), new { id }, new { id });
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Get(int id, CancellationToken ct)
        {
            var list = await _mediator.Send(new SearchTemplatesQuery { Term = null, Take = 1 }, ct);
            var tpl = list.FirstOrDefault(x => x.Id == id);
            if (tpl is null) return NotFound();
            return Ok(tpl);
        }


        [HttpGet("search")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Search([FromQuery] string? q, [FromQuery] int take = 20, CancellationToken ct = default)
        {
            var res = await _mediator.Send(new SearchTemplatesQuery { Term = q, Take = take }, ct);
            return Ok(res);
        }
    }
}
