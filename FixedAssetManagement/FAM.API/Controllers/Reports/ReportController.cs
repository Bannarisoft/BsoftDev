using Core.Application.Reports.AssetReport;
using Core.Application.Reports.AssetTransferReport;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FAM.API.Controllers.Reports
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportController : ApiControllerBase
    {

        public ReportController(ISender mediator)
        : base(mediator)
        {

        }

        [HttpGet("AssetReport")]
        public async Task<IActionResult> AssetReportAsync([FromQuery] string? fromDate, [FromQuery] string? toDate)
        {
            DateTimeOffset? parsedFromDate = null;
            DateTimeOffset? parsedToDate = null;

            if (!string.IsNullOrWhiteSpace(fromDate))  // Allow null or empty values
            {
                if (!DateTimeOffset.TryParse(fromDate, out var parsedDate))
                {
                    return BadRequest(new { message = "Invalid fromDate format. Use yyyy-MM-dd." });
                }
                parsedFromDate = parsedDate;
            }

            if (!string.IsNullOrWhiteSpace(toDate))  // Allow null or empty values
            {
                if (!DateTimeOffset.TryParse(toDate, out var parsedDate))
                {
                    return BadRequest(new { message = "Invalid toDate format. Use yyyy-MM-dd." });
                }
                parsedToDate = parsedDate;
            }

            var query = new AssetReportQuery
            {
                FromDate = parsedFromDate,
                ToDate = parsedToDate
            };
            var result = await Mediator.Send(query);

            if (result == null || result.Data == null || result.Data.Count == 0)
            {
                return NotFound(new
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = result?.Message ?? "No Asset Report found."
                });
            }

            return Ok(new
            {
                StatusCode = StatusCodes.Status200OK,
                Message = result.Message,
                Data = result.Data
            });
        }
        
        [HttpGet("AssetTransferReport")]
        public async Task<IActionResult> AssetTransferReportAsync(
            [FromQuery] DateTimeOffset? FromDate = null,
            [FromQuery] DateTimeOffset? ToDate = null)
        {
            var result = await Mediator.Send(new AssetTransferQuery
            {
                FromDate = FromDate,
                ToDate = ToDate
            });

            if (result?.Data == null || result.Data.Count == 0)
            {
                return NotFound(new
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = result?.Message ?? "No Asset Transfer Report found."
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