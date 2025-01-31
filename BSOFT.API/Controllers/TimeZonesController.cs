using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Core.Application.TimeZones.Queries.GetTimeZones;
using Core.Application.TimeZones.Queries.GetTimeZonesAutoComplete;
using Core.Application.TimeZones.Queries.GetTimeZonesById;
using BSOFT.Infrastructure.Data;
using MediatR;

namespace BSOFT.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TimeZonesController : ApiControllerBase
    {
        private readonly ILogger<TimeZonesController> _logger;
        private readonly ApplicationDbContext _dbContext;
        private readonly IMediator _mediator;

        public TimeZonesController(ILogger<TimeZonesController> logger, IMediator mediator, ApplicationDbContext dbContext)
         : base(mediator)
        {
            _logger = logger;
            _mediator = mediator;
            _dbContext = dbContext;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllTimeZonesAsync()
        {
        
        var result  = await Mediator.Send(new GetTimeZonesQuery());
        if (result == null || result.Data == null || !result.Data.Any())
        {
            _logger.LogWarning("No TimeZone Record {TimeZone} not found in DB.", result.Data);
            return NotFound(new
            {
                message = result.Message,
                statusCode = StatusCodes.Status404NotFound
              
            });
        }
        _logger.LogInformation("TimeZone {TimeZones} Listed successfully.", result.Data.Count);
        return Ok(new
        {
            
            message = result.Message,
            data = result.Data,
            statusCode = StatusCodes.Status200OK
        });
   
}
    [HttpGet("{id}")]
    [ActionName(nameof(GetByIdAsync))]
    public async Task<IActionResult> GetByIdAsync(int id)
    {
        
        if (id <= 0)
        {
            _logger.LogWarning("TimeZone {TimeZoneId} not found.", id);
            return BadRequest(new
            {
                StatusCode = StatusCodes.Status400BadRequest,
                message = "Invalid TimeZone ID"
            });
        }

        var result = await Mediator.Send(new GetTimeZoneByIdQuery { TimeZoneId = id });

        if (result.IsSuccess)
        {
              _logger.LogInformation("TimeZone {TimeZoneId} Listed successfully.", result.Data);
              return Ok(new
             {
                 message = result.Message,
                 statusCode = StatusCodes.Status200OK,
                 data = result.Data
             }); 
        }
        _logger.LogWarning("TimeZone {TimeZoneId} Not found.", result.Data);
        return NotFound(new
        {
            message = result.Message,
            statusCode = StatusCodes.Status404NotFound
        });
   
}
       [HttpGet("GetTimeZonessearch")]
        public async Task<IActionResult> GetTimeZones([FromQuery] string searchPattern)
        {       
      // Check if searchPattern is provided
        if (string.IsNullOrEmpty(searchPattern))
        {
            _logger.LogInformation("Search pattern cannot be empty.");
            return BadRequest(new
            {
                StatusCode = StatusCodes.Status400BadRequest,
                message = "Search pattern cannot be empty."
            });
        }

        // Fetch entities based on search pattern
        var result = await Mediator.Send(new GetTimeZonesAutocompleteQuery { SearchPattern = searchPattern });
        _logger.LogInformation("Search pattern: {SearchPattern}", searchPattern);
       if (result.IsSuccess)
        {
        _logger.LogInformation("TimeZone {TimeZones} Listed successfully.", result.Data.Count);
         return Ok(new  
            {
                message = result.Message,
                statusCode = StatusCodes.Status200OK,
                data = result.Data
            });
        }
        _logger.LogInformation("No TimeZone Record {TimeZone} not found in DB.", result.Data);
        return NotFound(new
        {
            message = result.Message,
            statusCode = StatusCodes.Status404NotFound
        });                  
}
   
    }
}