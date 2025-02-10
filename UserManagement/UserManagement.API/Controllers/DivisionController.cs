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
using Microsoft.AspNetCore.Authorization;
using System.Text.Json;

namespace UserManagement.API.Controllers
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
        public async Task<IActionResult> GetAllDivisionsAsync([FromQuery] int PageNumber,[FromQuery] int PageSize,[FromQuery] string? SearchTerm = null)
        {
           var divisions = await Mediator.Send(
            new GetDivisionQuery
            {
                PageNumber = PageNumber, 
                PageSize = PageSize, 
                SearchTerm = SearchTerm
            });
          //  var activedivisions = divisions.Data.ToList(); 
           
            return Ok( new 
            { 
                StatusCode=StatusCodes.Status200OK, 
                data = divisions.Data,
                TotalCount = divisions.TotalCount,
                PageNumber = divisions.PageNumber,
                PageSize = divisions.PageSize
                });
        }
         [HttpPost]
        public async Task<IActionResult> CreateAsync(CreateDivisionCommand command)
        {
            
            var validationResult = await _createDivisionCommandValidator.ValidateAsync(command);
            
            if (!validationResult.IsValid)
            {
                return BadRequest(new 
                {
                    StatusCode=StatusCodes.Status400BadRequest,message = "Validation failed", 
                    errors = validationResult.Errors.Select(e => e.ErrorMessage).ToArray() 
                });
            }
            var response = await Mediator.Send(command);
            if(response.IsSuccess)
            {
                // return CreatedAtAction(nameof(GetByIdAsync), new {  id = response.Data }, response);
                return Ok(new { StatusCode=StatusCodes.Status201Created, message = response.Message, errors = "", data = response.Data });
            }
             

            return BadRequest( new { StatusCode=StatusCodes.Status400BadRequest, message = response.Message, errors = "" }); 
            
        }
         [HttpGet("{id}")]
         [ActionName(nameof(GetByIdAsync))]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
           
            var division = await Mediator.Send(new GetDivisionByIdQuery() { Id = id});
          
             if(division == null)
            {
                return NotFound( new { StatusCode=StatusCodes.Status404NotFound, message = $"Division ID {id} not found.", errors = "" });
            }
            return Ok(new { StatusCode=StatusCodes.Status200OK, data = division.Data});
        }

        [HttpPut]
        public async Task<IActionResult> Update( UpdateDivisionCommand command )
        {
            var validationResult = await _updateDivisionCommandValidator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }
          

             var divisionExists = await Mediator.Send(new GetDivisionByIdQuery { Id = command.Id });

             if (divisionExists == null)
             {
                 return NotFound(new { StatusCode=StatusCodes.Status404NotFound, message = $"Division ID {command.Id} not found.", errors = "" }); 
             }

             var response = await Mediator.Send(command);
             if(response.IsSuccess)
             {
                 return Ok(new { StatusCode=StatusCodes.Status200OK, message = response.Message, errors = "" });
             }
            
           

            return BadRequest( new { StatusCode=StatusCodes.Status400BadRequest, message = response.Message, errors = "" }); 
        }


        [HttpDelete("{id}")]
        
        public async Task<IActionResult> Delete(int id)
        {
           
           var updatedDivision = await Mediator.Send(new DeleteDivisionCommand { Id = id });

           if(updatedDivision.IsSuccess)
           {
            return Ok(new { StatusCode=StatusCodes.Status200OK, message = updatedDivision.Message, errors = "" });
              
           }

            return BadRequest(new { StatusCode=StatusCodes.Status400BadRequest, message = updatedDivision.Message, errors = "" });
            
        }

         [HttpGet("by-name")]
        public async Task<IActionResult> GetDivision([FromQuery] string? name)
        {
           
           var companiesClaim = User.FindFirst("companyId")?.Value; 
           
            var divisions = await Mediator.Send(new GetDivisionAutoCompleteQuery {SearchPattern = name,Companies = companiesClaim});
            return Ok( new { StatusCode=StatusCodes.Status200OK, data = divisions.Data });
        }
      
      
    }
}