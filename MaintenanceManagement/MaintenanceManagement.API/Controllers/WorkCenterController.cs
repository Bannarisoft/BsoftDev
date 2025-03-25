using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.WorkCenter.Command.CreateWorkCenter;
using Core.Application.WorkCenter.Command.DeleteWorkCenter;
using Core.Application.WorkCenter.Command.UpdateWorkCenter;
using Core.Application.WorkCenter.Queries.GetWorkCenter;
using Core.Application.WorkCenter.Queries.GetWorkCenterAutoComplete;
using Core.Application.WorkCenter.Queries.GetWorkCenterById;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MaintenanceManagement.API.Controllers
{
    [Route("api/[controller]")]

    public class WorkCenterController : ApiControllerBase
    {
        private readonly ILogger<CostCenterController> _logger;
        private readonly IMediator _mediator;
        private readonly IValidator<CreateWorkCenterCommand> _createworkcentercommandvalidator;
        private readonly IValidator<UpdateWorkCenterCommand> _updateworkcentercommandvalidator;
        private readonly IValidator<DeleteWorkCenterCommand> _deleteworkcentercommandvalidator;


        
        public WorkCenterController(ILogger<CostCenterController> logger,IMediator mediator,IValidator<CreateWorkCenterCommand> createworkcentercommandvalidator,IValidator<UpdateWorkCenterCommand> updateworkcentercommandvalidator,IValidator<DeleteWorkCenterCommand> deleteworkcentercommandvalidator)
        : base(mediator)
        {
            _logger = logger;
            _mediator=mediator;
            _createworkcentercommandvalidator=createworkcentercommandvalidator;
            _updateworkcentercommandvalidator=updateworkcentercommandvalidator;
            _deleteworkcentercommandvalidator=deleteworkcentercommandvalidator;
        }

         [HttpGet]
        public async Task<IActionResult> GetAllWorkcenterAsync([FromQuery] int PageNumber,[FromQuery] int PageSize,[FromQuery] string? SearchTerm = null)
        {
           var workcenter = await Mediator.Send(
            new GetWorkCenterQuery
            {
                PageNumber = PageNumber, 
                PageSize = PageSize, 
                SearchTerm = SearchTerm
            });
            return Ok( new 
            { 
                StatusCode=StatusCodes.Status200OK, 
                data = workcenter.Data,
                TotalCount = workcenter.TotalCount,
                PageNumber = workcenter.PageNumber,
                PageSize = workcenter.PageSize
                });
        }
        [HttpGet("by-name")]
        public async Task<IActionResult> GetWorkcenter([FromQuery] string? WorkCenterName)
        {
        var workcenter = await Mediator.Send(new GetWorkCenterAutoCompleteQuery 
        { 
                SearchPattern = WorkCenterName ?? string.Empty 
        });

        return Ok(new { StatusCode = StatusCodes.Status200OK, data = workcenter.Data});
        }

        [HttpGet("{id}")]
        [ActionName(nameof(GetByIdAsync))]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var workcenter = await Mediator.Send(new GetWorkCenterByIdQuery() { Id = id});
          
            if(workcenter.IsSuccess)
            {
                
              return Ok(new { StatusCode=StatusCodes.Status200OK, data = workcenter.Data,message = workcenter.Message });
            }
            return NotFound( new { StatusCode=StatusCodes.Status404NotFound, message = workcenter.Message });   
        }
        [HttpPost]
        public async Task<IActionResult> CreateAsync(CreateWorkCenterCommand createWorkCenterCommand)
        {
            
            // Validate the incoming command
            var validationResult = await _createworkcentercommandvalidator.ValidateAsync(createWorkCenterCommand);
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
            var CreatedWorkCenterId = await _mediator.Send(createWorkCenterCommand);

            if (CreatedWorkCenterId.IsSuccess)
            {
            _logger.LogInformation($"WorkCenter {createWorkCenterCommand.WorkCenterCode} created successfully.");
            return Ok(new
            {
                StatusCode = StatusCodes.Status201Created,
                message =CreatedWorkCenterId.Message,
                data = CreatedWorkCenterId.Data
            });
            }
            _logger.LogWarning($"WorkCenter {createWorkCenterCommand.WorkCenterCode} Creation failed.");
            return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    message = CreatedWorkCenterId.Message
                });
        
        }
            [HttpPut]
            public async Task<IActionResult> UpdateAsync(UpdateWorkCenterCommand updateWorkCenterCommand)
            {
            
                    // Validate the incoming command
                    var validationResult = await _updateworkcentercommandvalidator.ValidateAsync(updateWorkCenterCommand);
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

                    var updatedworkcenter = await _mediator.Send(updateWorkCenterCommand);

                    if (updatedworkcenter.IsSuccess)
                    {
                        _logger.LogInformation($"WorkCenter {updateWorkCenterCommand.WorkCenterName} updated successfully.");
                    return Ok(new
                        {
                            message = updatedworkcenter.Message,
                            statusCode = StatusCodes.Status200OK
                        });
                    }
                    _logger.LogWarning($"WorkCenter {updateWorkCenterCommand.WorkCenterName} Update failed.");
                    return NotFound(new
                    {
                        message =updatedworkcenter.Message,
                        statusCode = StatusCodes.Status404NotFound
                    });   
            }
            [HttpDelete]
            public async Task<IActionResult> DeleteWorkCenterAsync(int id)
            {
                // Validate the incoming command
                    var validationResult = await _deleteworkcentercommandvalidator.ValidateAsync(new DeleteWorkCenterCommand { Id = id });
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
                    var result = await _mediator.Send(new DeleteWorkCenterCommand { Id = id });

                    if (result.IsSuccess) 
                    {
                        _logger.LogInformation($"WorkCenter {id} deleted successfully.");
                        return Ok(new
                        {
                            message = result.Message,
                            statusCode = StatusCodes.Status200OK
                        });
                        
                    }
                    _logger.LogWarning($"WorkCenter {id} Not Found or Invalid WorkCenterId.");
                    return NotFound(new
                    {
                        message = result.Message,
                        statusCode = StatusCodes.Status404NotFound
                    });
            
            }
      
    }
}