using Core.Application.Dashboard.DashboardQuery;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MaintenanceManagement.API.Controllers.Dashboard
{
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly IMediator _mediator;

        public DashboardController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("workOrder-summary")]
        public async Task<IActionResult> GetWorkOrderSummary([FromQuery] DateTime fromDate, [FromQuery] DateTime toDate, [FromQuery] string type)
        {
            var query = new DashboardQuery
            {
                FromDate = fromDate,
                ToDate = toDate,
                Type = type
            };
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }

}