using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using Core.Application.MRS.Command.CreateMRS;
using Core.Application.MRS.Queries;
using Core.Application.MRS.Queries.GetCategory;
using Core.Application.MRS.Queries.GetPendingQty;
using Core.Application.MRS.Queries.GetSubCostCenter;
using Core.Application.MRS.Queries.GetSubDepartment;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MaintenanceManagement.API.Controllers
{
    [Route("api/[controller]")]
    public class MRSController : ApiControllerBase
    {
        private readonly ILogger<MRSController> _logger;
        private readonly IMediator _mediator;
        private readonly IValidator<CreateMRSCommand> _createmrscommandvalidator;

        public MRSController(ILogger<MRSController> logger, IMediator mediator, IValidator<CreateMRSCommand> createmrscommandvalidator)
         : base(mediator)
        {
            _logger = logger;
            _mediator = mediator;
            _createmrscommandvalidator = createmrscommandvalidator;
        }

        [HttpGet("department/{oldUnitcode}")]
        [ActionName(nameof(GetDepartment))]
        public async Task<IActionResult> GetDepartment(string oldUnitcode)
        {
            var stockItem = await _mediator.Send(new GetDepartmentbyIdQuery
            {
                OldUnitcode = oldUnitcode

            });

            if (stockItem.IsSuccess)
            {
                return Ok(new
                {
                    StatusCode = StatusCodes.Status200OK,
                    data = stockItem.Data,
                    message = stockItem.Message
                });
            }

            return NotFound(new
            {
                StatusCode = StatusCodes.Status404NotFound,
                message = stockItem.Message
            });
        }
        [HttpGet("SubCostCenter/{oldUnitcode}")]
        [ActionName(nameof(GetSubCostCenter))]
        public async Task<IActionResult> GetSubCostCenter(string oldUnitcode)
        {
            var stockItem = await _mediator.Send(new GetSubCostCenterQuery
            {
                OldUnitcode = oldUnitcode

            });

            if (stockItem.IsSuccess)
            {
                return Ok(new
                {
                    StatusCode = StatusCodes.Status200OK,
                    data = stockItem.Data,
                    message = stockItem.Message
                });
            }

            return NotFound(new
            {
                StatusCode = StatusCodes.Status404NotFound,
                message = stockItem.Message
            });
        }

        [HttpGet("Category/{oldUnitcode}")]
        [ActionName(nameof(GetCategory))]
        public async Task<IActionResult> GetCategory(string oldUnitcode)
        {
            var stockItem = await _mediator.Send(new GetCategoryQuery
            {
                OldUnitcode = oldUnitcode

            });

            if (stockItem.IsSuccess)
            {
                return Ok(new
                {
                    StatusCode = StatusCodes.Status200OK,
                    data = stockItem.Data,
                    message = stockItem.Message
                });
            }

            return NotFound(new
            {
                StatusCode = StatusCodes.Status404NotFound,
                message = stockItem.Message
            });
        }
        [HttpGet("SubDepartment/{oldUnitcode}")]
        [ActionName(nameof(GetSubDepartment))]
        public async Task<IActionResult> GetSubDepartment(string oldUnitcode)
        {
            var stockItem = await _mediator.Send(new GetSubDepartmentQuery
            {
                OldUnitcode = oldUnitcode

            });

            if (stockItem.IsSuccess)
            {
                return Ok(new
                {
                    StatusCode = StatusCodes.Status200OK,
                    data = stockItem.Data,
                    message = stockItem.Message
                });
            }

            return NotFound(new
            {
                StatusCode = StatusCodes.Status404NotFound,
                message = stockItem.Message
            });
        }
        [HttpPost("CreateMRS")]
        public async Task<IActionResult> CreateMRS([FromBody] HeaderRequest headerRequest)
        {
            if (headerRequest == null)
            {
                return BadRequest(new ApiResponseDTO<int>
                {
                    IsSuccess = false,
                    Message = "HeaderRequest cannot be null",
                    Data = 0
                });
            }

            var command = new CreateMRSCommand { Header = headerRequest };

            var validationResult = await _createmrscommandvalidator.ValidateAsync(command);
            _logger.LogWarning($"Validation failed: {string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage))}");

            if (!validationResult.IsValid)
            {
                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    message = "Validation failed",
                    errors = validationResult.Errors.Select(e => e.ErrorMessage)
                });
            }

            var result = await _mediator.Send(command);

            if (result.IsSuccess)
            {

                return Ok(new
                {
                    StatusCode = StatusCodes.Status201Created,
                    message = result.Message,
                    data = result.Data
                });
            }

            return BadRequest(new
            {
                StatusCode = StatusCodes.Status400BadRequest,
                message = result.Message
            });
        }

        [HttpGet("pending-issue")]
        public async Task<IActionResult> GetPendingIssue([FromQuery] string oldUnitCode, [FromQuery] string itemCode)
        {
            var result = await _mediator.Send(new GetPendingQtyQuery
            {
                OldUnitcode = oldUnitCode,
                ItemCode = itemCode
            });

            if (result.IsSuccess)
            {
                return Ok(new
                {
                    StatusCode = StatusCodes.Status200OK,
                    data = result.Data ?? new GetPendingQtyDto { PendingQty = 0 },
                    message = result.Message
                });
            }

            return NotFound(new
            {
                StatusCode = StatusCodes.Status404NotFound,
                message = result.Message
            });

            
        }

       
    }
}