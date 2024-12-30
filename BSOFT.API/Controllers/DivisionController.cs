using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Core.Application.Divisions.Queries.GetDivisions;
using Core.Application.Divisions.Commands.CreateDivision;
using Core.Application.Divisions.Queries.GetDivisionById;
using Core.Application.Divisions.Commands.UpdateDivision;
using Core.Application.Divisions.Commands.DeleteDivision;
using Microsoft.AspNetCore.Http;
using System.IO;
using Core.Application.Divisions.Queries.GetDivisionAutoComplete;

namespace BSOFT.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DivisionController : ApiControllerBase
    {
        public DivisionController(ISender mediator) : base(mediator)
        {
        }
         [HttpGet]
        public async Task<IActionResult> GetAllDivisionsAsync()
        {
           
            var divisions = await Mediator.Send(new GetDivisionQuery());
            return Ok(divisions);
        }
         [HttpPost]
        public async Task<IActionResult> CreateAsync(CreateDivisionCommand command)
        {
            var createdDivision = await Mediator.Send(command);
            return CreatedAtAction(nameof(GetByIdAsync), new { DivId = createdDivision.DivId }, createdDivision);
        }
         [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
           
            var division = await Mediator.Send(new GetDivisionByIdQuery() { DivId = id});
          
             if(division == null)
            {
                return NotFound();
            }
            return Ok(division);
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(int id, UpdateDivisionCommand command )
        {
            if(id != command.DivId)
            {
                return BadRequest();
            }
            await Mediator.Send(command);

            return NoContent();
        }


        [HttpPut("delete/{id}")]
        
        public async Task<IActionResult> Delete(int id,DeleteDivisionCommand deleteDivisionCommand)
        {
             if(id != deleteDivisionCommand.DivId)
            {
                return BadRequest();
            }
            await Mediator.Send(deleteDivisionCommand);

            return NoContent();
        }
         [HttpGet("GetDivision")]
        public async Task<IActionResult> GetDivision([FromQuery] string searchPattern)
        {
           
            var divisions = await Mediator.Send(new GetDivisionAutoCompleteQuery {SearchPattern = searchPattern});
            return Ok(divisions);
        }
      
      
    }
}