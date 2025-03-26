using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.SwitchProfile.Commands.SwitchProfileByUnit;
using Core.Application.SwitchProfile.Queries.GetUnitProfile;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace UserManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SwitchProfileController : ApiControllerBase
    {
        public SwitchProfileController(ISender mediator)
        : base(mediator)
        {
            
        }
         [HttpGet("by-name")]
        public async Task<IActionResult> GetUnit()
        {
            var units = await Mediator.Send(new GetUnitProfileQuery {});
             
            if(units.IsSuccess)
            {
                
                return Ok(new
                {
                    message = units.Message,
                    statusCode = StatusCodes.Status200OK,
                    data = units.Data
                });
            }
            
            return NotFound(new
            {
                message = units.Message,
                statusCode = StatusCodes.Status404NotFound
            });
          
        }
           [HttpPost("SwitchProfile")]   
        public async Task<IActionResult> SwitchProfile(SwitchProfileByUnitCommand  command)
        { 
                          
            var result = await Mediator.Send(command);
            if(result.IsSuccess)
            {                
                return Ok(new { StatusCode=StatusCodes.Status201Created, message = result.Message, errors = "", data = result.Data });
            }
            

            return BadRequest( new { StatusCode=StatusCodes.Status400BadRequest, message = result.Message, errors = "" }); 
            
        }
    }
}