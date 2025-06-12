using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.Power.IFeeder;
using Core.Application.Power.Feeder.Command.CreateFeeder;
using Core.Application.Power.Feeder.Command.DeleteFeeder;
using Core.Application.Power.Feeder.Command.UpdateFeeder;
using Core.Application.Power.Feeder.Queries.GetFeeder;
using Core.Application.Power.Feeder.Queries.GetFeederAutoComplete;
using Core.Application.Power.Feeder.Queries.GetFeederById;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MaintenanceManagement.API.Controllers.Power
{
    [ApiController]
    [Route("api/[controller]")]
    public class FeederController : ApiControllerBase
    {
        private readonly ILogger<FeederController> _logger;
        private readonly IFeederQueryRepository _feederQueryRepository;
        private readonly IValidator<CreateFeederCommand> _feederCommandRepository;
        private readonly IValidator<UpdateFeederCommand> _updatefeederCommandRepository;
        private readonly IValidator<DeleteFeederCommand> _DeletefeederCommandRepository;

        public FeederController(ISender mediator, ILogger<FeederController> logger, IFeederQueryRepository feederQueryRepository, IValidator<CreateFeederCommand> feederCommandRepository, IValidator<UpdateFeederCommand> updatefeederCommandRepository, IValidator<DeleteFeederCommand> DeletefeederCommandRepository) : base(mediator)
        {
            _logger = logger;
            _feederQueryRepository = feederQueryRepository;
            _feederCommandRepository = feederCommandRepository;
            _updatefeederCommandRepository = updatefeederCommandRepository;
            _DeletefeederCommandRepository = DeletefeederCommandRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllFeederAsync([FromQuery] int PageNumber, [FromQuery] int PageSize, [FromQuery] string? SearchTerm = null)
        {
            var Feeder = await Mediator.Send(
             new GetFeederQuery
             {
                 PageNumber = PageNumber,
                 PageSize = PageSize,
                 SearchTerm = SearchTerm
             });
            return Ok(new
            {
                StatusCode = StatusCodes.Status200OK,
                data = Feeder.Data,
                TotalCount = Feeder.TotalCount,
                PageNumber = Feeder.PageNumber,
                PageSize = Feeder.PageSize
            });
        }

        [HttpGet("{id}")]

        public async Task<IActionResult> GetById(int id)
        {
            var result = await Mediator.Send(new GetFeederByIdQuery { Id = id });

            if (result.IsSuccess && result.Data != null)
            {
                return Ok(new
                {
                    StatusCode = StatusCodes.Status200OK,
                    data = result.Data,
                    message = result.Message
                });
            }

            return NotFound(new
            {
                StatusCode = StatusCodes.Status404NotFound,
                message = $"Feeder ID {id} not found.",
                errors = ""
            });
        }

        [HttpPost("create")]
        public async Task<ActionResult<ApiResponseDTO<int>>> Create([FromBody] CreateFeederCommand command)
        {
            // Optionally validate input
            var validationResult = await _feederCommandRepository.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    message = "Validation failed",
                    errors = validationResult.Errors.Select(e => e.ErrorMessage).ToArray()
                });
            }

            var response = await Mediator.Send(command);

            if (response.IsSuccess)
            {
                return CreatedAtAction(nameof(GetById), new { id = response.Data }, new
                {
                    StatusCode = StatusCodes.Status201Created,
                    message = response.Message,
                    errors = "",
                    data = response.Data
                });
            }

            return BadRequest(new
            {
                StatusCode = StatusCodes.Status400BadRequest,
                message = response.Message,
                errors = ""
            });
        }

        [HttpPut]
        public async Task<IActionResult> UpdateAsync(UpdateFeederCommand command)
        {
            var validationResult = await _updatefeederCommandRepository.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                return BadRequest(
                    new
                    {
                        StatusCode = StatusCodes.Status400BadRequest,
                        message = "Validation failed",
                        errors = validationResult.Errors.Select(e => e.ErrorMessage).ToArray()
                    }
                );
            }
            var result = await Mediator.Send(command);
            if (result.IsSuccess)
            {
                return Ok(new
                {
                    StatusCode = StatusCodes.Status200OK,
                    message = result.Message,
                    asset = result.Data
                });
            }

            return BadRequest(new
            {
                StatusCode = StatusCodes.Status400BadRequest,
                message = result.Message
            });

        }
         
           [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var command = new DeleteFeederCommand { Id = id };
            var validationResult = await _DeletefeederCommandRepository.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    message = "Validation failed",
                    errors = validationResult.Errors.Select(e => e.ErrorMessage).ToArray()
                });
            }

            var updatefeeder = await Mediator.Send(new DeleteFeederCommand { Id = id });

            if (updatefeeder.IsSuccess)
            {
                return Ok(new { StatusCode = StatusCodes.Status200OK, message = updatefeeder.Message, errors = "" });

            }

            return BadRequest(new { StatusCode = StatusCodes.Status400BadRequest, message = updatefeeder.Message, errors = "" });

        }


           [HttpGet("by-name")]
        public async Task<IActionResult> GetFeeder([FromQuery] string? name)
        {
          
            var feeder = await Mediator.Send(new GetFeederAutoCompleteQuery {SearchPattern = name});
            if(feeder.IsSuccess)
            {
            return Ok( new { StatusCode=StatusCodes.Status200OK, data = feeder.Data });
            }
            return NotFound( new { StatusCode=feeder.Message}) ;
        }
    }
}