using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.Power.IFeederGroup;

using Core.Application.Power.FeederGroup.Command.CreateFeederGroup;
using Core.Application.Power.FeederGroup.Command.DeleteFeederGroup;
using Core.Application.Power.FeederGroup.Command.UpdateFeederGroup;
using Core.Application.Power.FeederGroup.Queries.GetFeederGroup;
using Core.Application.Power.FeederGroup.Queries.GetFeederGroupAutoComplete;
using Core.Application.Power.FeederGroup.Queries.GetFeederGroupById;
using FluentValidation;
using MaintenanceManagement.API.Validation.Power.FeederGroup;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MaintenanceManagement.API.Controllers.Power
{
    [ApiController]
    [Route("api/[controller]")]
    public class FeederGroupController : ApiControllerBase
    {
        private readonly IFeederGroupQueryRepository _feederGroupQueryRepository;
        private readonly IValidator<CreateFeederGroupCommand> _feederGroupCommandRepository;
        private readonly IValidator<UpdateFeederGroupCommand> _updateFeederGroupCommandValidator;
        private readonly IValidator<DeleteFeederGroupCommand> _deletefeederGroupCommandDelegateRepository;


        private readonly ILogger _logger;

        public FeederGroupController(ISender mediator, ILogger<FeederGroupController> logger,
        IFeederGroupQueryRepository feederGroupQueryRepository,
        IValidator<CreateFeederGroupCommand> feederGroupCommandRepository,
        IValidator<UpdateFeederGroupCommand> updateFeederGroupCommandValidator,
        IValidator<DeleteFeederGroupCommand> feederGroupCommandDelegateRepository) : base(mediator)
        {
            _logger = logger;
            _feederGroupQueryRepository = feederGroupQueryRepository;
            _feederGroupCommandRepository = feederGroupCommandRepository;
            _updateFeederGroupCommandValidator = updateFeederGroupCommandValidator;
            _deletefeederGroupCommandDelegateRepository = feederGroupCommandDelegateRepository;

        }

        [HttpGet]
        public async Task<IActionResult> GetAllFeederGroupAsync([FromQuery] int PageNumber, [FromQuery] int PageSize, [FromQuery] string? SearchTerm = null)
        {
            var FeederGroup = await Mediator.Send(
             new GetFeederGroupQuery
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


        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await Mediator.Send(new GetFeederGroupByIdQuery { Id = id });

            if (!result.IsSuccess || result.Data == null)
                return NotFound(new ApiResponseDTO<GetFeederGroupByIdDto>
                {
                    IsSuccess = false,
                    Message = $"FeederGroup with Id {id} not found.",
                    Data = null
                });

            return Ok(result);
        }
        [HttpPost("create")]
        public async Task<ActionResult<ApiResponseDTO<int>>> Create([FromBody] CreateFeederGroupCommand command)
        {
            // Optionally validate input
            var validationResult = await _feederGroupCommandRepository.ValidateAsync(command);
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
        public async Task<IActionResult> UpdateAsync(UpdateFeederGroupCommand command)
        {
            var validationResult = await _updateFeederGroupCommandValidator.ValidateAsync(command);
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
            var command = new DeleteFeederGroupCommand { Id = id };
            var validationResult = await _deletefeederGroupCommandDelegateRepository.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    message = "Validation failed",
                    errors = validationResult.Errors.Select(e => e.ErrorMessage).ToArray()
                });
            }

            var updatefeederGroup = await Mediator.Send(new DeleteFeederGroupCommand { Id = id });

            if (updatefeederGroup.IsSuccess)
            {
                return Ok(new { StatusCode = StatusCodes.Status200OK, message = updatefeederGroup.Message, errors = "" });

            }

            return BadRequest(new { StatusCode = StatusCodes.Status400BadRequest, message = updatefeederGroup.Message, errors = "" });

        }

         [HttpGet("by-name")]
        public async Task<IActionResult> GetFeederGroup([FromQuery] string? name)
        {
          
            var miscmaster = await Mediator.Send(new GetFeederGroupAutoCompleteQuery {SearchPattern = name});
            if(miscmaster.IsSuccess)
            {
            return Ok( new { StatusCode=StatusCodes.Status200OK, data = miscmaster.Data });
            }
            return NotFound( new { StatusCode=miscmaster.Message}) ;
        }
            



      
    }
}