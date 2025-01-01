using MediatR;
using Microsoft.AspNetCore.Mvc;
using Core.Application.Units.Queries.GetUnits;
using Core.Application.Units.Queries.GetUnitById;
using Core.Application.Units.Commands.CreateUnit;
using Core.Application.Units.Commands.DeleteUnit;
using Core.Application.Units.Commands.UpdateUnit;
using Core.Application.Units.Queries.GetUnitAutoComplete;
using FluentValidation;
using BSOFT.Infrastructure.Data;

namespace BSOFT.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UnitController : ApiControllerBase
    {
        private readonly IValidator<CreateUnitCommand> _createUnitCommandValidator;
        private readonly IValidator<UpdateUnitCommand> _updateUnitCommandValidator;
        private readonly ApplicationDbContext _dbContext;
        public UnitController(ISender mediator,IValidator<CreateUnitCommand> createUnitCommandValidator,IValidator<UpdateUnitCommand> updateUnitCommandValidator,ApplicationDbContext dbContext) 
        : base(mediator)
        {
            _createUnitCommandValidator = createUnitCommandValidator;   
            _updateUnitCommandValidator = updateUnitCommandValidator; 
            _dbContext = dbContext;  
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
            var unit = await Mediator.Send(new GetUnitByIdQuery() { Id = id});
            if(unit == null)
            {
                BadRequest("ID in the URL does not match the command Unit.");
            }
            return Ok(unit);
        }

    [HttpPost]
    public async Task<IActionResult> CreateUnitAsync(CreateUnitCommand command)
    {
        var validationResult = await _createUnitCommandValidator.ValidateAsync(command);
        if (!validationResult.IsValid)
        {
        return BadRequest(validationResult.Errors);
        }
        var createdUnit = await Mediator.Send(command);
        return Ok("Created Successfully");
       
    }


    [HttpPut("update/{id}")]
    public async Task<IActionResult> UpdateUnitAsync(int id, UpdateUnitCommand command)
    {
        var validationResult = await _updateUnitCommandValidator.ValidateAsync(command);
        if (!validationResult.IsValid)
        {
        return BadRequest(validationResult.Errors);
        }
        if (id != command.UnitId)
        {
            return BadRequest("UnitId Mismatch");
        }
        command.UnitId = id;
        var result = await Mediator.Send(command);
        return Ok("Updated Successfully");
    }


    [HttpDelete("delete/{id}")]
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