using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Core.Application.Units.Queries.GetUnits;
using Core.Application.Units.Queries.GetUnitById;
using Core.Application.Units.Commands.CreateUnit;
using Core.Application.Units.Commands.DeleteUnit;
using Core.Application.Units.Commands.UpdateUnit;
using Core.Application.Common.Interfaces;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Units.Queries.GetUnitAutoComplete;

namespace BSOFT.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UnitController : ApiControllerBase
    {
        public UnitController(ISender mediator) : base(mediator)
        {
        }
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
    public async Task<IActionResult> CreateUnitAsync(CreateUnitCommand command)
    {
        var createdUnit = await Mediator.Send(command);
        return Ok("Created Successfully");
       
    }


    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUnitAsync(int id, UpdateUnitCommand command)
    {
        if (id != command.UnitId)
        {
            return BadRequest("UnitId Mismatch");
        }
        command.UnitId = id;
        var result = await Mediator.Send(command);
        return Ok("Updated Successfully");
    }


    [HttpPut("delete/{id}")]
    public async Task<IActionResult> DeleteUnitAsync(int id,DeleteUnitCommand command)
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