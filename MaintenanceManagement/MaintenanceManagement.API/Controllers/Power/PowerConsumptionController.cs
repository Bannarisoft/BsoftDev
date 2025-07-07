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
        
        private readonly IMediator _mediator;

        public PowerConsumptionController(IMediator mediator)
        : base(mediator)
        {
            
            _mediator = mediator;
            
        }
        [HttpGet("{id}")]
        [ActionName(nameof(GetFeederSubFeederByIdAsync))]
        public async Task<IActionResult> GetFeederSubFeederByIdAsync(int id)
        {
            var FeederSubFeeder = await Mediator.Send(new GetFeederSubFeederByIdQuery() { FeederTypeId = id });

        

                return Ok(new { StatusCode = StatusCodes.Status200OK, data = FeederSubFeeder, message = FeederSubFeeder });
            
            
        }
        [HttpGet("GetOpeningReaderValue/{feederId}")]
        [ActionName(nameof(GetOpeningReaderValueIdAsync))]
        public async Task<IActionResult> GetOpeningReaderValueIdAsync(int feederId)
        {
            var FeederopeningReading = await Mediator.Send(new GetClosingReaderValueByIdQuery() { FeederId = feederId });


                return Ok(new { StatusCode = StatusCodes.Status200OK, data = FeederopeningReading, message = FeederopeningReading });
          
        }
        [HttpPost]
        public async Task<IActionResult> CreateAsync(CreatePowerConsumptionCommand createPowerConsumptionCommand)
        {

         
            var CreatePowerConsumptionId = await _mediator.Send(createPowerConsumptionCommand);

                return Ok(new
                {
                    StatusCode = StatusCodes.Status201Created,
                    message = CreatePowerConsumptionId,
                    data = CreatePowerConsumptionId
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

            return Ok(new { StatusCode = StatusCodes.Status200OK, data = powerconsumption, message = powerconsumption });
            
            
        }

        


    }
}
