using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.ShiftMasterDetailCQRS.Commands.CreateShiftMasterDetail;
using Core.Application.ShiftMasterDetailCQRS.Commands.DeleteShiftMasterDetail;
using Core.Application.ShiftMasterDetailCQRS.Commands.UpdateShiftMasterDetail;
using Core.Application.ShiftMasterDetailCQRS.Queries.GetShiftMasterDetailById;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MaintenanceManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ShiftMasterDetailController : ApiControllerBase
    {
        private readonly IValidator<CreateShiftMasterDetailCommand> _createShiftMasterDetailCommandValidator;
        private readonly IValidator<UpdateShiftMasterDetailCommand> _updateShiftMasterDetailCommandValidator;
        private readonly IValidator<DeleteShiftMasterDetailCommand> _deleteShiftMasterDetailCommandValidator;
        public ShiftMasterDetailController(ISender mediator,IValidator<CreateShiftMasterDetailCommand> createShiftMasterDetailCommandValidator,IValidator<UpdateShiftMasterDetailCommand> updateShiftMasterDetailCommandValidator,IValidator<DeleteShiftMasterDetailCommand> deleteShiftMasterDetailCommandValidator) 
        : base(mediator)
        {
            _createShiftMasterDetailCommandValidator = createShiftMasterDetailCommandValidator;
            _updateShiftMasterDetailCommandValidator = updateShiftMasterDetailCommandValidator;
            _deleteShiftMasterDetailCommandValidator = deleteShiftMasterDetailCommandValidator;
        }
         
         [HttpPost]
        public async Task<IActionResult> CreateAsync(CreateShiftMasterDetailCommand command)
        {
            
            var validationResult = await _createShiftMasterDetailCommandValidator.ValidateAsync(command);
            
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
        public async Task<IActionResult> Update( UpdateShiftMasterDetailCommand command )
        {
            var validationResult = await _updateShiftMasterDetailCommandValidator.ValidateAsync(command);
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
            var command = new DeleteShiftMasterDetailCommand { Id = id };
             var validationResult = await  _deleteShiftMasterDetailCommandValidator.ValidateAsync(command);
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
    }
}