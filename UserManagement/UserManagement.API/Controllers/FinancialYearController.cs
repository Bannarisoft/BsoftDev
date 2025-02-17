using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using UserManagement.Infrastructure.Data;
using Core.Application.Common.Interfaces.IFinancialYear;
using Core.Application.FinancialYear.Command.CreateFinancialYear;
using Core.Application.FinancialYear.Command.DeleteFinancialYear;
using Core.Application.FinancialYear.Command.UpdateFinancialYear;
using Core.Application.FinancialYear.Queries.GetFinancialYear;
using Core.Application.FinancialYear.Queries.GetFinancialYearGetById;
using Core.Application.GetFinancialYearYear.Queries.GetFinancialYear;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Core.Application.FinancialYear.Queries.GetFinancialYearAutoComplete;

namespace UserManagement.API.Controllers
{
   [Route("api/[controller]")]
    [ApiController]
    public class FinancialYearController : ApiControllerBase
    {
      
        
          private readonly ApplicationDbContext _dbContext;
          private readonly ILogger<FinancialYearController> _logger;
          private readonly IValidator<CreateFinancialYearCommand> _createFinancialYearCommandValidator;
          private readonly IValidator<UpdateFinancialYearCommand> _updateFinancialYearCommandValidator;
   

        public FinancialYearController(ISender mediator ,ApplicationDbContext dbContext , ILogger<FinancialYearController> logger , IValidator<CreateFinancialYearCommand> createFinancialYearCommandValidator, IValidator<UpdateFinancialYearCommand> updateFinancialYearCommandValidator) : base(mediator)
        {
            _dbContext = dbContext;
            _logger = logger;
            _createFinancialYearCommandValidator = createFinancialYearCommandValidator;
            _updateFinancialYearCommandValidator =updateFinancialYearCommandValidator;

        }
        [HttpGet]
        public async Task<IActionResult> GetAllFinancialYearAsync([FromQuery] int PageNumber, [FromQuery] int PageSize, [FromQuery] string? SearchTerm = null)
            {
                _logger.LogInformation("Starting GetAllFinancialYearAsync request.");

                var financialYears = await Mediator.Send(new GetFinancialYearQuery
                {
                    PageNumber = PageNumber,
                    PageSize = PageSize,
                    SearchTerm = SearchTerm
                });

                if (financialYears == null || financialYears.Data == null || !financialYears.Data.Any())
                {
                    _logger.LogWarning("No financial year records found.");
                    return NotFound(new
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        Message = "No financial year records found."
                    });
                }

                _logger.LogInformation("Financial year records retrieved successfully.");
                return Ok(new
                {
                    StatusCode = StatusCodes.Status200OK,
                    Data = financialYears.Data,
                    TotalCount = financialYears.TotalCount,
                    PageNumber = financialYears.PageNumber,
                    PageSize = financialYears.PageSize
                });
            }

   

       [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            _logger.LogInformation($"Fetching FinancialYear with ID {id} request started." );
            var financialyr = await Mediator.Send(new GetFinancialYearByIdQuery  { Id = id });
            if (financialyr == null || financialyr.Data == null)
            {
                _logger.LogInformation($"FinancialYear with ID {id} not found in the database.");
                return NotFound(new
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = financialyr?.Message ?? "FinancialYear not found."
                });
            }
            return Ok(new
            {
                StatusCode = StatusCodes.Status200OK,
                Data = financialyr.Data
            });
        } 

        [HttpGet("by-Year")]
        public async Task<IActionResult> GetAllFinancialYearAutoCompleteSearchAsync([FromQuery] string? year)
        {
            _logger.LogInformation($"Starting GetAllFinancialYearAutoCompleteSearchAsync with search pattern: {year}");

            var query = new GetFinancialYearAutoCompleteQuery { SearchTerm = year ?? string.Empty };
            var result = await Mediator.Send(query);

            if (result.IsSuccess)
            {
                _logger.LogInformation($"Financial years found for search pattern: {year}. Returning data.");

                return Ok(new
                {
                    StatusCode = StatusCodes.Status200OK,
                    Data = result.Data
                });
            }

            _logger.LogWarning($"No financial years found for search pattern: {year}");

            return NotFound(new
            {
                StatusCode = StatusCodes.Status404NotFound,
                Message = "No matching financial years found."
            });
        }

       

        [HttpPost]
         
        public async Task<IActionResult>CreateAsync([FromBody] CreateFinancialYearCommand command)
        {
                _logger.LogInformation($"Create Financial Year request started with data: {command}");

            // Validate the command
            var validationResult = await _createFinancialYearCommandValidator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Validation failed for Create Financial Year request. Errors: {@Errors}", validationResult.Errors);

                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "Validation failed",
                    Errors = validationResult.Errors.Select(e => e.ErrorMessage).ToArray()
                });
            }

            // Process the command
            var createFinancialYear = await Mediator.Send(command);
            if (createFinancialYear.IsSuccess)
            {
                _logger.LogInformation($"Create Financial Year request succeeded. Financial Year created with ID: {createFinancialYear.Data.Id}");

                return Ok(new
                {
                    StatusCode = StatusCodes.Status201Created,
                    Message = createFinancialYear.Message,
                    Data = createFinancialYear.Data
                });
            }
            _logger.LogWarning($"Create FinancialYear request failed. Reason: {createFinancialYear.Message}" );

            return BadRequest(new
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Message = createFinancialYear.Message
            });
            
               
        }

        [HttpPut]   
        public async Task<IActionResult> UpdateAsync([FromBody] UpdateFinancialYearCommand command)
        {
                    if (command == null)
            {
                _logger.LogError("UpdateFinancialYearCommand is null.");
                return BadRequest("Invalid request: UpdateFinancialYearCommand is required.");
            }
             _logger.LogInformation($"Update Financial Year request started with data: {command}" );

           
             var financialyearresult = await Mediator.Send(new GetFinancialYearByIdQuery { Id = command.Id });
            if (financialyearresult == null)
            {
                _logger.LogWarning($"Financial Year with ID {command.Id} not found.");

                return NotFound(new
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = "Financial Year not found"
                });
            }


                var validationResult = await _updateFinancialYearCommandValidator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning($"Validation failed for Update Financial Year request. Errors: {validationResult.Errors}" );

                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "Validation failed",
                    Errors = validationResult.Errors.Select(e => e.ErrorMessage).ToArray()
                });
            }

                      if (command == null)
            {
                _logger.LogError("Command is null before sending to Mediator.");
                return BadRequest("Command is null before sending to Mediator.");
            }
            // Update the department
            var updateResult = await Mediator.Send(command);
            if (updateResult.IsSuccess)
            {
                _logger.LogInformation($"Financial Year  with ID {command.Id} updated successfully." );

                return Ok(new
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Financial Year  updated successfully"
                  
                });
            }

            _logger.LogWarning($"Failed to update Financial Year  with ID {command.Id}. Reason: {updateResult.Message}" );

            return BadRequest(new
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Message = updateResult.Message
            });
        

            }


             [HttpDelete("{id}")]   
             
            public async Task<IActionResult> Delete(int id )
            {
            _logger.LogInformation($"Delete FinancialYear Command request started with ID: {id}");

                // Check if the department exists
                var department = await Mediator.Send(new GetFinancialYearByIdQuery { Id = id });
                if (department == null)
                {
                    _logger.LogWarning($"FinancialYear  with ID {id} not found." );

                    return NotFound(new
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        Message = "FinancialYear not found"
                    });
                }

                _logger.LogInformation($"FinancialYear with ID {id} found. Proceeding with deletion.");

                // Attempt to delete the department
                var result = await Mediator.Send( new DeleteFinancialYearCommand { Id=id} );

                if (result.IsSuccess)
                {
                    _logger.LogInformation($"FinancialYear with ID {id} deleted successfully." );

                    return Ok(new
                    {
                        Message = result.Message,
                        StatusCode = StatusCodes.Status200OK
                      
                    });
                }

                _logger.LogWarning($"Failed to delete FinancialYear with ID {id}. Reason: {result.Message}");

                return BadRequest(new
                {
                    Message = result.Message,
                    StatusCode = StatusCodes.Status400BadRequest
                });


     
        }

      


    }

}