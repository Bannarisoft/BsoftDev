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
using FluentValidation;

namespace BSOFT.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DivisionController : ApiControllerBase
    {
        private readonly IValidator<CreateDivisionCommand> _createDivisionCommandValidator;
        private readonly IValidator<UpdateDivisionCommand> _updateDivisionCommandValidator;
        public DivisionController(ISender mediator,IValidator<CreateDivisionCommand> createDivisionCommandValidator,IValidator<UpdateDivisionCommand> updateDivisionCommandValidator) 
        : base(mediator)
        {
            _createDivisionCommandValidator = createDivisionCommandValidator;
            _updateDivisionCommandValidator = updateDivisionCommandValidator;
        }
         [HttpGet]
        public async Task<IActionResult> GetAllDivisionsAsync()
        {
           var divisions = await Mediator.Send(new GetDivisionQuery());
            var activedivisions = divisions.Where(c => c.IsActive == 1).ToList(); 
           
            return Ok(activedivisions);
        }
         [HttpPost]
        public async Task<IActionResult> CreateAsync(CreateDivisionCommand command)
        {
            
            var validationResult = await _createDivisionCommandValidator.ValidateAsync(command);
            
            if (!validationResult.IsValid)
            {
                var errorMessages = string.Join("; ", validationResult.Errors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}"));
                throw new BadHttpRequestException(errorMessages, statusCode: StatusCodes.Status400BadRequest);
                // return BadRequest(validationResult.Errors);
            }
            var response = await Mediator.Send(command);
            if(response.IsSuccess)
            {
                return CreatedAtAction(nameof(GetByIdAsync), new {  id = response.Data }, response);
            }
             if (!string.IsNullOrEmpty(response.ErrorCode))
             {

                 return StatusCode(StatusCodes.Status500InternalServerError, response);
             }

            return BadRequest(response);
            
        }
         [HttpGet("{id}")]
         [ActionName(nameof(GetByIdAsync))]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
           
            var division = await Mediator.Send(new GetDivisionByIdQuery() { Id = id});
          
             if(division == null)
            {
                return NotFound();
            }
            return Ok(division);
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(int id, UpdateDivisionCommand command )
        {
            var validationResult = await _updateDivisionCommandValidator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }
            if(id != command.Id)
            {
                return BadRequest("The ID in the URL does not match the ID in the command.");
            }

             var divisionExists = await Mediator.Send(new GetDivisionByIdQuery { Id = id });

             if (divisionExists == null)
             {
                 return NotFound($"Division with ID {id} not found."); 
             }

             var response = await Mediator.Send(command);
             if(response.IsSuccess)
             {
                 return Ok(response);
             }
             if (!string.IsNullOrEmpty(response.ErrorCode))
             {
                 return StatusCode(StatusCodes.Status500InternalServerError, response.Message); 
             }
           

            return BadRequest(response);
        }


        [HttpPut("delete/{id}")]
        
        public async Task<IActionResult> Delete(int id,DeleteDivisionCommand deleteDivisionCommand)
        {
             if(id != deleteDivisionCommand.Id)
            {
                return BadRequest();
            }
           var updatedDivision = await Mediator.Send(deleteDivisionCommand);

            return Ok(updatedDivision);
        }
         [HttpGet("GetDivision")]
        public async Task<IActionResult> GetDivision([FromQuery] string searchPattern)
        {
           
            var divisions = await Mediator.Send(new GetDivisionAutoCompleteQuery {SearchPattern = searchPattern});
            return Ok(divisions);
        }
      
      
    }
}