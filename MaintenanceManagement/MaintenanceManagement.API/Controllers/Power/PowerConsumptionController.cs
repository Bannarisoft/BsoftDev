using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Power.PowerConsumption.Command.CreatePowerConsumption;
using Core.Application.Power.PowerConsumption.Queries;
using Core.Application.Power.PowerConsumption.Queries.GetClosingReaderValueById;
using Core.Application.Power.PowerConsumption.Queries.GetPowerConsumption;
using Core.Application.Power.PowerConsumption.Queries.GetPowerConsumptionById;
using FluentValidation;
using MassTransit.Futures.Contracts;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MaintenanceManagement.API.Controllers.Power
{
    [ApiController]
    [Route("api/[controller]")]
    public class PowerConsumptionController : ApiControllerBase
    {
        private readonly ILogger<PowerConsumptionController> _logger;
        private readonly IValidator<CreatePowerConsumptionCommand> _createpowerconsumptioncommandvalidator;
        private readonly IMediator _mediator;

        public PowerConsumptionController(ILogger<PowerConsumptionController> logger, IMediator mediator, IValidator<CreatePowerConsumptionCommand> createpowerconsumptioncommandvalidator)
        : base(mediator)
        {
            _logger = logger;
            _mediator = mediator;
            _createpowerconsumptioncommandvalidator = createpowerconsumptioncommandvalidator;
        }
        [HttpGet("{id}")]
        [ActionName(nameof(GetFeederSubFeederByIdAsync))]
        public async Task<IActionResult> GetFeederSubFeederByIdAsync(int id)
        {
            var FeederSubFeeder = await Mediator.Send(new GetFeederSubFeederByIdQuery() { FeederTypeId = id });

            if (FeederSubFeeder.IsSuccess)
            {

                return Ok(new { StatusCode = StatusCodes.Status200OK, data = FeederSubFeeder.Data, message = FeederSubFeeder.Message });
            }
            return NotFound(new { StatusCode = StatusCodes.Status404NotFound, message = FeederSubFeeder.Message });
        }
        [HttpGet("GetOpeningReaderValue/{feederId}")]
        [ActionName(nameof(GetOpeningReaderValueIdAsync))]
        public async Task<IActionResult> GetOpeningReaderValueIdAsync(int feederId)
        {
            var FeederopeningReading = await Mediator.Send(new GetClosingReaderValueByIdQuery() { FeederId = feederId });

            if (FeederopeningReading.IsSuccess)
            {

                return Ok(new { StatusCode = StatusCodes.Status200OK, data = FeederopeningReading.Data, message = FeederopeningReading.Message });
            }
            return NotFound(new { StatusCode = StatusCodes.Status404NotFound, message = FeederopeningReading.Message });
        }
        [HttpPost]
        public async Task<IActionResult> CreateAsync(CreatePowerConsumptionCommand createPowerConsumptionCommand)
        {

            // Validate the incoming command
            var validationResult = await _createpowerconsumptioncommandvalidator.ValidateAsync(createPowerConsumptionCommand);
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
            var CreatePowerConsumptionId = await _mediator.Send(createPowerConsumptionCommand);

            if (CreatePowerConsumptionId.IsSuccess)
            {
                _logger.LogInformation($"PowerConsumption {createPowerConsumptionCommand.FeederId} created successfully.");
                return Ok(new
                {
                    StatusCode = StatusCodes.Status201Created,
                    message = CreatePowerConsumptionId.Message,
                    data = CreatePowerConsumptionId.Data
                });
            }
            _logger.LogWarning($"PowerConsumption {createPowerConsumptionCommand.FeederId} Creation failed.");
            return BadRequest(new
            {
                StatusCode = StatusCodes.Status400BadRequest,
                message = CreatePowerConsumptionId.Message
            });

        }
        [HttpGet]
        public async Task<IActionResult> GetAllPowerConsumptionAsync([FromQuery] int PageNumber, [FromQuery] int PageSize, [FromQuery] string? SearchTerm = null)
        {
            var FeederGroup = await Mediator.Send(
             new GetPowerConsumptionQuery
             {
                 PageNumber = PageNumber,
                 PageSize = PageSize,
                 SearchTerm = SearchTerm
             });
            return Ok(new
            {
                StatusCode = StatusCodes.Status200OK,
                data = FeederGroup.Data,
                TotalCount = FeederGroup.TotalCount,
                PageNumber = FeederGroup.PageNumber,
                PageSize = FeederGroup.PageSize
            });
        }
        
        [HttpGet("PowerConsumption/{id}")]
        [ActionName(nameof(GetByIdAsync))]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var powerconsumption = await Mediator.Send(new GetPowerConsumptionByIdQuery() { Id = id });

            if (powerconsumption.IsSuccess)
            {

                return Ok(new { StatusCode = StatusCodes.Status200OK, data = powerconsumption.Data, message = powerconsumption.Message });
            }
            return NotFound(new { StatusCode = StatusCodes.Status404NotFound, message = powerconsumption.Message });
        }

        


    }
}
