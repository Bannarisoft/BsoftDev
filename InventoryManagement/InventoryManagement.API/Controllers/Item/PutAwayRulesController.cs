using Core.Application.Item.PutAway.Commands.CreatePutAwayRule;
using Core.Application.Item.PutAway.Commands.DeletePutAwayRule;
using Core.Application.Item.PutAway.Commands.UpdatePutAwayRule;
using Core.Application.Item.PutAway.Queries.EvaluatePutAway;
using Core.Application.Item.PutAway.Queries.GetPutAwayRuleById;
using Core.Application.Item.PutAway.Queries.GetPutAwayRules;
using Core.Application.Item.PutAway.Queries.GetPutAwayTargets;
using InventoryManagement.API.Controllers;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Core.API.Controller.Item
{
    [Route("api/[controller]")]
    public class PutAwayRuleController : ApiControllerBase
    {
        private readonly IMediator _mediator;

        public PutAwayRuleController(IMediator mediator)
            : base(mediator)
        {
            _mediator = mediator;
        }

        // GET: api/PutAwayRule
        [HttpGet]
        public async Task<IActionResult> GetAllAsync([FromQuery] int PageNumber, [FromQuery] int PageSize, [FromQuery] string? SearchTerm = null)
        {
            var result = await Mediator.Send(new GetPutAwayRulesQuery
            {
                PageNumber = PageNumber,
                PageSize = PageSize,
                SearchTerm = SearchTerm
            });

            return Ok(new
            {
                StatusCode = StatusCodes.Status200OK,
                data = result.Data,
                TotalCount = result.TotalCount,
                PageNumber = result.PageNumber,
                PageSize = result.PageSize
            });
        }

        // GET: api/PutAwayRule/targets?WarehouseId=1&StorageTypeId=2&SearchPattern=R-01
   /*      [HttpGet("targets")]
        public async Task<IActionResult> GetTargetsAsync([FromQuery] int WarehouseId, [FromQuery] int StorageTypeId, [FromQuery] string? SearchPattern = null)
        {
            var rows = await Mediator.Send(new GetPutAwayTargetsQuery
            {
                WarehouseId = WarehouseId,
                StorageTypeId = StorageTypeId,   // from MiscMaster only
                SearchPattern = SearchPattern
            });

            return Ok(new
            {
                StatusCode = StatusCodes.Status200OK,
                data = rows
            });
        }
 */
        // GET: api/PutAwayRule/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var dto = await Mediator.Send(new GetPutAwayRuleByIdQuery { Id = id });
            return Ok(new
            {
                StatusCode = StatusCodes.Status200OK,
                data = dto,
                message = "Put-away rule fetched successfully"
            });
        }

        // POST: api/PutAwayRule
        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] CreatePutAwayRuleCommand command)
        {
            var createdId = await _mediator.Send(command);
            return Ok(new
            {
                StatusCode = StatusCodes.Status201Created,
                message = "Created successfully.",
                data = createdId
            });
        }

        // PUT: api/PutAwayRule
        [HttpPut]
        public async Task<IActionResult> UpdateAsync([FromBody] UpdatePutAwayRuleCommand command)
        {
            await _mediator.Send(command);
            return Ok(new
            {
                message = "Updated successfully.",
                statusCode = StatusCodes.Status200OK
            });
        }

        // DELETE: api/PutAwayRule?id=123
            [HttpDelete]
            public async Task<IActionResult> DeleteAsync([FromQuery] int id)
            {
                await _mediator.Send(new DeletePutAwayRuleCommand { Id = id });
                return Ok(new
                {
                    message = "Deleted successfully.",
                    statusCode = StatusCodes.Status200OK
                });
            }

      /*   // POST: api/PutAwayRule/evaluate
        [HttpPost("evaluate")]
        public async Task<IActionResult> EvaluateAsync([FromBody] EvaluatePutAwayQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(new
            {
                StatusCode = StatusCodes.Status200OK,
                data = result.Data,
                message = result.Message,
                isSuccess = result.IsSuccess
            });
        } */
    }
}
