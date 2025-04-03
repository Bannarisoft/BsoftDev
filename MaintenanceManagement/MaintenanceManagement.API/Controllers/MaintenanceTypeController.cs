using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.MaintenanceType.Command.CreateMaintenanceType;
using Core.Application.MaintenanceType.Command.DeleteMaintenanceType;
using Core.Application.MaintenanceType.Command.UpdateMaintenanceType;
using Core.Application.MaintenanceType.Queries.GetMaintenanceType;
using Core.Application.MaintenanceType.Queries.GetMaintenanceTypeAutoComplete;
using Core.Application.MaintenanceType.Queries.GetMaintenanceTypeById;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MaintenanceManagement.API.Controllers
{
     [Route("api/[controller]")]
    public class MaintenanceTypeController : ApiControllerBase
    {
        private readonly ILogger<MaintenanceTypeController> _logger;
          private readonly IMediator _mediator;
        private readonly IValidator<CreateMaintenanceTypeCommand> _createmaintenancecommandvalidator;
        private readonly IValidator<UpdateMaintenanceTypeCommand> _updatemaintenanceommandvalidator;
        private readonly IValidator<DeleteMaintenanceTypeCommand> _deletemaintenancecommandvalidator;


        public MaintenanceTypeController(ILogger<MaintenanceTypeController> logger,IValidator<CreateMaintenanceTypeCommand> createmaintenancecommandvalidator,IValidator<UpdateMaintenanceTypeCommand> updatemaintenanceommandvalidator,IValidator<DeleteMaintenanceTypeCommand> deletemaintenancecommandvalidator,IMediator mediator)
        : base(mediator)
        {
            _logger = logger;
            _mediator=mediator;
            _createmaintenancecommandvalidator=createmaintenancecommandvalidator;
            _updatemaintenanceommandvalidator=updatemaintenanceommandvalidator;
            _deletemaintenancecommandvalidator=deletemaintenancecommandvalidator;
        }

         [HttpGet]
        public async Task<IActionResult> GetAllMaintenanceTypeAsync([FromQuery] int PageNumber,[FromQuery] int PageSize,[FromQuery] string? SearchTerm = null)
        {
           var maintenancetype = await Mediator.Send(
            new GetMaintenanceTypeQuery
            {
                PageNumber = PageNumber, 
                PageSize = PageSize, 
                SearchTerm = SearchTerm
            });
            return Ok( new 
            { 
                StatusCode=StatusCodes.Status200OK, 
                data = maintenancetype.Data,
                TotalCount = maintenancetype.TotalCount,
                PageNumber = maintenancetype.PageNumber,
                PageSize = maintenancetype.PageSize
                });
        }

        [HttpGet("by-name")]
        public async Task<IActionResult> GetMaintenanceType([FromQuery] string? Typename)
        {
        var maintenancetype = await Mediator.Send(new GetMaintenanceTypeAutoCompleteQuery 
        { 
                SearchPattern = Typename ?? string.Empty 
        });

        return Ok(new { StatusCode = StatusCodes.Status200OK, data = maintenancetype.Data});
        }

        [HttpGet("{id}")]
        [ActionName(nameof(GetByIdAsync))]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var maintenancetype = await Mediator.Send(new GetMaintenanceTypeByIdQuery() { Id = id});
          
            if(maintenancetype.IsSuccess)
            {
                
              return Ok(new { StatusCode=StatusCodes.Status200OK, data = maintenancetype.Data,message = maintenancetype.Message });
            }
            return NotFound( new { StatusCode=StatusCodes.Status404NotFound, message = maintenancetype.Message });   
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync(CreateMaintenanceTypeCommand createMaintenanceTypeCommand)
        {
            
            // Validate the incoming command
            var validationResult = await _createmaintenancecommandvalidator.ValidateAsync(createMaintenanceTypeCommand);
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

            // Process the command
            var CreatedMaintenanceId = await _mediator.Send(createMaintenanceTypeCommand);

            if (CreatedMaintenanceId.IsSuccess)
            {
            _logger.LogInformation($"MaintenanceType {createMaintenanceTypeCommand.TypeName} created successfully.");
            return Ok(new
            {
                StatusCode = StatusCodes.Status201Created,
                message =CreatedMaintenanceId.Message,
                data = CreatedMaintenanceId.Data
            });
            }
            _logger.LogWarning($"MaintenanceType {createMaintenanceTypeCommand.TypeName} Creation failed.");
            return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    message = CreatedMaintenanceId.Message
                });
        
        }
            [HttpPut]
            public async Task<IActionResult> UpdateAsync(UpdateMaintenanceTypeCommand updateMaintenanceTypeCommand)
            {
            
                    // Validate the incoming command
                    var validationResult = await _updatemaintenanceommandvalidator.ValidateAsync(updateMaintenanceTypeCommand);
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

                    var updatedmaintenance = await _mediator.Send(updateMaintenanceTypeCommand);

                    if (updatedmaintenance.IsSuccess)
                    {
                        _logger.LogInformation($"MaintenanceType {updateMaintenanceTypeCommand.TypeName} updated successfully.");
                    return Ok(new
                        {
                            message = updatedmaintenance.Message,
                            statusCode = StatusCodes.Status200OK
                        });
                    }
                    _logger.LogWarning($"MaintenanceType {updateMaintenanceTypeCommand.TypeName} Update failed.");
                    return NotFound(new
                    {
                        message =updatedmaintenance.Message,
                        statusCode = StatusCodes.Status404NotFound
                    });   
            }

            [HttpDelete]
            public async Task<IActionResult> DeleteMaintenanceTypeAsync(int id)
            {
                // Validate the incoming command
                    var validationResult = await _deletemaintenancecommandvalidator.ValidateAsync(new DeleteMaintenanceTypeCommand { Id = id });
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

                    // Process the delete command
                    var result = await _mediator.Send(new DeleteMaintenanceTypeCommand { Id = id });

                    if (result.IsSuccess) 
                    {
                        _logger.LogInformation($"MaintenanceType {id} deleted successfully.");
                        return Ok(new
                        {
                            message = result.Message,
                            statusCode = StatusCodes.Status200OK
                        });
                        
                    }
                    _logger.LogWarning($"MaintenanceType {id} Not Found or Invalid MaintenanceTypeId.");
                    return NotFound(new
                    {
                        message = result.Message,
                        statusCode = StatusCodes.Status404NotFound
                    });
            
            }

       
    }
}