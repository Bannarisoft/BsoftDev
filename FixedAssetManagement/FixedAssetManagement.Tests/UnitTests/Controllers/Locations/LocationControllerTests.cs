using MediatR;
using Microsoft.AspNetCore.Mvc;
using Core.Application.Location.Command.CreateLocation;
using Core.Application.Location.Command.UpdateLocation;
using Core.Application.Location.Queries.GetLocations;
using Core.Application.Location.Command.DeleteLocation;
using Core.Application.Location.Queries.GetLocationAutoComplete;
using Core.Application.Location.Queries.GetLocationById;
using Microsoft.AspNetCore.Http;

namespace FAM.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LocationController : ApiControllerBase
    {
        public LocationController(ISender mediator) : base(mediator) { }

        [HttpGet]
        public async Task<IActionResult> GetAllLocationAsync(
            [FromQuery] int PageNumber,
            [FromQuery] int PageSize,
            [FromQuery] string? SearchTerm = null,
            CancellationToken ct = default)
        {
            var locations = await Mediator.Send(new GetLocationQuery
            {
                PageNumber = PageNumber,
                PageSize   = PageSize,
                SearchTerm = SearchTerm
            }, ct);

            return Ok(new
            {
                StatusCode = StatusCodes.Status200OK,
                data       = locations.Data.ToList(),
                TotalCount = locations.TotalCount,
                PageNumber = locations.PageNumber,
                PageSize   = locations.PageSize
            });
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync(
            [FromBody] CreateLocationCommand createlocationcommand,
            CancellationToken ct = default)
        {
            var result = await Mediator.Send(createlocationcommand, ct);
            return Ok(new
            {
                StatusCode = StatusCodes.Status201Created,
                message    = "Location Created Successfully",
                data       = result
            });
        }

        [HttpGet("{id}")]
        [ActionName(nameof(GetByIdAsync))]
        public async Task<IActionResult> GetByIdAsync(int id, CancellationToken ct = default)
        {
            if (id <= 0)
            {
                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    message    = "Invalid Location ID"
                });
            }

            var result = await Mediator.Send(new GetLocationByIdQuery { Id = id }, ct);

            return Ok(new
            {
                StatusCode = StatusCodes.Status200OK,
                data       = result
            });
        }

        [HttpPut]
        public async Task<IActionResult> Update(
            [FromBody] UpdateLocationCommand updateLocationcommand,
            CancellationToken ct = default)
        {
            var locationExists = await Mediator.Send(new GetLocationByIdQuery { Id = updateLocationcommand.Id }, ct);

            if (locationExists == null)
            {
                return NotFound(new
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    message    = $"Location ID {updateLocationcommand.Id} not found.",
                    errors     = ""
                });
            }

            var result = await Mediator.Send(updateLocationcommand, ct);

            return Ok(new
            {
                StatusCode = StatusCodes.Status201Created,
                message    = "Location Updated Successfully",
                data       = result
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, CancellationToken ct = default)
        {
            if (id <= 0)
            {
                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    message    = "Invalid Location ID"
                });
            }

            await Mediator.Send(new DeleteLocationCommand { Id = id }, ct);

            return Ok(new
            {
                StatusCode = StatusCodes.Status200OK,
                data       = $"Location ID {id} Deleted",
                message    = "Location Deleted Successfully"
            });
        }

        [HttpGet("by-name")]
        public async Task<IActionResult> GetLocation([FromQuery] string? name, CancellationToken ct = default)
        {
            var result = await Mediator.Send(new GetLocationAutoCompleteQuery { SearchPattern = name }, ct);

            return Ok(new
            {
                StatusCode = StatusCodes.Status200OK,
                message    = result,
                data       = result
            });
        }
    }
}
