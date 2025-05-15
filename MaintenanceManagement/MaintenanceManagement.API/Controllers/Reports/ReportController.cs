using Core.Application.Reports.MaintenanceRequestReport;
using MaintenanceManagement.API.Controllers;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MaintenanceManagement.API.Controllers.Reports
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportController  : ApiControllerBase
    {
        
        public ReportController(ISender mediator) 
        : base(mediator)
        {
            
        }
        [HttpGet("RequestReport")]
        public async Task<IActionResult> MaintenanceReportAsync(
            [FromQuery] DateTimeOffset? requestFromDate,
            [FromQuery] DateTimeOffset? requestToDate,
            [FromQuery] int RequestType,
            [FromQuery] int requestStatus,
            [FromQuery] int departmentId
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
                Data = result.Data
            });
        }
    }
}