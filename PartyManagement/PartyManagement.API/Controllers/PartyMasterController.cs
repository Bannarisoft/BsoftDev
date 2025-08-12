using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.PartyMaster.Queries.GetPartyGroupLoad;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace PartyManagement.API.Controllers
{
    [Route("api/[controller]")]
    public class PartyMasterController : ApiControllerBase
    {
        private readonly IMediator _mediator;

        public PartyMasterController(IMediator mediator)
        : base(mediator)
        {
            _mediator = mediator;
        }
        [HttpGet("load")]
        public async Task<IActionResult> GetPartyGroups([FromQuery] string groupTypeIds)
        {
            if (string.IsNullOrWhiteSpace(groupTypeIds))
            {
                return BadRequest("GroupTypeIds are required.");
            }
            var parsedIds = groupTypeIds
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(id => int.Parse(id.Trim()))
                .ToList();

            var query = new GetPartyGroupLoadQuery { GroupTypeIds = parsedIds };

            var result = await _mediator.Send(query);

            if (result == null || !result.Any())
            {
                return NotFound(new
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    data = (object?)null,
                    message = $"PartyGroup with ID {groupTypeIds} not found"
                });
            }

            return Ok(new
            {
                StatusCode = StatusCodes.Status200OK,
                data = result,
                message = "ID fetched successfully"
            });

        }

       
    }
}