using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Menu.Queries.GetChildMenuByModule;
using Core.Application.Menu.Queries.GetMenuByModule;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace UserManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MenuController : ApiControllerBase
    {
        public MenuController(ISender mediator) : base(mediator)
        {
        }
             [HttpPost("by-module")]
        public async Task<IActionResult> GetParentMenuByModule([FromBody] List<int> id)
        {
            if (id.Count <= 0 || id == null) 
            return BadRequest(new 
            { 
                StatusCode=StatusCodes.Status400BadRequest,
                 message = "Invalid Module ID", 
                 errors = "" 
            });
           var menus = await Mediator.Send(new GetMenuByModuleQuery() { ModuleId = id});
            
            return Ok( new 
            { 
                StatusCode=StatusCodes.Status200OK,
                 data = menus.Data.ToList()
            });
        }
        [HttpPost("by-parent")]
        public async Task<IActionResult> GetChildMenuByModule([FromBody] List<int> id)
        {
            if (id.Count <= 0 || id == null) 
            return BadRequest(new 
            { 
                StatusCode=StatusCodes.Status400BadRequest,
                 message = "Invalid ParentId Menu ID", 
                 errors = "" 
            });
           var menus = await Mediator.Send(new GetChildMenuByModuleQuery() { ParentId = id});
            
            return Ok( new 
            { 
                StatusCode=StatusCodes.Status200OK,
                 data = menus.Data.ToList()
            });
        }
    }
}