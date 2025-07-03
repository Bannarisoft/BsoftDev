

using Core.Application.WDVDepreciation.Commands.CreateDepreciation;
using Core.Application.WDVDepreciation.Commands.DeleteDepreciation;
using Core.Application.WDVDepreciation.Commands.LockDepreciation;
using Core.Application.WDVDepreciation.Queries.GetDepreciation;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FAM.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WDVDepreciationController  : ApiControllerBase
    {
         public WDVDepreciationController(ISender mediator) 
        : base(mediator) { }    
        [HttpGet("GetWDV")]
        public async Task<IActionResult> GetDepreciationAsync([FromQuery] GetDepreciationQuery request)
        {
            var data = await Mediator.Send(request);
            return Ok(data);
        }
        
        [HttpPost]               
        public async Task<IActionResult> CreateAsync(CreateDepreciationCommand  command)
        {             
            var result = await Mediator.Send(command);
            if (result.IsSuccess)
            {
                return Ok(new 
                { 
                    StatusCode=StatusCodes.Status201Created,
                    message = result.Message, 
                    data = result.Data
                });
            }                      
            return BadRequest(new 
            { 
                StatusCode=StatusCodes.Status400BadRequest,
                message = result.Message 
            });
        }        
        [HttpDelete]        
        public async Task<IActionResult> DeleteAsync(DeleteDepreciationCommand  command)
        {             
            var result = await Mediator.Send(command);
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
                data =$"WDV Depreciation Details Deleted" ,
                message = result.Message
            });
        }
         [HttpPut]        
        public async Task<IActionResult> LockDepreciationAsync(LockDepreciationCommand  command)
        {             
            var result = await Mediator.Send(command);
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
                data =$"Depreciation Details Locked" ,
                message = result.Message
            });
        }                       
    }
}