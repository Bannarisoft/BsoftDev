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
using Core.Application.MaintenanceRequest.Command.UpdateMaintenanceRequestStatusCommand;
using Core.Application.MaintenanceRequest.Queries.GetMaintenanceExternalRequest;
using Core.Application.MaintenanceRequest.Command.CreateExternalRequestWorkOrder;
using Core.Application.MaintenanceRequest.Queries.GetExternalRequestById;

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


         [HttpGet("InternalRequest")]         
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
         [HttpGet("ExternalRequest")] 
        public async Task<IActionResult> GetAllMaintenanceExternalRequestAsync([FromQuery] int PageNumber,[FromQuery] int PageSize,[FromQuery] string? SearchTerm = null)
        {
            var maintenanceexternalrequest = await Mediator.Send(
            new GetMaintenanceExternalRequestQuery
            {
                PageNumber = PageNumber, 
                PageSize = PageSize, 
                SearchTerm = SearchTerm
            });
           

            return Ok(new 
            { 
                StatusCode=StatusCodes.Status200OK, 
                data = maintenanceexternalrequest.Data,
                TotalCount = maintenanceexternalrequest.TotalCount,
                PageNumber = maintenanceexternalrequest.PageNumber,
                PageSize = maintenanceexternalrequest.PageSize
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

        [HttpGet("external-requests/by-ids")]
            public async Task<IActionResult> GetExternalRequestsByIds([FromQuery] string ids)
            {
                if (string.IsNullOrWhiteSpace(ids))
                {
                    return BadRequest(new ApiResponseDTO<List<GetExternalRequestByIdDto>>
                    {
                        IsSuccess = false,
                        Message = "No IDs provided.",
                        Data = new List<GetExternalRequestByIdDto>()
                    });
                }

                var parsedIds = ids
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(id => int.TryParse(id, out var parsed) ? parsed : (int?)null)
                    .Where(id => id.HasValue)
                    .Select(id => id.Value)
                    .ToList();

                if (!parsedIds.Any())
                {
                    return BadRequest(new ApiResponseDTO<List<GetExternalRequestByIdDto>>
                    {
                        IsSuccess = false,
                        Message = "No valid IDs found.",
                        Data = new List<GetExternalRequestByIdDto>()
                    });
                }

                var query = new GetExternalRequestsByIdsQuery
                {
                    Ids = parsedIds
                };

                var result = await Mediator.Send(query);

                return Ok(result);
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
            [HttpPost("create-from-request")]
            public async Task<ActionResult> CreateFromExternalRequest(CreateExternalRequestWorkOrderCommand command)
            {
         
                // var command = new CreateExternalRequestWorkOrderCommand { Ids = new List<int> { id } };
               
                var response = await Mediator.Send(command);
             
                if (!response.IsSuccess)
                {
                    return BadRequest(response);
                }    
                return Ok(response);
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


        [HttpPatch("status/{id}")]
            public async Task<IActionResult> UpdateMaintenanceRequestStatus([FromRoute] int id)
            {
                var command = new UpdateMaintenanceRequestStatusCommand
                {
                    Id = id
                };

                var result = await Mediator.Send(command);

                if (!result.IsSuccess)
                    return NotFound(new
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        Message = result.Message
                    });

                return Ok(new
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = result.Message,
                    Data = result.Data
                });
            }

        //    [HttpPatch("{id}/status")]
        //     public async Task<IActionResult> UpdateStatus(  UpdateStatusDto dto)
        //     {
        //         var command = new UpdateMaintenanceRequestStatusCommand
        //         {
        //             Id = dto.Id,
                  
        //         };

        //         var result = await Mediator.Send(command);
        //          return Ok(new
        //             {
        //                 StatusCode = StatusCodes.Status200OK,
        //                 Data = result.Data
        //             });
        //     }
        
    }
}