using MediatR;
using Microsoft.AspNetCore.Mvc;
using FluentValidation;
using Core.Application.Location.Command.CreateLocation;
using Core.Application.Location.Command.UpdateLocation;
using Core.Application.Location.Queries.GetLocations;
using Core.Application.Location.Command.DeleteLocation;
using Core.Application.Location.Queries.GetLocationAutoComplete;
using Core.Application.Location.Queries.GetLocationById;

namespace FAM.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class LocationController : ApiControllerBase
    {
        private readonly IValidator<CreateLocationCommand> _createLocationCommandValidator;
        private readonly IValidator<UpdateLocationCommand> _updateLocationCommandValidator;

        public LocationController(ISender mediator, IValidator<CreateLocationCommand> createLocationCommandValidator, IValidator<UpdateLocationCommand> updateLocationCommandValidator)
        : base(mediator)
        {
            _createLocationCommandValidator = createLocationCommandValidator;
            _updateLocationCommandValidator = updateLocationCommandValidator;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllLocationAsync([FromQuery] int PageNumber, [FromQuery] int PageSize, [FromQuery] string? SearchTerm = null)
        {
            var locations = await Mediator.Send(
                new GetLocationQuery
                {
                    PageNumber = PageNumber,
                    PageSize = PageSize,
                    SearchTerm = SearchTerm
                });

            return Ok(new
            {
                StatusCode = StatusCodes.Status200OK,
                data = locations.Data.ToList(),
                TotalCount = locations.TotalCount,
                PageNumber = locations.PageNumber,
                PageSize = locations.PageSize
            });
        }
        [HttpPost]
        public async Task<IActionResult> CreateAsync(CreateLocationCommand createlocationcommand)
        {

            var validationResult = await _createLocationCommandValidator.ValidateAsync(createlocationcommand);

            if (!validationResult.IsValid)
            {
                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    message = "Validation failed",
                    errors = validationResult.Errors.Select(e => e.ErrorMessage).ToArray()
                });
            }
            var result = await Mediator.Send(createlocationcommand);
            if (result.IsSuccess)
            {
                return Ok(new
                {
                    StatusCode = StatusCodes.Status201Created,
                    message = result.Message,
                    data = result.Data
                });
            }
            return BadRequest(new
            {
                StatusCode = StatusCodes.Status400BadRequest,
                message = result.Message
            });

        }
        [HttpGet("{id}")]
        [ActionName(nameof(GetByIdAsync))]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    message = "Invalid Location ID"
                });
            }
            var result = await Mediator.Send(new GetLocationByIdQuery() { Id = id });

            if (!result.IsSuccess)
            {
                return NotFound(new
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    message = result.Message
                });
            }
            return Ok(new
            {
                StatusCode = StatusCodes.Status200OK,
                data = result.Data
            });
        }

        [HttpPut]
        public async Task<IActionResult> Update(UpdateLocationCommand updateLocationcommand)
        {
            var validationResult = await _updateLocationCommandValidator.ValidateAsync(updateLocationcommand);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }


            var locationExists = await Mediator.Send(new GetLocationByIdQuery { Id = updateLocationcommand.Id });

            if (locationExists == null)
            {
                return NotFound(new { StatusCode = StatusCodes.Status404NotFound, message = $"Location ID {updateLocationcommand.Id} not found.", errors = "" });
            }

            var result = await Mediator.Send(updateLocationcommand);
            if (result.IsSuccess)
            {
                return Ok(new
                {
                    StatusCode = StatusCodes.Status201Created,
                    message = result.Message,
                    data = result.Data
                });
            }
            else
            {
                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    message = result.Message
                });
            }
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    message = "Invalid Location ID"
                });
            }
            var deletedlocation = await Mediator.Send(new DeleteLocationCommand { Id = id });

            if (!deletedlocation.IsSuccess)
            {
                return NotFound(new
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    message = deletedlocation.Message
                });
            }
            return Ok(new
            {
                StatusCode = StatusCodes.Status200OK,
                data = $"Location ID {id} Deleted",
                message = deletedlocation.Message
            });

        }

        [HttpGet("by-name")]
        public async Task<IActionResult> GetLocation([FromQuery] string? name)
        {
            var result = await Mediator.Send(new GetLocationAutoCompleteQuery { SearchPattern = name });
            if (!result.IsSuccess)
            {
                return NotFound(new
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    message = result.Message
                });
            }
            return Ok(new
            {
                StatusCode = StatusCodes.Status200OK,
                message = result.Message,
                data = result.Data
            });
        }
    }
}