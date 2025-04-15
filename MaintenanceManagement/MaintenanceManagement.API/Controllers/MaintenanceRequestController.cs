using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using MassTransit.Mediator;
using Microsoft.AspNetCore.Mvc;
using FluentValidation;
using Microsoft.Extensions.Logging;
using MediatR;
using Core.Application.MaintenanceRequest.Queries.GetMaintenanceRequest;
using Core.Application.MaintenanceRequest.Queries.GetMaintenanceRequestById;
using Core.Application.Common.HttpResponse;
using Core.Application.MaintenanceRequest.Command.CreateMaintenanceRequest;
using Core.Application.MaintenanceRequest.Command.UpdateMaintenanceRequestCommand;
using Core.Application.MaintenanceRequest.Queries.GetExistingVendorDetails;
using Core.Application.Common.Interfaces.IMaintenanceCategory;

namespace MaintenanceManagement.API.Controllers
{
    [Route("api/[controller]")]
    public class MaintenanceRequestController : ApiControllerBase
    {
        private readonly ILogger<MaintenanceRequestController> _logger;

        private readonly MassTransit.Mediator.IMediator _mediator;
         private readonly IValidator<CreateMaintenanceRequestCommand> _createmaintenancecommandvalidator;

         private readonly IValidator<UpdateMaintenanceRequestCommand> _updateMaintenanceRequestCommandValidator;
         private readonly IMaintenanceCategoryQueryRepository _maintenanceCategoryQueryRepository;

            
        public MaintenanceRequestController(ISender mediator,ILogger<MaintenanceRequestController> logger,IValidator<CreateMaintenanceRequestCommand> createmaintenancecommandvalidator,IValidator<UpdateMaintenanceRequestCommand> updatemaintenancerequestcommandvalidator,IMaintenanceCategoryQueryRepository maintenanceCategoryQueryRepository)
        : base(mediator)
        {
            _logger = logger;
            _createmaintenancecommandvalidator = createmaintenancecommandvalidator;
            _updateMaintenanceRequestCommandValidator = updatemaintenancerequestcommandvalidator;
            _maintenanceCategoryQueryRepository = maintenanceCategoryQueryRepository;
           
        }

        [HttpGet] 
          public async Task<IActionResult> GetAllMaintenanceRequestAsync([FromQuery] int PageNumber,[FromQuery] int PageSize,[FromQuery] string? SearchTerm = null)
        {
            var maintenancerequest = await Mediator.Send(
            new GetMaintenanceRequestQuery
            {
                PageNumber = PageNumber, 
                PageSize = PageSize, 
                SearchTerm = SearchTerm
            });
           

            return Ok(new 
            { 
                StatusCode=StatusCodes.Status200OK, 
                data = maintenancerequest.Data,
                TotalCount = maintenancerequest.TotalCount,
                PageNumber = maintenancerequest.PageNumber,
                PageSize = maintenancerequest.PageSize
            });
        }

          [HttpGet("{id}")]
        [ActionName(nameof(GetByIdAsync))]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var maintenancerequest = await Mediator.Send(new GetMaintenanceRequestByIdQuery() { Id = id});
          
            if(maintenancerequest.IsSuccess)
            {
                
              return Ok(new { StatusCode=StatusCodes.Status200OK, data = maintenancerequest.Data,message = maintenancerequest.Message });
            }
            return NotFound( new { StatusCode=StatusCodes.Status404NotFound, message = maintenancerequest.Message });   
        }

        [HttpPost("create")]
        public async Task<ActionResult<ApiResponseDTO<GetMaintenanceRequestDto>>> Create([FromBody] CreateMaintenanceRequestCommand command)
        {
             var validationResult = await _createmaintenancecommandvalidator.ValidateAsync(command);
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var response = await Mediator.Send(command);

                        if (response.IsSuccess)
                {
                    return Ok(new
                    {
                        StatusCode = StatusCodes.Status201Created,
                        message = response.Message,
                        errors = "",
                        data = response.Data
                    });
                }

                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    message = response.Message,
                    errors = ""
                });
        }    
        [HttpPut]
        public async Task<IActionResult> Update(UpdateMaintenanceRequestCommand command)
        {
            var validationResult = await _updateMaintenanceRequestCommandValidator.ValidateAsync(command); // Line 105?
            
            if (!validationResult.IsValid)
            {
                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    message = "Validation failed",
                    errors = validationResult.Errors.Select(e => e.ErrorMessage).ToArray()
                });
            }

            var response = await Mediator.Send(command);

            if (response.IsSuccess)
            {
                return Ok(new
                {
                    StatusCode = StatusCodes.Status200OK,
                    message = response.Message,
                    errors = ""
                });
            }

            return BadRequest(new
            {
                StatusCode = StatusCodes.Status400BadRequest,
                message = response.Message,
                errors = ""
            });
        } 

        [HttpGet("GetExistingVendor/{oldUnitId}/{VendorCode}")]
        public async Task<IActionResult> GetExistingVendor(string oldUnitId, string VendorCode)
        {
            if (oldUnitId == null)
            {
                return BadRequest(new { StatusCode = StatusCodes.Status400BadRequest, Message = "Invalid OldUnitId" });
            }
            if (VendorCode =="0")
            {
                return BadRequest(new { StatusCode = StatusCodes.Status400BadRequest, Message = "Invalid VendorCode" });
            }

            var result = await Mediator.Send(new GetExistingVendorDetailsQuery { OldUnitCode = oldUnitId,VendorCode = VendorCode });

            if (result == null || !result.IsSuccess || result.Data == null)
            {
                return NotFound(new { StatusCode = StatusCodes.Status404NotFound, Message = "No Vendor details found" });
            }

            return Ok(new { StatusCode = StatusCodes.Status200OK, Data = result.Data });
        }
        
    }
}