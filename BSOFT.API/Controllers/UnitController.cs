using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BSOFT.Application.Units.Queries.GetUnits;
using BSOFT.Application.Units.Queries.GetUnitById;
using BSOFT.Application.Units.Commands.CreateUnit;
using BSOFT.Application.Units.Commands.DeleteUnit;
using BSOFT.Application.Units.Commands.UpdateUnit;
using BSOFT.Domain.Interfaces;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BSOFT.Application.Units.Queries.GetUnitAutoComplete;

namespace BSOFT.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UnitController : ApiControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAllUnitsAsync()
        {
            var units = await Mediator.Send(new GetUnitQuery());
            return Ok(units);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var unit = await Mediator.Send(new GetUnitByIdQuery() { UnitId = id});
            if(unit == null)
            {
                BadRequest("ID in the URL does not match the command Unit.");
            }
            return Ok(unit);
        }

    [HttpPost]
    public async Task<IActionResult> CreateAsync(CreateUnitCommand command)
    {
        var createdUnit = await Mediator.Send(command);
        return Ok("Created Successfully");
        return CreatedAtAction(nameof(GetByIdAsync), new { id = createdUnit.UnitId }, createdUnit);
    }


    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAsync(int id, UpdateUnitCommand command)
    {
        if (id != command.UnitId)
        {
            return BadRequest("UnitId Mismatch");
        }

        var updatedunit = await Mediator.Send(command);
        return Ok("Updated Successfully");
    }


    [HttpPut("delete/{id}")]
    public async Task<IActionResult> DeleteAsync(int id,DeleteUnitCommand command)
    {
         if(id != command.UnitId)
        {
           return BadRequest("UnitId Mismatch"); 
        }
        await Mediator.Send(command);

        return Ok("Status Closed Successfully");
    }

       [HttpGet("GetUnit")]
        public async Task<IActionResult> GetUnit([FromQuery] string searchPattern)
        {
           
            var units = await Mediator.Send(new GetUnitAutoCompleteQuery {SearchPattern = searchPattern});
            return Ok(units);
        }
     
    }
}