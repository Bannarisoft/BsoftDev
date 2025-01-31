using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using BSOFT.Infrastructure.Data;
using Core.Application.Common.Interfaces.IFinancialYear;
using Core.Application.FinancialYear.Command.CreateFinancialYear;
using Core.Application.FinancialYear.Command.UpdateFinancialYear;
using Core.Application.FinancialYear.Queries.GetFinancialYear;
using Core.Application.FinancialYear.Queries.GetFinancialYearGetById;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BSOFT.API.Controllers
{
    [Route("[controller]")]
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
       public async Task<IActionResult> GetAllFinancialYearAsync()
        {       
           _logger.LogInformation("Fetching All FinancialYear Request started.");
           
            var financialyr =await Mediator.Send(new GetFinancialYearQuery());
           if (financialyr.Data == null || !financialyr.Data.Any())
            {
               
                _logger.LogInformation("No FinancialYear records found in the database. Total count: {Count}", financialyr?.Data?.Count ?? 0);
                 return NotFound(new
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        message = financialyr.Message
                    });
             }           
          
            return Ok(new
            {
                StatusCode = StatusCodes.Status200OK,
                data = financialyr.Data
            });
        }

 [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            _logger.LogInformation("Fetching FinancialYear with ID {Id} request started.", id);
            var financialyr = await Mediator.Send(new GetFinancialYearByIdQuery  { FYId = id });
            if (financialyr == null || financialyr.Data == null)
            {
                _logger.LogInformation("FinancialYear with ID {Id} not found in the database.", id);
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


         [HttpPost]
         [Route("Create")]
        public async Task<IActionResult>CreateAsync([FromBody] CreateFinancialYearCommand command)
        {
                _logger.LogInformation("Create Financial Year request started with data: {@Command}", command);

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
                _logger.LogInformation("Create Financial Year request succeeded. Financial Year created with ID: {FYId}", createFinancialYear.Data);

                return Ok(new
                {
                    StatusCode = StatusCodes.Status201Created,
                    Message = createFinancialYear.Message,
                    Data = createFinancialYear.Data
                });
            }
            _logger.LogWarning("Create FinancialYear request failed. Reason: {Message}", createFinancialYear.Message);

            return BadRequest(new
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Message = createFinancialYear.Message
            });
            
               
        }
        [HttpPut("update")]
   
        public async Task<IActionResult> UpdateAsync([FromBody] UpdateFinancialYearCommand command)
        {
                    if (command == null)
            {
                _logger.LogError("UpdateFinancialYearCommand is null.");
                return BadRequest("Invalid request: UpdateFinancialYearCommand is required.");
            }
             _logger.LogInformation("Update Financial Year request started with data: {@Command}", command);

           
             var financialyearresult = await Mediator.Send(new GetFinancialYearByIdQuery { FYId = command.Id });
            if (financialyearresult == null)
            {
                _logger.LogWarning("Financial Year with ID {FYId} not found.", command.Id);

                return NotFound(new
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = "Financial Year not found"
                });
            }


                var validationResult = await _updateFinancialYearCommandValidator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Validation failed for Update Financial Year request. Errors: {@Errors}", validationResult.Errors);

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
                _logger.LogInformation("Financial Year  with ID {FYId} updated successfully.", command.Id);

                return Ok(new
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Financial Year  updated successfully"
                  
                });
            }

            _logger.LogWarning("Failed to update Financial Year  with ID {FYId}. Reason: {Message}", command.Id, updateResult.Message);

            return BadRequest(new
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Message = updateResult.Message
            });
        

            }




    }

}