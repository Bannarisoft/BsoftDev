using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Menu.Commands.CreateMenu;
using Core.Application.Menu.Commands.DeleteMenu;
using Core.Application.Menu.Commands.UpdateMenu;
using Core.Application.Menu.Commands.UploadMenu;
using Core.Application.Menu.Queries.GetChildMenuByModule;
using Core.Application.Menu.Queries.GetMenu;
using Core.Application.Menu.Queries.GetMenuByModule;
using Core.Application.Menu.Queries.GetParentMenu;
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
                    StatusCode = StatusCodes.Status400BadRequest,
                    message = "Invalid Module ID",
                    errors = ""
                });
            var menus = await Mediator.Send(new GetMenuByModuleQuery() { ModuleId = id });

            return Ok(new
            {
                StatusCode = StatusCodes.Status200OK,
                data = menus.Data.ToList()
            });
        }
        [HttpPost("by-parent")]
        public async Task<IActionResult> GetChildMenuByModule([FromBody] List<int> id)
        {
            if (id.Count <= 0 || id == null)
                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    message = "Invalid ParentId Menu ID",
                    errors = ""
                });
            var menus = await Mediator.Send(new GetChildMenuByModuleQuery() { ParentId = id });

            return Ok(new
            {
                StatusCode = StatusCodes.Status200OK,
                data = menus.Data.ToList()
            });
        }
        [HttpGet]
        public async Task<IActionResult> GetAllMenusAsync([FromQuery] int PageNumber, [FromQuery] int PageSize, [FromQuery] string? SearchTerm = null)
        {
            var menus = await Mediator.Send(
             new GetMenuQuery
             {
                 PageNumber = PageNumber,
                 PageSize = PageSize,
                 SearchTerm = SearchTerm
             });


            return Ok(new
            {
                StatusCode = StatusCodes.Status200OK,
                data = menus.Data,
                TotalCount = menus.TotalCount,
                PageNumber = menus.PageNumber,
                PageSize = menus.PageSize
            });
        }
        [HttpPost]
        public async Task<IActionResult> CreateAsync(CreateMenuCommand command)
        {

            // var validationResult = await _createDivisionCommandValidator.ValidateAsync(command);

            // if (!validationResult.IsValid)
            // {
            //     return BadRequest(new 
            //     {
            //         StatusCode=StatusCodes.Status400BadRequest,message = "Validation failed", 
            //         errors = validationResult.Errors.Select(e => e.ErrorMessage).ToArray() 
            //     });
            // }
            var response = await Mediator.Send(command);
            if (response.IsSuccess)
            {

                return Ok(new { StatusCode = StatusCodes.Status201Created, message = response.Message, errors = "", data = response.Data });
            }


            return BadRequest(new { StatusCode = StatusCodes.Status400BadRequest, message = response.Message, errors = "" });

        }
        //  [HttpGet("{id}")]
        //  [ActionName(nameof(GetByIdAsync))]
        // public async Task<IActionResult> GetByIdAsync(int id)
        // {

        //     var division = await Mediator.Send(new GetDivisionByIdQuery() { Id = id});

        //      if(division == null)
        //     {
        //         return NotFound( new { StatusCode=StatusCodes.Status404NotFound, message = $"Division ID {id} not found.", errors = "" });
        //     }
        //     return Ok(new { StatusCode=StatusCodes.Status200OK, data = division.Data});
        // }

        [HttpPut]
        public async Task<IActionResult> Update(UpdateMenuCommand command)
        {
            // var validationResult = await _updateDivisionCommandValidator.ValidateAsync(command);
            // if (!validationResult.IsValid)
            // {
            //     return BadRequest(validationResult.Errors);
            // }


            //  var divisionExists = await Mediator.Send(new GetDivisionByIdQuery { Id = command.Id });

            //  if (divisionExists == null)
            //  {
            //      return NotFound(new { StatusCode=StatusCodes.Status404NotFound, message = $"Division ID {command.Id} not found.", errors = "" }); 
            //  }

            var response = await Mediator.Send(command);
            if (response.IsSuccess)
            {
                return Ok(new { StatusCode = StatusCodes.Status200OK, message = response.Message, errors = "" });
            }



            return BadRequest(new { StatusCode = StatusCodes.Status400BadRequest, message = response.Message, errors = "" });
        }


        [HttpDelete("{id}")]

        public async Task<IActionResult> Delete(int id)
        {
            var command = new DeleteMenuCommand { Id = id };

            var updatedDivision = await Mediator.Send(command);

            if (updatedDivision.IsSuccess)
            {
                return Ok(new { StatusCode = StatusCodes.Status200OK, message = updatedDivision.Message, errors = "" });

            }

            return BadRequest(new { StatusCode = StatusCodes.Status400BadRequest, message = updatedDivision.Message, errors = "" });

        }

        [HttpPost("bulk-upload-menu")]
        public async Task<IActionResult> UploadPreventiveSchedule(UploadMenuCommand command)
        {

            var result = await Mediator.Send(command);
            return Ok(result);
        }
            [HttpGet("by-name")]
        public async Task<IActionResult> GetMenu([FromQuery] string? name)
        {
           
            var MenuList = await Mediator.Send(new GetParentMenuQuery {SearchPattern = name});
            return Ok( new { StatusCode=StatusCodes.Status200OK, data = MenuList.Data });
        }
    }
}