using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.MachineSpecification.Command.CreateMachineSpecfication;
using Core.Application.MachineSpecification.DeleteMachineSpecfication;
using Core.Application.MachineSpecification.Queries.GetMachineSpecificationById;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MaintenanceManagement.API.Controllers
{
    [Route("api/[controller]")]
    public class MachineSpecificationController : ApiControllerBase
    {
        private readonly ILogger<MachineSpecificationController> _logger;
        private readonly IValidator<CreateMachineSpecficationCommand> _createmachinemastercommand;
        private readonly IValidator<DeleteMachineSpecficationCommand> _deletemachinemastercommand;
        private readonly IMediator _mediator;

        public MachineSpecificationController(ILogger<MachineSpecificationController> logger, IMediator mediator, IValidator<CreateMachineSpecficationCommand> createmachinemastercommand, IValidator<DeleteMachineSpecficationCommand> deletemachinemastercommand)
        : base(mediator)
        {
            _logger = logger;
            _mediator = mediator;
            _createmachinemastercommand = createmachinemastercommand;
            _deletemachinemastercommand = deletemachinemastercommand;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync(CreateMachineSpecficationCommand createMachineSpecficationCommand)
        {

            // Validate the incoming command
            var validationResult = await _createmachinemastercommand.ValidateAsync(createMachineSpecficationCommand);
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
            var CreateMachineId = await _mediator.Send(createMachineSpecficationCommand);

            if (CreateMachineId.IsSuccess)
            {
                _logger.LogInformation($"MachineSpecification {createMachineSpecficationCommand.SpecificationId} created successfully.");
                return Ok(new
                {
                    StatusCode = StatusCodes.Status201Created,
                    message = CreateMachineId.Message,
                    data = CreateMachineId.Data
                });
            }
            _logger.LogWarning($"MachineSpecification {createMachineSpecficationCommand.SpecificationId} Creation failed.");
            return BadRequest(new
            {
                StatusCode = StatusCodes.Status400BadRequest,
                message = CreateMachineId.Message
            });

        }

        [HttpDelete]
        public async Task<IActionResult> DeleteMachineSpecificationAsync(int id)
        {
            // Validate the incoming command
            var validationResult = await _deletemachinemastercommand.ValidateAsync(new DeleteMachineSpecficationCommand { Id = id });
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
            var result = await _mediator.Send(new DeleteMachineSpecficationCommand { Id = id });

            if (result.IsSuccess)
            {
                _logger.LogInformation($"MachineSpecification {id} deleted successfully.");
                return Ok(new
                {
                    message = result.Message,
                    statusCode = StatusCodes.Status200OK
                });

            }
            _logger.LogWarning($"MachineSpecification {id} Not Found or Invalid MachineMasterId.");
            return NotFound(new
            {
                message = result.Message,
                statusCode = StatusCodes.Status404NotFound
            });

        }
        
         [HttpGet("{id}")]
        [ActionName(nameof(GetByIdAsync))]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var machine = await Mediator.Send(new GetMachineSpecificationByIdQuery() { Id = id });

            if (machine.IsSuccess)
            {

                return Ok(new { StatusCode = StatusCodes.Status200OK, data = machine.Data, message = machine.Message });
            }
            return NotFound(new { StatusCode = StatusCodes.Status404NotFound, message = machine.Message });
        }

       
    }
}