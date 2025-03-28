using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.MaintenanceCategory.Command.CreateMaintenanceCategory;
using Core.Application.MaintenanceCategory.Command.DeleteMaintenanceCategory;
using Core.Application.MaintenanceCategory.Command.UpdateMaintenanceCategory;
using Core.Application.MaintenanceCategory.Queries.GetMaintenanceCategory;
using Core.Application.MaintenanceCategory.Queries.GetMaintenanceCategoryAutoComplete;
using Core.Application.MaintenanceCategory.Queries.GetMaintenanceCategoryById;
using Core.Application.MaintenanceType.Queries.GetMaintenanceType;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MaintenanceManagement.API.Controllers
{
    [Route("api/[controller]")]
    public class MaintenanceCategoryController : ApiControllerBase
    {
        private readonly ILogger<MaintenanceCategoryController> _logger;
        private readonly IMediator _mediator;
        private readonly IValidator<CreateMaintenanceCategoryCommand> _createmaintenancecategorycommandvalidator;
        private readonly IValidator<UpdateMaintenanceCategoryCommand> _updatemaintenancecategoryommandvalidator;
        private readonly IValidator<DeleteMaintenanceCategoryCommand> _deletemaintenancecategorycommandvalidator;


        public MaintenanceCategoryController(ILogger<MaintenanceCategoryController> logger,IMediator mediator,IValidator<CreateMaintenanceCategoryCommand> createmaintenancecategorycommandvalidator,IValidator<UpdateMaintenanceCategoryCommand> updatemaintenancecategoryommandvalidator,IValidator<DeleteMaintenanceCategoryCommand> deletemaintenancecategorycommandvalidator)
        : base(mediator)
        {
            _logger = logger;
            _mediator=mediator;
            _createmaintenancecategorycommandvalidator=createmaintenancecategorycommandvalidator;
            _updatemaintenancecategoryommandvalidator=updatemaintenancecategoryommandvalidator; 
            _deletemaintenancecategorycommandvalidator=deletemaintenancecategorycommandvalidator;
        }
         [HttpGet]
        public async Task<IActionResult> GetAllMaintenanceCategoryAsync([FromQuery] int PageNumber,[FromQuery] int PageSize,[FromQuery] string? SearchTerm = null)
        {
           var maintenanceCategory = await Mediator.Send(
            new GetMaintenanceCategoryQuery
            {
                PageNumber = PageNumber, 
                PageSize = PageSize, 
                SearchTerm = SearchTerm
            });
            return Ok( new 
            { 
                StatusCode=StatusCodes.Status200OK, 
                data = maintenanceCategory.Data,
                TotalCount = maintenanceCategory.TotalCount,
                PageNumber = maintenanceCategory.PageNumber,
                PageSize = maintenanceCategory.PageSize
                });
        }

        [HttpGet("by-name")]
        public async Task<IActionResult> GetMaintenanceCategory([FromQuery] string? CateggoryName)
        {
        var maintenancetype = await Mediator.Send(new GetMaintenanceCategoryAutoCompleteQuery 
        { 
                SearchPattern = CateggoryName ?? string.Empty 
        });

        return Ok(new { StatusCode = StatusCodes.Status200OK, data = maintenancetype.Data});
        }

        [HttpGet("{id}")]
        [ActionName(nameof(GetByIdAsync))]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var maintenancecategory = await Mediator.Send(new GetMaintenanceCategoryByIdQuery() { Id = id});
          
            if(maintenancecategory.IsSuccess)
            {
                
              return Ok(new { StatusCode=StatusCodes.Status200OK, data = maintenancecategory.Data,message = maintenancecategory.Message });
            }
            return NotFound( new { StatusCode=StatusCodes.Status404NotFound, message = maintenancecategory.Message });   
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync(CreateMaintenanceCategoryCommand createMaintenanceCategoryCommand)
        {
            
            // Validate the incoming command
            var validationResult = await _createmaintenancecategorycommandvalidator.ValidateAsync(createMaintenanceCategoryCommand);
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
            var CreatedMaintenanceId = await _mediator.Send(createMaintenanceCategoryCommand);

            if (CreatedMaintenanceId.IsSuccess)
            {
            _logger.LogInformation($"MaintenanceCategory {createMaintenanceCategoryCommand.CategoryName} created successfully.");
            return Ok(new
            {
                StatusCode = StatusCodes.Status201Created,
                message =CreatedMaintenanceId.Message,
                data = CreatedMaintenanceId.Data
            });
            }
            _logger.LogWarning($"MaintenanceCategory {createMaintenanceCategoryCommand.CategoryName} Creation failed.");
            return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    message = CreatedMaintenanceId.Message
                });
        
        }
            [HttpPut]
            public async Task<IActionResult> UpdateAsync(UpdateMaintenanceCategoryCommand updateMaintenanceCategoryCommand)
            {
            
                    // Validate the incoming command
                    var validationResult = await _updatemaintenancecategoryommandvalidator.ValidateAsync(updateMaintenanceCategoryCommand);
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

                    var updatedmaintenance = await _mediator.Send(updateMaintenanceCategoryCommand);

                    if (updatedmaintenance.IsSuccess)
                    {
                        _logger.LogInformation($"MaintenanceCategory {updateMaintenanceCategoryCommand.CategoryName} updated successfully.");
                    return Ok(new
                        {
                            message = updatedmaintenance.Message,
                            statusCode = StatusCodes.Status200OK
                        });
                    }
                    _logger.LogWarning($"MaintenanceCategory {updateMaintenanceCategoryCommand.CategoryName} Update failed.");
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
                    var validationResult = await _deletemaintenancecategorycommandvalidator.ValidateAsync(new DeleteMaintenanceCategoryCommand { Id = id });
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
                    var result = await _mediator.Send(new DeleteMaintenanceCategoryCommand { Id = id });

                    if (result.IsSuccess) 
                    {
                        _logger.LogInformation($"MaintenanceCategory {id} deleted successfully.");
                        return Ok(new
                        {
                            message = result.Message,
                            statusCode = StatusCodes.Status200OK
                        });
                        
                    }
                    _logger.LogWarning($"MaintenanceCategory {id} Not Found or Invalid MaintenanceCategoryId.");
                    return NotFound(new
                    {
                        message = result.Message,
                        statusCode = StatusCodes.Status404NotFound
                    });
            
            }

      
    }
}