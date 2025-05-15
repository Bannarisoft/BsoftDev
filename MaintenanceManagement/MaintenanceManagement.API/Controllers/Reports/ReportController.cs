using Core.Application.Reports.MaintenanceRequestReport;
using Core.Application.Reports.WorkOderCheckListReport;
using MaintenanceManagement.API.Controllers;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MaintenanceManagement.API.Controllers.Reports
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportController : ApiControllerBase
    {

        public ReportController(ISender mediator)
        : base(mediator)
        {

        }
        [HttpGet("RequestReport")]
        public async Task<IActionResult> MaintenanceReportAsync(
            [FromQuery] DateTimeOffset? requestFromDate,
            [FromQuery] DateTimeOffset? requestToDate,
            [FromQuery] int? RequestType,
            [FromQuery] int? requestStatus,
            [FromQuery] int? departmentId
            )
        {
            var query = new RequestReportQuery
            {
                RequestFromDate = requestFromDate,
                RequestToDate = requestToDate,
                RequestType = RequestType,
                RequestStatus = requestStatus,
                DepartmentId = departmentId
            };

            var result = await Mediator.Send(query);

            if (result == null || result.Data == null || result.Data.Count == 0)
            {
                return NotFound(new
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = result?.Message ?? "No maintenance requests found."
                });
            }

            return Ok(new
            {
                StatusCode = StatusCodes.Status200OK,
                Message = result.Message,
                Data = result?.Data ??  new List<RequestReportDto>()
            });
        }
        
        [HttpGet("WorkOrderChecklistReport")]
            public async Task<IActionResult> WorkOrderChecklistReportAsync(
                [FromQuery] DateTimeOffset? WorkOrderFromDate,
                [FromQuery] DateTimeOffset? WorkOrderToDate,
                [FromQuery] int? MachineGroupId,
                [FromQuery] int? machineId,
                [FromQuery] int? ActivityId
                )
            {
                var query = new WorkOderCheckListReportQuery
                {
                    WorkOrderFromDate = WorkOrderFromDate,
                    WorkOrderToDate = WorkOrderToDate,
                    MachineGroupId = MachineGroupId,
                    MachineId = machineId,
                    ActivityId = ActivityId
                };

                var result = await Mediator.Send(query);

                if (result == null || result.Data == null || result.Data.Count == 0)
                {
                    return NotFound(new
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        Message = result?.Message ?? "No Work Order Checklist records found."
                    });
                }

                return Ok(new
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = result.Message,
                    //Data = result.Data
                    Data = result?.Data ?? new List<WorkOderCheckListReportDto>()
                });
            }

    }
}