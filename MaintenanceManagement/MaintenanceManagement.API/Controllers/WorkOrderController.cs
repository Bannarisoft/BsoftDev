using Core.Application.WorkOrder.Command.CreateWorkOrder;
using Core.Application.WorkOrder.Command.CreateWorkOrder.CreateSchedule;
using Core.Application.WorkOrder.Command.DeleteFileWorkOrder;
using Core.Application.WorkOrder.Command.UpdateWorkOrder;
using Core.Application.WorkOrder.Command.UpdateWorkOrder.UpdateSchedule;
using Core.Application.WorkOrder.Command.UploadFileWorOrder;
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
        private readonly IValidator<CreateWorkOrderCommand> _createWorkOrderCommandValidator;
        private readonly IValidator<UpdateWorkOrderCommand> _updateWorkOrderCommandValidator;
        private readonly IValidator<UpdateWOScheduleCommand> _updateWoScheduleCommandValidator;        
        private readonly IValidator<CreateWOScheduleCommand> _createWoScheduleCommandValidator;  
        private readonly IValidator<UploadFileWorkOrderCommand> _uploadFileCommandValidator;        

        public WorkOrderController( ISender mediator, 
            IValidator<CreateWorkOrderCommand> createWorkOrderCommandValidator, 
            IValidator<UpdateWorkOrderCommand> updateWorkOrderCommandValidator ,
            IValidator<UpdateWOScheduleCommand> updateWoScheduleCommandValidator ,
            IValidator<CreateWOScheduleCommand> createWoScheduleCommandValidator ,
            IValidator<UploadFileWorkOrderCommand> uploadFileCommandValidator            
            ) 
            : base(mediator)

        {
            _createWorkOrderCommandValidator = createWorkOrderCommandValidator;
            _updateWorkOrderCommandValidator = updateWorkOrderCommandValidator;
            _updateWoScheduleCommandValidator=updateWoScheduleCommandValidator;
            _createWoScheduleCommandValidator=createWoScheduleCommandValidator;
             _uploadFileCommandValidator = uploadFileCommandValidator;
        }
       [HttpPost]
        public async Task<IActionResult> CreateAsync(CreateWorkOrderCommand command)
        {
            var validationResult = await _createWorkOrderCommandValidator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,

                    message = "Validation failed",
                    errors = validationResult.Errors.Select(e => e.ErrorMessage).ToArray()
                });
            }            
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
            var validationResult = await _updateWorkOrderCommandValidator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    message = "Validation failed",
                    errors = validationResult.Errors.Select(e => e.ErrorMessage).ToArray()
                });

            }
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
            var validationResult = await _createWoScheduleCommandValidator.ValidateAsync(command);
            if (!validationResult.IsValid)            
            {
                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    message = "Validation failed",
                    errors = validationResult.Errors.Select(e => e.ErrorMessage).ToArray()
                });

            }
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
            var validationResult = await _updateWoScheduleCommandValidator.ValidateAsync(command);
            if (!validationResult.IsValid)            
            {
                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    message = "Validation failed",
                    errors = validationResult.Errors.Select(e => e.ErrorMessage).ToArray()
                });

            }
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
            var validationResult = await _uploadFileCommandValidator.ValidateAsync(uploadFileCommand);
            if (!validationResult.IsValid)
            {

                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    message = "Validation failed",
                    errors = validationResult.Errors.Select(e => e.ErrorMessage).ToArray()
                });
            }
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
            if (deleteFileCommand == null || string.IsNullOrWhiteSpace(deleteFileCommand.assetPath))
            {
                return BadRequest(new 
                { 
                    StatusCode = StatusCodes.Status400BadRequest, 
                    message = "Invalid request. 'assetPath' cannot be null or empty.",
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
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            if (id <= 0)
            {

                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,

                    message = "Invalid Asset ID"
                });
            }
            var result = await Mediator.Send(new GetWorkOrderByIdQuery { Id = id });
            if (result is null)
            {
                return NotFound(new
                {
                    StatusCode = StatusCodes.Status404NotFound,

                    message = $"AssetId {id} not found",
                });
            }
            return Ok(new
            {
                StatusCode = StatusCodes.Status200OK,
                data = result.Data

            });
        }
    }
}