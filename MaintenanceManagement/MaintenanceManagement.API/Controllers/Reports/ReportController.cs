using Core.Application.Reports.GetCurrentAllStockItems;
using Core.Application.Reports.GetStockLegerReport;
using Core.Application.Reports.MaintenanceRequestReport;
using Core.Application.Reports.WorkOrderItemConsuption;
using Core.Application.Reports.WorkOrderReport;
using Core.Application.Reports.WorkOderCheckListReport;
using MaintenanceManagement.API.Controllers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Core.Application.Reports.MRS;
using Core.Application.Reports.ScheduleReport;
using Core.Application.Reports.MaterialPlanningReport;

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




        [HttpGet("WorkOrderReport")]

        public async Task<IActionResult> WorkOrderReportAsync([FromQuery] string? fromDate, [FromQuery] string? toDate, [FromQuery] int requestTypeId)
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

            var query = new WorkOrderReportQuery
            {
                FromDate = parsedFromDate,

                ToDate = parsedToDate,
                RequestTypeId = requestTypeId
            };
            var result = await Mediator.Send(query);

            if (result == null || result.Data == null || result.Data.Count == 0)
            {
                return NotFound(new
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = result?.Message ?? "No Work Order Report found."
                });
            }

            return Ok(new
            {
                StatusCode = StatusCodes.Status200OK,
                Message = result.Message,
                Data = result.Data
            });
        }

        [HttpGet("ItemConsumption")]
        public async Task<IActionResult> GetAllItemConsuption(
            [FromQuery] string? fromDate,
            [FromQuery] string? toDate,
            [FromQuery] int? maintenanceTypeId)
        {
            DateTimeOffset? parsedFromDate = null;
            DateTimeOffset? parsedToDate = null;

            if (!string.IsNullOrWhiteSpace(fromDate))
            {
                if (!DateTimeOffset.TryParse(fromDate, out var fromParsed))
                {
                    return BadRequest(new { message = "Invalid fromDate format. Use yyyy-MM-dd." });
                }
                parsedFromDate = fromParsed;
            }

            if (!string.IsNullOrWhiteSpace(toDate))
            {
                if (!DateTimeOffset.TryParse(toDate, out var toParsed))
                {
                    return BadRequest(new { message = "Invalid toDate format. Use yyyy-MM-dd." });
                }
                parsedToDate = toParsed;
            }

            if (maintenanceTypeId is null or <= 0)
            {
                return BadRequest(new { message = "Invalid Maintenance Type Id" });
            }

            var workOrder = await Mediator.Send(new WorkOrderIssueQuery
            {
                IssueFrom = parsedFromDate,
                IssueTo = parsedToDate,
                MaintenanceTypeId = maintenanceTypeId.Value
            });

            return Ok(new
            {
                StatusCode = StatusCodes.Status200OK,
                message = workOrder.Message,
                data = workOrder.Data?.ToList()
            });
        }

        [HttpGet("SubStoresStockLedger")]
        [ActionName(nameof(GetSubStoresStockLedger))]
        public async Task<IActionResult> GetSubStoresStockLedger(
            [FromQuery] string oldUnitcode,
            [FromQuery] DateTime fromDate,
            [FromQuery] DateTime toDate,
            [FromQuery] string? itemcode = null)
        {
            // Manual validation
            if (string.IsNullOrWhiteSpace(oldUnitcode))
            {
                return BadRequest(new
                {
                    statusCode = StatusCodes.Status400BadRequest,
                    message = "'oldUnitcode' query parameter is required."
                });
            }

            if (fromDate > toDate)
            {
                return BadRequest(new
                {
                    statusCode = StatusCodes.Status400BadRequest,
                    message = "'fromDate' must be less than or equal to 'toDate'."
                });
            }

            if (!IsSameFinancialYear(fromDate, toDate))
            {
                return BadRequest(new
                {
                    statusCode = StatusCodes.Status400BadRequest,
                    message = "'fromDate' and 'toDate' must fall within the same financial year (April to March)."
                });
            }

            var result = await Mediator.Send(new GetStockLegerReportQuery { OldUnitcode = oldUnitcode, FromDate = fromDate, ToDate = toDate, ItemCode = itemcode });

            if (result.IsSuccess && result.Data != null)
            {
                return Ok(new
                {
                    statusCode = StatusCodes.Status200OK,
                    data = result.Data?.ToList(),
                    message = "Success"
                });
            }

            return NotFound(new
            {
                statusCode = StatusCodes.Status404NotFound,
                message = "No stock ledger data found for the given criteria."
            });
        }

        // Ensure both dates fall in the same financial year (April 1 to March 31)
        bool IsSameFinancialYear(DateTime date1, DateTime date2)
        {
            int fyStartYear1 = date1.Month >= 4 ? date1.Year : date1.Year - 1;
            int fyStartYear2 = date2.Month >= 4 ? date2.Year : date2.Year - 1;
            return fyStartYear1 == fyStartYear2;
        }
        [HttpGet("CurrentStock/{oldUnitCode}")]
        [ActionName(nameof(GetAllStockItemDetails))]
        public async Task<IActionResult> GetAllStockItemDetails(string oldUnitCode)
        {
            var result = await Mediator.Send(new GetCurrentAllStockItemsQuery { OldUnitcode = oldUnitCode });

            if (result.IsSuccess)
            {
                return Ok(new
                {
                    StatusCode = StatusCodes.Status200OK,
                    data = result.Data,
                    message = result.Message
                });
            }

            return NotFound(new
            {
                StatusCode = StatusCodes.Status404NotFound,
                message = result.Message
            });
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
                Data = result?.Data ?? new List<RequestReportDto>()
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

        [HttpGet("MRSReport")]
        public async Task<IActionResult> GetMRSReport(
           [FromQuery] string? fromDate,
           [FromQuery] string? toDate,
           [FromQuery] string? OldUnitCode)
        {
            DateTimeOffset? parsedFromDate = null;
            DateTimeOffset? parsedToDate = null;

            if (!string.IsNullOrWhiteSpace(fromDate))
            {
                if (!DateTimeOffset.TryParse(fromDate, out var fromParsed))
                {
                    return BadRequest(new { message = "Invalid fromDate format. Use yyyy-MM-dd." });
                }
                parsedFromDate = fromParsed;
            }

            if (!string.IsNullOrWhiteSpace(toDate))
            {
                if (!DateTimeOffset.TryParse(toDate, out var toParsed))
                {
                    return BadRequest(new { message = "Invalid toDate format. Use yyyy-MM-dd." });
                }
                parsedToDate = toParsed;
            }

            if (OldUnitCode is null)
            {
                return BadRequest(new { message = "Invalid OldUnitCode" });
            }

            var workOrder = await Mediator.Send(new MRSReportQuery
            {
                FromDate = parsedFromDate,
                ToDate = parsedToDate,
                OldUnitCode = OldUnitCode
            });

            return Ok(new
            {
                StatusCode = StatusCodes.Status200OK,
                message = workOrder.Message,
                data = workOrder.Data?.ToList()
            });
        }

        [HttpGet("SchedulerReport")]
        public async Task<IActionResult> SchedulerReportAsync(
               [FromQuery] string? MachineDepartment,
               [FromQuery] string? MachineGroup,
               [FromQuery] string? MaintenanceCategory,
               [FromQuery] string? Activity,
               [FromQuery] string? ActivityType
               )
        {
            var query = new ScheduleReportQuery
            {
                MachineDepartment = MachineDepartment,
                MachineGroup = MachineGroup,
                MaintenanceCategory = MaintenanceCategory,
                Activity = Activity,
                ActivityType = ActivityType
            };

            var result = await Mediator.Send(query);

            if (result == null || result.Data == null || result.Data.Count == 0)
            {
                return NotFound(new
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = result?.Message ?? "No Scheduler records found."
                });
            }

            return Ok(new
            {
                StatusCode = StatusCodes.Status200OK,
                Message = result.Message,
                Data = result?.Data
            });
        }
           [HttpGet("MaterialPlanningReport")]
        public async Task<IActionResult> MaterialPlanningReportAsync(
                [FromQuery] DateTime? FromDueDate,
                [FromQuery] DateTime? ToDueDate,
                [FromQuery] string? MaintenanceCategory,
                [FromQuery] string? MachineName,
                [FromQuery] string? Activity,
                [FromQuery] string? MaterialCode
                )
        {
            var query = new MaterialPlanningReportQuery
            {
                FromDueDate = FromDueDate,
                ToDueDate = ToDueDate,
                MaintenanceCategory = MaintenanceCategory,
                MachineName = MachineName,
                Activity = Activity,
                MaterialCode = MaterialCode
            };

            var result = await Mediator.Send(query);

            if (result == null || result.Data == null || result.Data.Count == 0)
            {
                return NotFound(new
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = result?.Message ?? "No Material Planning Report records found."
                });
            }

            return Ok(new
            {
                StatusCode = StatusCodes.Status200OK,
                Message = result.Message,
                Data = result?.Data
            });
        }
    }
}