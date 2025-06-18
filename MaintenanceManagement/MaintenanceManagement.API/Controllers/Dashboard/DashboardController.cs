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

        [HttpGet("workorder-summary")]
        public async Task<IActionResult> GetWorkOrderSummary([FromQuery] DashboardQuery request)
        {
            request.Type = "workordersummary";
            var data = await _mediator.Send(request);
            return Ok(data);
        }

        [HttpGet("item-consumption")]
        public async Task<IActionResult> GetItemConsumption([FromQuery] DashboardQuery request)
        {
            request.Type = "itemconsumption";
            var data = await _mediator.Send(request);
            return Ok(data);
        }

        [HttpGet("maintenance-hours")]
        public async Task<IActionResult> GetMaintenanceHours([FromQuery] DashboardQuery request)
        {
            request.Type = request.MachineGroupId != null ? "maintenancehrs-machine" :
                        request.DepartmentId != null ? "maintenancehrs-machinegroup" :
                        "maintenancehrs-dept";

            var data = await _mediator.Send(request);
            return Ok(data);
        }
        [HttpGet("itemconsumption-dept")]
        public async Task<IActionResult> GetItemConsumptionDept([FromQuery] DashboardQuery request)
        {
            request.Type = request.DepartmentId == null ? "itemconsumption-dept" :                        
                        "itemconsumption-machinegroup";

            var data = await _mediator.Send(request);
            return Ok(data);
        }
    }

}