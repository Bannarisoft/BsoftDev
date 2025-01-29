using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using BSOFT.Infrastructure.Data;
using Core.Application.Common.Interfaces.IFinancialYear;
using Core.Application.FinancialYear.Command.CreateFinancialYear;
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

        public FinancialYearController(ISender mediator ,ApplicationDbContext dbContext , ILogger<FinancialYearController> logger) : base(mediator)
        {
            _dbContext = dbContext;
            _logger = logger;

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
                _logger.LogInformation("Create Financial Year request succeeded. Financial Year created with ID: {FYId}", createFinancialYear.Data.Id);

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


    }

}