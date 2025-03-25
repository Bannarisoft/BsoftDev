using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.ShiftMasterCQRS.Commands.CreateShiftMaster;
using Core.Application.ShiftMasterCQRS.Commands.DeleteShiftMaster;
using Core.Application.ShiftMasterCQRS.Commands.UpdateShiftMaster;
using Core.Application.ShiftMasterCQRS.Queries.GetShiftMaster;
using Core.Application.ShiftMasterCQRS.Queries.GetShiftMasterAutoComplete;
using Core.Application.ShiftMasterCQRS.Queries.GetShiftMasterById;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MaintenanceManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ShiftMasterController : ApiControllerBase
    {
        private readonly IValidator<CreateShiftMasterCommand> _createShiftMasterCommandValidator;
        private readonly IValidator<UpdateShiftMasterCommand> _updateShiftMasterCommandValidator;
        private readonly IValidator<DeleteShiftMasterCommand> _deleteShiftMasterCommandValidator;
        public ShiftMasterController(ISender mediator, IValidator<CreateShiftMasterCommand> createDivisionCommandValidator, IValidator<UpdateShiftMasterCommand> updateDivisionCommandValidator, IValidator<DeleteShiftMasterCommand> deleteDivisionCommandValidator)
        : base(mediator)
        {
            _createShiftMasterCommandValidator = createDivisionCommandValidator;
            _updateShiftMasterCommandValidator = updateDivisionCommandValidator;
            _deleteShiftMasterCommandValidator = deleteDivisionCommandValidator;
        }
           [HttpGet]
        public async Task<IActionResult> GetAllShiftMastersAsync([FromQuery] int PageNumber,[FromQuery] int PageSize,[FromQuery] string? SearchTerm = null)
        {
           var divisions = await Mediator.Send(
            new GetShiftMasterQuery
            {
                PageNumber = PageNumber, 
                PageSize = PageSize, 
                SearchTerm = SearchTerm
            });
           
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
        public async Task<IActionResult> CreateAsync(CreateShiftMasterCommand command)
        {
            
            var validationResult = await _createShiftMasterCommandValidator.ValidateAsync(command);
            
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
                return Ok(new 
                { 
                    StatusCode=StatusCodes.Status201Created, 
                    message = response.Message, 
                    errors = "", 
                    data = response.Data 
                });
            }
             

            return BadRequest( new 
            { 
                StatusCode=StatusCodes.Status400BadRequest, 
                message = response.Message, 
                errors = "" 
            }); 
            
        }
         [HttpGet("{id}")]
         [ActionName(nameof(GetByIdAsync))]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
               
            var shiftMaster = await Mediator.Send(new GetShiftMasterByIdQuery { Id = id});
          
             if(shiftMaster == null)
            {
                return NotFound( new 
                { 
                    StatusCode=StatusCodes.Status404NotFound, 
                    message = $"Shift Master ID {id} not found.", 
                    errors = "" 
                });
            }
            return Ok(new 
            { 
                StatusCode=StatusCodes.Status200OK, 
                data = shiftMaster.Data
            });
        }

        [HttpPut]
        public async Task<IActionResult> Update( UpdateShiftMasterCommand command )
        {
            var validationResult = await _updateShiftMasterCommandValidator.ValidateAsync(command);
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
                 return Ok(new 
                 { 
                    StatusCode=StatusCodes.Status200OK, 
                    message = response.Message, 
                    errors = "" 
                });
             }
            
           

            return BadRequest( new 
            { 
                StatusCode=StatusCodes.Status400BadRequest, 
                message = response.Message, 
                errors = "" 
            }); 
        }


        [HttpDelete("{id}")]
        
        public async Task<IActionResult> Delete(int id)
        {
            var command = new DeleteShiftMasterCommand { Id = id };
             var validationResult = await  _deleteShiftMasterCommandValidator.ValidateAsync(command);
               if (!validationResult.IsValid)
                {
                    return BadRequest(new 
                {
                    StatusCode=StatusCodes.Status400BadRequest,message = "Validation failed", 
                    errors = validationResult.Errors.Select(e => e.ErrorMessage).ToArray() 
                });
                }
           var updatedShiftMaster = await Mediator.Send(command);

           if(updatedShiftMaster.IsSuccess)
           {
            return Ok(new 
            { 
                StatusCode=StatusCodes.Status200OK, 
                message = updatedShiftMaster.Message, 
                errors = "" 
            });
              
           }

            return BadRequest(new 
            { 
                StatusCode=StatusCodes.Status400BadRequest, 
                message = updatedShiftMaster.Message, 
                errors = "" 
            });
            
        }

         [HttpGet("by-name")]
        public async Task<IActionResult> GetShiftMaster([FromQuery] string? name)
        {
           
           var shiftMaster = await Mediator.Send(new GetShiftMasterAutoCompleteQuery {SearchPattern = name});
            return Ok( new 
            { 
                StatusCode=StatusCodes.Status200OK, 
                data = shiftMaster.Data 
            });
        }
    }
}