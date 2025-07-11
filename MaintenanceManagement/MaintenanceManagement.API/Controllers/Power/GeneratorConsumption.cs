using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Power.GeneratorConsumption.Command;
using Core.Application.Power.GeneratorConsumption.Queries.GetClosingEnergyReaderValueById;
using Core.Application.Power.GeneratorConsumption.Queries.GetUnitIdBasedOnMachineId;
using MassTransit.Futures.Contracts;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MaintenanceManagement.API.Controllers.Power
{
    [ApiController]
    [Route("api/[controller]")]
    public class GeneratorConsumption : ApiControllerBase
    {
        private readonly IMediator _mediator;

        public GeneratorConsumption(IMediator mediator)
        : base(mediator)
        {
            _mediator = mediator;
        }
        [HttpGet("GetOpeningReaderValue/{generatorId}")]
        [ActionName(nameof(GetOpeningReaderValueIdAsync))]
        public async Task<IActionResult> GetOpeningReaderValueIdAsync(int generatorId)
        {
            var generatoropeningReading = await Mediator.Send(new GetClosingEnergyReaderValueByIdQuery() { GeneratorId = generatorId });


            return Ok(new { StatusCode = StatusCodes.Status200OK, data = generatoropeningReading, message = generatoropeningReading });

        }
        [HttpPost]
        public async Task<IActionResult> CreateAsync(CreateGeneratorConsumptionCommand createGeneratorConsumptionCommand)
        {

            var CreatePowerConsumptionId = await _mediator.Send(createGeneratorConsumptionCommand);

            return Ok(new
            {
                StatusCode = StatusCodes.Status201Created,
                message = "GeneratorConsumption Created Successfully",
                data = CreatePowerConsumptionId
            });



        }
        
        [HttpGet]
        public async Task<IActionResult> GetMachineBasedonUnitIdAsync()
        {
            var Machinename = await Mediator.Send(new GetUnitIdBasedOnMachineIdQuery() {});
                return Ok(new { StatusCode = StatusCodes.Status200OK, data = Machinename, message = "MachineName Fetched Successfully" });
               
        }
        

      
    }
}