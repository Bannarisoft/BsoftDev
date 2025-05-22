using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.MachineMaster.Command.CreateMachineMaster;
using Core.Application.MachineMaster.Command.DeleteMachineMaster;
using Core.Application.MachineMaster.Command.UpdateMachineMaster;
using Core.Application.MachineMaster.Queries.GetMachineDepartmentbyId;
using Core.Application.MachineMaster.Queries.GetMachineLineNo;
using Core.Application.MachineMaster.Queries.GetMachineMaster;
using Core.Application.MachineMaster.Queries.GetMachineMasterAutoComplete;
using Core.Application.MachineMaster.Queries.GetMachineMasterById;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MaintenanceManagement.API.Controllers
{
    [Route("api/[controller]")]
    public class MachineController : ApiControllerBase
    {
        private readonly ILogger<MachineController> _logger;
        private readonly IMediator _mediator;
        private readonly IValidator<CreateMachineMasterCommand> _createmachinemastercommand;
        private readonly IValidator<UpdateMachineMasterCommand> _updatemachinemastercommand;
        private readonly IValidator<DeleteMachineMasterCommand> _deletemachinemastercommand;


        public MachineController(ILogger<MachineController> logger, IMediator mediator, IValidator<CreateMachineMasterCommand> createmachinemastercommand, IValidator<UpdateMachineMasterCommand> updatemachinemastercommand, IValidator<DeleteMachineMasterCommand> deletemachinemastercommand)
        : base(mediator)
        {
            _logger = logger;
            _mediator = mediator;
            _createmachinemastercommand = createmachinemastercommand;
            _updatemachinemastercommand = updatemachinemastercommand;
            _deletemachinemastercommand = deletemachinemastercommand;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllMachineMasterAsync([FromQuery] int PageNumber, [FromQuery] int PageSize, [FromQuery] string? SearchTerm = null)
        {
            var MachineMaster = await Mediator.Send(
             new GetMachineMasterQuery
             {
                 PageNumber = PageNumber,
                 PageSize = PageSize,
                 SearchTerm = SearchTerm
             });
            return Ok(new
            {
                StatusCode = StatusCodes.Status200OK,
                data = MachineMaster.Data,
                TotalCount = MachineMaster.TotalCount,
                PageNumber = MachineMaster.PageNumber,
                PageSize = MachineMaster.PageSize
            });
        }

        [HttpGet("by-name")]
        public async Task<IActionResult> GetMachineMasterAsync([FromQuery] string? Typename)
        {
            var MachineMaster = await Mediator.Send(new GetMachineMasterAutoCompleteQuery
            {
                SearchPattern = Typename ?? string.Empty
            });

            return Ok(new { StatusCode = StatusCodes.Status200OK, data = MachineMaster.Data });
        }

        [HttpGet("{id}")]
        [ActionName(nameof(GetByIdAsync))]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var machine = await Mediator.Send(new GetMachineMasterByIdQuery() { Id = id });

            if (machine.IsSuccess)
            {

                return Ok(new { StatusCode = StatusCodes.Status200OK, data = machine.Data, message = machine.Message });
            }
            return NotFound(new { StatusCode = StatusCodes.Status404NotFound, message = machine.Message });
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync(CreateMachineMasterCommand createMachineMasterCommand)
        {

            // Validate the incoming command
            var validationResult = await _createmachinemastercommand.ValidateAsync(createMachineMasterCommand);
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
            var CreateMachineId = await _mediator.Send(createMachineMasterCommand);

            if (CreateMachineId.IsSuccess)
            {
                _logger.LogInformation($"MachineMaster {createMachineMasterCommand.MachineName} created successfully.");
                return Ok(new
                {
                    StatusCode = StatusCodes.Status201Created,
                    message = CreateMachineId.Message,
                    data = CreateMachineId.Data
                });
            }
            _logger.LogWarning($"MachineMaster {createMachineMasterCommand.MachineName} Creation failed.");
            return BadRequest(new
            {
                StatusCode = StatusCodes.Status400BadRequest,
                message = CreateMachineId.Message
            });

        }
        [HttpPut]
        public async Task<IActionResult> UpdateAsync(UpdateMachineMasterCommand updateMachineMasterCommand)
        {

            // Validate the incoming command
            var validationResult = await _updatemachinemastercommand.ValidateAsync(updateMachineMasterCommand);
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

            var updatemachinemaster = await _mediator.Send(updateMachineMasterCommand);

            if (updatemachinemaster.IsSuccess)
            {
                _logger.LogInformation($"MachineMaster {updateMachineMasterCommand.MachineName} updated successfully.");
                return Ok(new
                {
                    message = updatemachinemaster.Message,
                    statusCode = StatusCodes.Status200OK
                });
            }
            _logger.LogWarning($"MachineMaster {updateMachineMasterCommand.MachineName} Update failed.");
            return NotFound(new
            {
                message = updatemachinemaster.Message,
                statusCode = StatusCodes.Status404NotFound
            });
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteMachineMasterAsync(int id)
        {
            // Validate the incoming command
            var validationResult = await _deletemachinemastercommand.ValidateAsync(new DeleteMachineMasterCommand { Id = id });
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
            var result = await _mediator.Send(new DeleteMachineMasterCommand { Id = id });

            if (result.IsSuccess)
            {
                _logger.LogInformation($"MachineMaster {id} deleted successfully.");
                return Ok(new
                {
                    message = result.Message,
                    statusCode = StatusCodes.Status200OK
                });

            }
            _logger.LogWarning($"MachineMaster {id} Not Found or Invalid MachineMasterId.");
            return NotFound(new
            {
                message = result.Message,
                statusCode = StatusCodes.Status404NotFound
            });

        }

        [HttpGet("MachineLineNo")]
        public async Task<IActionResult> GetMachineLineNo()
        {
            var result = await Mediator.Send(new GetMachineLinenoQuery());

            if (result == null || result.Data == null || result.Data.Count == 0)
            {
                return NotFound(new
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    message = "No MachineLineNo found."
                });
            }
            return Ok(new
            {
                StatusCode = StatusCodes.Status200OK,
                message = "MachineLineNo fetched successfully.",
                data = result.Data
            });
        }
        
        
        [HttpGet("MachineGroup/{MachineGroupId}")]
        [ActionName(nameof(GetMachineDepartmentByIdAsync))]
        public async Task<IActionResult> GetMachineDepartmentByIdAsync(int MachineGroupId)
        {
            var machine = await Mediator.Send(new GetMachineDepartmentbyIdQuery() { MachineGroupId = MachineGroupId });

            if (machine.IsSuccess)
            {

                return Ok(new { StatusCode = StatusCodes.Status200OK, data = machine.Data, message = machine.Message });
            }
            return NotFound(new { StatusCode = StatusCodes.Status404NotFound, message = machine.Message });
        }


        
    }
}