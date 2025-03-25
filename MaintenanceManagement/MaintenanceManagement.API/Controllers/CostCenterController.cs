using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.CostCenter.Command.CreateCostCenter;
using Core.Application.CostCenter.Command.DeleteCostCenter;
using Core.Application.CostCenter.Command.UpdateCostCenter;
using Core.Application.CostCenter.Queries.GetCostCenter;
using Core.Application.CostCenter.Queries.GetCostCenterAutoComplete;
using Core.Application.CostCenter.Queries.GetCostCenterById;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MaintenanceManagement.API.Controllers
{
     [Route("api/[controller]")]
    public class CostCenterController :  ApiControllerBase
    {
        private readonly ILogger<CostCenterController> _logger;
        private readonly IMediator _mediator;
        private readonly IValidator<CreateCostCenterCommand> _createcostcentercommandvalidator;
        private readonly IValidator<UpdateCostCenterCommand> _updatecostcentercommandvalidator;
        private readonly IValidator<DeleteCostCenterCommand> _deletecostcentercommandvalidator;

        public CostCenterController(ILogger<CostCenterController> logger,IMediator mediator,IValidator<CreateCostCenterCommand> createcostcentercommandvalidator,IValidator<UpdateCostCenterCommand> updatecostcentercommandvalidator,IValidator<DeleteCostCenterCommand> deletecostcentercommandvalidator)
        : base(mediator)
        {
            _logger = logger;
            _mediator=mediator;
            _createcostcentercommandvalidator=createcostcentercommandvalidator;
            _updatecostcentercommandvalidator=updatecostcentercommandvalidator;
            _deletecostcentercommandvalidator=deletecostcentercommandvalidator;
        }
        
        [HttpGet]
        public async Task<IActionResult> GetAllCostcenterAsync([FromQuery] int PageNumber,[FromQuery] int PageSize,[FromQuery] string? SearchTerm = null)
        {
           var costcenter = await Mediator.Send(
            new GetCostCenterQuery
            {
                PageNumber = PageNumber, 
                PageSize = PageSize, 
                SearchTerm = SearchTerm
            });
            return Ok( new 
            { 
                StatusCode=StatusCodes.Status200OK, 
                data = costcenter.Data,
                TotalCount = costcenter.TotalCount,
                PageNumber = costcenter.PageNumber,
                PageSize = costcenter.PageSize
                });
        }

        [HttpGet("by-name")]
        public async Task<IActionResult> GetCostcenter([FromQuery] string? CostCenterName)
        {
        var costcenter = await Mediator.Send(new GetCostCenterAutoCompleteQuery 
        { 
                SearchPattern = CostCenterName ?? string.Empty 
        });

        return Ok(new { StatusCode = StatusCodes.Status200OK, data = costcenter.Data});
        }

        [HttpGet("{id}")]
        [ActionName(nameof(GetByIdAsync))]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var costcenter = await Mediator.Send(new GetCostCenterByIdQuery() { Id = id});
          
            if(costcenter.IsSuccess)
            {
                
              return Ok(new { StatusCode=StatusCodes.Status200OK, data = costcenter.Data,message = costcenter.Message });
            }
            return NotFound( new { StatusCode=StatusCodes.Status404NotFound, message = costcenter.Message });   
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync(CreateCostCenterCommand createCostCenterCommand)
        {
            
            // Validate the incoming command
            var validationResult = await _createcostcentercommandvalidator.ValidateAsync(createCostCenterCommand);
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
            var CreatedCostCenterId = await _mediator.Send(createCostCenterCommand);

            if (CreatedCostCenterId.IsSuccess)
            {
            _logger.LogInformation($"CostCenter {createCostCenterCommand.CostCenterCode} created successfully.");
            return Ok(new
            {
                StatusCode = StatusCodes.Status201Created,
                message =CreatedCostCenterId.Message,
                data = CreatedCostCenterId.Data
            });
            }
            _logger.LogWarning($"CostCenter {createCostCenterCommand.CostCenterCode} Creation failed.");
            return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    message = CreatedCostCenterId.Message
                });
        
        }
            [HttpPut]
            public async Task<IActionResult> UpdateAsync(UpdateCostCenterCommand updateCostCenterCommand)
            {
            
                    // Validate the incoming command
                    var validationResult = await _updatecostcentercommandvalidator.ValidateAsync(updateCostCenterCommand);
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

                    var updatedcostcenter = await _mediator.Send(updateCostCenterCommand);

                    if (updatedcostcenter.IsSuccess)
                    {
                        _logger.LogInformation($"CostCenter {updateCostCenterCommand.CostCenterName} updated successfully.");
                    return Ok(new
                        {
                            message = updatedcostcenter.Message,
                            statusCode = StatusCodes.Status200OK
                        });
                    }
                    _logger.LogWarning($"CostCenter {updateCostCenterCommand.CostCenterName} Update failed.");
                    return NotFound(new
                    {
                        message =updatedcostcenter.Message,
                        statusCode = StatusCodes.Status404NotFound
                    });   
            }

            [HttpDelete]
            public async Task<IActionResult> DeleteCostCenterAsync(int id)
            {
                // Validate the incoming command
                    var validationResult = await _deletecostcentercommandvalidator.ValidateAsync(new DeleteCostCenterCommand { Id = id });
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
                    var result = await _mediator.Send(new DeleteCostCenterCommand { Id = id });

                    if (result.IsSuccess) 
                    {
                        _logger.LogInformation($"CostCenter {id} deleted successfully.");
                        return Ok(new
                        {
                            message = result.Message,
                            statusCode = StatusCodes.Status200OK
                        });
                        
                    }
                    _logger.LogWarning($"CostCenter {id} Not Found or Invalid CostCenterId.");
                    return NotFound(new
                    {
                        message = result.Message,
                        statusCode = StatusCodes.Status404NotFound
                    });
            
            }
                
    }
}