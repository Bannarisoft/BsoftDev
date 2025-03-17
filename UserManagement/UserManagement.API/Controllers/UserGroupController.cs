using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.UserGroup.Queries.GetUserGroupAutoComplete;
using MediatR;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;

namespace UserManagement.API.Controllers
{
    [ApiController]
    [Microsoft.AspNetCore.Components.Route("api/[controller]")]

    public class UserGroupController  : ApiControllerBase
    {
         public UserGroupController(ISender mediator
                           ) 
         : base(mediator)
        {        
          
        }
        [HttpGet("by-name")]     
        public async Task<IActionResult> GetUserGroup([FromQuery] string? name)
        {
            var result = await Mediator.Send(new GetUserGroupAutoCompleteQuery { SearchPattern = name });
            if (!result.IsSuccess)
            {
                return NotFound(new 
                { 
                    StatusCode = StatusCodes.Status404NotFound,
                    message = result.Message,
                    data = result.Data
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