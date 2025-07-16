using Core.Application.Reports.WorkOrderItemConsuption;
using Core.Application.WorkOrder.Command.CreateWorkOrder;
using Core.Application.WorkOrder.Command.CreateWorkOrder.CreateSchedule;
using Core.Application.WorkOrder.Command.DeleteFileWorkOrder;
using Core.Application.WorkOrder.Command.DeleteFileWorkOrder.Item;
using Core.Application.WorkOrder.Command.UpdateWorkOrder;
using Core.Application.WorkOrder.Command.UpdateWorkOrder.UpdateSchedule;
using Core.Application.WorkOrder.Command.UploadFileWorOrder;
using Core.Application.WorkOrder.Command.UploadFileWorOrder.Item;
using Core.Application.WorkOrder.Queries.GetRequestType;
using Core.Application.WorkOrder.Queries.GetWorkOderDropdown;
using Core.Application.WorkOrder.Queries.GetWorkOrder;
using Core.Application.WorkOrder.Queries.GetWorkOrderById;
using Core.Application.WorkOrder.Queries.GetWorkOrderRootCause;
using Core.Application.WorkOrder.Queries.GetWorkOrderSource;
using Core.Application.WorkOrder.Queries.GetWorkOrderStatus;
using Core.Application.WorkOrder.Queries.GetWorkOrderStoreType;
using FluentValidation;
using MaintenanceManagement.API.Validation.WorkOrder;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MaintenanceManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WorkOrderController : ApiControllerBase
    {
   

        public WorkOrderController(ISender mediator
            )
            : base(mediator)

        {
          
        }
        [HttpPost]
        public async Task<IActionResult> CreateAsync(CreateWorkOrderCommand command)
        {
           
            var result = await Mediator.Send(command);
            if (result.IsSuccess)
            {

                return Ok(new
                {
                    StatusCode = StatusCodes.Status201Created,

                    message = result.Message,
                    data = result.Data
                });

            }
            else

            {
                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    message = result.Message
                });

            }
        }
        [HttpPut]
        public async Task<IActionResult> UpdateAsync(UpdateWorkOrderCommand command)
        {
          
            var result = await Mediator.Send(command);
            if (result.IsSuccess)
            {

                return Ok(new
                {
                    StatusCode = StatusCodes.Status200OK,

                    message = result.Message,
                    asset = result.Data
                });
            }
            return BadRequest(new
            {
                StatusCode = StatusCodes.Status400BadRequest,
                message = result.Message
            });
        }
        [HttpPost("schedule/Create")]
        public async Task<IActionResult> CreateScheduleAsync(CreateWOScheduleCommand command)
        {
        
            var result = await Mediator.Send(command);
            if (result.IsSuccess)
            {

                return Ok(new
                {
                    StatusCode = StatusCodes.Status200OK,
                    message = result.Message,
                    //asset = result.Data
                });
            }
            return BadRequest(new
            {
                StatusCode = StatusCodes.Status400BadRequest,
                message = result.Message
            });
        }
        [HttpPut("schedule/Update")]
        public async Task<IActionResult> UpdateScheduleAsync(UpdateWOScheduleCommand command)
        {
           
            var result = await Mediator.Send(command);
            if (result.IsSuccess)
            {

                return Ok(new
                {
                    StatusCode = StatusCodes.Status200OK,

                    message = result.Message,
                    asset = result.Data
                });
            }
            return BadRequest(new
            {
                StatusCode = StatusCodes.Status400BadRequest,
                message = result.Message
            });
        }
        [HttpPost("upload-image")]
        public async Task<IActionResult> UploadLogo(UploadFileWorkOrderCommand uploadFileCommand)
        {
          
            var file = await Mediator.Send(uploadFileCommand);
            if (!file.IsSuccess)
            {

                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    message = file.Message,
                    errors = ""
                });
            }

            return Ok(new
            {
                StatusCode = StatusCodes.Status200OK,
                message = file.Message,
                data = file.Data,
                errors = ""
            });
        }
        [HttpDelete("delete-image")]
        public async Task<IActionResult> DeleteLogo([FromBody] DeleteFileWorkOrderCommand deleteFileCommand)
        {
            if (deleteFileCommand == null || string.IsNullOrWhiteSpace(deleteFileCommand.Image))
            {
                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    message = "Invalid request. 'ImagePath' cannot be null or empty.",
                    errors = ""
                });
            }

            var file = await Mediator.Send(deleteFileCommand);
            if (!file.IsSuccess)
            {

                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    message = file.Message,
                    errors = ""
                });
            }

            return Ok(new
            {
                StatusCode = StatusCodes.Status200OK,
                message = file.Message,
                errors = ""
            });
        }
        [HttpPost("upload-image/Item")]
        public async Task<IActionResult> UploadItemLogo(UploadFileItemCommand uploadFileCommand)
        {
           
            var file = await Mediator.Send(uploadFileCommand);
            if (!file.IsSuccess)
            {

                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    message = file.Message,
                    errors = ""
                });
            }

            return Ok(new
            {
                StatusCode = StatusCodes.Status200OK,
                message = file.Message,
                data = file.Data,
                errors = ""
            });
        }
        [HttpDelete("delete-image/Item")]
        public async Task<IActionResult> DeleteItemLogo([FromBody] DeleteFileItemCommand deleteFileCommand)
        {
            if (deleteFileCommand == null || string.IsNullOrWhiteSpace(deleteFileCommand.Image))
            {
                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    message = "Invalid request. 'ImagePath' cannot be null or empty.",
                    errors = ""
                });
            }

            var file = await Mediator.Send(deleteFileCommand);
            if (!file.IsSuccess)
            {

                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    message = file.Message,
                    errors = ""
                });
            }

            return Ok(new
            {
                StatusCode = StatusCodes.Status200OK,
                message = file.Message,
                errors = ""
            });
        }
        [HttpGet("Status")]
        public async Task<IActionResult> GetWorkOrderStatus()
        {
            var result = await Mediator.Send(new GetWorkOrderStatusQuery());
            if (result == null || result.Data == null || result.Data.Count == 0)
            {
                return NotFound(new
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    message = "No WorkOrder Status found."
                });
            }
            return Ok(new
            {
                StatusCode = StatusCodes.Status200OK,
                message = "WorkOrder Status fetched successfully.",
                data = result.Data
            });
        }
        [HttpGet("RootCause")]
        public async Task<IActionResult> GetWorkOrderRootCause()
        {
            var result = await Mediator.Send(new GetWorkOrderRootCauseQuery());
            if (result == null || result.Data == null || result.Data.Count == 0)
            {
                return NotFound(new
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    message = "No WorkOrder Status found."
                });
            }
            return Ok(new
            {
                StatusCode = StatusCodes.Status200OK,
                message = "WorkOrder Status fetched successfully.",
                data = result.Data
            });
        }
        [HttpGet("Source")]
        public async Task<IActionResult> GetWorkOrderSource()
        {
            var result = await Mediator.Send(new GetWorkOrderSourceQuery());
            if (result == null || result.Data == null || result.Data.Count == 0)
            {
                return NotFound(new
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    message = "No WorkOrder Status found."
                });
            }
            return Ok(new
            {
                StatusCode = StatusCodes.Status200OK,
                message = "WorkOrder Status fetched successfully.",
                data = result.Data
            });
        }
        [HttpGet("StoreType")]
        public async Task<IActionResult> GetStoreType()
        {
            var result = await Mediator.Send(new GetWorkOrderStoreTypeQuery());
            if (result == null || result.Data == null || result.Data.Count == 0)
            {
                return NotFound(new
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    message = "No StoreType found."
                });
            }
            return Ok(new
            {
                StatusCode = StatusCodes.Status200OK,
                message = "StoreType fetched successfully.",
                data = result.Data
            });
        }
        [HttpGet("RequestType")]
        public async Task<IActionResult> GetRequestType()
        {
            var result = await Mediator.Send(new GetRequestTypeQuery());
            if (result == null || result.Data == null || result.Data.Count == 0)
            {
                return NotFound(new
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    message = "No Request Type found."
                });
            }
            return Ok(new
            {
                StatusCode = StatusCodes.Status200OK,
                message = "RequestType fetched successfully.",
                data = result.Data
            });
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            if (id <= 0)
            {

                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,

                    message = "Invalid WorkOrder ID"
                });
            }
            var result = await Mediator.Send(new GetWorkOrderByIdQuery { Id = id });
            if (result is null)
            {
                return NotFound(new
                {
                    StatusCode = StatusCodes.Status404NotFound,

                    message = $"WorkOrder ID {id} not found",
                });
            }
            return Ok(new
            {
                StatusCode = StatusCodes.Status200OK,
                data = result.Data

            });
        }
        [HttpGet]
        public async Task<IActionResult> GetByAllAsync([FromQuery] string? fromDate, [FromQuery] string? toDate, [FromQuery] int? requestTypeId
       , [FromQuery] int? departmentId, [FromQuery] int? machineId)
        {
            DateTimeOffset? parsedStartDate = null;
            DateTimeOffset? parsedEndDate = null;

            if (!string.IsNullOrWhiteSpace(fromDate))  // Allow null or empty values
            {
                if (!DateTimeOffset.TryParse(fromDate, out var parsedDate))
                {
                    return BadRequest(new { message = "Invalid fromDate format. Use yyyy-MM-dd." });
                }
                parsedStartDate = parsedDate;
            }

            if (!string.IsNullOrWhiteSpace(toDate))  // Allow null or empty values
            {
                if (!DateTimeOffset.TryParse(toDate, out var parsedDate))
                {
                    return BadRequest(new { message = "Invalid toDate format. Use yyyy-MM-dd." });
                }
                parsedEndDate = parsedDate;
            }
            if (requestTypeId <= 0)
            {
                return BadRequest(new { message = "Invalid Request Id" });
            }
            var workOrder = await Mediator.Send(
               new GetWorkOrderQuery
               {
                   fromDate = parsedStartDate,
                   toDate = parsedEndDate,
                   requestTypeId = requestTypeId,
                   departmentId = departmentId,
                   machineId = machineId
               });
            return Ok(new
            {
                StatusCode = StatusCodes.Status200OK,
                message = workOrder.Message,
                data = workOrder.Data.ToList()
            });
        }
        [HttpGet("GetWorkOrderDropdown")]
        public async Task<IActionResult> GetWorkOrderAsync()
        {
            var result = await Mediator.Send(new GetWorkOderDropdownQuery());
            if (result == null || result.Data == null || result.Data.Count == 0)
            {
                return NotFound(new
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    message = "No Workorder found."
                });
            }
            return Ok(new
            {
                StatusCode = StatusCodes.Status200OK,
                message = "Workorder fetched successfully.",
                data = result.Data
            });
        }

        
    }
}